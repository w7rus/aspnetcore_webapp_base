using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using BLL.Handlers.Base;
using BLL.Services.Advanced;
using BLL.Services.Entity;
using Common.Enums;
using Common.Exceptions;
using Common.Models;
using Common.Models.Base;
using DAL.Data;
using DAL.Repository;
using Domain.Entities;
using DTO.Models.Generic;
using DTO.Models.UserGroup;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace BLL.Handlers;

public interface IUserGroupActionsHandler
{
    Task<IDtoResultBase> Join(UserGroupJoinDto data, CancellationToken cancellationToken = default);
    Task<IDtoResultBase> Leave(UserGroupLeaveDto data, CancellationToken cancellationToken = default);
    Task<IDtoResultBase> InitTransfer(UserGroupTransferInitDto data, CancellationToken cancellationToken = default);
    Task<IDtoResultBase> ManageTransfer(UserGroupTransferManageDto data, CancellationToken cancellationToken = default);
    Task<IDtoResultBase> InitInviteUser(UserGroupInitInviteUserDto data, CancellationToken cancellationToken = default);
    Task<IDtoResultBase> ManageInviteUser(UserGroupManageInviteUserDto data, CancellationToken cancellationToken = default);
    Task<IDtoResultBase> AddUser(UserGroupAddUserDto data, CancellationToken cancellationToken = default);
    Task<IDtoResultBase> KickUser(UserGroupKickUserDto data, CancellationToken cancellationToken = default);
}

public class UserGroupActionsHandler : HandlerBase, IUserGroupActionsHandler
{
    #region Fields

    private readonly ILogger<HandlerBase> _logger;
    private readonly IAppDbContextAction _appDbContextAction;
    private readonly IMapper _mapper;
    private readonly IUserAdvancedService _userAdvancedService;
    private readonly IAuthorizeAdvancedService _authorizeAdvancedService;
    private readonly IUserRepository _userRepository;
    private readonly IUserGroupRepository _userGroupRepository;
    private readonly IUserToUserGroupMappingRepository _userToUserGroupMappingRepository;
    private readonly IUserEntityService _userEntityService;
    private readonly IUserGroupEntityService _userGroupEntityService;
    private readonly IUserToUserGroupMappingEntityService _userToUserGroupMappingEntityService;
    private readonly IPermissionValueEntityCollectionService _permissionValueEntityCollectionService;
    private readonly IPermissionEntityService _permissionEntityService;
    private readonly IUserGroupTransferRequestEntityService _userGroupTransferRequestEntityService;
    private readonly IUserGroupInviteRequestEntityService _userGroupInviteRequestEntityService;
    private readonly IAuthorizeEntityService _authorizeEntityService;

    #endregion

    #region Ctor

    public UserGroupActionsHandler(
        ILogger<HandlerBase> logger,
        IAppDbContextAction appDbContextAction,
        IMapper mapper,
        IUserAdvancedService userAdvancedService,
        IAuthorizeAdvancedService authorizeAdvancedService,
        IUserRepository userRepository,
        IUserGroupRepository userGroupRepository,
        IUserToUserGroupMappingRepository userToUserGroupMappingRepository,
        IUserEntityService userEntityService,
        IUserGroupEntityService userGroupEntityService,
        IUserToUserGroupMappingEntityService userToUserGroupMappingEntityService,
        IPermissionValueEntityCollectionService permissionValueEntityCollectionService,
        IPermissionEntityService permissionEntityService,
        IUserGroupTransferRequestEntityService userGroupTransferRequestEntityService,
        IUserGroupInviteRequestEntityService userGroupInviteRequestEntityService,
        IAuthorizeEntityService authorizeEntityService
    )
    {
        _logger = logger;
        _appDbContextAction = appDbContextAction;
        _mapper = mapper;
        _userAdvancedService = userAdvancedService;
        _authorizeAdvancedService = authorizeAdvancedService;
        _userRepository = userRepository;
        _userGroupRepository = userGroupRepository;
        _userToUserGroupMappingRepository = userToUserGroupMappingRepository;
        _userEntityService = userEntityService;
        _userGroupEntityService = userGroupEntityService;
        _userToUserGroupMappingEntityService = userToUserGroupMappingEntityService;
        _permissionValueEntityCollectionService = permissionValueEntityCollectionService;
        _permissionEntityService = permissionEntityService;
        _userGroupTransferRequestEntityService = userGroupTransferRequestEntityService;
        _userGroupInviteRequestEntityService = userGroupInviteRequestEntityService;
        _authorizeEntityService = authorizeEntityService;
    }

    #endregion

    #region Methods
    
