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
    private readonly IAuthorizeEntityService _authorizeEntityService;

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

            //Authorize g_group_a_create_o_usergroup against User
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

            //Authorize g_group_a_create_o_usergroup_* against User: Automapper
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

            //If user is not permitted to customize UserGroup.Priority, select next available
            if (!autoMapperModelAuthorizeData.FieldAuthorizeResultDictionary.TryGetValue(nameof(UserGroup.Priority),
                    out var userGroupPriorityAuthorizeResult) || !userGroupPriorityAuthorizeResult)
            {
                long priority = 0;

                while (priority >= 0 || await _userGroupEntityService.GetIsPriorityClaimed(priority))
                {
                    priority -= 1;
                }

                data.Priority = priority;
                autoMapperModelAuthorizeData.FieldAuthorizeResultDictionary[nameof(UserGroup.Priority)] = true;
            }

            //Create UserGroup entity and save it
            var userGroup = _mapper.Map<UserGroup>(data,
                opts => { opts.Items[Consts.AutoMapperModelAuthorizeDataKey] = autoMapperModelAuthorizeData; });
            
            await _userGroupEntityService.Save(userGroup, cancellationToken);

            //Create UserGroup PermissionValues and save them
            var userGroupPermissionValues = new PermissionValue[]
            {
                #region Any

                #region Create
                
                new()
                {
                    Value = BitConverter.GetBytes(Consts.TrueValue),
                    PermissionId = (await _permissionEntityService.GetByAliasTypeAsync(
                            Consts.PermissionAlias.g_any_a_create_o_permissionvalue,
                            PermissionType.ValueNeededOwner))
                        .Id,
                    EntityId = userGroup.Id
                },
                new()
                {
                    Value = BitConverter.GetBytes(Consts.RootUserGroupValue),
                    PermissionId = (await _permissionEntityService.GetByAliasTypeAsync(
                            Consts.PermissionAlias.g_any_a_create_o_permissionvalue,
                            PermissionType.ValueNeededOthers))
                        .Id,
                    EntityId = userGroup.Id
                },

                #endregion
                
                #region Read
                
                new()
                {
                    Value = BitConverter.GetBytes(Consts.TrueValue),
                    PermissionId = (await _permissionEntityService.GetByAliasTypeAsync(
                            Consts.PermissionAlias.g_any_a_read_o_permissionvalue,
                            PermissionType.ValueNeededOwner))
                        .Id,
                    EntityId = userGroup.Id
                },
                new()
                {
                    Value = BitConverter.GetBytes(Consts.RootUserGroupValue),
                    PermissionId = (await _permissionEntityService.GetByAliasTypeAsync(
                            Consts.PermissionAlias.g_any_a_read_o_permissionvalue,
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
                            Consts.PermissionAlias.g_any_a_update_o_permissionvalue,
                            PermissionType.ValueNeededOwner))
                        .Id,
                    EntityId = userGroup.Id
                },
                new()
                {
                    Value = BitConverter.GetBytes(Consts.RootUserGroupValue),
                    PermissionId = (await _permissionEntityService.GetByAliasTypeAsync(
                            Consts.PermissionAlias.g_any_a_update_o_permissionvalue,
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
                            Consts.PermissionAlias.g_any_a_delete_o_permissionvalue,
                            PermissionType.ValueNeededOwner))
                        .Id,
                    EntityId = userGroup.Id
                },
                new()
                {
                    Value = BitConverter.GetBytes(Consts.RootUserGroupValue),
                    PermissionId = (await _permissionEntityService.GetByAliasTypeAsync(
                            Consts.PermissionAlias.g_any_a_delete_o_permissionvalue,
                            PermissionType.ValueNeededOthers))
                        .Id,
                    EntityId = userGroup.Id
                },

                #endregion

                #endregion
                
                #region Group

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

                #region TransferUserGroup

                new()
                {
                    Value = BitConverter.GetBytes(Consts.TrueValue),
                    PermissionId = (await _permissionEntityService.GetByAliasTypeAsync(
                            Consts.PermissionAlias.g_group_a_transfer_o_usergroup,
                            PermissionType.Value))
                        .Id,
                    EntityId = userGroup.Id
                },
                new()
                {
                    Value = BitConverter.GetBytes(Consts.TrueValue),
                    PermissionId = (await _permissionEntityService.GetByAliasTypeAsync(
                            Consts.PermissionAlias.g_group_a_transfer_o_usergroup,
                            PermissionType.ValueNeededOwner))
                        .Id,
                    EntityId = userGroup.Id
                },
                new()
                {
                    Value = BitConverter.GetBytes(Consts.RootUserGroupValue),
                    PermissionId = (await _permissionEntityService.GetByAliasTypeAsync(
                            Consts.PermissionAlias.g_group_a_transfer_o_usergroup,
                            PermissionType.ValueNeededOthers))
                        .Id,
                    EntityId = userGroup.Id
                },

                #endregion

                #region TransferUserGroupManage

                new()
                {
                    Value = BitConverter.GetBytes(Consts.TrueValue),
                    PermissionId = (await _permissionEntityService.GetByAliasTypeAsync(
                            Consts.PermissionAlias.g_group_a_transfer_o_usergroup_a_manage,
                            PermissionType.Value))
                        .Id,
                    EntityId = userGroup.Id
                },
                new()
                {
                    Value = BitConverter.GetBytes(Consts.TrueValue),
                    PermissionId = (await _permissionEntityService.GetByAliasTypeAsync(
                            Consts.PermissionAlias.g_group_a_transfer_o_usergroup_a_manage,
                            PermissionType.ValueNeededOwner))
                        .Id,
                    EntityId = userGroup.Id
                },
                new()
                {
                    Value = BitConverter.GetBytes(Consts.RootUserGroupValue),
                    PermissionId = (await _permissionEntityService.GetByAliasTypeAsync(
                            Consts.PermissionAlias.g_group_a_transfer_o_usergroup_a_manage,
                            PermissionType.ValueNeededOthers))
                        .Id,
                    EntityId = userGroup.Id
                },

                #endregion

                #region InviteUser
                
                new()
                {
                    Value = BitConverter.GetBytes(Consts.TrueValue),
                    PermissionId = (await _permissionEntityService.GetByAliasTypeAsync(
                            Consts.PermissionAlias.g_group_a_inviteuser_o_usergroup,
                            PermissionType.Value))
                        .Id,
                    EntityId = userGroup.Id
                },
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
                    Value = BitConverter.GetBytes(Consts.RootUserGroupValue),
                    PermissionId = (await _permissionEntityService.GetByAliasTypeAsync(
                            Consts.PermissionAlias.g_group_a_inviteuser_o_usergroup,
                            PermissionType.ValueNeededOthers))
                        .Id,
                    EntityId = userGroup.Id
                },
                new()
                {
                    Value = BitConverter.GetBytes(Consts.TrueValue),
                    PermissionId = (await _permissionEntityService.GetByAliasTypeAsync(
                            Consts.PermissionAlias.g_ingroup_a_inviteuser_o_usergroup,
                            PermissionType.ValueNeededOwner))
                        .Id,
                    EntityId = userGroup.Id
                },
                new()
                {
                    Value = BitConverter.GetBytes(Consts.TrueValue),
                    PermissionId = (await _permissionEntityService.GetByAliasTypeAsync(
                            Consts.PermissionAlias.g_ingroup_a_inviteuser_o_usergroup,
                            PermissionType.ValueNeededOthers))
                        .Id,
                    EntityId = userGroup.Id
                },

                #endregion

                #region InviteUserManage

                new()
                {
                    Value = BitConverter.GetBytes(Consts.TrueValue),
                    PermissionId = (await _permissionEntityService.GetByAliasTypeAsync(
                            Consts.PermissionAlias.g_group_a_inviteuser_o_usergroup_a_manage,
                            PermissionType.Value))
                        .Id,
                    EntityId = userGroup.Id
                },
                new()
                {
                    Value = BitConverter.GetBytes(Consts.TrueValue),
                    PermissionId = (await _permissionEntityService.GetByAliasTypeAsync(
                            Consts.PermissionAlias.g_group_a_inviteuser_o_usergroup_a_manage,
                            PermissionType.ValueNeededOwner))
                        .Id,
                    EntityId = userGroup.Id
                },
                new()
                {
                    Value = BitConverter.GetBytes(Consts.RootUserGroupValue),
                    PermissionId = (await _permissionEntityService.GetByAliasTypeAsync(
                            Consts.PermissionAlias.g_group_a_inviteuser_o_usergroup_a_manage,
                            PermissionType.ValueNeededOthers))
                        .Id,
                    EntityId = userGroup.Id
                },
                new()
                {
                    Value = BitConverter.GetBytes(Consts.TrueValue),
                    PermissionId = (await _permissionEntityService.GetByAliasTypeAsync(
                            Consts.PermissionAlias.g_ingroup_a_inviteuser_o_usergroup_a_manage,
                            PermissionType.ValueNeededOwner))
                        .Id,
                    EntityId = userGroup.Id
                },
                new()
                {
                    Value = BitConverter.GetBytes(Consts.TrueValue),
                    PermissionId = (await _permissionEntityService.GetByAliasTypeAsync(
                            Consts.PermissionAlias.g_ingroup_a_inviteuser_o_usergroup_a_manage,
                            PermissionType.ValueNeededOthers))
                        .Id,
                    EntityId = userGroup.Id
                },

                #endregion

                #region KickUser

                new()
                {
                    Value = BitConverter.GetBytes(Consts.TrueValue),
                    PermissionId = (await _permissionEntityService.GetByAliasTypeAsync(
                            Consts.PermissionAlias.g_group_a_kickuser_o_usergroup,
                            PermissionType.Value))
                        .Id,
                    EntityId = userGroup.Id
                },
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
                    Value = BitConverter.GetBytes(Consts.RootUserGroupValue),
                    PermissionId = (await _permissionEntityService.GetByAliasTypeAsync(
                            Consts.PermissionAlias.g_group_a_kickuser_o_usergroup,
                            PermissionType.ValueNeededOthers))
                        .Id,
                    EntityId = userGroup.Id
                },
                new()
                {
                    Value = BitConverter.GetBytes(Consts.TrueValue),
                    PermissionId = (await _permissionEntityService.GetByAliasTypeAsync(
                            Consts.PermissionAlias.g_ingroup_a_kickuser_o_usergroup,
                            PermissionType.ValueNeededOwner))
                        .Id,
                    EntityId = userGroup.Id
                },
                new()
                {
                    Value = BitConverter.GetBytes(Consts.TrueValue),
                    PermissionId = (await _permissionEntityService.GetByAliasTypeAsync(
                            Consts.PermissionAlias.g_ingroup_a_kickuser_o_usergroup,
                            PermissionType.ValueNeededOthers))
                        .Id,
                    EntityId = userGroup.Id
                },

                #endregion

                #region AddUser

                new()
                {
                    Value = BitConverter.GetBytes(Consts.RootUserGroupValue),
                    PermissionId = (await _permissionEntityService.GetByAliasTypeAsync(
                            Consts.PermissionAlias.g_group_a_adduser_o_usergroup,
                            PermissionType.ValueNeededOwner))
                        .Id,
                    EntityId = userGroup.Id
                },
                new()
                {
                    Value = BitConverter.GetBytes(Consts.RootUserGroupValue),
                    PermissionId = (await _permissionEntityService.GetByAliasTypeAsync(
                            Consts.PermissionAlias.g_group_a_kickuser_o_usergroup,
                            PermissionType.ValueNeededOthers))
                        .Id,
                    EntityId = userGroup.Id
                },

                #endregion

                #endregion
            };

            await _permissionValueEntityCollectionService.Save(userGroupPermissionValues, cancellationToken);

            //Create UserToUserGroupMappings for (Owner, Public, GroupMember) and save them
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

            //Create UserToUserGroupMappings PermissionValues and save them
            var userToUserGroupMappingPermissionValues = new PermissionValue[]
            {
                #region Owner

                new()
                {
                    Value = BitConverter.GetBytes(Consts.TrueValue),
                    PermissionId = (await _permissionEntityService.GetByAliasTypeAsync(
                            Consts.PermissionAlias.g_ingroup_a_inviteuser_o_usergroup,
                            PermissionType.Value))
                        .Id,
                    EntityId = userToUserGroupMappingOwner.Id
                },
                new()
                {
                    Value = BitConverter.GetBytes(Consts.TrueValue),
                    PermissionId = (await _permissionEntityService.GetByAliasTypeAsync(
                            Consts.PermissionAlias.g_ingroup_a_kickuser_o_usergroup,
                            PermissionType.Value))
                        .Id,
                    EntityId = userToUserGroupMappingOwner.Id
                },
                new()
                {
                    Value = BitConverter.GetBytes(Consts.TrueValue),
                    PermissionId = (await _permissionEntityService.GetByAliasTypeAsync(
                            Consts.PermissionAlias.g_ingroup_a_inviteuser_o_usergroup_a_manage,
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
                            Consts.PermissionAlias.g_ingroup_a_inviteuser_o_usergroup,
                            PermissionType.Value))
                        .Id,
                    EntityId = userToUserGroupMappingPublic.Id
                },
                new()
                {
                    Value = BitConverter.GetBytes(Consts.FalseValue),
                    PermissionId = (await _permissionEntityService.GetByAliasTypeAsync(
                            Consts.PermissionAlias.g_ingroup_a_kickuser_o_usergroup,
                            PermissionType.Value))
                        .Id,
                    EntityId = userToUserGroupMappingPublic.Id
                },
                new()
                {
                    Value = BitConverter.GetBytes(Consts.FalseValue),
                    PermissionId = (await _permissionEntityService.GetByAliasTypeAsync(
                            Consts.PermissionAlias.g_ingroup_a_inviteuser_o_usergroup_a_manage,
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
                            Consts.PermissionAlias.g_ingroup_a_inviteuser_o_usergroup,
                            PermissionType.Value))
                        .Id,
                    EntityId = userToUserGroupMappingGroupMember.Id
                },
                new()
                {
                    Value = BitConverter.GetBytes(Consts.FalseValue),
                    PermissionId = (await _permissionEntityService.GetByAliasTypeAsync(
                            Consts.PermissionAlias.g_ingroup_a_kickuser_o_usergroup,
                            PermissionType.Value))
                        .Id,
                    EntityId = userToUserGroupMappingGroupMember.Id
                },
                new()
                {
                    Value = BitConverter.GetBytes(Consts.FalseValue),
                    PermissionId = (await _permissionEntityService.GetByAliasTypeAsync(
                            Consts.PermissionAlias.g_ingroup_a_inviteuser_o_usergroup_a_manage,
                            PermissionType.Value))
                        .Id,
                    EntityId = userToUserGroupMappingGroupMember.Id
                },

                #endregion
            };
            
            await _permissionValueEntityCollectionService.Save(userToUserGroupMappingPermissionValues, cancellationToken);
            
            //Delete Authorize cache when joining group (for the user that joined)
            await _authorizeEntityService.PurgeByEntityIdAsync(user.Id, cancellationToken);
            await _authorizeEntityService.PurgeByEntityIdAsync(Consts.PublicUserId, cancellationToken);
            await _authorizeEntityService.PurgeByEntityIdAsync(Consts.GroupMemberUserId, cancellationToken);

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

            //Authorize g_group_a_read_o_usergroup against UserGroup TODO: + against User (user that initiates action)
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
        _logger.Log(LogLevel.Information, Localize.Log.MethodStart(GetType(), nameof(ReadFSPCollection)));

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
                        EntityRightIdRawSql = "\"EntityId\"", //TODO: Id?
                        EntityRightPermissionAlias = Consts.PermissionAlias.g_group_a_read_o_usergroup,
                        SqlExpressionPermissionTypeValueNeededOwner = "T1.\"Id\" = T2.\"UserId\""
                    }, cancellationToken: cancellationToken);

            await _appDbContextAction.CommitTransactionAsync();

            _logger.Log(LogLevel.Information, Localize.Log.MethodEnd(GetType(), nameof(ReadFSPCollection)));

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

            //Authorize g_group_a_update_o_usergroup against UserGroup TODO: + against User (user that initiates action)
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

            //Authorize g_group_a_update_o_usergroup_* against UserGroup: Automapper TODO: + against User (user that initiates action)
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

            //If user is not permitted to customize UserGroup.Priority, select next available
            if (!autoMapperModelAuthorizeData.FieldAuthorizeResultDictionary.TryGetValue(nameof(UserGroup.Priority),
                    out var userGroupPriorityAuthorizeResult) || !userGroupPriorityAuthorizeResult)
            {
                long priority = 0;

                while (priority >= 0 || await _userGroupEntityService.GetIsPriorityClaimed(priority))
                {
                    priority -= 1;
                }

                data.Priority = priority;
                autoMapperModelAuthorizeData.FieldAuthorizeResultDictionary[nameof(UserGroup.Priority)] = true;
            }

            //Update UserGroup entity and save it
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

            //Authorize g_group_a_delete_o_usergroup against UserGroup TODO: + against User (user that initiates action)
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

            //Delete UserGroup PermissionValues associated with this group
            await _permissionValueEntityCollectionService.PurgeAsync(userGroup.Id, cancellationToken);
            
            //Delete Authorize cache when deleting group (for all Users and all UserToUserGroupMappings associated with this group)
            for (var page = 1;; page += 1)
            {
                var entities = await _userToUserGroupMappingEntityService.GetByUserGroupIdAsync(userGroup.Id,
                    new PageModel
                    {
                        Page = page,
                        PageSize = 512
                    }, cancellationToken);

                foreach (var userToUserGroupMapping in entities)
                {
                    await _authorizeEntityService.PurgeByEntityIdAsync(userToUserGroupMapping.Id, cancellationToken);
                    await _authorizeEntityService.PurgeByEntityIdAsync(userToUserGroupMapping.EntityLeftId, cancellationToken);
                }
                
                await _appDbContextAction.CommitAsync(cancellationToken);

                if (entities.Count < 512)
                    break;
            }
            
            //Delete UserToUserGroupMappings PermissionValues associated with this group
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
    
    #endregion
}