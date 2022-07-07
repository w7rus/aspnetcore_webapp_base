using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using BLL.Handlers.Base;
using BLL.Services.Advanced;
using BLL.Services.Entity;
using Common.Enums;
using Common.Exceptions;
using Common.Helpers;
using Common.Models;
using Common.Models.Base;
using DAL.Data;
using DAL.Repository;
using Domain.Entities;
using Domain.Enums;
using DTO.Models.Generic;
using DTO.Models.UserGroup;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace BLL.Handlers;

public interface IUserGroupActionsHandler
{
    Task<IDtoResultBase> UserGroupJoinUser(UserGroupJoinUserDto data, CancellationToken cancellationToken = default);
    Task<IDtoResultBase> UserGroupLeaveUser(UserGroupLeaveUserDto data, CancellationToken cancellationToken = default);

    Task<IDtoResultBase> UserGroupTransferRequestCreate(
        UserGroupTransferRequestCreateDto data,
        CancellationToken cancellationToken = default
    );

    Task<IDtoResultBase> UserGroupTransferRequestUpdate(
        UserGroupTransferRequestUpdateDto data,
        CancellationToken cancellationToken = default
    );

    Task<IDtoResultBase> UserGroupTransferRequestRead(
        UserGroupTransferRequestReadDto data,
        CancellationToken cancellationToken = default
    );

    Task<IDtoResultBase> UserGroupTransferRequestReceiverReadCollection(
        UserGroupTransferRequestReadCollectionDto data,
        CancellationToken cancellationToken = default
    );

    Task<IDtoResultBase> UserGroupTransferRequestSenderReadCollection(
        UserGroupTransferRequestReadCollectionDto data,
        CancellationToken cancellationToken = default
    );

    Task<IDtoResultBase> UserGroupInviteRequestCreate(
        UserGroupInviteRequestCreateDto data,
        CancellationToken cancellationToken = default
    );

    Task<IDtoResultBase> UserGroupInviteRequestUpdate(
        UserGroupInviteRequestUpdateDto data,
        CancellationToken cancellationToken = default
    );

    Task<IDtoResultBase> UserGroupInviteRequestRead(
        UserGroupInviteRequestReadDto data,
        CancellationToken cancellationToken = default
    );

    Task<IDtoResultBase> UserGroupInviteRequestReceiverReadCollection(
        UserGroupInviteRequestReceiverReadCollectionDto data,
        CancellationToken cancellationToken = default
    );

    Task<IDtoResultBase> UserGroupInviteRequestSenderReadCollection(
        UserGroupInviteRequestSenderReadCollectionDto data,
        CancellationToken cancellationToken = default
    );

    Task<IDtoResultBase> UserGroupAddUser(UserGroupAddUserDto data, CancellationToken cancellationToken = default);

    Task<IDtoResultBase> UserGroupDeleteUser(
        UserGroupDeleteUserDto data,
        CancellationToken cancellationToken = default
    );
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
    private readonly IPermissionValueEntityService _permissionValueEntityService;
    private readonly IPermissionEntityService _permissionEntityService;
    private readonly IUserGroupTransferRequestEntityService _userGroupTransferRequestEntityService;
    private readonly IUserGroupInviteRequestEntityService _userGroupInviteRequestEntityService;
    private readonly IAuthorizeEntityService _authorizeEntityService;