    public async Task<IDtoResultBase> Join(UserGroupJoinDto data, CancellationToken cancellationToken = default)
    {
        _logger.Log(LogLevel.Information, Localize.Log.MethodStart(GetType(), nameof(Join)));

        if (ValidateModel(data) is { } validationResult)
            return validationResult;

        try
        {
            await _appDbContextAction.BeginTransactionAsync();

            var user = await _userAdvancedService.GetFromHttpContext(cancellationToken);
            if (user == null)
                throw new HttpResponseException(StatusCodes.Status400BadRequest, ErrorType.HttpContext,
                    Localize.Error.UserNotFoundOrHttpContextMissingClaims);

            var userGroup = await _userGroupEntityService.GetByIdAsync(data.Id, cancellationToken);
            if (userGroup == null)
                throw new HttpResponseException(StatusCodes.Status404NotFound, ErrorType.Generic,
                    Localize.Error.UserGroupNotFound);

            //Authorize g_group_a_join_o_usergroup against UserGroup TODO: + against User (user that initiates action)
            var authorizeResult = _authorizeAdvancedService.Authorize(new AuthorizeModel
            {
                EntityLeftTableName = _userRepository.GetTableName(),
                EntityLeftGroupsTableName = _userGroupRepository.GetTableName(),
                EntityLeftEntityToEntityMappingsTableName = _userToUserGroupMappingRepository.GetTableName(),
                EntityLeftId = user.Id,
                EntityLeftPermissionAlias = Consts.PermissionAlias.g_group_a_join_o_usergroup,
                EntityRightTableName = _userGroupRepository.GetTableName(),
                EntityRightGroupsTableName = null,
                EntityRightEntityToEntityMappingsTableName = null,
                EntityRightId = userGroup.Id,
                EntityRightPermissionAlias = Consts.PermissionAlias.g_group_a_join_o_usergroup,
                SqlExpressionPermissionTypeValueNeededOwner = "T1.\"Id\" = T2.\"UserId\""
            });
            
            if (!authorizeResult)
                throw new HttpResponseException(StatusCodes.Status403Forbidden, ErrorType.Permission,
                    Localize.Error.PermissionInsufficientPermissions);
            
            //Do not allow to join User associated with UserGroup
            if (await _userToUserGroupMappingEntityService.GetByUserIdUserGroupIdAsync(user.Id, userGroup.Id) != null)
                throw new HttpResponseException(StatusCodes.Status400BadRequest, ErrorType.Generic,
                    Localize.Error.UserToUserGroupMappingAlreadyExists);

            await _userToUserGroupMappingEntityService.Save(new UserToUserGroupMapping
            {
                EntityLeftId = user.Id,
                EntityRightId = userGroup.Id
            }, cancellationToken);
            
            //Delete Authorize cache when joining group (for the user that joined)
            await _authorizeEntityService.PurgeByEntityIdAsync(user.Id, cancellationToken);

            await _appDbContextAction.CommitTransactionAsync();

            _logger.Log(LogLevel.Information, Localize.Log.MethodEnd(GetType(), nameof(Join)));

            return new OkResultDto();
        }
        catch (Exception)
        {
            await _appDbContextAction.RollbackTransactionAsync();

            throw;
        }
    }

    public async Task<IDtoResultBase> Leave(UserGroupLeaveDto data, CancellationToken cancellationToken = default)
    {
        _logger.Log(LogLevel.Information, Localize.Log.MethodStart(GetType(), nameof(Leave)));

        if (ValidateModel(data) is { } validationResult)
            return validationResult;

        try
        {
            await _appDbContextAction.BeginTransactionAsync();

            var user = await _userAdvancedService.GetFromHttpContext(cancellationToken);
            if (user == null)
                throw new HttpResponseException(StatusCodes.Status400BadRequest, ErrorType.HttpContext,
                    Localize.Error.UserNotFoundOrHttpContextMissingClaims);

            var userGroup = await _userGroupEntityService.GetByIdAsync(data.Id, cancellationToken);
            if (userGroup == null)
                throw new HttpResponseException(StatusCodes.Status404NotFound, ErrorType.Generic,
                    Localize.Error.UserGroupNotFound);

            //Authorize g_group_a_leave_o_usergroup against UserGroup TODO: + against User (user that initiates action)
            var authorizeResult = _authorizeAdvancedService.Authorize(new AuthorizeModel
            {
                EntityLeftTableName = _userRepository.GetTableName(),
                EntityLeftGroupsTableName = _userGroupRepository.GetTableName(),
                EntityLeftEntityToEntityMappingsTableName = _userToUserGroupMappingRepository.GetTableName(),
                EntityLeftId = user.Id,
                EntityLeftPermissionAlias = Consts.PermissionAlias.g_group_a_leave_o_usergroup,
                EntityRightTableName = _userGroupRepository.GetTableName(),
                EntityRightGroupsTableName = null,
                EntityRightEntityToEntityMappingsTableName = null,
                EntityRightId = userGroup.Id,
                EntityRightPermissionAlias = Consts.PermissionAlias.g_group_a_leave_o_usergroup,
                SqlExpressionPermissionTypeValueNeededOwner = "T1.\"Id\" = T2.\"UserId\""
            });
            
            if (!authorizeResult)
                throw new HttpResponseException(StatusCodes.Status403Forbidden, ErrorType.Permission,
                    Localize.Error.PermissionInsufficientPermissions);

            var userToUserGroupMapping =
                await _userToUserGroupMappingEntityService.GetByUserIdUserGroupIdAsync(user.Id, userGroup.Id);
            if (userToUserGroupMapping == null)
                throw new HttpResponseException(StatusCodes.Status404NotFound, ErrorType.Generic,
                    Localize.Error.UserToUserGroupMappingNotFound);
            
            await _permissionValueEntityCollectionService.PurgeAsync(userToUserGroupMapping.Id, cancellationToken);
            
            //Delete Authorize cache when leaving group (for the user that left)
            await _authorizeEntityService.PurgeByEntityIdAsync(user.Id, cancellationToken);
            await _authorizeEntityService.PurgeByEntityIdAsync(userToUserGroupMapping.Id, cancellationToken);

            await _userToUserGroupMappingEntityService.Delete(userToUserGroupMapping, cancellationToken);

            await _appDbContextAction.CommitTransactionAsync();

            _logger.Log(LogLevel.Information, Localize.Log.MethodEnd(GetType(), nameof(Leave)));

            return new OkResultDto();
        }
        catch (Exception)
        {
            await _appDbContextAction.RollbackTransactionAsync();

            throw;
        }
    }
    
