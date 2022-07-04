using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using BLL.Handlers.Base;
using BLL.Maps;
using BLL.Services.Advanced;
using BLL.Services.Entity;
using Common.Enums;
using Common.Exceptions;
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

public interface IUserGroupHandler
{
    Task<IDtoResultBase> Create(UserGroupCreateDto data, CancellationToken cancellationToken = default);
    Task<IDtoResultBase> Read(UserGroupReadDto data, CancellationToken cancellationToken = default);

    Task<IDtoResultBase> ReadFSPCollection(
        UserGroupReadEntityCollectionDto data,
        CancellationToken cancellationToken = default
    );

    Task<IDtoResultBase> Update(UserGroupUpdateDto data, CancellationToken cancellationToken = default);
    Task<IDtoResultBase> Delete(UserGroupDeleteDto data, CancellationToken cancellationToken = default);
    Task<IDtoResultBase> Join(UserGroupJoinDto data, CancellationToken cancellationToken = default);
    Task<IDtoResultBase> Leave(UserGroupLeaveDto data, CancellationToken cancellationToken = default);
    Task<IDtoResultBase> InitTransfer(UserGroupTransferInitDto data, CancellationToken cancellationToken = default);
    Task<IDtoResultBase> ManageTransfer(UserGroupTransferManageDto data, CancellationToken cancellationToken = default);
    Task<IDtoResultBase> InitInviteUser(UserGroupInitInviteUserDto data, CancellationToken cancellationToken = default);
    Task<IDtoResultBase> ManageInviteUser(UserGroupManageInviteUserDto data, CancellationToken cancellationToken = default);
    Task<IDtoResultBase> AddUser(UserGroupAddUserDto data, CancellationToken cancellationToken = default);
    Task<IDtoResultBase> KickUser(UserGroupDeleteUserDto data, CancellationToken cancellationToken = default);
}