    private static readonly string UserGroupEntityDiscriminatorHash = typeof(UserGroup).EntityAqnHash();

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
        IPermissionValueEntityService permissionValueEntityService,
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
        _permissionValueEntityService = permissionValueEntityService;
        _permissionEntityService = permissionEntityService;
        _userGroupTransferRequestEntityService = userGroupTransferRequestEntityService;
        _userGroupInviteRequestEntityService = userGroupInviteRequestEntityService;
        _authorizeEntityService = authorizeEntityService;
    }

    #endregion

    #region Methods

    public async Task<IDtoResultBase> UserGroupJoinUser(
        UserGroupJoinUserDto data,
        CancellationToken cancellationToken = default
    )
    {
        _logger.Log(LogLevel.Information, Localize.Log.MethodStart(GetType(), nameof(UserGroupJoinUser)));

        if (ValidateModel(data) is { } validationResult)
            return validationResult;

        try
        {
            await _appDbContextAction.BeginTransactionAsync();

            var user = await _userAdvancedService.GetFromHttpContext(cancellationToken: cancellationToken);
            if (user == null)
                throw new HttpResponseException(StatusCodes.Status400BadRequest, ErrorType.HttpContext,
                    Localize.Error.UserNotFoundOrHttpContextMissingClaims);

            var userGroup = await _userGroupEntityService.GetByIdAsync(data.UserGroupId, cancellationToken);
            if (userGroup == null)
                throw new HttpResponseException(StatusCodes.Status404NotFound, ErrorType.Generic,
                    Localize.Error.UserGroupNotFound);

            //Authorize against UserGroup TODO: + against User (user that initiates action)
            var authorizeResult = _authorizeAdvancedService.Authorize(new AuthorizeModel
            {
                EntityLeftTableName = _userRepository.GetTableName(),
                EntityLeftGroupsTableName = _userGroupRepository.GetTableName(),
                EntityLeftEntityToEntityMappingsTableName = _userToUserGroupMappingRepository.GetTableName(),
                EntityLeftId = user.Id,
                EntityLeftPermissionAlias = Consts.PermissionAlias.UserGroupJoin,
                EntityRightTableName = _userGroupRepository.GetTableName(),
                EntityRightGroupsTableName = null,
                EntityRightEntityToEntityMappingsTableName = null,
                EntityRightId = userGroup.Id,
                EntityRightPermissionAlias = Consts.PermissionAlias.UserGroupJoin,
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

            _logger.Log(LogLevel.Information, Localize.Log.MethodEnd(GetType(), nameof(UserGroupJoinUser)));

            return new OkResultDto();
        }
        catch (Exception)
        {
            await _appDbContextAction.RollbackTransactionAsync();

            throw;
        }
    }

    public async Task<IDtoResultBase> UserGroupLeaveUser(
        UserGroupLeaveUserDto data,
        CancellationToken cancellationToken = default
    )
    {
        _logger.Log(LogLevel.Information, Localize.Log.MethodStart(GetType(), nameof(UserGroupLeaveUser)));

        if (ValidateModel(data) is { } validationResult)
            return validationResult;

        try
        {
            await _appDbContextAction.BeginTransactionAsync();

            var user = await _userAdvancedService.GetFromHttpContext(cancellationToken: cancellationToken);
            if (user == null)
                throw new HttpResponseException(StatusCodes.Status400BadRequest, ErrorType.HttpContext,
                    Localize.Error.UserNotFoundOrHttpContextMissingClaims);

            var userGroup = await _userGroupEntityService.GetByIdAsync(data.UserGroupId, cancellationToken);
            if (userGroup == null)
                throw new HttpResponseException(StatusCodes.Status404NotFound, ErrorType.Generic,
                    Localize.Error.UserGroupNotFound);

            //Authorize against UserGroup TODO: + against User (user that initiates action)
            var authorizeResult = _authorizeAdvancedService.Authorize(new AuthorizeModel
            {
                EntityLeftTableName = _userRepository.GetTableName(),
                EntityLeftGroupsTableName = _userGroupRepository.GetTableName(),
                EntityLeftEntityToEntityMappingsTableName = _userToUserGroupMappingRepository.GetTableName(),
                EntityLeftId = user.Id,
                EntityLeftPermissionAlias = Consts.PermissionAlias.UserGroupLeave,
                EntityRightTableName = _userGroupRepository.GetTableName(),
                EntityRightGroupsTableName = null,
                EntityRightEntityToEntityMappingsTableName = null,
                EntityRightId = userGroup.Id,
                EntityRightPermissionAlias = Consts.PermissionAlias.UserGroupLeave,
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

            await _permissionValueEntityService.PurgeAsync(userToUserGroupMapping.Id, cancellationToken);

            //Delete Authorize cache when leaving group (for the user that left)
            await _authorizeEntityService.PurgeByEntityIdAsync(user.Id, cancellationToken);
            await _authorizeEntityService.PurgeByEntityIdAsync(userToUserGroupMapping.Id, cancellationToken);

            await _userToUserGroupMappingEntityService.Delete(userToUserGroupMapping, cancellationToken);

            await _appDbContextAction.CommitTransactionAsync();

            _logger.Log(LogLevel.Information, Localize.Log.MethodEnd(GetType(), nameof(UserGroupLeaveUser)));

            return new OkResultDto();
        }
        catch (Exception)
        {
            await _appDbContextAction.RollbackTransactionAsync();

            throw;
        }
    }

    public async Task<IDtoResultBase> UserGroupTransferRequestCreate(
        UserGroupTransferRequestCreateDto data,
        CancellationToken cancellationToken = default
    )
    {
        _logger.Log(LogLevel.Information, Localize.Log.MethodStart(GetType(), nameof(UserGroupTransferRequestCreate)));

        if (ValidateModel(data) is { } validationResult)
            return validationResult;

        try
        {
            await _appDbContextAction.BeginTransactionAsync();

            var user = await _userAdvancedService.GetFromHttpContext(cancellationToken: cancellationToken);
            if (user == null)
                throw new HttpResponseException(StatusCodes.Status400BadRequest, ErrorType.HttpContext,
                    Localize.Error.UserNotFoundOrHttpContextMissingClaims);

            var userGroup = await _userGroupEntityService.GetByIdAsync(data.UserGroupId, cancellationToken);
            if (userGroup == null)
                throw new HttpResponseException(StatusCodes.Status404NotFound, ErrorType.Generic,
                    Localize.Error.UserGroupNotFound);

            var userTarget = await _userEntityService.GetByIdAsync(data.TargetUserId, cancellationToken);
            if (userTarget == null)
                throw new HttpResponseException(StatusCodes.Status404NotFound, ErrorType.HttpContext,
                    Localize.Error.UserNotFound);

            //Authorize against UserGroup TODO: + against User (user that initiates action, user that receives request)
            var authorizeResult = _authorizeAdvancedService.Authorize(new AuthorizeModel
            {
                EntityLeftTableName = _userRepository.GetTableName(),
                EntityLeftGroupsTableName = _userGroupRepository.GetTableName(),
                EntityLeftEntityToEntityMappingsTableName = _userToUserGroupMappingRepository.GetTableName(),
                EntityLeftId = user.Id,
                EntityLeftPermissionAlias = Consts.PermissionAlias.UserGroupTransferRequestCreate,
                EntityRightTableName = _userGroupRepository.GetTableName(),
                EntityRightGroupsTableName = null,
                EntityRightEntityToEntityMappingsTableName = null,
                EntityRightId = userGroup.Id,
                EntityRightPermissionAlias = Consts.PermissionAlias.UserGroupTransferRequestCreate,
                SqlExpressionPermissionTypeValueNeededOwner = "T1.\"Id\" = T2.\"UserId\""
            });

            if (!authorizeResult)
                throw new HttpResponseException(StatusCodes.Status403Forbidden, ErrorType.Permission,
                    Localize.Error.PermissionInsufficientPermissions);

            if (user.Id == data.TargetUserId)
                throw new HttpResponseException(StatusCodes.Status400BadRequest, ErrorType.Generic,
                    Localize.Error.UserGroupTransferSameUserNotAllowed);

            var userGroupTransferRequest = _mapper.Map<UserGroupTransferRequest>(data);

            userGroupTransferRequest.SrcUserId = user.Id;

            await _userGroupTransferRequestEntityService.Save(userGroupTransferRequest, cancellationToken);

            await _appDbContextAction.CommitTransactionAsync();

            _logger.Log(LogLevel.Information,
                Localize.Log.MethodEnd(GetType(), nameof(UserGroupTransferRequestCreate)));

            return _mapper.Map<UserGroupTransferRequestCreateResultDto>(userGroupTransferRequest);
        }
        catch (Exception)
        {
            await _appDbContextAction.RollbackTransactionAsync();

            throw;
        }
    }

    public async Task<IDtoResultBase> UserGroupTransferRequestUpdate(
        UserGroupTransferRequestUpdateDto data,
        CancellationToken cancellationToken = default
    )
    {
        _logger.Log(LogLevel.Information, Localize.Log.MethodStart(GetType(), nameof(UserGroupTransferRequestUpdate)));

        if (ValidateModel(data) is { } validationResult)
            return validationResult;

        try
        {
            await _appDbContextAction.BeginTransactionAsync();

            var user = await _userAdvancedService.GetFromHttpContext(cancellationToken: cancellationToken);
            if (user == null)
                throw new HttpResponseException(StatusCodes.Status400BadRequest, ErrorType.HttpContext,
                    Localize.Error.UserNotFoundOrHttpContextMissingClaims);

            var userGroupTransferRequest =
                await _userGroupTransferRequestEntityService.GetByIdAsync(data.UserGroupTransferRequestId, cancellationToken);
            if (userGroupTransferRequest == null)
                throw new HttpResponseException(StatusCodes.Status404NotFound, ErrorType.Generic,
                    Localize.Error.UserGroupTransferRequestNotFound);

            var userGroup =
                await _userGroupEntityService.GetByIdAsync(userGroupTransferRequest.UserGroupId, cancellationToken);
            if (userGroup == null)
                throw new HttpResponseException(StatusCodes.Status404NotFound, ErrorType.Generic,
                    Localize.Error.UserGroupNotFound);

            //Action is taken by creator of UserGroupTransferRequest
            if (user.Id == userGroupTransferRequest.SrcUserId)
            {
                //Authorize against UserGroup TODO: + against User (user that initiates action)
                var authorizeResult = _authorizeAdvancedService.Authorize(new AuthorizeModel
                {
                    EntityLeftTableName = _userRepository.GetTableName(),
                    EntityLeftGroupsTableName = _userGroupRepository.GetTableName(),
                    EntityLeftEntityToEntityMappingsTableName = _userToUserGroupMappingRepository.GetTableName(),
                    EntityLeftId = user.Id,
                    EntityLeftPermissionAlias = Consts.PermissionAlias.UserGroupTransferRequestUpdate,
                    EntityRightTableName = _userGroupRepository.GetTableName(),
                    EntityRightGroupsTableName = null,
                    EntityRightEntityToEntityMappingsTableName = null,
                    EntityRightId = userGroup.Id,
                    EntityRightPermissionAlias = Consts.PermissionAlias.UserGroupTransferRequestUpdate,
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
                        await _userGroupTransferRequestEntityService.Delete(userGroupTransferRequest,
                            cancellationToken);
                        break;
                    }
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            //Action is taken by receiver of UserGroupTransferRequest
            else if (user.Id == userGroupTransferRequest.DestUserId)
            {
                //TODO: + against User (user that receives request)
                switch (data.Choice)
                {
                    case Choice.None:
                        break;
                    case Choice.Accept:
                    {
                        var userToUserGroupMappingPreviousOwner =
                            await _userToUserGroupMappingEntityService.GetByUserIdUserGroupIdAsync(userGroup.UserId,
                                userGroup.Id);
                        if (userToUserGroupMappingPreviousOwner == null)
                            throw new HttpResponseException(StatusCodes.Status404NotFound, ErrorType.Generic,
                                Localize.Error.UserToUserGroupMappingNotFound);

                        await _permissionValueEntityService.PurgeAsync(userToUserGroupMappingPreviousOwner.Id,
                            cancellationToken);

                        //Delete Authorize cache when group owner changes (for previous owner)
                        await _authorizeEntityService.PurgeByEntityIdAsync(userToUserGroupMappingPreviousOwner.Id,
                            cancellationToken);
                        await _authorizeEntityService.PurgeByEntityIdAsync(userGroup.UserId, cancellationToken);

                        //Delete Authorize cache when group owner changes (for new owner)
                        var userToUserGroupMappingNewOwner =
                            await _userToUserGroupMappingEntityService.GetByUserIdUserGroupIdAsync(
                                userGroupTransferRequest.DestUserId, userGroup.Id);
                        if (userToUserGroupMappingNewOwner != null)
                            await _authorizeEntityService.PurgeByEntityIdAsync(userToUserGroupMappingNewOwner.Id,
                                cancellationToken);
                        await _authorizeEntityService.PurgeByEntityIdAsync(userGroupTransferRequest.DestUserId,
                            cancellationToken);

                        userGroup.UserId = userGroupTransferRequest.DestUserId;
                        await _userGroupEntityService.Save(userGroup, cancellationToken);
                        await _userGroupTransferRequestEntityService.Delete(userGroupTransferRequest,
                            cancellationToken);
                        break;
                    }
                    case Choice.Reject:
                    {
                        await _userGroupTransferRequestEntityService.Delete(userGroupTransferRequest,
                            cancellationToken);
                        break;
                    }
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            //Action is taken by 3rd party
            else
            {
                //Authorize against UserGroup TODO: + against User (user that initiates action)
                var authorizeResult = _authorizeAdvancedService.Authorize(new AuthorizeModel
                {
                    EntityLeftTableName = _userRepository.GetTableName(),
                    EntityLeftGroupsTableName = _userGroupRepository.GetTableName(),
                    EntityLeftEntityToEntityMappingsTableName = _userToUserGroupMappingRepository.GetTableName(),
                    EntityLeftId = user.Id,
                    EntityLeftPermissionAlias = Consts.PermissionAlias.UserGroupTransferRequestUpdate,
                    EntityRightTableName = _userGroupRepository.GetTableName(),
                    EntityRightGroupsTableName = null,
                    EntityRightEntityToEntityMappingsTableName = null,
                    EntityRightId = userGroup.Id,
                    EntityRightPermissionAlias = Consts.PermissionAlias.UserGroupTransferRequestUpdate,
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
                            await _userToUserGroupMappingEntityService.GetByUserIdUserGroupIdAsync(userGroup.UserId,
                                userGroup.Id);
                        if (userToUserGroupMappingPreviousOwner == null)
                            throw new HttpResponseException(StatusCodes.Status404NotFound, ErrorType.Generic,
                                Localize.Error.UserToUserGroupMappingNotFound);

                        await _permissionValueEntityService.PurgeAsync(userToUserGroupMappingPreviousOwner.Id,
                            cancellationToken);

                        //Delete Authorize cache when group owner changes (for previous owner)
                        await _authorizeEntityService.PurgeByEntityIdAsync(userToUserGroupMappingPreviousOwner.Id,
                            cancellationToken);
                        await _authorizeEntityService.PurgeByEntityIdAsync(userGroup.UserId, cancellationToken);

                        //Delete Authorize cache when group owner changes (for new owner)
                        var userToUserGroupMappingNewOwner =
                            await _userToUserGroupMappingEntityService.GetByUserIdUserGroupIdAsync(
                                userGroupTransferRequest.DestUserId, userGroup.Id);
                        if (userToUserGroupMappingNewOwner != null)
                            await _authorizeEntityService.PurgeByEntityIdAsync(userToUserGroupMappingNewOwner.Id,
                                cancellationToken);
                        await _authorizeEntityService.PurgeByEntityIdAsync(userGroupTransferRequest.DestUserId,
                            cancellationToken);

                        userGroup.UserId = userGroupTransferRequest.DestUserId;
                        await _userGroupEntityService.Save(userGroup, cancellationToken);
                        await _userGroupTransferRequestEntityService.Delete(userGroupTransferRequest,
                            cancellationToken);
                        break;
                    }
                    case Choice.Reject:
                    {
                        await _userGroupTransferRequestEntityService.Delete(userGroupTransferRequest,
                            cancellationToken);
                        break;
                    }
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            await _appDbContextAction.CommitTransactionAsync();

            _logger.Log(LogLevel.Information,
                Localize.Log.MethodEnd(GetType(), nameof(UserGroupTransferRequestUpdate)));

            return new OkResultDto();
        }
        catch (Exception)
        {
            await _appDbContextAction.RollbackTransactionAsync();

            throw;
        }
    }

    public async Task<IDtoResultBase> UserGroupTransferRequestRead(
        UserGroupTransferRequestReadDto data,
        CancellationToken cancellationToken = default
    )
    {
        _logger.Log(LogLevel.Information, Localize.Log.MethodStart(GetType(), nameof(UserGroupTransferRequestRead)));

        if (ValidateModel(data) is { } validationResult)
            return validationResult;

        try
        {
            await _appDbContextAction.BeginTransactionAsync();

            //User read transfer request if (Owner - [DestUser, SrcUser] | Others)

            var user = await _userAdvancedService.GetFromHttpContext(cancellationToken: cancellationToken);
            if (user == null)
                throw new HttpResponseException(StatusCodes.Status400BadRequest, ErrorType.HttpContext,
                    Localize.Error.UserNotFoundOrHttpContextMissingClaims);

            var userGroupTransferRequest =
                await _userGroupTransferRequestEntityService.GetByIdAsync(data.UserGroupTransferRequestId, cancellationToken);
            if (userGroupTransferRequest == null)
                throw new HttpResponseException(StatusCodes.Status404NotFound, ErrorType.Generic,
                    Localize.Error.UserGroupTransferRequestNotFound);

            var userGroup =
                await _userGroupEntityService.GetByIdAsync(userGroupTransferRequest.UserGroupId, cancellationToken);
            if (userGroup == null)
                throw new HttpResponseException(StatusCodes.Status404NotFound, ErrorType.Generic,
                    Localize.Error.UserGroupNotFound);

            //Action is taken by receiver of UserGroupTransferRequest
            if (user.Id == userGroupTransferRequest.DestUserId)
            {
                //TODO: + against User (user that receives request)
            }
            //Action is taken by creator of UserGroupTransferRequest or by 3rd party
            else
            {
                //Authorize against UserGroup TODO: + against User (user that initiates action)
                var authorizeResult = _authorizeAdvancedService.Authorize(new AuthorizeModel
                {
                    EntityLeftTableName = _userRepository.GetTableName(),
                    EntityLeftGroupsTableName = _userGroupRepository.GetTableName(),
                    EntityLeftEntityToEntityMappingsTableName = _userToUserGroupMappingRepository.GetTableName(),
                    EntityLeftId = user.Id,
                    EntityLeftPermissionAlias = Consts.PermissionAlias.UserGroupTransferRequestRead,
                    EntityRightTableName = _userGroupRepository.GetTableName(),
                    EntityRightGroupsTableName = null,
                    EntityRightEntityToEntityMappingsTableName = null,
                    EntityRightId = userGroup.Id,
                    EntityRightPermissionAlias = Consts.PermissionAlias.UserGroupTransferRequestRead,
                    SqlExpressionPermissionTypeValueNeededOwner = "T1.\"Id\" = T2.\"UserId\""
                });

                if (!authorizeResult)
                    throw new HttpResponseException(StatusCodes.Status403Forbidden, ErrorType.Permission,
                        Localize.Error.PermissionInsufficientPermissions);
            }

            await _appDbContextAction.CommitTransactionAsync();

            _logger.Log(LogLevel.Information, Localize.Log.MethodEnd(GetType(), nameof(UserGroupTransferRequestRead)));

            return _mapper.Map<UserGroupTransferRequestReadResultDto>(userGroupTransferRequest);
        }
        catch (Exception)
        {
            await _appDbContextAction.RollbackTransactionAsync();

            throw;
        }
    }

    public async Task<IDtoResultBase> UserGroupTransferRequestReceiverReadCollection(
        UserGroupTransferRequestReadCollectionDto data,
        CancellationToken cancellationToken = default
    )
    {
        _logger.Log(LogLevel.Information,
            Localize.Log.MethodStart(GetType(), nameof(UserGroupTransferRequestReceiverReadCollection)));

        if (ValidateModel(data) is { } validationResult)
            return validationResult;

        try
        {
            await _appDbContextAction.BeginTransactionAsync();

            //User read transfer request if (Owner - [DestUser, SrcUser] | Others)

            var user = await _userAdvancedService.GetFromHttpContext(cancellationToken: cancellationToken);
            if (user == null)
                throw new HttpResponseException(StatusCodes.Status400BadRequest, ErrorType.HttpContext,
                    Localize.Error.UserNotFoundOrHttpContextMissingClaims);

            (int total, IReadOnlyCollection<UserGroupTransferRequest> entities) userGroupTransferRequests;

            //Action is taken by receiver of UserGroupTransferRequest
            if (user.Id == data.TargetUserId)
            {
                //TODO: + against User (user that receives request)
                userGroupTransferRequests = await _userGroupTransferRequestEntityService.GetFiltered(
                    data.FilterExpressionModel, data.FilterSortModel,
                    data.PageModel, null, new FilterExpressionModel
                    {
                        ExpressionLogicalOperation = ExpressionLogicalOperation.None,
                        Items = new List<FilterExpressionModelItem>
                        {
                            new FilterExpressionModelItemExpression
                            {
                                ExpressionLogicalOperation = ExpressionLogicalOperation.None,
                                Key = "DestUserId",
                                Value = data.TargetUserId.ToByteArray(),
                                FilterMatchOperation = FilterMatchOperation.Equal
                            }
                        }
                    }, cancellationToken);
            }
            //Action is taken by 3rd party
            else
            {
                //Authorize against UserGroup TODO: + against User (user that initiates action)
                userGroupTransferRequests = await _userGroupTransferRequestEntityService.GetFiltered(
                    data.FilterExpressionModel, data.FilterSortModel,
                    data.PageModel, new AuthorizeModel
                    {
                        EntityLeftTableName = _userRepository.GetTableName(),
                        EntityLeftGroupsTableName = _userGroupRepository.GetTableName(),
                        EntityLeftEntityToEntityMappingsTableName = _userToUserGroupMappingRepository.GetTableName(),
                        EntityLeftId = user.Id,
                        EntityLeftPermissionAlias = Consts.PermissionAlias.UserGroupTransferRequestRead,
                        EntityRightTableName = _userGroupRepository.GetTableName(),
                        EntityRightGroupsTableName = null,
                        EntityRightEntityToEntityMappingsTableName = null,
                        EntityRightIdRawSql = "\"UserGroupId\"",
                        EntityRightPermissionAlias = Consts.PermissionAlias.UserGroupTransferRequestRead,
                        SqlExpressionPermissionTypeValueNeededOwner = "T1.\"Id\" = T2.\"UserId\""
                    }, new FilterExpressionModel
                    {
                        ExpressionLogicalOperation = ExpressionLogicalOperation.None,
                        Items = new List<FilterExpressionModelItem>
                        {
                            new FilterExpressionModelItemExpression
                            {
                                ExpressionLogicalOperation = ExpressionLogicalOperation.None,
                                Key = "DestUserId",
                                Value = data.TargetUserId.ToByteArray(),
                                FilterMatchOperation = FilterMatchOperation.Equal
                            }
                        }
                    }, cancellationToken);
            }

            await _appDbContextAction.CommitTransactionAsync();

            _logger.Log(LogLevel.Information,
                Localize.Log.MethodEnd(GetType(), nameof(UserGroupTransferRequestReceiverReadCollection)));

            return new UserGroupTransferRequestReadCollectionResultDto
            {
                Total = userGroupTransferRequests.total,
                Items = userGroupTransferRequests.entities.Select(
                    _ => _mapper.ProjectTo<UserGroupTransferRequestReadCollectionItemResultDto>(new[] {_}.AsQueryable())
                        .Single())
            };
        }
        catch (Exception)
        {
            await _appDbContextAction.RollbackTransactionAsync();

            throw;
        }
    }

    public async Task<IDtoResultBase> UserGroupTransferRequestSenderReadCollection(
        UserGroupTransferRequestReadCollectionDto data,
        CancellationToken cancellationToken = default
    )
    {
        _logger.Log(LogLevel.Information,
            Localize.Log.MethodStart(GetType(), nameof(UserGroupTransferRequestSenderReadCollection)));

        if (ValidateModel(data) is { } validationResult)
            return validationResult;

        try
        {
            await _appDbContextAction.BeginTransactionAsync();

            //User read transfer request if (Owner - [DestUser, SrcUser] | Others)

            var user = await _userAdvancedService.GetFromHttpContext(cancellationToken: cancellationToken);
            if (user == null)
                throw new HttpResponseException(StatusCodes.Status400BadRequest, ErrorType.HttpContext,
                    Localize.Error.UserNotFoundOrHttpContextMissingClaims);

            //Action is taken by creator of UserGroupTransferRequest or by 3rd party
            //Authorize against UserGroup TODO: + against User (user that initiates action)
            var userGroupTransferRequests = await _userGroupTransferRequestEntityService.GetFiltered(
                data.FilterExpressionModel, data.FilterSortModel,
                data.PageModel, new AuthorizeModel
                {
                    EntityLeftTableName = _userRepository.GetTableName(),
                    EntityLeftGroupsTableName = _userGroupRepository.GetTableName(),
                    EntityLeftEntityToEntityMappingsTableName = _userToUserGroupMappingRepository.GetTableName(),
                    EntityLeftId = user.Id,
                    EntityLeftPermissionAlias = Consts.PermissionAlias.UserGroupTransferRequestRead,
                    EntityRightTableName = _userGroupRepository.GetTableName(),
                    EntityRightGroupsTableName = null,
                    EntityRightEntityToEntityMappingsTableName = null,
                    EntityRightIdRawSql = "\"UserGroupId\"",
                    EntityRightPermissionAlias = Consts.PermissionAlias.UserGroupTransferRequestRead,
                    SqlExpressionPermissionTypeValueNeededOwner = "T1.\"Id\" = T2.\"UserId\""
                }, new FilterExpressionModel
                {
                    ExpressionLogicalOperation = ExpressionLogicalOperation.None,
                    Items = new List<FilterExpressionModelItem>
                    {
                        new FilterExpressionModelItemExpression
                        {
                            ExpressionLogicalOperation = ExpressionLogicalOperation.None,
                            Key = "SrcUserId",
                            Value = data.TargetUserId.ToByteArray(),
                            FilterMatchOperation = FilterMatchOperation.Equal
                        }
                    }
                }, cancellationToken);

            await _appDbContextAction.CommitTransactionAsync();

            _logger.Log(LogLevel.Information,
                Localize.Log.MethodEnd(GetType(), nameof(UserGroupTransferRequestSenderReadCollection)));

            return new UserGroupTransferRequestReadCollectionResultDto
            {
                Total = userGroupTransferRequests.total,
                Items = userGroupTransferRequests.entities.Select(
                    _ => _mapper.ProjectTo<UserGroupTransferRequestReadCollectionItemResultDto>(new[] {_}.AsQueryable())
                        .Single())
            };
        }
        catch (Exception)
        {
            await _appDbContextAction.RollbackTransactionAsync();

            throw;
        }
    }

    public async Task<IDtoResultBase> UserGroupInviteRequestCreate(
        UserGroupInviteRequestCreateDto data,
        CancellationToken cancellationToken = default
    )
    {
        _logger.Log(LogLevel.Information, Localize.Log.MethodStart(GetType(), nameof(UserGroupInviteRequestCreate)));

        if (ValidateModel(data) is { } validationResult)
            return validationResult;

        try
        {
            await _appDbContextAction.BeginTransactionAsync();

            var user = await _userAdvancedService.GetFromHttpContext(cancellationToken: cancellationToken);
            if (user == null)
                throw new HttpResponseException(StatusCodes.Status400BadRequest, ErrorType.HttpContext,
                    Localize.Error.UserNotFoundOrHttpContextMissingClaims);

            var userGroup = await _userGroupEntityService.GetByIdAsync(data.UserGroupId, cancellationToken);
            if (userGroup == null)
                throw new HttpResponseException(StatusCodes.Status404NotFound, ErrorType.Generic,
                    Localize.Error.UserGroupNotFound);

            var userTarget = await _userEntityService.GetByIdAsync(data.TargetUserId, cancellationToken);
            if (userTarget == null)
                throw new HttpResponseException(StatusCodes.Status404NotFound, ErrorType.HttpContext,
                    Localize.Error.UserNotFound);

            //Authorize against UserGroup TODO: + against User (user that initiates action, user that receives request)
            var authorizeResult = _authorizeAdvancedService.Authorize(new AuthorizeModel
            {
                EntityLeftTableName = _userRepository.GetTableName(),
                EntityLeftGroupsTableName = _userGroupRepository.GetTableName(),
                EntityLeftEntityToEntityMappingsTableName = _userToUserGroupMappingRepository.GetTableName(),
                EntityLeftId = user.Id,
                EntityLeftPermissionAlias = Consts.PermissionAlias.UserGroupInviteRequestCreate,
                EntityRightTableName = _userGroupRepository.GetTableName(),
                EntityRightGroupsTableName = null,
                EntityRightEntityToEntityMappingsTableName = null,
                EntityRightId = userGroup.Id,
                EntityRightPermissionAlias = Consts.PermissionAlias.UserGroupInviteRequestCreate,
                SqlExpressionPermissionTypeValueNeededOwner = "T1.\"Id\" = T2.\"UserId\""
            });
            
            if (!authorizeResult)
            {
                //Authorize via UserToUserGroupMapping against UserGroup (Specific User, Public, GroupMember)
                UserToUserGroupMapping userToUserGroupMappingUserSpecificGroupMemberPublic = null;
                    var userToUserGroupMappingUserSpecific =
                        await _userToUserGroupMappingEntityService.GetByUserIdUserGroupIdAsync(user.Id, userGroup.Id);
                    var userToUserGroupMappingUserGroupMember =
                        await _userToUserGroupMappingEntityService.GetByUserIdUserGroupIdAsync(Consts.GroupMemberUserId,
                            userGroup.Id);
                    var userToUserGroupMappingUserPublic =
                        await _userToUserGroupMappingEntityService.GetByUserIdUserGroupIdAsync(Consts.PublicUserId,
                            userGroup.Id);

                    if (userToUserGroupMappingUserSpecific != null &&
                        await _authorizeAdvancedService.IsPermissionValueSet(
                            Consts.PermissionAlias.UserGroupMemberInviteRequestCreate,
                            userToUserGroupMappingUserSpecific.Id,
                            PermissionType.Value, cancellationToken))
                        userToUserGroupMappingUserSpecificGroupMemberPublic = userToUserGroupMappingUserSpecific;
                    else if (userToUserGroupMappingUserGroupMember != null && userToUserGroupMappingUserSpecific != null &&
                             await _authorizeAdvancedService.IsPermissionValueSet(
                                 Consts.PermissionAlias.UserGroupMemberInviteRequestCreate,
                                 userToUserGroupMappingUserGroupMember.Id,
                                 PermissionType.Value, cancellationToken))
                        userToUserGroupMappingUserSpecificGroupMemberPublic = userToUserGroupMappingUserGroupMember;
                    else if (userToUserGroupMappingUserPublic != null &&
                             await _authorizeAdvancedService.IsPermissionValueSet(
                                 Consts.PermissionAlias.UserGroupMemberInviteRequestCreate,
                                 userToUserGroupMappingUserPublic.Id,
                                 PermissionType.Value, cancellationToken))
                        userToUserGroupMappingUserSpecificGroupMemberPublic = userToUserGroupMappingUserPublic;
                    else
                        throw new HttpResponseException(StatusCodes.Status403Forbidden, ErrorType.Permission,
                            Localize.Error.PermissionInsufficientPermissions);
                
                authorizeResult |= _authorizeAdvancedService.Authorize(new AuthorizeModel
                {
                    EntityLeftTableName = _userToUserGroupMappingRepository.GetTableName(),
                    EntityLeftGroupsTableName = null,
                    EntityLeftEntityToEntityMappingsTableName = null,
                    EntityLeftId = userToUserGroupMappingUserSpecificGroupMemberPublic.Id,
                    EntityLeftPermissionAlias = Consts.PermissionAlias.UserGroupMemberInviteRequestCreate,
                    EntityRightTableName = _userGroupRepository.GetTableName(),
                    EntityRightGroupsTableName = null,
                    EntityRightEntityToEntityMappingsTableName = null,
                    EntityRightId = userGroup.Id,
                    EntityRightPermissionAlias = Consts.PermissionAlias.UserGroupMemberInviteRequestCreate,
                    SqlExpressionPermissionTypeValueNeededOwner = "T1.\"EntityLeftId\" = T2.\"UserId\""
                });

                if (!authorizeResult)
                    throw new HttpResponseException(StatusCodes.Status403Forbidden, ErrorType.Permission,
                        Localize.Error.PermissionInsufficientPermissions);
            }

            if (user.Id == data.TargetUserId)
                throw new HttpResponseException(StatusCodes.Status400BadRequest, ErrorType.Generic,
                    Localize.Error.UserGroupInviteRequestSameUserNotAllowed);

            if (userTarget.Id == Consts.PublicUserId || userTarget.Id == Consts.GroupMemberUserId)
                throw new HttpResponseException(StatusCodes.Status400BadRequest, ErrorType.Generic,
                    Localize.Error.UserToUserGroupMappingAddNotAllowed);

            //Do not allow to invite User associated with UserGroup
            if (await _userToUserGroupMappingEntityService.GetByUserIdUserGroupIdAsync(userTarget.Id, userGroup.Id) !=
                null)
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

            _logger.Log(LogLevel.Information, Localize.Log.MethodEnd(GetType(), nameof(UserGroupInviteRequestCreate)));

            return _mapper.Map<UserGroupInviteRequestCreateResultDto>(userGroupInviteRequest);
        }
        catch (Exception)
        {
            await _appDbContextAction.RollbackTransactionAsync();

            throw;
        }
    }

    public async Task<IDtoResultBase> UserGroupInviteRequestUpdate(
        UserGroupInviteRequestUpdateDto data,
        CancellationToken cancellationToken = default
    )
    {
        _logger.Log(LogLevel.Information, Localize.Log.MethodStart(GetType(), nameof(UserGroupInviteRequestUpdate)));

        if (ValidateModel(data) is { } validationResult)
            return validationResult;

        try
        {
            await _appDbContextAction.BeginTransactionAsync();

            var user = await _userAdvancedService.GetFromHttpContext(cancellationToken: cancellationToken);
            if (user == null)
                throw new HttpResponseException(StatusCodes.Status400BadRequest, ErrorType.HttpContext,
                    Localize.Error.UserNotFoundOrHttpContextMissingClaims);

            var userGroupInviteRequest =
                await _userGroupInviteRequestEntityService.GetByIdAsync(data.UserGroupInviteRequestId, cancellationToken);
            if (userGroupInviteRequest == null)
                throw new HttpResponseException(StatusCodes.Status404NotFound, ErrorType.Generic,
                    Localize.Error.UserGroupInviteRequestNotFound);

            var userGroup =
                await _userGroupEntityService.GetByIdAsync(userGroupInviteRequest.UserGroupId, cancellationToken);
            if (userGroup == null)
                throw new HttpResponseException(StatusCodes.Status404NotFound, ErrorType.Generic,
                    Localize.Error.UserGroupNotFound);

            //Action is taken by creator of UserGroupInviteRequest
            if (user.Id == userGroupInviteRequest.SrcUserId)
            {
                //Authorize against UserGroup TODO: + against User (user that initiates action)
                var authorizeResult = _authorizeAdvancedService.Authorize(new AuthorizeModel
                {
                    EntityLeftTableName = _userRepository.GetTableName(),
                    EntityLeftGroupsTableName = _userGroupRepository.GetTableName(),
                    EntityLeftEntityToEntityMappingsTableName = _userToUserGroupMappingRepository.GetTableName(),
                    EntityLeftId = user.Id,
                    EntityLeftPermissionAlias = Consts.PermissionAlias.UserGroupInviteRequestUpdate,
                    EntityRightTableName = _userGroupRepository.GetTableName(),
                    EntityRightGroupsTableName = null,
                    EntityRightEntityToEntityMappingsTableName = null,
                    EntityRightId = userGroup.Id,
                    EntityRightPermissionAlias = Consts.PermissionAlias.UserGroupInviteRequestUpdate,
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
                //TODO: + against User (user that receives request)
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
                        await _authorizeEntityService.PurgeByEntityIdAsync(userGroupInviteRequest.DestUserId,
                            cancellationToken);

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
                //Authorize against UserGroup TODO: + against User (user that initiates action)
                var authorizeResult = _authorizeAdvancedService.Authorize(new AuthorizeModel
                {
                    EntityLeftTableName = _userRepository.GetTableName(),
                    EntityLeftGroupsTableName = _userGroupRepository.GetTableName(),
                    EntityLeftEntityToEntityMappingsTableName = _userToUserGroupMappingRepository.GetTableName(),
                    EntityLeftId = user.Id,
                    EntityLeftPermissionAlias = Consts.PermissionAlias.UserGroupInviteRequestUpdate,
                    EntityRightTableName = _userGroupRepository.GetTableName(),
                    EntityRightGroupsTableName = null,
                    EntityRightEntityToEntityMappingsTableName = null,
                    EntityRightId = userGroup.Id,
                    EntityRightPermissionAlias = Consts.PermissionAlias.UserGroupInviteRequestUpdate,
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
                        await _authorizeEntityService.PurgeByEntityIdAsync(userGroupInviteRequest.DestUserId,
                            cancellationToken);

                        await _userGroupInviteRequestEntityService.Delete(userGroupInviteRequest, cancellationToken);
                        break;
                    }
                    case Choice.Reject:
                    {
                        if (!authorizeResult)
                        {
                            //Authorize g_ingroup_a_inviteuser_o_usergroup_a_manage via UserToUserGroupMapping against UserGroup (Specific User, Public, GroupMember)
                            UserToUserGroupMapping userToUserGroupMappingUserSpecificGroupMemberPublic = null;
                            var userToUserGroupMappingUserSpecific =
                                await _userToUserGroupMappingEntityService.GetByUserIdUserGroupIdAsync(user.Id,
                                    userGroup.Id);
                            var userToUserGroupMappingUserGroupMember =
                                await _userToUserGroupMappingEntityService.GetByUserIdUserGroupIdAsync(
                                    Consts.GroupMemberUserId,
                                    userGroup.Id);
                            var userToUserGroupMappingUserPublic =
                                await _userToUserGroupMappingEntityService.GetByUserIdUserGroupIdAsync(
                                    Consts.PublicUserId,
                                    userGroup.Id);

                            if (userToUserGroupMappingUserSpecific != null &&
                                await _authorizeAdvancedService.IsPermissionValueSet(
                                    Consts.PermissionAlias.UserGroupMemberInviteRequestUpdate,
                                    userToUserGroupMappingUserSpecific.Id,
                                    PermissionType.Value, cancellationToken))
                                userToUserGroupMappingUserSpecificGroupMemberPublic =
                                    userToUserGroupMappingUserSpecific;
                            else if (userToUserGroupMappingUserGroupMember != null &&
                                     userToUserGroupMappingUserSpecific != null &&
                                     await _authorizeAdvancedService.IsPermissionValueSet(
                                         Consts.PermissionAlias.UserGroupMemberInviteRequestUpdate,
                                         userToUserGroupMappingUserGroupMember.Id,
                                         PermissionType.Value, cancellationToken))
                                userToUserGroupMappingUserSpecificGroupMemberPublic =
                                    userToUserGroupMappingUserGroupMember;
                            else if (userToUserGroupMappingUserPublic != null &&
                                     await _authorizeAdvancedService.IsPermissionValueSet(
                                         Consts.PermissionAlias.UserGroupMemberInviteRequestUpdate,
                                         userToUserGroupMappingUserPublic.Id,
                                         PermissionType.Value, cancellationToken))
                                userToUserGroupMappingUserSpecificGroupMemberPublic = userToUserGroupMappingUserPublic;
                            else
                                throw new HttpResponseException(StatusCodes.Status403Forbidden, ErrorType.Permission,
                                    Localize.Error.PermissionInsufficientPermissions);

                            authorizeResult |= _authorizeAdvancedService.Authorize(new AuthorizeModel
                            {
                                EntityLeftTableName = _userToUserGroupMappingRepository.GetTableName(),
                                EntityLeftGroupsTableName = null,
                                EntityLeftEntityToEntityMappingsTableName = null,
                                EntityLeftId = userToUserGroupMappingUserSpecificGroupMemberPublic.Id,
                                EntityLeftPermissionAlias = Consts.PermissionAlias.UserGroupMemberInviteRequestUpdate,
                                EntityRightTableName = _userGroupRepository.GetTableName(),
                                EntityRightGroupsTableName = null,
                                EntityRightEntityToEntityMappingsTableName = null,
                                EntityRightId = userGroup.Id,
                                EntityRightPermissionAlias = Consts.PermissionAlias.UserGroupMemberInviteRequestUpdate,
                                SqlExpressionPermissionTypeValueNeededOwner = "T1.\"EntityLeftId\" = T2.\"UserId\""
                            });

                            if (!authorizeResult)
                                throw new HttpResponseException(StatusCodes.Status403Forbidden, ErrorType.Permission,
                                    Localize.Error.PermissionInsufficientPermissions);
                        }

                        await _userGroupInviteRequestEntityService.Delete(userGroupInviteRequest, cancellationToken);
                        break;
                    }
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            await _appDbContextAction.CommitTransactionAsync();

            _logger.Log(LogLevel.Information, Localize.Log.MethodEnd(GetType(), nameof(UserGroupInviteRequestUpdate)));

            return new OkResultDto();
        }
        catch (Exception)
        {
            await _appDbContextAction.RollbackTransactionAsync();

            throw;
        }
    }

    public async Task<IDtoResultBase> UserGroupInviteRequestRead(
        UserGroupInviteRequestReadDto data,
        CancellationToken cancellationToken = default
    )
    {
        _logger.Log(LogLevel.Information, Localize.Log.MethodStart(GetType(), nameof(UserGroupInviteRequestRead)));

        if (ValidateModel(data) is { } validationResult)
            return validationResult;

        try
        {
            await _appDbContextAction.BeginTransactionAsync();

            //User read invite request if (Owner - [DestUser, SrcUser] | Others [UserToUserGroupMapping])
            
            var user = await _userAdvancedService.GetFromHttpContext(cancellationToken: cancellationToken);
            if (user == null)
                throw new HttpResponseException(StatusCodes.Status400BadRequest, ErrorType.HttpContext,
                    Localize.Error.UserNotFoundOrHttpContextMissingClaims);

            var userGroupInviteRequest =
                await _userGroupInviteRequestEntityService.GetByIdAsync(data.UserGroupInviteRequestId, cancellationToken);
            if (userGroupInviteRequest == null)
                throw new HttpResponseException(StatusCodes.Status404NotFound, ErrorType.Generic,
                    Localize.Error.UserGroupInviteRequestNotFound);

            var userGroup =
                await _userGroupEntityService.GetByIdAsync(userGroupInviteRequest.UserGroupId, cancellationToken);
            if (userGroup == null)
                throw new HttpResponseException(StatusCodes.Status404NotFound, ErrorType.Generic,
                    Localize.Error.UserGroupNotFound);
            
            //Action is taken by receiver of UserGroupTransferRequest
            if (user.Id == userGroupInviteRequest.DestUserId)
            {
                //TODO: + against User (user that receives request)
            }
            //Action is taken by creator of UserGroupTransferRequest or by 3rd party
            else
            {
                //Authorize against UserGroup TODO: + against User (user that initiates action)
                var authorizeResult = _authorizeAdvancedService.Authorize(new AuthorizeModel
                {
                    EntityLeftTableName = _userRepository.GetTableName(),
                    EntityLeftGroupsTableName = _userGroupRepository.GetTableName(),
                    EntityLeftEntityToEntityMappingsTableName = _userToUserGroupMappingRepository.GetTableName(),
                    EntityLeftId = user.Id,
                    EntityLeftPermissionAlias = Consts.PermissionAlias.UserGroupInviteRequestRead,
                    EntityRightTableName = _userGroupRepository.GetTableName(),
                    EntityRightGroupsTableName = null,
                    EntityRightEntityToEntityMappingsTableName = null,
                    EntityRightId = userGroup.Id,
                    EntityRightPermissionAlias = Consts.PermissionAlias.UserGroupInviteRequestRead,
                    SqlExpressionPermissionTypeValueNeededOwner = "T1.\"Id\" = T2.\"UserId\""
                });

                if (!authorizeResult)
                {
                    //Authorize via UserToUserGroupMapping against UserGroup (Specific User, Public, GroupMember)
                    UserToUserGroupMapping userToUserGroupMappingUserSpecificGroupMemberPublic = null;
                    var userToUserGroupMappingUserSpecific =
                        await _userToUserGroupMappingEntityService.GetByUserIdUserGroupIdAsync(user.Id, userGroup.Id);
                    var userToUserGroupMappingUserGroupMember =
                        await _userToUserGroupMappingEntityService.GetByUserIdUserGroupIdAsync(Consts.GroupMemberUserId,
                            userGroup.Id);
                    var userToUserGroupMappingUserPublic =
                        await _userToUserGroupMappingEntityService.GetByUserIdUserGroupIdAsync(Consts.PublicUserId,
                            userGroup.Id);

                    if (userToUserGroupMappingUserSpecific != null &&
                        await _authorizeAdvancedService.IsPermissionValueSet(
                            Consts.PermissionAlias.UserGroupMemberInviteRequestRead,
                            userToUserGroupMappingUserSpecific.Id,
                            PermissionType.Value, cancellationToken))
                        userToUserGroupMappingUserSpecificGroupMemberPublic = userToUserGroupMappingUserSpecific;
                    else if (userToUserGroupMappingUserGroupMember != null && userToUserGroupMappingUserSpecific != null &&
                             await _authorizeAdvancedService.IsPermissionValueSet(
                                 Consts.PermissionAlias.UserGroupMemberInviteRequestRead,
                                 userToUserGroupMappingUserGroupMember.Id,
                                 PermissionType.Value, cancellationToken))
                        userToUserGroupMappingUserSpecificGroupMemberPublic = userToUserGroupMappingUserGroupMember;
                    else if (userToUserGroupMappingUserPublic != null &&
                             await _authorizeAdvancedService.IsPermissionValueSet(
                                 Consts.PermissionAlias.UserGroupMemberInviteRequestRead,
                                 userToUserGroupMappingUserPublic.Id,
                                 PermissionType.Value, cancellationToken))
                        userToUserGroupMappingUserSpecificGroupMemberPublic = userToUserGroupMappingUserPublic;
                    else
                        throw new HttpResponseException(StatusCodes.Status403Forbidden, ErrorType.Permission,
                            Localize.Error.PermissionInsufficientPermissions);

                    authorizeResult |= _authorizeAdvancedService.Authorize(new AuthorizeModel
                    {
                        EntityLeftTableName = _userToUserGroupMappingRepository.GetTableName(),
                        EntityLeftGroupsTableName = null,
                        EntityLeftEntityToEntityMappingsTableName = null,
                        EntityLeftId = userToUserGroupMappingUserSpecificGroupMemberPublic.Id,
                        EntityLeftPermissionAlias = Consts.PermissionAlias.UserGroupMemberInviteRequestRead,
                        EntityRightTableName = _userGroupRepository.GetTableName(),
                        EntityRightGroupsTableName = null,
                        EntityRightEntityToEntityMappingsTableName = null,
                        EntityRightId = userGroup.Id,
                        EntityRightPermissionAlias = Consts.PermissionAlias.UserGroupMemberInviteRequestRead,
                        SqlExpressionPermissionTypeValueNeededOwner = "T1.\"EntityLeftId\" = T2.\"UserId\""
                    });

                    if (!authorizeResult)
                        throw new HttpResponseException(StatusCodes.Status403Forbidden, ErrorType.Permission,
                            Localize.Error.PermissionInsufficientPermissions);
                }
            }

            await _appDbContextAction.CommitTransactionAsync();

            _logger.Log(LogLevel.Information, Localize.Log.MethodEnd(GetType(), nameof(UserGroupInviteRequestRead)));

            return _mapper.Map<UserGroupInviteRequestReadResultDto>(userGroupInviteRequest);
        }
        catch (Exception)
        {
            await _appDbContextAction.RollbackTransactionAsync();

            throw;
        }
    }

    public async Task<IDtoResultBase> UserGroupInviteRequestReceiverReadCollection(
        UserGroupInviteRequestReceiverReadCollectionDto data,
        CancellationToken cancellationToken = default
    )
    {
        _logger.Log(LogLevel.Information,
            Localize.Log.MethodStart(GetType(), nameof(UserGroupInviteRequestReceiverReadCollection)));

        if (ValidateModel(data) is { } validationResult)
            return validationResult;

        try
        {
            await _appDbContextAction.BeginTransactionAsync();

            //User read invite request if (Owner - [DestUser, SrcUser] | Others [UserToUserGroupMapping])
            
            var user = await _userAdvancedService.GetFromHttpContext(cancellationToken: cancellationToken);
            if (user == null)
                throw new HttpResponseException(StatusCodes.Status400BadRequest, ErrorType.HttpContext,
                    Localize.Error.UserNotFoundOrHttpContextMissingClaims);
            
            (int total, IReadOnlyCollection<UserGroupInviteRequest> entities) userGroupInviteRequests;
            
            //Action is taken by receiver of UserGroupTransferRequest
            if (user.Id == data.UserId)
            {
                //TODO: + against User (user that receives request)
                userGroupInviteRequests = await _userGroupInviteRequestEntityService.GetFiltered(
                    data.FilterExpressionModel, data.FilterSortModel,
                    data.PageModel, null,new FilterExpressionModel
                    {
                        ExpressionLogicalOperation = ExpressionLogicalOperation.None,
                        Items = new List<FilterExpressionModelItem>
                        {
                            new FilterExpressionModelItemExpression
                            {
                                ExpressionLogicalOperation = ExpressionLogicalOperation.None,
                                Key = "DestUserId",
                                Value = data.UserId.ToByteArray(),
                                FilterMatchOperation = FilterMatchOperation.Equal
                            }
                        }
                    }, cancellationToken);
            }
            //Action is taken by 3rd party
            else
            {
                //Authorize against UserGroup TODO: + against User (user that initiates action)
                userGroupInviteRequests = await _userGroupInviteRequestEntityService.GetFiltered(
                    data.FilterExpressionModel, data.FilterSortModel,
                    data.PageModel, new AuthorizeModel
                    {
                        EntityLeftTableName = _userRepository.GetTableName(),
                        EntityLeftGroupsTableName = _userGroupRepository.GetTableName(),
                        EntityLeftEntityToEntityMappingsTableName = _userToUserGroupMappingRepository.GetTableName(),
                        EntityLeftId = user.Id,
                        EntityLeftPermissionAlias = Consts.PermissionAlias.UserGroupInviteRequestRead,
                        EntityRightTableName = _userGroupRepository.GetTableName(),
                        EntityRightGroupsTableName = null,
                        EntityRightEntityToEntityMappingsTableName = null,
                        EntityRightIdRawSql = "\"UserGroupId\"",
                        EntityRightPermissionAlias = Consts.PermissionAlias.UserGroupInviteRequestRead,
                        SqlExpressionPermissionTypeValueNeededOwner = "T1.\"Id\" = T2.\"UserId\""
                    }, new FilterExpressionModel
                    {
                        ExpressionLogicalOperation = ExpressionLogicalOperation.None,
                        Items = new List<FilterExpressionModelItem>
                        {
                            new FilterExpressionModelItemExpression
                            {
                                ExpressionLogicalOperation = ExpressionLogicalOperation.None,
                                Key = "DestUserId",
                                Value = data.UserId.ToByteArray(),
                                FilterMatchOperation = FilterMatchOperation.Equal
                            }
                        }
                    }, cancellationToken);
            }

            await _appDbContextAction.CommitTransactionAsync();

            _logger.Log(LogLevel.Information,
                Localize.Log.MethodEnd(GetType(), nameof(UserGroupInviteRequestReceiverReadCollection)));

            return new UserGroupInviteRequestReadCollectionResultDto
            {
                Total = userGroupInviteRequests.total,
                Items = userGroupInviteRequests.entities.Select(
                    _ => _mapper.ProjectTo<UserGroupInviteRequestReadCollectionItemResultDto>(new[] {_}.AsQueryable())
                        .Single())
            };
        }
        catch (Exception)
        {
            await _appDbContextAction.RollbackTransactionAsync();

            throw;
        }
    }

    public async Task<IDtoResultBase> UserGroupInviteRequestSenderReadCollection(
        UserGroupInviteRequestSenderReadCollectionDto data,
        CancellationToken cancellationToken = default
    )
    {
        _logger.Log(LogLevel.Information,
            Localize.Log.MethodStart(GetType(), nameof(UserGroupInviteRequestSenderReadCollection)));

        if (ValidateModel(data) is { } validationResult)
            return validationResult;

        try
        {
            await _appDbContextAction.BeginTransactionAsync();

            //User read invite request if (Owner - [DestUser, SrcUser] | Others [UserToUserGroupMapping])
            
            var user = await _userAdvancedService.GetFromHttpContext(cancellationToken: cancellationToken);
            if (user == null)
                throw new HttpResponseException(StatusCodes.Status400BadRequest, ErrorType.HttpContext,
                    Localize.Error.UserNotFoundOrHttpContextMissingClaims);
            
            var userGroup = await _userGroupEntityService.GetByIdAsync(data.UserGroupId, cancellationToken);
            if (userGroup == null)
                throw new HttpResponseException(StatusCodes.Status404NotFound, ErrorType.Generic,
                    Localize.Error.UserGroupNotFound);
            
            //Action is taken by creator of UserGroupTransferRequest or by 3rd party
            //Authorize against UserGroup TODO: + against User (user that initiates action)
            var userGroupInviteRequests = await _userGroupInviteRequestEntityService.GetFiltered(
                data.FilterExpressionModel, data.FilterSortModel,
                data.PageModel, new AuthorizeModel
                {
                    EntityLeftTableName = _userRepository.GetTableName(),
                    EntityLeftGroupsTableName = _userGroupRepository.GetTableName(),
                    EntityLeftEntityToEntityMappingsTableName = _userToUserGroupMappingRepository.GetTableName(),
                    EntityLeftId = user.Id,
                    EntityLeftPermissionAlias = Consts.PermissionAlias.UserGroupInviteRequestRead,
                    EntityRightTableName = _userGroupRepository.GetTableName(),
                    EntityRightGroupsTableName = null,
                    EntityRightEntityToEntityMappingsTableName = null,
                    EntityRightIdRawSql = "\"UserGroupId\"",
                    EntityRightPermissionAlias = Consts.PermissionAlias.UserGroupInviteRequestRead,
                    SqlExpressionPermissionTypeValueNeededOwner = "T1.\"Id\" = T2.\"UserId\""
                }, new FilterExpressionModel
                {
                    ExpressionLogicalOperation = ExpressionLogicalOperation.None,
                    Items = new List<FilterExpressionModelItem>
                    {
                        new FilterExpressionModelItemExpression
                        {
                            ExpressionLogicalOperation = ExpressionLogicalOperation.None,
                            Key = "UserGroupId",
                            Value = data.UserGroupId.ToByteArray(),
                            FilterMatchOperation = FilterMatchOperation.Equal
                        }
                    }
                }, cancellationToken);

            if (userGroupInviteRequests.total == 0)
            {
                //Authorize via UserToUserGroupMapping against UserGroup (Specific User, Public, GroupMember)
                UserToUserGroupMapping userToUserGroupMappingUserSpecificGroupMemberPublic = null;
                    var userToUserGroupMappingUserSpecific =
                        await _userToUserGroupMappingEntityService.GetByUserIdUserGroupIdAsync(user.Id, userGroup.Id);
                    var userToUserGroupMappingUserGroupMember =
                        await _userToUserGroupMappingEntityService.GetByUserIdUserGroupIdAsync(Consts.GroupMemberUserId,
                            userGroup.Id);
                    var userToUserGroupMappingUserPublic =
                        await _userToUserGroupMappingEntityService.GetByUserIdUserGroupIdAsync(Consts.PublicUserId,
                            userGroup.Id);

                    if (userToUserGroupMappingUserSpecific != null &&
                        await _authorizeAdvancedService.IsPermissionValueSet(
                            Consts.PermissionAlias.UserGroupMemberInviteRequestRead,
                            userToUserGroupMappingUserSpecific.Id,
                            PermissionType.Value, cancellationToken))
                        userToUserGroupMappingUserSpecificGroupMemberPublic = userToUserGroupMappingUserSpecific;
                    else if (userToUserGroupMappingUserGroupMember != null && userToUserGroupMappingUserSpecific != null &&
                             await _authorizeAdvancedService.IsPermissionValueSet(
                                 Consts.PermissionAlias.UserGroupMemberInviteRequestRead,
                                 userToUserGroupMappingUserGroupMember.Id,
                                 PermissionType.Value, cancellationToken))
                        userToUserGroupMappingUserSpecificGroupMemberPublic = userToUserGroupMappingUserGroupMember;
                    else if (userToUserGroupMappingUserPublic != null &&
                             await _authorizeAdvancedService.IsPermissionValueSet(
                                 Consts.PermissionAlias.UserGroupMemberInviteRequestRead,
                                 userToUserGroupMappingUserPublic.Id,
                                 PermissionType.Value, cancellationToken))
                        userToUserGroupMappingUserSpecificGroupMemberPublic = userToUserGroupMappingUserPublic;
                    else
                        throw new HttpResponseException(StatusCodes.Status403Forbidden, ErrorType.Permission,
                            Localize.Error.PermissionInsufficientPermissions);
                
                userGroupInviteRequests = await _userGroupInviteRequestEntityService.GetFiltered(
                    data.FilterExpressionModel, data.FilterSortModel,
                    data.PageModel, new AuthorizeModel
                    {
                        EntityLeftTableName = _userToUserGroupMappingRepository.GetTableName(),
                        EntityLeftGroupsTableName = null,
                        EntityLeftEntityToEntityMappingsTableName = null,
                        EntityLeftId = userToUserGroupMappingUserSpecificGroupMemberPublic.Id,
                        EntityLeftPermissionAlias = Consts.PermissionAlias.UserGroupMemberInviteRequestRead,
                        EntityRightTableName = _userGroupRepository.GetTableName(),
                        EntityRightGroupsTableName = null,
                        EntityRightEntityToEntityMappingsTableName = null,
                        EntityRightIdRawSql = "\"UserGroupId\"",
                        EntityRightPermissionAlias = Consts.PermissionAlias.UserGroupMemberInviteRequestRead,
                        SqlExpressionPermissionTypeValueNeededOwner = "T1.\"EntityLeftId\" = T2.\"UserId\""
                    }, new FilterExpressionModel
                    {
                        ExpressionLogicalOperation = ExpressionLogicalOperation.None,
                        Items = new List<FilterExpressionModelItem>
                        {
                            new FilterExpressionModelItemExpression
                            {
                                ExpressionLogicalOperation = ExpressionLogicalOperation.None,
                                Key = "UserGroupId",
                                Value = data.UserGroupId.ToByteArray(),
                                FilterMatchOperation = FilterMatchOperation.Equal
                            }
                        }
                    }, cancellationToken);
            }

            await _appDbContextAction.CommitTransactionAsync();

            _logger.Log(LogLevel.Information,
                Localize.Log.MethodEnd(GetType(), nameof(UserGroupInviteRequestSenderReadCollection)));

            return new UserGroupInviteRequestReadCollectionResultDto
            {
                Total = userGroupInviteRequests.total,
                Items = userGroupInviteRequests.entities.Select(
                    _ => _mapper.ProjectTo<UserGroupInviteRequestReadCollectionItemResultDto>(new[] {_}.AsQueryable())
                        .Single())
            };
        }
        catch (Exception)
        {
            await _appDbContextAction.RollbackTransactionAsync();

            throw;
        }
    }

    public async Task<IDtoResultBase> UserGroupAddUser(
        UserGroupAddUserDto data,
        CancellationToken cancellationToken = default
    )
    {
        _logger.Log(LogLevel.Information, Localize.Log.MethodStart(GetType(), nameof(UserGroupAddUser)));

        if (ValidateModel(data) is { } validationResult)
            return validationResult;

        try
        {
            await _appDbContextAction.BeginTransactionAsync();

            var user = await _userAdvancedService.GetFromHttpContext(cancellationToken: cancellationToken);
            if (user == null)
                throw new HttpResponseException(StatusCodes.Status400BadRequest, ErrorType.HttpContext,
                    Localize.Error.UserNotFoundOrHttpContextMissingClaims);

            var userGroup = await _userGroupEntityService.GetByIdAsync(data.UserGroupId, cancellationToken);
            if (userGroup == null)
                throw new HttpResponseException(StatusCodes.Status404NotFound, ErrorType.Generic,
                    Localize.Error.UserGroupNotFound);

            var userTarget = await _userEntityService.GetByIdAsync(data.UserId, cancellationToken);
            if (userTarget == null)
                throw new HttpResponseException(StatusCodes.Status404NotFound, ErrorType.Generic,
                    Localize.Error.UserNotFound);

            //Authorize to UserGroup TODO: + against User (user that initiates action, user that is being targeted)
            var authorizeResult = _authorizeAdvancedService.Authorize(new AuthorizeModel
            {
                EntityLeftTableName = _userRepository.GetTableName(),
                EntityLeftGroupsTableName = _userGroupRepository.GetTableName(),
                EntityLeftEntityToEntityMappingsTableName = _userToUserGroupMappingRepository.GetTableName(),
                EntityLeftId = user.Id,
                EntityLeftPermissionAlias = Consts.PermissionAlias.UserGroupAddUser,
                EntityRightTableName = _userGroupRepository.GetTableName(),
                EntityRightGroupsTableName = null,
                EntityRightEntityToEntityMappingsTableName = null,
                EntityRightId = userGroup.Id,
                EntityRightPermissionAlias = Consts.PermissionAlias.UserGroupAddUser,
                SqlExpressionPermissionTypeValueNeededOwner = "T1.\"Id\" = T2.\"UserId\""
            });

            if (!authorizeResult)
                throw new HttpResponseException(StatusCodes.Status403Forbidden, ErrorType.Permission,
                    Localize.Error.PermissionInsufficientPermissions);

            if (userTarget.Id == Consts.PublicUserId || userTarget.Id == Consts.GroupMemberUserId)
                throw new HttpResponseException(StatusCodes.Status400BadRequest, ErrorType.Generic,
                    Localize.Error.UserToUserGroupMappingAddNotAllowed);

            //Do not allow to add User associated with UserGroup
            if (await _userToUserGroupMappingEntityService.GetByUserIdUserGroupIdAsync(userTarget.Id, userGroup.Id) !=
                null)
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

            _logger.Log(LogLevel.Information, Localize.Log.MethodEnd(GetType(), nameof(UserGroupAddUser)));

            return new OkResultDto();
        }
        catch (Exception)
        {
            await _appDbContextAction.RollbackTransactionAsync();

            throw;
        }
    }

    public async Task<IDtoResultBase> UserGroupDeleteUser(
        UserGroupDeleteUserDto data,
        CancellationToken cancellationToken = default
    )
    {
        _logger.Log(LogLevel.Information, Localize.Log.MethodStart(GetType(), nameof(UserGroupDeleteUser)));

        if (ValidateModel(data) is { } validationResult)
            return validationResult;

        try
        {
            await _appDbContextAction.BeginTransactionAsync();

            var user = await _userAdvancedService.GetFromHttpContext(cancellationToken: cancellationToken);
            if (user == null)
                throw new HttpResponseException(StatusCodes.Status400BadRequest, ErrorType.HttpContext,
                    Localize.Error.UserNotFoundOrHttpContextMissingClaims);

            var userGroup = await _userGroupEntityService.GetByIdAsync(data.UserGroupId, cancellationToken);
            if (userGroup == null)
                throw new HttpResponseException(StatusCodes.Status404NotFound, ErrorType.Generic,
                    Localize.Error.UserGroupNotFound);

            var userTarget = await _userEntityService.GetByIdAsync(data.UserId, cancellationToken);
            if (userTarget == null)
                throw new HttpResponseException(StatusCodes.Status404NotFound, ErrorType.Generic,
                    Localize.Error.UserNotFound);

            //Authorize against UserGroup TODO: + against User (user that initiates action, user that is being targeted)
            var authorizeResult = _authorizeAdvancedService.Authorize(new AuthorizeModel
            {
                EntityLeftTableName = _userRepository.GetTableName(),
                EntityLeftGroupsTableName = _userGroupRepository.GetTableName(),
                EntityLeftEntityToEntityMappingsTableName = _userToUserGroupMappingRepository.GetTableName(),
                EntityLeftId = user.Id,
                EntityLeftPermissionAlias = Consts.PermissionAlias.UserGroupKickUser,
                EntityRightTableName = _userGroupRepository.GetTableName(),
                EntityRightGroupsTableName = null,
                EntityRightEntityToEntityMappingsTableName = null,
                EntityRightId = userGroup.Id,
                EntityRightPermissionAlias = Consts.PermissionAlias.UserGroupKickUser,
                SqlExpressionPermissionTypeValueNeededOwner = "T1.\"Id\" = T2.\"UserId\""
            });

            if (!authorizeResult)
            {
                //Authorize g_ingroup_a_kickuser_o_usergroup via UserToUserGroupMapping against UserGroup (Specific User, Public, GroupMember)
                UserToUserGroupMapping userToUserGroupMappingUserSpecificGroupMemberPublic = null;
                    var userToUserGroupMappingUserSpecific =
                        await _userToUserGroupMappingEntityService.GetByUserIdUserGroupIdAsync(user.Id, userGroup.Id);
                    var userToUserGroupMappingUserGroupMember =
                        await _userToUserGroupMappingEntityService.GetByUserIdUserGroupIdAsync(Consts.GroupMemberUserId,
                            userGroup.Id);
                    var userToUserGroupMappingUserPublic =
                        await _userToUserGroupMappingEntityService.GetByUserIdUserGroupIdAsync(Consts.PublicUserId,
                            userGroup.Id);

                    if (userToUserGroupMappingUserSpecific != null &&
                        await _authorizeAdvancedService.IsPermissionValueSet(
                            Consts.PermissionAlias.UserGroupMemberKickUser,
                            userToUserGroupMappingUserSpecific.Id,
                            PermissionType.Value, cancellationToken))
                        userToUserGroupMappingUserSpecificGroupMemberPublic = userToUserGroupMappingUserSpecific;
                    else if (userToUserGroupMappingUserGroupMember != null && userToUserGroupMappingUserSpecific != null &&
                             await _authorizeAdvancedService.IsPermissionValueSet(
                                 Consts.PermissionAlias.UserGroupMemberKickUser,
                                 userToUserGroupMappingUserGroupMember.Id,
                                 PermissionType.Value, cancellationToken))
                        userToUserGroupMappingUserSpecificGroupMemberPublic = userToUserGroupMappingUserGroupMember;
                    else if (userToUserGroupMappingUserPublic != null &&
                             await _authorizeAdvancedService.IsPermissionValueSet(
                                 Consts.PermissionAlias.UserGroupMemberKickUser,
                                 userToUserGroupMappingUserPublic.Id,
                                 PermissionType.Value, cancellationToken))
                        userToUserGroupMappingUserSpecificGroupMemberPublic = userToUserGroupMappingUserPublic;
                    else
                        throw new HttpResponseException(StatusCodes.Status403Forbidden, ErrorType.Permission,
                            Localize.Error.PermissionInsufficientPermissions);
                
                authorizeResult |= _authorizeAdvancedService.Authorize(new AuthorizeModel
                {
                    EntityLeftTableName = _userToUserGroupMappingRepository.GetTableName(),
                    EntityLeftGroupsTableName = null,
                    EntityLeftEntityToEntityMappingsTableName = null,
                    EntityLeftId = userToUserGroupMappingUserSpecificGroupMemberPublic.Id,
                    EntityLeftPermissionAlias = Consts.PermissionAlias.UserGroupMemberKickUser,
                    EntityRightTableName = _userGroupRepository.GetTableName(),
                    EntityRightGroupsTableName = null,
                    EntityRightEntityToEntityMappingsTableName = null,
                    EntityRightId = userGroup.Id,
                    EntityRightPermissionAlias = Consts.PermissionAlias.UserGroupMemberKickUser,
                    SqlExpressionPermissionTypeValueNeededOwner = "T1.\"EntityIdLeft\" = T2.\"UserId\""
                });

                if (!authorizeResult)
                    throw new HttpResponseException(StatusCodes.Status403Forbidden, ErrorType.Permission,
                        Localize.Error.PermissionInsufficientPermissions);
            }

            if (userTarget.Id == Consts.PublicUserId || userTarget.Id == Consts.GroupMemberUserId)
                throw new HttpResponseException(StatusCodes.Status400BadRequest, ErrorType.Generic,
                    Localize.Error.UserToUserGroupMappingDeleteNotAllowed);

            var userToUserGroupMappingUserTarget =
                await _userToUserGroupMappingEntityService.GetByUserIdUserGroupIdAsync(userTarget.Id, userGroup.Id);
            if (userToUserGroupMappingUserTarget == null)
                throw new HttpResponseException(StatusCodes.Status404NotFound, ErrorType.Generic,
                    Localize.Error.UserToUserGroupMappingNotFound);

            await _permissionValueEntityService.PurgeAsync(userToUserGroupMappingUserTarget.Id,
                cancellationToken);

            //Delete Authorize cache when leaving group (for the user that left)
            await _authorizeEntityService.PurgeByEntityIdAsync(userTarget.Id, cancellationToken);
            await _authorizeEntityService.PurgeByEntityIdAsync(userToUserGroupMappingUserTarget.Id, cancellationToken);

            await _userToUserGroupMappingEntityService.Delete(userToUserGroupMappingUserTarget, cancellationToken);

            await _appDbContextAction.CommitTransactionAsync();

            _logger.Log(LogLevel.Information, Localize.Log.MethodEnd(GetType(), nameof(UserGroupDeleteUser)));

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