    public async Task<IDtoResultBase> InitTransfer(UserGroupTransferInitDto data, CancellationToken cancellationToken = default)
    {
        _logger.Log(LogLevel.Information, Localize.Log.MethodStart(GetType(), nameof(InitTransfer)));

        if (ValidateModel(data) is { } validationResult)
            return validationResult;
        
        try
        {
            await _appDbContextAction.BeginTransactionAsync();

            var user = await _userAdvancedService.GetFromHttpContext(cancellationToken);
            if (user == null)
                throw new HttpResponseException(StatusCodes.Status400BadRequest, ErrorType.HttpContext,
                    Localize.Error.UserNotFoundOrHttpContextMissingClaims);

            var userGroup = await _userGroupEntityService.GetByIdAsync(data.UserGroupId, cancellationToken);
            if (userGroup == null)
                throw new HttpResponseException(StatusCodes.Status404NotFound, ErrorType.Generic,
                    Localize.Error.UserGroupNotFound);

            var userTarget = await _userEntityService.GetByIdAsync(data.UserId, cancellationToken);
            if (userTarget == null)
                throw new HttpResponseException(StatusCodes.Status404NotFound, ErrorType.HttpContext,
                    Localize.Error.UserNotFound);

            //Authorize g_group_a_transfer_o_usergroup against UserGroup TODO: + against User (user that initiates action, user that receives request)
            var authorizeResult = _authorizeAdvancedService.Authorize(new AuthorizeModel
            {
                EntityLeftTableName = _userRepository.GetTableName(),
                EntityLeftGroupsTableName = _userGroupRepository.GetTableName(),
                EntityLeftEntityToEntityMappingsTableName = _userToUserGroupMappingRepository.GetTableName(),
                EntityLeftId = user.Id,
                EntityLeftPermissionAlias = Consts.PermissionAlias.g_group_a_transfer_o_usergroup,
                EntityRightTableName = _userGroupRepository.GetTableName(),
                EntityRightGroupsTableName = null,
                EntityRightEntityToEntityMappingsTableName = null,
                EntityRightId = userGroup.Id,
                EntityRightPermissionAlias = Consts.PermissionAlias.g_group_a_transfer_o_usergroup,
                SqlExpressionPermissionTypeValueNeededOwner = "T1.\"Id\" = T2.\"UserId\""
            });

            if (!authorizeResult)
                throw new HttpResponseException(StatusCodes.Status403Forbidden, ErrorType.Permission,
                    Localize.Error.PermissionInsufficientPermissions);

            if (user.Id == data.UserId)
                throw new HttpResponseException(StatusCodes.Status400BadRequest, ErrorType.Generic,
                    Localize.Error.UserGroupTransferSameUserNotAllowed);

            var userGroupTransferRequest = _mapper.Map<UserGroupTransferRequest>(data);

            userGroupTransferRequest.SrcUserId = user.Id;

            await _userGroupTransferRequestEntityService.Save(userGroupTransferRequest, cancellationToken);

            await _appDbContextAction.CommitTransactionAsync();

            _logger.Log(LogLevel.Information, Localize.Log.MethodEnd(GetType(), nameof(InitTransfer)));

            return _mapper.Map<UserGroupTransferInitResultDto>(userGroupTransferRequest);
        }
        catch (Exception)
        {
            await _appDbContextAction.RollbackTransactionAsync();

            throw;
        }
    }