public class UserGroupHandler : HandlerBase, IUserGroupHandler
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

    #endregion

    #region Ctor

    public UserGroupHandler(
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
        IUserGroupInviteRequestEntityService userGroupInviteRequestEntityService
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
    }

    #endregion

    #region Methods

    public async Task<IDtoResultBase> Create(UserGroupCreateDto data, CancellationToken cancellationToken = default)
    {
        _logger.Log(LogLevel.Information, Localize.Log.MethodStart(GetType(), nameof(Create)));

        if (ValidateModel(data) is { } validationResult)
            return validationResult;

        try
        {
            await _appDbContextAction.BeginTransactionAsync();

            var user = await _userAdvancedService.GetFromHttpContext(cancellationToken);
            if (user == null)
                throw new HttpResponseException(StatusCodes.Status500InternalServerError, ErrorType.HttpContext,
                    Localize.Error.UserNotFoundOrHttpContextMissingClaims);

            var userOwner = await _userEntityService.GetByIdAsync(data.UserId, cancellationToken);
            if (userOwner == null)
                throw new HttpResponseException(StatusCodes.Status500InternalServerError, ErrorType.Generic,
                    Localize.Error.UserNotFound);

            //Authorize group create
            var authorizeResult = _authorizeAdvancedService.Authorize(new AuthorizeModel
            {
                EntityLeftTableName = _userRepository.GetTableName(),
                EntityLeftGroupsTableName = _userGroupRepository.GetTableName(),
                EntityLeftEntityToEntityMappingsTableName = _userToUserGroupMappingRepository.GetTableName(),
                EntityLeftId = user.Id,
                EntityLeftPermissionAlias = Consts.PermissionAlias.g_group_a_create_o_usergroup,
                EntityRightTableName = _userRepository.GetTableName(),
                EntityRightGroupsTableName = _userGroupRepository.GetTableName(),
                EntityRightEntityToEntityMappingsTableName =
                    _userToUserGroupMappingRepository.GetTableName(),
                EntityRightId = userOwner.Id,
                EntityRightPermissionAlias = Consts.PermissionAlias.g_group_a_create_o_usergroup,
                SqlExpressionPermissionTypeValueNeededOwner = "T1.\"Id\" = T2.\"Id\""
            });

            if (!authorizeResult)
                throw new HttpResponseException(StatusCodes.Status403Forbidden, ErrorType.Permission,
                    Localize.Error.PermissionInsufficientPermissions);

            var autoMapperModelAuthorizeData = new AutoMapperModelAuthorizeData
            {
                FieldAuthorizeResultDictionary = new Dictionary<string, bool>
                {
                    {
                        nameof(UserGroup.Alias),
                        _authorizeAdvancedService.Authorize(new AuthorizeModel
                        {
                            EntityLeftTableName = _userRepository.GetTableName(),
                            EntityLeftGroupsTableName = _userGroupRepository.GetTableName(),
                            EntityLeftEntityToEntityMappingsTableName =
                                _userToUserGroupMappingRepository.GetTableName(),
                            EntityLeftId = user.Id,
                            EntityLeftPermissionAlias =
                                Consts.PermissionAlias.g_group_a_create_o_usergroup_o_alias_l_automapper,
                            EntityRightTableName = _userRepository.GetTableName(),
                            EntityRightGroupsTableName = _userGroupRepository.GetTableName(),
                            EntityRightEntityToEntityMappingsTableName =
                                _userToUserGroupMappingRepository.GetTableName(),
                            EntityRightId = userOwner.Id,
                            EntityRightPermissionAlias =
                                Consts.PermissionAlias.g_group_a_create_o_usergroup_o_alias_l_automapper,
                            SqlExpressionPermissionTypeValueNeededOwner = "T1.\"Id\" = T2.\"Id\""
                        })
                    },
                    {
                        nameof(UserGroup.Description),
                        _authorizeAdvancedService.Authorize(new AuthorizeModel
                        {
                            EntityLeftTableName = _userRepository.GetTableName(),
                            EntityLeftGroupsTableName = _userGroupRepository.GetTableName(),
                            EntityLeftEntityToEntityMappingsTableName =
                                _userToUserGroupMappingRepository.GetTableName(),
                            EntityLeftId = user.Id,
                            EntityLeftPermissionAlias =
                                Consts.PermissionAlias.g_group_a_create_o_usergroup_o_description_l_automapper,
                            EntityRightTableName = _userRepository.GetTableName(),
                            EntityRightGroupsTableName = _userGroupRepository.GetTableName(),
                            EntityRightEntityToEntityMappingsTableName =
                                _userToUserGroupMappingRepository.GetTableName(),
                            EntityRightId = userOwner.Id,
                            EntityRightPermissionAlias =
                                Consts.PermissionAlias.g_group_a_create_o_usergroup_o_description_l_automapper,
                            SqlExpressionPermissionTypeValueNeededOwner = "T1.\"Id\" = T2.\"Id\""
                        })
                    },
                    {
                        nameof(UserGroup.Priority),
                        _authorizeAdvancedService.Authorize(new AuthorizeModel
                        {
                            EntityLeftTableName = _userRepository.GetTableName(),
                            EntityLeftGroupsTableName = _userGroupRepository.GetTableName(),
                            EntityLeftEntityToEntityMappingsTableName =
                                _userToUserGroupMappingRepository.GetTableName(),
                            EntityLeftId = user.Id,
                            EntityLeftPermissionAlias =
                                Consts.PermissionAlias.g_group_a_create_o_usergroup_o_priority_l_automapper,
                            EntityRightTableName = _userRepository.GetTableName(),
                            EntityRightGroupsTableName = _userGroupRepository.GetTableName(),
                            EntityRightEntityToEntityMappingsTableName =
                                _userToUserGroupMappingRepository.GetTableName(),
                            EntityRightId = userOwner.Id,
                            EntityRightPermissionAlias =
                                Consts.PermissionAlias.g_group_a_create_o_usergroup_o_priority_l_automapper,
                            SqlExpressionPermissionTypeValueNeededOwner = "T1.\"Id\" = T2.\"Id\""
                        })
                    }
                }
            };

            if (!autoMapperModelAuthorizeData.FieldAuthorizeResultDictionary.TryGetValue(nameof(UserGroup.Priority),
                    out var userGroupPriorityAuthorizeResult) || !userGroupPriorityAuthorizeResult)
            {
                var rand = RandomNumberGenerator.Create();
                long priority = 0;
                var buffer = new byte[8];

                while (priority >= 0 || await _userGroupEntityService.GetIsPriorityClaimed(priority))
                {
                    rand.GetBytes(buffer);
                    priority = BitConverter.ToInt64(buffer);
                }

                data.Priority = priority;
                autoMapperModelAuthorizeData.FieldAuthorizeResultDictionary[nameof(UserGroup.Priority)] = true;
            }

            var userGroup = _mapper.Map<UserGroup>(data,
                opts => { opts.Items[Consts.AutoMapperModelAuthorizeDataKey] = autoMapperModelAuthorizeData; });

            await _userGroupEntityService.Save(userGroup, cancellationToken);

            var userGroupPermissionValues = new PermissionValue[]
            {
                #region Read

                new()
                {
                    Value = BitConverter.GetBytes(Consts.TrueValue),
                    PermissionId = (await _permissionEntityService.GetByAliasTypeAsync(
                            Consts.PermissionAlias.g_group_a_read_o_usergroup,
                            PermissionType.ValueNeededOwner))
                        .Id,
                    EntityId = userGroup.Id
                },
                new()
                {
                    Value = BitConverter.GetBytes(Consts.RootUserGroupValue),
                    PermissionId = (await _permissionEntityService.GetByAliasTypeAsync(
                            Consts.PermissionAlias.g_group_a_read_o_usergroup,
                            PermissionType.ValueNeededOthers))
                        .Id,
                    EntityId = userGroup.Id
                },

                #endregion

                #region Update

                new()
                {
                    Value = BitConverter.GetBytes(Consts.TrueValue),
                    PermissionId = (await _permissionEntityService.GetByAliasTypeAsync(
                            Consts.PermissionAlias.g_group_a_update_o_usergroup,
                            PermissionType.ValueNeededOwner))
                        .Id,
                    EntityId = userGroup.Id
                },
                new()
                {
                    Value = BitConverter.GetBytes(Consts.RootUserGroupValue),
                    PermissionId = (await _permissionEntityService.GetByAliasTypeAsync(
                            Consts.PermissionAlias.g_group_a_update_o_usergroup,
                            PermissionType.ValueNeededOthers))
                        .Id,
                    EntityId = userGroup.Id
                },
                new()
                {
                    Value = BitConverter.GetBytes(Consts.TrueValue),
                    PermissionId = (await _permissionEntityService.GetByAliasTypeAsync(
                            Consts.PermissionAlias.g_group_a_update_o_usergroup_o_alias_l_automapper,
                            PermissionType.ValueNeededOwner))
                        .Id,
                    EntityId = userGroup.Id
                },
                new()
                {
                    Value = BitConverter.GetBytes(Consts.RootUserGroupValue),
                    PermissionId = (await _permissionEntityService.GetByAliasTypeAsync(
                            Consts.PermissionAlias.g_group_a_update_o_usergroup_o_alias_l_automapper,
                            PermissionType.ValueNeededOthers))
                        .Id,
                    EntityId = userGroup.Id
                },
                new()
                {
                    Value = BitConverter.GetBytes(Consts.TrueValue),
                    PermissionId = (await _permissionEntityService.GetByAliasTypeAsync(
                            Consts.PermissionAlias.g_group_a_update_o_usergroup_o_description_l_automapper,
                            PermissionType.ValueNeededOwner))
                        .Id,
                    EntityId = userGroup.Id
                },
                new()
                {
                    Value = BitConverter.GetBytes(Consts.RootUserGroupValue),
                    PermissionId = (await _permissionEntityService.GetByAliasTypeAsync(
                            Consts.PermissionAlias.g_group_a_update_o_usergroup_o_description_l_automapper,
                            PermissionType.ValueNeededOthers))
                        .Id,
                    EntityId = userGroup.Id
                },
                new()
                {
                    Value = BitConverter.GetBytes(Consts.TrueValue),
                    PermissionId = (await _permissionEntityService.GetByAliasTypeAsync(
                            Consts.PermissionAlias.g_group_a_update_o_usergroup_o_priority_l_automapper,
                            PermissionType.ValueNeededOwner))
                        .Id,
                    EntityId = userGroup.Id
                },
                new()
                {
                    Value = BitConverter.GetBytes(Consts.RootUserGroupValue),
                    PermissionId = (await _permissionEntityService.GetByAliasTypeAsync(
                            Consts.PermissionAlias.g_group_a_update_o_usergroup_o_priority_l_automapper,
                            PermissionType.ValueNeededOthers))
                        .Id,
                    EntityId = userGroup.Id
                },

                #endregion

                #region Delete

                new()
                {
                    Value = BitConverter.GetBytes(Consts.TrueValue),
                    PermissionId = (await _permissionEntityService.GetByAliasTypeAsync(
                            Consts.PermissionAlias.g_group_a_delete_o_usergroup,
                            PermissionType.ValueNeededOwner))
                        .Id,
                    EntityId = userGroup.Id
                },
                new()
                {
                    Value = BitConverter.GetBytes(Consts.RootUserGroupValue),
                    PermissionId = (await _permissionEntityService.GetByAliasTypeAsync(
                            Consts.PermissionAlias.g_group_a_delete_o_usergroup,
                            PermissionType.ValueNeededOthers))
                        .Id,
                    EntityId = userGroup.Id
                },

                #endregion

                #region Join

                new()
                {
                    Value = BitConverter.GetBytes(Consts.TrueValue),
                    PermissionId = (await _permissionEntityService.GetByAliasTypeAsync(
                            Consts.PermissionAlias.g_group_a_join_o_usergroup,
                            PermissionType.ValueNeededOwner))
                        .Id,
                    EntityId = userGroup.Id
                },
                new()
                {
                    Value = BitConverter.GetBytes(Consts.RootUserGroupValue),
                    PermissionId = (await _permissionEntityService.GetByAliasTypeAsync(
                            Consts.PermissionAlias.g_group_a_join_o_usergroup,
                            PermissionType.ValueNeededOthers))
                        .Id,
                    EntityId = userGroup.Id
                },

                #endregion

                #region Leave

                new()
                {
                    Value = BitConverter.GetBytes(Consts.TrueValue),
                    PermissionId = (await _permissionEntityService.GetByAliasTypeAsync(
                            Consts.PermissionAlias.g_group_a_leave_o_usergroup,
                            PermissionType.ValueNeededOwner))
                        .Id,
                    EntityId = userGroup.Id
                },
                new()
                {
                    Value = BitConverter.GetBytes(Consts.RootUserGroupValue),
                    PermissionId = (await _permissionEntityService.GetByAliasTypeAsync(
                            Consts.PermissionAlias.g_group_a_leave_o_usergroup,
                            PermissionType.ValueNeededOthers))
                        .Id,
                    EntityId = userGroup.Id
                },

                #endregion
                
                //TODO: Invite

                #region Invite
                
                new()
                {
                    Value = BitConverter.GetBytes(Consts.TrueValue),
                    PermissionId = (await _permissionEntityService.GetByAliasTypeAsync(
                            Consts.PermissionAlias.g_group_a_inviteuser_o_usergroup,
                            PermissionType.ValueNeededOwner))
                        .Id,
                    EntityId = userGroup.Id
                },
                new()
                {
                    Value = BitConverter.GetBytes(Consts.TrueValue),
                    PermissionId = (await _permissionEntityService.GetByAliasTypeAsync(
                            Consts.PermissionAlias.g_group_a_inviteuser_o_usergroup,
                            PermissionType.ValueNeededOthers))
                        .Id,
                    EntityId = userGroup.Id
                },

                #endregion

                #region Kick

                new()
                {
                    Value = BitConverter.GetBytes(Consts.TrueValue),
                    PermissionId = (await _permissionEntityService.GetByAliasTypeAsync(
                            Consts.PermissionAlias.g_group_a_kickuser_o_usergroup,
                            PermissionType.ValueNeededOwner))
                        .Id,
                    EntityId = userGroup.Id
                },
                new()
                {
                    Value = BitConverter.GetBytes(Consts.TrueValue),
                    PermissionId = (await _permissionEntityService.GetByAliasTypeAsync(
                            Consts.PermissionAlias.g_group_a_kickuser_o_usergroup,
                            PermissionType.ValueNeededOthers))
                        .Id,
                    EntityId = userGroup.Id
                }

                #endregion
                
                //TODO: Kick
            };

            await _permissionValueEntityCollectionService.Save(userGroupPermissionValues, cancellationToken);

            var userToUserGroupMappingOwner = await _userToUserGroupMappingEntityService.Save(new UserToUserGroupMapping
            {
                EntityLeftId = user.Id,
                EntityRightId = userGroup.Id
            }, cancellationToken);
            
            var userToUserGroupMappingPublic = await _userToUserGroupMappingEntityService.Save(new UserToUserGroupMapping
            {
                EntityLeftId = Consts.PublicUserId,
                EntityRightId = userGroup.Id
            }, cancellationToken);
            
            var userToUserGroupMappingGroupMember = await _userToUserGroupMappingEntityService.Save(new UserToUserGroupMapping
            {
                EntityLeftId = Consts.GroupMemberUserId,
                EntityRightId = userGroup.Id
            }, cancellationToken);

            var userToUserGroupMappingPermissionValues = new PermissionValue[]
            {
                #region Owner

                new()
                {
                    Value = BitConverter.GetBytes(Consts.TrueValue),
                    PermissionId = (await _permissionEntityService.GetByAliasTypeAsync(
                            Consts.PermissionAlias.g_group_a_inviteuser_o_usergroup,
                            PermissionType.Value))
                        .Id,
                    EntityId = userToUserGroupMappingOwner.Id
                },
                new()
                {
                    Value = BitConverter.GetBytes(Consts.TrueValue),
                    PermissionId = (await _permissionEntityService.GetByAliasTypeAsync(
                            Consts.PermissionAlias.g_group_a_kickuser_o_usergroup,
                            PermissionType.Value))
                        .Id,
                    EntityId = userToUserGroupMappingOwner.Id
                },

                #endregion
                
                #region Public

                new()
                {
                    Value = BitConverter.GetBytes(Consts.FalseValue),
                    PermissionId = (await _permissionEntityService.GetByAliasTypeAsync(
                            Consts.PermissionAlias.g_group_a_inviteuser_o_usergroup,
                            PermissionType.Value))
                        .Id,
                    EntityId = userToUserGroupMappingPublic.Id
                },
                new()
                {
                    Value = BitConverter.GetBytes(Consts.FalseValue),
                    PermissionId = (await _permissionEntityService.GetByAliasTypeAsync(
                            Consts.PermissionAlias.g_group_a_kickuser_o_usergroup,
                            PermissionType.Value))
                        .Id,
                    EntityId = userToUserGroupMappingPublic.Id
                },

                #endregion

                #region GroupMember

                new()
                {
                    Value = BitConverter.GetBytes(Consts.TrueValue),
                    PermissionId = (await _permissionEntityService.GetByAliasTypeAsync(
                            Consts.PermissionAlias.g_group_a_inviteuser_o_usergroup,
                            PermissionType.Value))
                        .Id,
                    EntityId = userToUserGroupMappingGroupMember.Id
                },
                new()
                {
                    Value = BitConverter.GetBytes(Consts.FalseValue),
                    PermissionId = (await _permissionEntityService.GetByAliasTypeAsync(
                            Consts.PermissionAlias.g_group_a_kickuser_o_usergroup,
                            PermissionType.Value))
                        .Id,
                    EntityId = userToUserGroupMappingGroupMember.Id
                },

                #endregion
            };
            
            await _permissionValueEntityCollectionService.Save(userToUserGroupMappingPermissionValues, cancellationToken);

            await _appDbContextAction.CommitTransactionAsync();

            _logger.Log(LogLevel.Information, Localize.Log.MethodEnd(GetType(), nameof(Create)));

            return _mapper.Map<UserGroupCreateResultDto>(userGroup);
        }
        catch (Exception)
        {
            await _appDbContextAction.RollbackTransactionAsync();

            throw;
        }
    }

    public async Task<IDtoResultBase> Read(UserGroupReadDto data, CancellationToken cancellationToken = default)
    {
        _logger.Log(LogLevel.Information, Localize.Log.MethodStart(GetType(), nameof(Read)));

        if (ValidateModel(data) is { } validationResult)
            return validationResult;

        try
        {
            await _appDbContextAction.BeginTransactionAsync();

            var user = await _userAdvancedService.GetFromHttpContext(cancellationToken);
            if (user == null)
                throw new HttpResponseException(StatusCodes.Status500InternalServerError, ErrorType.HttpContext,
                    Localize.Error.UserNotFoundOrHttpContextMissingClaims);

            var userGroup = await _userGroupEntityService.GetByIdAsync(data.Id, cancellationToken);
            if (userGroup == null)
                throw new HttpResponseException(StatusCodes.Status500InternalServerError, ErrorType.Generic,
                    Localize.Error.UserGroupNotFound);

            //Authorize group read
            var authorizeResult = _authorizeAdvancedService.Authorize(new AuthorizeModel
            {
                EntityLeftTableName = _userRepository.GetTableName(),
                EntityLeftGroupsTableName = _userGroupRepository.GetTableName(),
                EntityLeftEntityToEntityMappingsTableName = _userToUserGroupMappingRepository.GetTableName(),
                EntityLeftId = user.Id,
                EntityLeftPermissionAlias = Consts.PermissionAlias.g_group_a_read_o_usergroup,
                EntityRightTableName = _userGroupRepository.GetTableName(),
                EntityRightGroupsTableName = null,
                EntityRightEntityToEntityMappingsTableName = null,
                EntityRightId = userGroup.Id,
                EntityRightPermissionAlias = Consts.PermissionAlias.g_group_a_read_o_usergroup,
                SqlExpressionPermissionTypeValueNeededOwner = "T1.\"Id\" = T2.\"UserId\""
            });

            if (!authorizeResult)
                throw new HttpResponseException(StatusCodes.Status403Forbidden, ErrorType.Permission,
                    Localize.Error.PermissionInsufficientPermissions);

            await _appDbContextAction.CommitTransactionAsync();

            _logger.Log(LogLevel.Information, Localize.Log.MethodEnd(GetType(), nameof(Read)));

            return _mapper.Map<UserGroupReadResultDto>(userGroup);
        }
        catch (Exception)
        {
            await _appDbContextAction.RollbackTransactionAsync();

            throw;
        }
    }

    //TODO: If you ever want to sort groups by membercount, there needs to be a custom view created, and a separate repository for a view entity (groups do not store membercount themselves)
    public async Task<IDtoResultBase> ReadFSPCollection(
        UserGroupReadEntityCollectionDto data,
        CancellationToken cancellationToken = default
    )
    {
        _logger.Log(LogLevel.Information, Localize.Log.MethodStart(GetType(), nameof(Read)));

        if (ValidateModel(data) is { } validationResult)
            return validationResult;

        try
        {
            await _appDbContextAction.BeginTransactionAsync();

            var user = await _userAdvancedService.GetFromHttpContext(cancellationToken);
            if (user == null)
                throw new HttpResponseException(StatusCodes.Status500InternalServerError, ErrorType.HttpContext,
                    Localize.Error.UserNotFoundOrHttpContextMissingClaims);

            var userGroups =
                await _userGroupEntityService.GetFilteredSortedPaged(data.FilterExpressionModel, data.FilterSortModel,
                    data.PageModel, new AuthorizeModel
                    {
                        EntityLeftTableName = _userRepository.GetTableName(),
                        EntityLeftGroupsTableName = _userGroupRepository.GetTableName(),
                        EntityLeftEntityToEntityMappingsTableName = _userToUserGroupMappingRepository.GetTableName(),
                        EntityLeftId = user.Id,
                        EntityLeftPermissionAlias = Consts.PermissionAlias.g_group_a_read_o_usergroup,
                        EntityRightTableName = _userGroupRepository.GetTableName(),
                        EntityRightGroupsTableName = null,
                        EntityRightEntityToEntityMappingsTableName = null,
                        EntityRightIdRawSql = "\"Id\"",
                        EntityRightPermissionAlias = Consts.PermissionAlias.g_group_a_read_o_usergroup,
                        SqlExpressionPermissionTypeValueNeededOwner = "T1.\"Id\" = T2.\"UserId\""
                    }, cancellationToken: cancellationToken);

            await _appDbContextAction.CommitTransactionAsync();

            _logger.Log(LogLevel.Information, Localize.Log.MethodEnd(GetType(), nameof(Read)));

            return new UserGroupReadDtoReadFSPCollectionResultDto
            {
                Total = userGroups.total,
                Items = userGroups.entities.Select(_ =>
                    _mapper.ProjectTo<UserGroupReadDtoReadFSPCollectionItemResultDto>(new[] {_}.AsQueryable()).Single())
            };
        }
        catch (Exception)
        {
            await _appDbContextAction.RollbackTransactionAsync();

            throw;
        }
    }

    public async Task<IDtoResultBase> Update(UserGroupUpdateDto data, CancellationToken cancellationToken = default)
    {
        _logger.Log(LogLevel.Information, Localize.Log.MethodStart(GetType(), nameof(Update)));

        if (ValidateModel(data) is { } validationResult)
            return validationResult;

        try
        {
            await _appDbContextAction.BeginTransactionAsync();

            var user = await _userAdvancedService.GetFromHttpContext(cancellationToken);
            if (user == null)
                throw new HttpResponseException(StatusCodes.Status500InternalServerError, ErrorType.HttpContext,
                    Localize.Error.UserNotFoundOrHttpContextMissingClaims);

            var userGroup = await _userGroupEntityService.GetByIdAsync(data.Id, cancellationToken);
            if (userGroup == null)
                throw new HttpResponseException(StatusCodes.Status500InternalServerError, ErrorType.Generic,
                    Localize.Error.UserGroupNotFound);

            //Authorize group update
            var authorizeResult = _authorizeAdvancedService.Authorize(new AuthorizeModel
            {
                EntityLeftTableName = _userRepository.GetTableName(),
                EntityLeftGroupsTableName = _userGroupRepository.GetTableName(),
                EntityLeftEntityToEntityMappingsTableName = _userToUserGroupMappingRepository.GetTableName(),
                EntityLeftId = user.Id,
                EntityLeftPermissionAlias = Consts.PermissionAlias.g_group_a_update_o_usergroup,
                EntityRightTableName = _userGroupRepository.GetTableName(),
                EntityRightGroupsTableName = null,
                EntityRightEntityToEntityMappingsTableName = null,
                EntityRightId = userGroup.Id,
                EntityRightPermissionAlias = Consts.PermissionAlias.g_group_a_update_o_usergroup,
                SqlExpressionPermissionTypeValueNeededOwner = "T1.\"Id\" = T2.\"UserId\""
            });

            if (!authorizeResult)
                throw new HttpResponseException(StatusCodes.Status403Forbidden, ErrorType.Permission,
                    Localize.Error.PermissionInsufficientPermissions);

            var autoMapperModelAuthorizeData = new AutoMapperModelAuthorizeData
            {
                FieldAuthorizeResultDictionary = new Dictionary<string, bool>
                {
                    {
                        nameof(UserGroup.Alias),
                        _authorizeAdvancedService.Authorize(new AuthorizeModel
                        {
                            EntityLeftTableName = _userRepository.GetTableName(),
                            EntityLeftGroupsTableName = _userGroupRepository.GetTableName(),
                            EntityLeftEntityToEntityMappingsTableName =
                                _userToUserGroupMappingRepository.GetTableName(),
                            EntityLeftId = user.Id,
                            EntityLeftPermissionAlias =
                                Consts.PermissionAlias.g_group_a_create_o_usergroup_o_alias_l_automapper,
                            EntityRightTableName = _userRepository.GetTableName(),
                            EntityRightGroupsTableName = _userGroupRepository.GetTableName(),
                            EntityRightEntityToEntityMappingsTableName =
                                _userToUserGroupMappingRepository.GetTableName(),
                            EntityRightId = userGroup.UserId,
                            EntityRightPermissionAlias =
                                Consts.PermissionAlias.g_group_a_create_o_usergroup_o_alias_l_automapper,
                            SqlExpressionPermissionTypeValueNeededOwner = "T1.\"Id\" = T2.\"Id\""
                        })
                    },
                    {
                        nameof(UserGroup.Description),
                        _authorizeAdvancedService.Authorize(new AuthorizeModel
                        {
                            EntityLeftTableName = _userRepository.GetTableName(),
                            EntityLeftGroupsTableName = _userGroupRepository.GetTableName(),
                            EntityLeftEntityToEntityMappingsTableName =
                                _userToUserGroupMappingRepository.GetTableName(),
                            EntityLeftId = user.Id,
                            EntityLeftPermissionAlias =
                                Consts.PermissionAlias.g_group_a_create_o_usergroup_o_description_l_automapper,
                            EntityRightTableName = _userRepository.GetTableName(),
                            EntityRightGroupsTableName = _userGroupRepository.GetTableName(),
                            EntityRightEntityToEntityMappingsTableName =
                                _userToUserGroupMappingRepository.GetTableName(),
                            EntityRightId = userGroup.UserId,
                            EntityRightPermissionAlias =
                                Consts.PermissionAlias.g_group_a_create_o_usergroup_o_description_l_automapper,
                            SqlExpressionPermissionTypeValueNeededOwner = "T1.\"Id\" = T2.\"Id\""
                        })
                    },
                    {
                        nameof(UserGroup.Priority),
                        _authorizeAdvancedService.Authorize(new AuthorizeModel
                        {
                            EntityLeftTableName = _userRepository.GetTableName(),
                            EntityLeftGroupsTableName = _userGroupRepository.GetTableName(),
                            EntityLeftEntityToEntityMappingsTableName =
                                _userToUserGroupMappingRepository.GetTableName(),
                            EntityLeftId = user.Id,
                            EntityLeftPermissionAlias =
                                Consts.PermissionAlias.g_group_a_create_o_usergroup_o_priority_l_automapper,
                            EntityRightTableName = _userRepository.GetTableName(),
                            EntityRightGroupsTableName = _userGroupRepository.GetTableName(),
                            EntityRightEntityToEntityMappingsTableName =
                                _userToUserGroupMappingRepository.GetTableName(),
                            EntityRightId = userGroup.UserId,
                            EntityRightPermissionAlias =
                                Consts.PermissionAlias.g_group_a_create_o_usergroup_o_priority_l_automapper,
                            SqlExpressionPermissionTypeValueNeededOwner = "T1.\"Id\" = T2.\"Id\""
                        })
                    }
                }
            };

            if (!autoMapperModelAuthorizeData.FieldAuthorizeResultDictionary.TryGetValue(nameof(UserGroup.Priority),
                    out var userGroupPriorityAuthorizeResult) || !userGroupPriorityAuthorizeResult)
            {
                var rand = RandomNumberGenerator.Create();
                long priority = 0;
                var buffer = new byte[8];

                while (priority >= 0 || await _userGroupEntityService.GetIsPriorityClaimed(priority))
                {
                    rand.GetBytes(buffer);
                    priority = BitConverter.ToInt64(buffer);
                }

                data.Priority = priority;
                autoMapperModelAuthorizeData.FieldAuthorizeResultDictionary[nameof(UserGroup.Priority)] = true;
            }

            _mapper.Map(data, userGroup,
                opts => { opts.Items[Consts.AutoMapperModelAuthorizeDataKey] = autoMapperModelAuthorizeData; });

            await _userGroupEntityService.Save(userGroup, cancellationToken);

            await _appDbContextAction.CommitTransactionAsync();

            _logger.Log(LogLevel.Information, Localize.Log.MethodEnd(GetType(), nameof(Update)));

            return _mapper.Map<UserGroupUpdateResultDto>(userGroup);
        }
        catch (Exception)
        {
            await _appDbContextAction.RollbackTransactionAsync();

            throw;
        }
    }

    public async Task<IDtoResultBase> Delete(UserGroupDeleteDto data, CancellationToken cancellationToken = default)
    {
        _logger.Log(LogLevel.Information, Localize.Log.MethodStart(GetType(), nameof(Delete)));

        if (ValidateModel(data) is { } validationResult)
            return validationResult;

        try
        {
            await _appDbContextAction.BeginTransactionAsync();

            var user = await _userAdvancedService.GetFromHttpContext(cancellationToken);
            if (user == null)
                throw new HttpResponseException(StatusCodes.Status500InternalServerError, ErrorType.HttpContext,
                    Localize.Error.UserNotFoundOrHttpContextMissingClaims);

            var userGroup = await _userGroupEntityService.GetByIdAsync(data.Id, cancellationToken);
            if (userGroup == null)
                throw new HttpResponseException(StatusCodes.Status500InternalServerError, ErrorType.Generic,
                    Localize.Error.UserGroupNotFound);

            //Authorize group delete
            var authorizeResult = _authorizeAdvancedService.Authorize(new AuthorizeModel
            {
                EntityLeftTableName = _userRepository.GetTableName(),
                EntityLeftGroupsTableName = _userGroupRepository.GetTableName(),
                EntityLeftEntityToEntityMappingsTableName = _userToUserGroupMappingRepository.GetTableName(),
                EntityLeftId = user.Id,
                EntityLeftPermissionAlias = Consts.PermissionAlias.g_group_a_delete_o_usergroup,
                EntityRightTableName = _userGroupRepository.GetTableName(),
                EntityRightGroupsTableName = null,
                EntityRightEntityToEntityMappingsTableName = null,
                EntityRightId = userGroup.Id,
                EntityRightPermissionAlias = Consts.PermissionAlias.g_group_a_delete_o_usergroup,
                SqlExpressionPermissionTypeValueNeededOwner = "T1.\"Id\" = T2.\"UserId\""
            });

            if (!authorizeResult)
                throw new HttpResponseException(StatusCodes.Status403Forbidden, ErrorType.Permission,
                    Localize.Error.PermissionInsufficientPermissions);

            await _permissionValueEntityCollectionService.PurgeAsync(userGroup.Id, cancellationToken);
            
            for (var page = 1;; page += 1)
            {
                var entities = await _userToUserGroupMappingEntityService.GetByUserGroupIdAsync(userGroup.Id,
                    new PageModel
                    {
                        Page = page,
                        PageSize = 512
                    }, cancellationToken);

                foreach (var userToUserGroupMapping in entities)
                    await _permissionValueEntityCollectionService.PurgeAsync(userToUserGroupMapping.Id, cancellationToken);

                await _appDbContextAction.CommitAsync(cancellationToken);

                if (entities.Count < 512)
                    break;
            }

            await _userGroupEntityService.Delete(userGroup, cancellationToken);

            await _appDbContextAction.CommitTransactionAsync();

            _logger.Log(LogLevel.Information, Localize.Log.MethodEnd(GetType(), nameof(Delete)));

            return new OkResultDto();
        }
        catch (Exception)
        {
            await _appDbContextAction.RollbackTransactionAsync();

            throw;
        }
    }

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
                throw new HttpResponseException(StatusCodes.Status500InternalServerError, ErrorType.HttpContext,
                    Localize.Error.UserNotFoundOrHttpContextMissingClaims);

            var userGroup = await _userGroupEntityService.GetByIdAsync(data.Id, cancellationToken);
            if (userGroup == null)
                throw new HttpResponseException(StatusCodes.Status500InternalServerError, ErrorType.Generic,
                    Localize.Error.UserGroupNotFound);

            //Authorize group join
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

            await _userToUserGroupMappingEntityService.Save(new UserToUserGroupMapping
            {
                EntityLeftId = user.Id,
                EntityRightId = userGroup.Id
            }, cancellationToken);
            
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
                throw new HttpResponseException(StatusCodes.Status500InternalServerError, ErrorType.HttpContext,
                    Localize.Error.UserNotFoundOrHttpContextMissingClaims);

            var userGroup = await _userGroupEntityService.GetByIdAsync(data.Id, cancellationToken);
            if (userGroup == null)
                throw new HttpResponseException(StatusCodes.Status500InternalServerError, ErrorType.Generic,
                    Localize.Error.UserGroupNotFound);

            //Authorize group leave
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
                throw new HttpResponseException(StatusCodes.Status500InternalServerError, ErrorType.Generic,
                    Localize.Error.UserToUserGroupMappingNotFound);
            
            await _permissionValueEntityCollectionService.PurgeAsync(userToUserGroupMapping.Id, cancellationToken);

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
    
    //TODO: UserGroupTransferRequestHandler
    
    public async Task<IDtoResultBase> InitTransfer(UserGroupTransferInitDto data, CancellationToken cancellationToken = default)
    {
        _logger.Log(LogLevel.Information, Localize.Log.MethodStart(GetType(), nameof(Update)));

        if (ValidateModel(data) is { } validationResult)
            return validationResult;
        
        try
        {
            await _appDbContextAction.BeginTransactionAsync();

            var user = await _userAdvancedService.GetFromHttpContext(cancellationToken);
            if (user == null)
                throw new HttpResponseException(StatusCodes.Status500InternalServerError, ErrorType.HttpContext,
                    Localize.Error.UserNotFoundOrHttpContextMissingClaims);

            var userGroup = await _userGroupEntityService.GetByIdAsync(data.Id, cancellationToken);
            if (userGroup == null)
                throw new HttpResponseException(StatusCodes.Status500InternalServerError, ErrorType.Generic,
                    Localize.Error.UserGroupNotFound);

            var userTarget = await _userEntityService.GetByIdAsync(data.UserId, cancellationToken);
            if (userTarget == null)
                throw new HttpResponseException(StatusCodes.Status500InternalServerError, ErrorType.HttpContext,
                    Localize.Error.UserNotFound);

            //Authorize group transfer
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
            
            // authorizeResult &= _authorizeAdvancedService.Authorize(new AuthorizeModel
            // {
            //     EntityLeftTableName = _userRepository.GetTableName(),
            //     EntityLeftGroupsTableName = _userGroupRepository.GetTableName(),
            //     EntityLeftEntityToEntityMappingsTableName = _userToUserGroupMappingRepository.GetTableName(),
            //     EntityLeftId = user.Id,
            //     EntityLeftPermissionAlias = Consts.PermissionAlias.g_group_a_transfer_o_usergroup,
            //     EntityRightTableName = _userRepository.GetTableName(),
            //     EntityRightGroupsTableName = _userGroupRepository.GetTableName(),
            //     EntityRightEntityToEntityMappingsTableName = _userToUserGroupMappingRepository.GetTableName(),
            //     EntityRightId = userTarget.Id,
            //     EntityRightPermissionAlias = Consts.PermissionAlias.g_group_a_transfer_o_usergroup,
            //     SqlExpressionPermissionTypeValueNeededOwner = "T1.\"Id\" = T2.\"Id\""
            // });

            if (!authorizeResult)
                throw new HttpResponseException(StatusCodes.Status403Forbidden, ErrorType.Permission,
                    Localize.Error.PermissionInsufficientPermissions);

            var userGroupTransferRequest = _mapper.Map<UserGroupTransferRequest>(data);

            userGroupTransferRequest.SrcUserId = user.Id;

            await _userGroupTransferRequestEntityService.Save(userGroupTransferRequest, cancellationToken);

            await _appDbContextAction.CommitTransactionAsync();

            _logger.Log(LogLevel.Information, Localize.Log.MethodEnd(GetType(), nameof(Update)));

            return _mapper.Map<UserGroupUpdateResultDto>(userGroup);
        }
        catch (Exception)
        {
            await _appDbContextAction.RollbackTransactionAsync();

            throw;
        }
    }

    public async Task<IDtoResultBase> ManageTransfer(UserGroupTransferManageDto data, CancellationToken cancellationToken = default)
    {
         _logger.Log(LogLevel.Information, Localize.Log.MethodStart(GetType(), nameof(Update)));

        if (ValidateModel(data) is { } validationResult)
            return validationResult;
        
        try
        {
            await _appDbContextAction.BeginTransactionAsync();

            var user = await _userAdvancedService.GetFromHttpContext(cancellationToken);
            if (user == null)
                throw new HttpResponseException(StatusCodes.Status500InternalServerError, ErrorType.HttpContext,
                    Localize.Error.UserNotFoundOrHttpContextMissingClaims);
            
            var userGroupTransferRequest = await _userGroupTransferRequestEntityService.GetByIdAsync(data.Id, cancellationToken);
            if (userGroupTransferRequest == null)
                throw new HttpResponseException(StatusCodes.Status500InternalServerError, ErrorType.Generic,
                    Localize.Error.UserGroupTransferRequestNotFound);
                    
            var userGroup = await _userGroupEntityService.GetByIdAsync(userGroupTransferRequest.UserGroupId, cancellationToken);
            if (userGroup == null)
                throw new HttpResponseException(StatusCodes.Status500InternalServerError, ErrorType.Generic,
                    Localize.Error.UserGroupNotFound);

            if (user.Id == userGroupTransferRequest.SrcUserId)
            {
                switch (data.Choice)
                {
                    case Choice.None:
                        break;
                    case Choice.Accept:
                        throw new HttpResponseException(StatusCodes.Status500InternalServerError, ErrorType.Generic,
                            Localize.Error.UserGroupTransferRequestChoiceNotAllowed);
                        break;
                    case Choice.Reject:
                        await _userGroupTransferRequestEntityService.Delete(userGroupTransferRequest, cancellationToken);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            } 
            else if (user.Id == userGroupTransferRequest.DestUserId)
            {
                switch (data.Choice)
                {
                    case Choice.None:
                        break;
                    case Choice.Accept:
                    {
                        var userToUserGroupMapping =
                            await _userToUserGroupMappingEntityService.GetByUserIdUserGroupIdAsync(userGroup.UserId, userGroup.Id);
                        if (userToUserGroupMapping == null)
                            throw new HttpResponseException(StatusCodes.Status500InternalServerError, ErrorType.Generic,
                                Localize.Error.UserToUserGroupMappingNotFound);
                        
                        await _permissionValueEntityCollectionService.PurgeAsync(userToUserGroupMapping.Id, cancellationToken);
                        
                        userGroup.UserId = userGroupTransferRequest.DestUserId;
                        await _userGroupEntityService.Save(userGroup, cancellationToken);
                        await _userGroupTransferRequestEntityService.Delete(userGroupTransferRequest,
                            cancellationToken);
                        break;
                    }
                    case Choice.Reject:
                        await _userGroupTransferRequestEntityService.Delete(userGroupTransferRequest, cancellationToken);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            else
            {
                //TODO: Root somehow must also access it
                throw new HttpResponseException(StatusCodes.Status500InternalServerError, ErrorType.Generic,
                    Localize.Error.UserGroupTransferRequestNotFound);
            }

            await _appDbContextAction.CommitTransactionAsync();

            _logger.Log(LogLevel.Information, Localize.Log.MethodEnd(GetType(), nameof(Update)));

            return new OkResultDto();
        }
        catch (Exception)
        {
            await _appDbContextAction.RollbackTransactionAsync();

            throw;
        }
    }

    //TODO: UserGroupInviteRequestHandler
    
    public async Task<IDtoResultBase> InitInviteUser(UserGroupInitInviteUserDto data, CancellationToken cancellationToken = default)
    {
        _logger.Log(LogLevel.Information, Localize.Log.MethodStart(GetType(), nameof(Read)));

        if (ValidateModel(data) is { } validationResult)
            return validationResult;

        try
        {
            await _appDbContextAction.BeginTransactionAsync();
            
            var user = await _userAdvancedService.GetFromHttpContext(cancellationToken);
            if (user == null)
                throw new HttpResponseException(StatusCodes.Status500InternalServerError, ErrorType.HttpContext,
                    Localize.Error.UserNotFoundOrHttpContextMissingClaims);

            var userGroup = await _userGroupEntityService.GetByIdAsync(data.Id, cancellationToken);
            if (userGroup == null)
                throw new HttpResponseException(StatusCodes.Status500InternalServerError, ErrorType.Generic,
                    Localize.Error.UserGroupNotFound);
            
            var userTarget = await _userAdvancedService.GetFromHttpContext(cancellationToken);
            if (userTarget == null)
                throw new HttpResponseException(StatusCodes.Status500InternalServerError, ErrorType.HttpContext,
                    Localize.Error.UserNotFound);
            
            //Authorize group invite
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
            
            var userToUserGroupMapping =
                await _userToUserGroupMappingEntityService.GetByUserIdUserGroupIdAsync(user.Id, userGroup.Id);
            if (userToUserGroupMapping != null)
            {
                authorizeResult |= _authorizeAdvancedService.Authorize(new AuthorizeModel
                {
                    EntityLeftTableName = _userToUserGroupMappingRepository.GetTableName(),
                    EntityLeftGroupsTableName = null,
                    EntityLeftEntityToEntityMappingsTableName = null,
                    EntityLeftId = userToUserGroupMapping.Id,
                    EntityLeftPermissionAlias = Consts.PermissionAlias.g_group_a_inviteuser_o_usergroup,
                    EntityRightTableName = _userGroupRepository.GetTableName(),
                    EntityRightGroupsTableName = null,
                    EntityRightEntityToEntityMappingsTableName = null,
                    EntityRightId = userGroup.Id,
                    EntityRightPermissionAlias = Consts.PermissionAlias.g_group_a_inviteuser_o_usergroup,
                    SqlExpressionPermissionTypeValueNeededOwner = "FALSE"
                });
            }

            if (!authorizeResult)
                throw new HttpResponseException(StatusCodes.Status403Forbidden, ErrorType.Permission,
                    Localize.Error.PermissionInsufficientPermissions);
            
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
        _logger.Log(LogLevel.Information, Localize.Log.MethodStart(GetType(), nameof(Update)));

        if (ValidateModel(data) is { } validationResult)
            return validationResult;

        try
        {
            await _appDbContextAction.BeginTransactionAsync();

            var user = await _userAdvancedService.GetFromHttpContext(cancellationToken);
            if (user == null)
                throw new HttpResponseException(StatusCodes.Status500InternalServerError, ErrorType.HttpContext,
                    Localize.Error.UserNotFoundOrHttpContextMissingClaims);
            
            var userGroupInviteRequest = await _userGroupInviteRequestEntityService.GetByIdAsync(data.Id, cancellationToken);
            if (userGroupInviteRequest == null)
                throw new HttpResponseException(StatusCodes.Status500InternalServerError, ErrorType.Generic,
                    Localize.Error.UserGroupInviteRequestNotFound);
                    
            var userGroup = await _userGroupEntityService.GetByIdAsync(userGroupInviteRequest.UserGroupId, cancellationToken);
            if (userGroup == null)
                throw new HttpResponseException(StatusCodes.Status500InternalServerError, ErrorType.Generic,
                    Localize.Error.UserGroupNotFound);
            
            if (user.Id == userGroupInviteRequest.SrcUserId)
            {
                switch (data.Choice)
                {
                    case Choice.None:
                        break;
                    case Choice.Accept:
                        throw new HttpResponseException(StatusCodes.Status500InternalServerError, ErrorType.Generic,
                            Localize.Error.UserGroupTransferRequestChoiceNotAllowed);
                        break;
                    case Choice.Reject:
                        await _userGroupInviteRequestEntityService.Delete(userGroupInviteRequest, cancellationToken);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            } 
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
                        
                        await _userGroupInviteRequestEntityService.Delete(userGroupInviteRequest, cancellationToken);
                        break;
                    }
                    case Choice.Reject:
                        await _userGroupInviteRequestEntityService.Delete(userGroupInviteRequest, cancellationToken);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            else
            {
                //TODO: Root somehow must also access it
                throw new HttpResponseException(StatusCodes.Status500InternalServerError, ErrorType.Generic,
                    Localize.Error.UserGroupInviteRequestNotFound);
            }
            
            await _appDbContextAction.CommitTransactionAsync();

            _logger.Log(LogLevel.Information, Localize.Log.MethodEnd(GetType(), nameof(Update)));

            return new OkResultDto();
        }
        catch (Exception)
        {
            await _appDbContextAction.RollbackTransactionAsync();

            throw;
        }
    }
    
    //TODO: Fix Logging fn names

    //TODO: Task<DTOResultBase> AddUser(... data, CancellationToken cancellationToken = default) g_group_a_manage_o_usergroup_a_add_o_user
    public async Task<IDtoResultBase> AddUser(UserGroupAddUserDto data, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    //TODO: Task<DTOResultBase> KickUser(... data, CancellationToken cancellationToken = default) g_group_a_manage_o_usergroup_a_delete_o_user + via PermissionValues for UserToUserGroupMapping
    public async Task<IDtoResultBase> KickUser(UserGroupDeleteUserDto data, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    //TODO: Create PermissionValues for UserToUserGroupMapping to handle per user permissions inside group (boolean), invite, kick and more, those permissions are compared against g_any_true or g_any_false
    //TODO: UserToUserGroupMappingPermissionValueHandler

    #endregion
}