    public async Task<IDtoResultBase> ManageTransfer(UserGroupTransferManageDto data, CancellationToken cancellationToken = default)
    {
         _logger.Log(LogLevel.Information, Localize.Log.MethodStart(GetType(), nameof(ManageTransfer)));

        if (ValidateModel(data) is { } validationResult)
            return validationResult;
        
        try
        {
            await _appDbContextAction.BeginTransactionAsync();

            var user = await _userAdvancedService.GetFromHttpContext(cancellationToken);
            if (user == null)
                throw new HttpResponseException(StatusCodes.Status400BadRequest, ErrorType.HttpContext,
                    Localize.Error.UserNotFoundOrHttpContextMissingClaims);
            
            var userGroupTransferRequest = await _userGroupTransferRequestEntityService.GetByIdAsync(data.Id, cancellationToken);
            if (userGroupTransferRequest == null)
                throw new HttpResponseException(StatusCodes.Status404NotFound, ErrorType.Generic,
                    Localize.Error.UserGroupTransferRequestNotFound);
                    
            var userGroup = await _userGroupEntityService.GetByIdAsync(userGroupTransferRequest.UserGroupId, cancellationToken);
            if (userGroup == null)
                throw new HttpResponseException(StatusCodes.Status404NotFound, ErrorType.Generic,
                    Localize.Error.UserGroupNotFound);

            //Action is taken by creator of UserGroupTransferRequest
            if (user.Id == userGroupTransferRequest.SrcUserId)
            {
                //Authorize g_group_a_transfer_o_usergroup_a_manage against UserGroup TODO: + against User (user that initiates action)
                var authorizeResult = _authorizeAdvancedService.Authorize(new AuthorizeModel
                {
                    EntityLeftTableName = _userRepository.GetTableName(),
                    EntityLeftGroupsTableName = _userGroupRepository.GetTableName(),
                    EntityLeftEntityToEntityMappingsTableName = _userToUserGroupMappingRepository.GetTableName(),
                    EntityLeftId = user.Id,
                    EntityLeftPermissionAlias = Consts.PermissionAlias.g_group_a_transfer_o_usergroup_a_manage,
                    EntityRightTableName = _userGroupRepository.GetTableName(),
                    EntityRightGroupsTableName = null,
                    EntityRightEntityToEntityMappingsTableName = null,
                    EntityRightId = userGroup.Id,
                    EntityRightPermissionAlias = Consts.PermissionAlias.g_group_a_transfer_o_usergroup_a_manage,
                    SqlExpressionPermissionTypeValueNeededOwner = "T1.\"Id\" = T2.\"UserId\""
                });

                if (!authorizeResult)
                    throw new HttpResponseException(StatusCodes.Status403Forbidden, ErrorType.Permission,
                        Localize.Error.PermissionInsufficientPermissions);
                
                switch (data.Choice)
                {
                    case Choice.None:
                        break;
                    case Choice.Accept:
                        throw new HttpResponseException(StatusCodes.Status400BadRequest, ErrorType.Generic,
                            Localize.Error.UserGroupTransferRequestSelectedChoiceNotAllowed);
                    case Choice.Reject:
                    {
                        await _userGroupTransferRequestEntityService.Delete(userGroupTransferRequest, cancellationToken);
                        break;
                    }
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            //Action is taken by receiver of UserGroupTransferRequest
            else if (user.Id == userGroupTransferRequest.DestUserId)
            {
                switch (data.Choice)
                {
                    case Choice.None:
                        break;
                    case Choice.Accept:
                    {
                        var userToUserGroupMappingPreviousOwner =
                            await _userToUserGroupMappingEntityService.GetByUserIdUserGroupIdAsync(userGroup.UserId, userGroup.Id);
                        if (userToUserGroupMappingPreviousOwner == null)
                            throw new HttpResponseException(StatusCodes.Status404NotFound, ErrorType.Generic,
                                Localize.Error.UserToUserGroupMappingNotFound);
                        
                        await _permissionValueEntityCollectionService.PurgeAsync(userToUserGroupMappingPreviousOwner.Id, cancellationToken);
                        
                        //Delete Authorize cache when group owner changes (for previous owner)
                        await _authorizeEntityService.PurgeByEntityIdAsync(userToUserGroupMappingPreviousOwner.Id, cancellationToken);
                        await _authorizeEntityService.PurgeByEntityIdAsync(userGroup.UserId, cancellationToken);
                        
                        //Delete Authorize cache when group owner changes (for new owner)
                        var userToUserGroupMappingNewOwner =
                            await _userToUserGroupMappingEntityService.GetByUserIdUserGroupIdAsync(userGroupTransferRequest.DestUserId, userGroup.Id);
                        if (userToUserGroupMappingNewOwner != null)
                            await _authorizeEntityService.PurgeByEntityIdAsync(userToUserGroupMappingNewOwner.Id, cancellationToken);
                        await _authorizeEntityService.PurgeByEntityIdAsync(userGroupTransferRequest.DestUserId, cancellationToken);

                        userGroup.UserId = userGroupTransferRequest.DestUserId;
                        await _userGroupEntityService.Save(userGroup, cancellationToken);
                        await _userGroupTransferRequestEntityService.Delete(userGroupTransferRequest,
                            cancellationToken);
                        break;
                    }
                    case Choice.Reject:
                    {
                        await _userGroupTransferRequestEntityService.Delete(userGroupTransferRequest, cancellationToken);
                        break;
                    }
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            //Action is taken by 3rd party
            else
            {
                //Authorize g_group_a_transfer_o_usergroup_a_manage against UserGroup TODO: + against User (user that initiates action)
                var authorizeResult = _authorizeAdvancedService.Authorize(new AuthorizeModel
                {
                    EntityLeftTableName = _userRepository.GetTableName(),
                    EntityLeftGroupsTableName = _userGroupRepository.GetTableName(),
                    EntityLeftEntityToEntityMappingsTableName = _userToUserGroupMappingRepository.GetTableName(),
                    EntityLeftId = user.Id,
                    EntityLeftPermissionAlias = Consts.PermissionAlias.g_group_a_transfer_o_usergroup_a_manage,
                    EntityRightTableName = _userGroupRepository.GetTableName(),
                    EntityRightGroupsTableName = null,
                    EntityRightEntityToEntityMappingsTableName = null,
                    EntityRightId = userGroup.Id,
                    EntityRightPermissionAlias = Consts.PermissionAlias.g_group_a_transfer_o_usergroup_a_manage,
                    SqlExpressionPermissionTypeValueNeededOwner = "T1.\"Id\" = T2.\"UserId\""
                });

                if (!authorizeResult)
                    throw new HttpResponseException(StatusCodes.Status403Forbidden, ErrorType.Permission,
                        Localize.Error.PermissionInsufficientPermissions);
                
                switch (data.Choice)
                {
                    case Choice.None:
                        break;
                    case Choice.Accept:
                    {
                        var userToUserGroupMappingPreviousOwner =
                            await _userToUserGroupMappingEntityService.GetByUserIdUserGroupIdAsync(userGroup.UserId, userGroup.Id);
                        if (userToUserGroupMappingPreviousOwner == null)
                            throw new HttpResponseException(StatusCodes.Status404NotFound, ErrorType.Generic,
                                Localize.Error.UserToUserGroupMappingNotFound);
                        
                        await _permissionValueEntityCollectionService.PurgeAsync(userToUserGroupMappingPreviousOwner.Id, cancellationToken);
                        
                        //Delete Authorize cache when group owner changes (for previous owner)
                        await _authorizeEntityService.PurgeByEntityIdAsync(userToUserGroupMappingPreviousOwner.Id, cancellationToken);
                        await _authorizeEntityService.PurgeByEntityIdAsync(userGroup.UserId, cancellationToken);
                        
                        //Delete Authorize cache when group owner changes (for new owner)
                        var userToUserGroupMappingNewOwner =
                            await _userToUserGroupMappingEntityService.GetByUserIdUserGroupIdAsync(userGroupTransferRequest.DestUserId, userGroup.Id);
                        if (userToUserGroupMappingNewOwner != null)
                            await _authorizeEntityService.PurgeByEntityIdAsync(userToUserGroupMappingNewOwner.Id, cancellationToken);
                        await _authorizeEntityService.PurgeByEntityIdAsync(userGroupTransferRequest.DestUserId, cancellationToken);
                        
                        userGroup.UserId = userGroupTransferRequest.DestUserId;
                        await _userGroupEntityService.Save(userGroup, cancellationToken);
                        await _userGroupTransferRequestEntityService.Delete(userGroupTransferRequest,
                            cancellationToken);
                        break;
                    }
                    case Choice.Reject:
                    {
                        await _userGroupTransferRequestEntityService.Delete(userGroupTransferRequest, cancellationToken);
                        break;
                    }
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            await _appDbContextAction.CommitTransactionAsync();

            _logger.Log(LogLevel.Information, Localize.Log.MethodEnd(GetType(), nameof(ManageTransfer)));

            return new OkResultDto();
        }
        catch (Exception)
        {
            await _appDbContextAction.RollbackTransactionAsync();

            throw;
        }
    }
    
    public async Task<IDtoResultBase> InitInviteUser(UserGroupInitInviteUserDto data, CancellationToken cancellationToken = default)
    {
        _logger.Log(LogLevel.Information, Localize.Log.MethodStart(GetType(), nameof(InitInviteUser)));

        if (ValidateModel(data) is { } validationResult)
            return validationResult;

        try
        {
            await _appDbContextAction.BeginTransactionAsync();
            
            var user = await _userAdvancedService.GetFromHttpContext(cancellationToken);
            if (user == null)
                throw new HttpResponseException(StatusCodes.Status400BadRequest, ErrorType.HttpContext,
                    Localize.Error.UserNotFoundOrHttpContextMissingClaims);

            var userGroup = await _userGroupEntityService.GetByIdAsync(data.UserGroupId, cancellationToken);
            if (userGroup == null)
                throw new HttpResponseException(StatusCodes.Status404NotFound, ErrorType.Generic,
                    Localize.Error.UserGroupNotFound);
            
            var userTarget = await _userEntityService.GetByIdAsync(data.UserId, cancellationToken);
            if (userTarget == null)
                throw new HttpResponseException(StatusCodes.Status404NotFound, ErrorType.HttpContext,
                    Localize.Error.UserNotFound);
            
            //Authorize g_group_a_inviteuser_o_usergroup against UserGroup TODO: + against User (user that initiates action, user that receives request)
            var authorizeResult = _authorizeAdvancedService.Authorize(new AuthorizeModel
            {
                EntityLeftTableName = _userRepository.GetTableName(),
                EntityLeftGroupsTableName = _userGroupRepository.GetTableName(),
                EntityLeftEntityToEntityMappingsTableName = _userToUserGroupMappingRepository.GetTableName(),
                EntityLeftId = user.Id,
                EntityLeftPermissionAlias = Consts.PermissionAlias.g_group_a_inviteuser_o_usergroup,
                EntityRightTableName = _userGroupRepository.GetTableName(),
                EntityRightGroupsTableName = null,
                EntityRightEntityToEntityMappingsTableName = null,
                EntityRightId = userGroup.Id,
                EntityRightPermissionAlias = Consts.PermissionAlias.g_group_a_inviteuser_o_usergroup,
                SqlExpressionPermissionTypeValueNeededOwner = "T1.\"Id\" = T2.\"UserId\""
            });
            
            //Authorize g_ingroup_a_inviteuser_o_usergroup via UserToUserGroupMapping against UserGroup (Specific User, Public, GroupMember)
            var userToUserGroupMappingUserSpecificGroupMemberPublic =
                await _userToUserGroupMappingEntityService.GetByUserIdUserGroupIdAsync(user.Id, userGroup.Id) ??
                await _userToUserGroupMappingEntityService.GetByUserIdUserGroupIdAsync(Consts.GroupMemberUserId, userGroup.Id) ??
                await _userToUserGroupMappingEntityService.GetByUserIdUserGroupIdAsync(Consts.PublicUserId, userGroup.Id);
            if (userToUserGroupMappingUserSpecificGroupMemberPublic != null)
            {
                authorizeResult |= _authorizeAdvancedService.Authorize(new AuthorizeModel
                {
                    EntityLeftTableName = _userToUserGroupMappingRepository.GetTableName(),
                    EntityLeftGroupsTableName = null,
                    EntityLeftEntityToEntityMappingsTableName = null,
                    EntityLeftId = userToUserGroupMappingUserSpecificGroupMemberPublic.Id,
                    EntityLeftPermissionAlias = Consts.PermissionAlias.g_ingroup_a_inviteuser_o_usergroup,
                    EntityRightTableName = _userGroupRepository.GetTableName(),
                    EntityRightGroupsTableName = null,
                    EntityRightEntityToEntityMappingsTableName = null,
                    EntityRightId = userGroup.Id,
                    EntityRightPermissionAlias = Consts.PermissionAlias.g_ingroup_a_inviteuser_o_usergroup,
                    SqlExpressionPermissionTypeValueNeededOwner = "T1.\"EntityLeftId\" = T2.\"UserId\""
                });
            }

            if (!authorizeResult)
                throw new HttpResponseException(StatusCodes.Status403Forbidden, ErrorType.Permission,
                    Localize.Error.PermissionInsufficientPermissions);
            
            if (user.Id == data.UserId)
                throw new HttpResponseException(StatusCodes.Status400BadRequest, ErrorType.Generic,
                    Localize.Error.UserGroupInviteRequestSameUserNotAllowed);
            
            if (userTarget.Id == Consts.PublicUserId || userTarget.Id == Consts.GroupMemberUserId)
                throw new HttpResponseException(StatusCodes.Status400BadRequest, ErrorType.Generic,
                    Localize.Error.UserToUserGroupMappingAddNotAllowed);
            
            //Do not allow to invite User associated with UserGroup
            if (await _userToUserGroupMappingEntityService.GetByUserIdUserGroupIdAsync(userTarget.Id, userGroup.Id) != null)
                throw new HttpResponseException(StatusCodes.Status400BadRequest, ErrorType.Generic,
                    Localize.Error.UserToUserGroupMappingAlreadyExists);
            
            var userGroupInviteRequest = await _userGroupInviteRequestEntityService.Save(new UserGroupInviteRequest
            {
                UserGroupId = userGroup.Id,
                SrcUserId = user.Id,
                DestUserId = userTarget.Id,
                ExpiresAt = default
            }, cancellationToken);
            
            await _appDbContextAction.CommitTransactionAsync();

            _logger.Log(LogLevel.Information, Localize.Log.MethodEnd(GetType(), nameof(InitInviteUser)));
            
            return _mapper.Map<UserGroupInitInviteUserResultDto>(userGroupInviteRequest);
        }
        catch (Exception)
        {
            await _appDbContextAction.RollbackTransactionAsync();

            throw;
        }
    }

    public async Task<IDtoResultBase> ManageInviteUser(UserGroupManageInviteUserDto data, CancellationToken cancellationToken = default)
    {
        _logger.Log(LogLevel.Information, Localize.Log.MethodStart(GetType(), nameof(ManageInviteUser)));

        if (ValidateModel(data) is { } validationResult)
            return validationResult;

        try
        {
            await _appDbContextAction.BeginTransactionAsync();

            var user = await _userAdvancedService.GetFromHttpContext(cancellationToken);
            if (user == null)
                throw new HttpResponseException(StatusCodes.Status400BadRequest, ErrorType.HttpContext,
                    Localize.Error.UserNotFoundOrHttpContextMissingClaims);
            
            var userGroupInviteRequest = await _userGroupInviteRequestEntityService.GetByIdAsync(data.Id, cancellationToken);
            if (userGroupInviteRequest == null)
                throw new HttpResponseException(StatusCodes.Status404NotFound, ErrorType.Generic,
                    Localize.Error.UserGroupInviteRequestNotFound);
                    
            var userGroup = await _userGroupEntityService.GetByIdAsync(userGroupInviteRequest.UserGroupId, cancellationToken);
            if (userGroup == null)
                throw new HttpResponseException(StatusCodes.Status404NotFound, ErrorType.Generic,
                    Localize.Error.UserGroupNotFound);
            
            //Action is taken by creator of UserGroupInviteRequest
            if (user.Id == userGroupInviteRequest.SrcUserId)
            {
                //Authorize g_group_a_inviteuser_o_usergroup_a_manage against UserGroup TODO: + against User (user that initiates action)
                var authorizeResult = _authorizeAdvancedService.Authorize(new AuthorizeModel
                {
                    EntityLeftTableName = _userRepository.GetTableName(),
                    EntityLeftGroupsTableName = _userGroupRepository.GetTableName(),
                    EntityLeftEntityToEntityMappingsTableName = _userToUserGroupMappingRepository.GetTableName(),
                    EntityLeftId = user.Id,
                    EntityLeftPermissionAlias = Consts.PermissionAlias.g_group_a_inviteuser_o_usergroup_a_manage,
                    EntityRightTableName = _userGroupRepository.GetTableName(),
                    EntityRightGroupsTableName = null,
                    EntityRightEntityToEntityMappingsTableName = null,
                    EntityRightId = userGroup.Id,
                    EntityRightPermissionAlias = Consts.PermissionAlias.g_group_a_inviteuser_o_usergroup_a_manage,
                    SqlExpressionPermissionTypeValueNeededOwner = "T1.\"Id\" = T2.\"UserId\""
                });

                if (!authorizeResult)
                    throw new HttpResponseException(StatusCodes.Status403Forbidden, ErrorType.Permission,
                        Localize.Error.PermissionInsufficientPermissions);
                
                switch (data.Choice)
                {
                    case Choice.None:
                        break;
                    case Choice.Accept:
                        throw new HttpResponseException(StatusCodes.Status400BadRequest, ErrorType.Generic,
                            Localize.Error.UserGroupInviteRequestSelectedChoiceNotAllowed);
                    case Choice.Reject:
                    {
                        await _userGroupInviteRequestEntityService.Delete(userGroupInviteRequest, cancellationToken);
                        break;
                    }
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            //Action is taken by receiver of UserGroupInviteRequest
            else if (user.Id == userGroupInviteRequest.DestUserId)
            {
                switch (data.Choice)
                {
                    case Choice.None:
                        break;
                    case Choice.Accept:
                    {
                        await _userToUserGroupMappingEntityService.Save(new UserToUserGroupMapping
                        {
                            EntityLeftId = userGroupInviteRequest.DestUserId,
                            EntityRightId = userGroup.Id,
                        }, cancellationToken);
                        
                        //Delete Authorize cache when joining group (for the user that joined)
                        await _authorizeEntityService.PurgeByEntityIdAsync(userGroupInviteRequest.DestUserId, cancellationToken);
                        
                        await _userGroupInviteRequestEntityService.Delete(userGroupInviteRequest, cancellationToken);
                        break;
                    }
                    case Choice.Reject:
                    {
                        await _userGroupInviteRequestEntityService.Delete(userGroupInviteRequest, cancellationToken);
                        break;
                    }
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            //Action is taken by 3rd party
            else
            {
                //Authorize g_group_a_inviteuser_o_usergroup_a_manage against UserGroup TODO: + against User (user that initiates action)
                var authorizeResult = _authorizeAdvancedService.Authorize(new AuthorizeModel
                {
                    EntityLeftTableName = _userRepository.GetTableName(),
                    EntityLeftGroupsTableName = _userGroupRepository.GetTableName(),
                    EntityLeftEntityToEntityMappingsTableName = _userToUserGroupMappingRepository.GetTableName(),
                    EntityLeftId = user.Id,
                    EntityLeftPermissionAlias = Consts.PermissionAlias.g_group_a_inviteuser_o_usergroup_a_manage,
                    EntityRightTableName = _userGroupRepository.GetTableName(),
                    EntityRightGroupsTableName = null,
                    EntityRightEntityToEntityMappingsTableName = null,
                    EntityRightId = userGroup.Id,
                    EntityRightPermissionAlias = Consts.PermissionAlias.g_group_a_inviteuser_o_usergroup_a_manage,
                    SqlExpressionPermissionTypeValueNeededOwner = "T1.\"Id\" = T2.\"UserId\""
                });

                switch (data.Choice)
                {
                    case Choice.None:
                        break;
                    case Choice.Accept:
                    {
                        if (!authorizeResult)
                            throw new HttpResponseException(StatusCodes.Status403Forbidden, ErrorType.Permission,
                                Localize.Error.PermissionInsufficientPermissions);
                        
                        await _userToUserGroupMappingEntityService.Save(new UserToUserGroupMapping
                        {
                            EntityLeftId = userGroupInviteRequest.DestUserId,
                            EntityRightId = userGroup.Id,
                        }, cancellationToken);
                        
                        //Delete Authorize cache when joining group (for the user that joined)
                        await _authorizeEntityService.PurgeByEntityIdAsync(userGroupInviteRequest.DestUserId, cancellationToken);
                        
                        await _userGroupInviteRequestEntityService.Delete(userGroupInviteRequest, cancellationToken);
                        break;
                    }
                    case Choice.Reject:
                    {
                        //Authorize g_ingroup_a_inviteuser_o_usergroup_a_manage via UserToUserGroupMapping against UserGroup (Specific User, Public, GroupMember)
                        var userToUserGroupMappingUserSpecificGroupMemberPublic =
                            await _userToUserGroupMappingEntityService.GetByUserIdUserGroupIdAsync(user.Id, userGroup.Id) ??
                            await _userToUserGroupMappingEntityService.GetByUserIdUserGroupIdAsync(Consts.GroupMemberUserId, userGroup.Id) ??
                            await _userToUserGroupMappingEntityService.GetByUserIdUserGroupIdAsync(Consts.PublicUserId, userGroup.Id);
                        if (userToUserGroupMappingUserSpecificGroupMemberPublic != null)
                        {
                            authorizeResult |= _authorizeAdvancedService.Authorize(new AuthorizeModel
                            {
                                EntityLeftTableName = _userToUserGroupMappingRepository.GetTableName(),
                                EntityLeftGroupsTableName = null,
                                EntityLeftEntityToEntityMappingsTableName = null,
                                EntityLeftId = userToUserGroupMappingUserSpecificGroupMemberPublic.Id,
                                EntityLeftPermissionAlias = Consts.PermissionAlias.g_ingroup_a_inviteuser_o_usergroup_a_manage,
                                EntityRightTableName = _userGroupRepository.GetTableName(),
                                EntityRightGroupsTableName = null,
                                EntityRightEntityToEntityMappingsTableName = null,
                                EntityRightId = userGroup.Id,
                                EntityRightPermissionAlias = Consts.PermissionAlias.g_ingroup_a_inviteuser_o_usergroup_a_manage,
                                SqlExpressionPermissionTypeValueNeededOwner = "T1.\"EntityLeftId\" = T2.\"UserId\""
                            });
                        }
                
                        if (!authorizeResult)
                            throw new HttpResponseException(StatusCodes.Status403Forbidden, ErrorType.Permission,
                                Localize.Error.PermissionInsufficientPermissions);
                        
                        await _userGroupInviteRequestEntityService.Delete(userGroupInviteRequest, cancellationToken);
                        break;
                    }
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            
            await _appDbContextAction.CommitTransactionAsync();

            _logger.Log(LogLevel.Information, Localize.Log.MethodEnd(GetType(), nameof(ManageInviteUser)));

            return new OkResultDto();
        }
        catch (Exception)
        {
            await _appDbContextAction.RollbackTransactionAsync();

            throw;
        }
    }
    
    public async Task<IDtoResultBase> AddUser(UserGroupAddUserDto data, CancellationToken cancellationToken = default)
    {
        _logger.Log(LogLevel.Information, Localize.Log.MethodStart(GetType(), nameof(AddUser)));

        if (ValidateModel(data) is { } validationResult)
            return validationResult;

        try
        {
            await _appDbContextAction.BeginTransactionAsync();
            
            var user = await _userAdvancedService.GetFromHttpContext(cancellationToken);
            if (user == null)
                throw new HttpResponseException(StatusCodes.Status400BadRequest, ErrorType.HttpContext,
                    Localize.Error.UserNotFoundOrHttpContextMissingClaims);

            var userGroup = await _userGroupEntityService.GetByIdAsync(data.Id, cancellationToken);
            if (userGroup == null)
                throw new HttpResponseException(StatusCodes.Status404NotFound, ErrorType.Generic,
                    Localize.Error.UserGroupNotFound);
            
            var userTarget = await _userAdvancedService.GetFromHttpContext(cancellationToken);
            if (userTarget == null)
                throw new HttpResponseException(StatusCodes.Status404NotFound, ErrorType.Generic,
                    Localize.Error.UserNotFound);
            
            //Authorize g_group_a_adduser_o_usergroup to UserGroup TODO: + against User (user that initiates action, user that is being targeted)
            var authorizeResult = _authorizeAdvancedService.Authorize(new AuthorizeModel
            {
                EntityLeftTableName = _userRepository.GetTableName(),
                EntityLeftGroupsTableName = _userGroupRepository.GetTableName(),
                EntityLeftEntityToEntityMappingsTableName = _userToUserGroupMappingRepository.GetTableName(),
                EntityLeftId = user.Id,
                EntityLeftPermissionAlias = Consts.PermissionAlias.g_group_a_adduser_o_usergroup,
                EntityRightTableName = _userGroupRepository.GetTableName(),
                EntityRightGroupsTableName = null,
                EntityRightEntityToEntityMappingsTableName = null,
                EntityRightId = userGroup.Id,
                EntityRightPermissionAlias = Consts.PermissionAlias.g_group_a_adduser_o_usergroup,
                SqlExpressionPermissionTypeValueNeededOwner = "T1.\"Id\" = T2.\"UserId\""
            });
            
            if (!authorizeResult)
                throw new HttpResponseException(StatusCodes.Status403Forbidden, ErrorType.Permission,
                    Localize.Error.PermissionInsufficientPermissions);
            
            if (userTarget.Id == Consts.PublicUserId || userTarget.Id == Consts.GroupMemberUserId)
                throw new HttpResponseException(StatusCodes.Status400BadRequest, ErrorType.Generic,
                    Localize.Error.UserToUserGroupMappingAddNotAllowed);
            
            //Do not allow to add User associated with UserGroup
            if (await _userToUserGroupMappingEntityService.GetByUserIdUserGroupIdAsync(userTarget.Id, userGroup.Id) != null)
                throw new HttpResponseException(StatusCodes.Status400BadRequest, ErrorType.Generic,
                    Localize.Error.UserToUserGroupMappingAlreadyExists);
            
            await _userToUserGroupMappingEntityService.Save(new UserToUserGroupMapping
            {
                EntityLeftId = userTarget.Id,
                EntityRightId = userGroup.Id
            }, cancellationToken);
            
            //Delete Authorize cache when joining group (for the user that joined)
            await _authorizeEntityService.PurgeByEntityIdAsync(user.Id, cancellationToken);

            await _appDbContextAction.CommitTransactionAsync();

            _logger.Log(LogLevel.Information, Localize.Log.MethodEnd(GetType(), nameof(AddUser)));

            return new OkResultDto();
        }
        catch (Exception)
        {
            await _appDbContextAction.RollbackTransactionAsync();

            throw;
        }
    }
    
    public async Task<IDtoResultBase> KickUser(UserGroupKickUserDto data, CancellationToken cancellationToken = default)
    {
        _logger.Log(LogLevel.Information, Localize.Log.MethodStart(GetType(), nameof(KickUser)));

        if (ValidateModel(data) is { } validationResult)
            return validationResult;

        try
        {
            await _appDbContextAction.BeginTransactionAsync();

            var user = await _userAdvancedService.GetFromHttpContext(cancellationToken);
            if (user == null)
                throw new HttpResponseException(StatusCodes.Status400BadRequest, ErrorType.HttpContext,
                    Localize.Error.UserNotFoundOrHttpContextMissingClaims);

            var userGroup = await _userGroupEntityService.GetByIdAsync(data.Id, cancellationToken);
            if (userGroup == null)
                throw new HttpResponseException(StatusCodes.Status404NotFound, ErrorType.Generic,
                    Localize.Error.UserGroupNotFound);
            
            var userTarget = await _userAdvancedService.GetFromHttpContext(cancellationToken);
            if (userTarget == null)
                throw new HttpResponseException(StatusCodes.Status404NotFound, ErrorType.Generic,
                    Localize.Error.UserNotFound);

            //Authorize g_group_a_kickuser_o_usergroup against UserGroup TODO: + against User (user that initiates action, user that is being targeted)
            var authorizeResult = _authorizeAdvancedService.Authorize(new AuthorizeModel
            {
                EntityLeftTableName = _userRepository.GetTableName(),
                EntityLeftGroupsTableName = _userGroupRepository.GetTableName(),
                EntityLeftEntityToEntityMappingsTableName = _userToUserGroupMappingRepository.GetTableName(),
                EntityLeftId = user.Id,
                EntityLeftPermissionAlias = Consts.PermissionAlias.g_group_a_kickuser_o_usergroup,
                EntityRightTableName = _userGroupRepository.GetTableName(),
                EntityRightGroupsTableName = null,
                EntityRightEntityToEntityMappingsTableName = null,
                EntityRightId = userGroup.Id,
                EntityRightPermissionAlias = Consts.PermissionAlias.g_group_a_kickuser_o_usergroup,
                SqlExpressionPermissionTypeValueNeededOwner = "T1.\"Id\" = T2.\"UserId\""
            });
            
            //Authorize g_ingroup_a_kickuser_o_usergroup via UserToUserGroupMapping against UserGroup (Specific User, Public, GroupMember)
            var userToUserGroupMappingUserSpecificGroupMemberPublic =
                await _userToUserGroupMappingEntityService.GetByUserIdUserGroupIdAsync(user.Id, userGroup.Id) ??
                await _userToUserGroupMappingEntityService.GetByUserIdUserGroupIdAsync(Consts.GroupMemberUserId, userGroup.Id) ??
                await _userToUserGroupMappingEntityService.GetByUserIdUserGroupIdAsync(Consts.PublicUserId, userGroup.Id);
            if (userToUserGroupMappingUserSpecificGroupMemberPublic != null)
            {
                authorizeResult |= _authorizeAdvancedService.Authorize(new AuthorizeModel
                {
                    EntityLeftTableName = _userToUserGroupMappingRepository.GetTableName(),
                    EntityLeftGroupsTableName = null,
                    EntityLeftEntityToEntityMappingsTableName = null,
                    EntityLeftId = userToUserGroupMappingUserSpecificGroupMemberPublic.Id,
                    EntityLeftPermissionAlias = Consts.PermissionAlias.g_ingroup_a_kickuser_o_usergroup,
                    EntityRightTableName = _userGroupRepository.GetTableName(),
                    EntityRightGroupsTableName = null,
                    EntityRightEntityToEntityMappingsTableName = null,
                    EntityRightId = userGroup.Id,
                    EntityRightPermissionAlias = Consts.PermissionAlias.g_ingroup_a_kickuser_o_usergroup,
                    SqlExpressionPermissionTypeValueNeededOwner = "T1.\"EntityIdLeft\" = T2.\"UserId\""
                });
            }
            
            if (!authorizeResult)
                throw new HttpResponseException(StatusCodes.Status403Forbidden, ErrorType.Permission,
                    Localize.Error.PermissionInsufficientPermissions);
            
            if (userTarget.Id == Consts.PublicUserId || userTarget.Id == Consts.GroupMemberUserId)
                throw new HttpResponseException(StatusCodes.Status400BadRequest, ErrorType.Generic,
                    Localize.Error.UserToUserGroupMappingDeleteNotAllowed);
            
            var userToUserGroupMappingUserTarget =
                await _userToUserGroupMappingEntityService.GetByUserIdUserGroupIdAsync(userTarget.Id, userGroup.Id);
            if (userToUserGroupMappingUserTarget == null)
                throw new HttpResponseException(StatusCodes.Status404NotFound, ErrorType.Generic,
                    Localize.Error.UserToUserGroupMappingNotFound);
            
            await _permissionValueEntityCollectionService.PurgeAsync(userToUserGroupMappingUserTarget.Id, cancellationToken);
            
            //Delete Authorize cache when leaving group (for the user that left)
            await _authorizeEntityService.PurgeByEntityIdAsync(userTarget.Id, cancellationToken);
            await _authorizeEntityService.PurgeByEntityIdAsync(userToUserGroupMappingUserTarget.Id, cancellationToken);

            await _userToUserGroupMappingEntityService.Delete(userToUserGroupMappingUserTarget, cancellationToken);
            
            await _appDbContextAction.CommitTransactionAsync();

            _logger.Log(LogLevel.Information, Localize.Log.MethodEnd(GetType(), nameof(KickUser)));

            return new OkResultDto();
        }
        catch (Exception)
        {
            await _appDbContextAction.RollbackTransactionAsync();

            throw;
        }
    }

    #endregion
}