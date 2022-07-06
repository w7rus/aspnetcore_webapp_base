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

    Task<IDtoResultBase> ReadCollection(
        UserGroupReadCollectionDto data,
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
    private readonly IPermissionValueEntityService _permissionValueEntityService;
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

    public async Task<IDtoResultBase> Create(UserGroupCreateDto data, CancellationToken cancellationToken = default)
    {
        _logger.Log(LogLevel.Information, Localize.Log.MethodStart(GetType(), nameof(Create)));

        if (ValidateModel(data) is { } validationResult)
            return validationResult;

        try
        {
            await _appDbContextAction.BeginTransactionAsync();

            var user = await _userAdvancedService.GetFromHttpContext(cancellationToken: cancellationToken);
            if (user == null)
                throw new HttpResponseException(StatusCodes.Status400BadRequest, ErrorType.HttpContext,
                    Localize.Error.UserNotFoundOrHttpContextMissingClaims);

            var userOwner = await _userEntityService.GetByIdAsync(data.UserId, cancellationToken);
            if (userOwner == null)
                throw new HttpResponseException(StatusCodes.Status404NotFound, ErrorType.Generic,
                    Localize.Error.UserNotFound);

            //Authorize g_group_a_create_o_usergroup against User
            var authorizeResult = _authorizeAdvancedService.Authorize(new AuthorizeModel
            {
                EntityLeftTableName = _userRepository.GetTableName(),
                EntityLeftGroupsTableName = _userGroupRepository.GetTableName(),
                EntityLeftEntityToEntityMappingsTableName = _userToUserGroupMappingRepository.GetTableName(),
                EntityLeftId = user.Id,
                EntityLeftPermissionAlias = Consts.PermissionAlias.UserGroupCreate,
                EntityRightTableName = _userRepository.GetTableName(),
                EntityRightGroupsTableName = _userGroupRepository.GetTableName(),
                EntityRightEntityToEntityMappingsTableName =
                    _userToUserGroupMappingRepository.GetTableName(),
                EntityRightId = userOwner.Id,
                EntityRightPermissionAlias = Consts.PermissionAlias.UserGroupCreate,
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
                                Consts.PermissionAlias.UserGroupCreate_Alias,
                            EntityRightTableName = _userRepository.GetTableName(),
                            EntityRightGroupsTableName = _userGroupRepository.GetTableName(),
                            EntityRightEntityToEntityMappingsTableName =
                                _userToUserGroupMappingRepository.GetTableName(),
                            EntityRightId = userOwner.Id,
                            EntityRightPermissionAlias =
                                Consts.PermissionAlias.UserGroupCreate_Alias,
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
                                Consts.PermissionAlias.UserGroupCreate_Description,
                            EntityRightTableName = _userRepository.GetTableName(),
                            EntityRightGroupsTableName = _userGroupRepository.GetTableName(),
                            EntityRightEntityToEntityMappingsTableName =
                                _userToUserGroupMappingRepository.GetTableName(),
                            EntityRightId = userOwner.Id,
                            EntityRightPermissionAlias =
                                Consts.PermissionAlias.UserGroupCreate_Description,
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
                                Consts.PermissionAlias.UserGroupCreate_Priority,
                            EntityRightTableName = _userRepository.GetTableName(),
                            EntityRightGroupsTableName = _userGroupRepository.GetTableName(),
                            EntityRightEntityToEntityMappingsTableName =
                                _userToUserGroupMappingRepository.GetTableName(),
                            EntityRightId = userOwner.Id,
                            EntityRightPermissionAlias =
                                Consts.PermissionAlias.UserGroupCreate_Priority,
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
                            Consts.PermissionAlias.PermissionValueCreate,
                            PermissionType.ValueNeededOwner))
                        .Id,
                    EntityId = userGroup.Id
                },
                new()
                {
                    Value = BitConverter.GetBytes(Consts.RootUserGroupValue),
                    PermissionId = (await _permissionEntityService.GetByAliasTypeAsync(
                            Consts.PermissionAlias.PermissionValueCreate,
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
                            Consts.PermissionAlias.PermissionValueRead,
                            PermissionType.ValueNeededOwner))
                        .Id,
                    EntityId = userGroup.Id
                },
                new()
                {
                    Value = BitConverter.GetBytes(Consts.RootUserGroupValue),
                    PermissionId = (await _permissionEntityService.GetByAliasTypeAsync(
                            Consts.PermissionAlias.PermissionValueRead,
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
                            Consts.PermissionAlias.PermissionValueUpdate,
                            PermissionType.ValueNeededOwner))
                        .Id,
                    EntityId = userGroup.Id
                },
                new()
                {
                    Value = BitConverter.GetBytes(Consts.RootUserGroupValue),
                    PermissionId = (await _permissionEntityService.GetByAliasTypeAsync(
                            Consts.PermissionAlias.PermissionValueUpdate,
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
                            Consts.PermissionAlias.PermissionValueDelete,
                            PermissionType.ValueNeededOwner))
                        .Id,
                    EntityId = userGroup.Id
                },
                new()
                {
                    Value = BitConverter.GetBytes(Consts.RootUserGroupValue),
                    PermissionId = (await _permissionEntityService.GetByAliasTypeAsync(
                            Consts.PermissionAlias.PermissionValueDelete,
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
                            Consts.PermissionAlias.UserGroupRead,
                            PermissionType.ValueNeededOwner))
                        .Id,
                    EntityId = userGroup.Id
                },
                new()
                {
                    Value = BitConverter.GetBytes(Consts.RootUserGroupValue),
                    PermissionId = (await _permissionEntityService.GetByAliasTypeAsync(
                            Consts.PermissionAlias.UserGroupRead,
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
                            Consts.PermissionAlias.UserGroupUpdate,
                            PermissionType.ValueNeededOwner))
                        .Id,
                    EntityId = userGroup.Id
                },
                new()
                {
                    Value = BitConverter.GetBytes(Consts.RootUserGroupValue),
                    PermissionId = (await _permissionEntityService.GetByAliasTypeAsync(
                            Consts.PermissionAlias.UserGroupUpdate,
                            PermissionType.ValueNeededOthers))
                        .Id,
                    EntityId = userGroup.Id
                },
                new()
                {
                    Value = BitConverter.GetBytes(Consts.TrueValue),
                    PermissionId = (await _permissionEntityService.GetByAliasTypeAsync(
                            Consts.PermissionAlias.UserGroupUpdate_Alias,
                            PermissionType.ValueNeededOwner))
                        .Id,
                    EntityId = userGroup.Id
                },
                new()
                {
                    Value = BitConverter.GetBytes(Consts.RootUserGroupValue),
                    PermissionId = (await _permissionEntityService.GetByAliasTypeAsync(
                            Consts.PermissionAlias.UserGroupUpdate_Alias,
                            PermissionType.ValueNeededOthers))
                        .Id,
                    EntityId = userGroup.Id
                },
                new()
                {
                    Value = BitConverter.GetBytes(Consts.TrueValue),
                    PermissionId = (await _permissionEntityService.GetByAliasTypeAsync(
                            Consts.PermissionAlias.UserGroupUpdate_Description,
                            PermissionType.ValueNeededOwner))
                        .Id,
                    EntityId = userGroup.Id
                },
                new()
                {
                    Value = BitConverter.GetBytes(Consts.RootUserGroupValue),
                    PermissionId = (await _permissionEntityService.GetByAliasTypeAsync(
                            Consts.PermissionAlias.UserGroupUpdate_Description,
                            PermissionType.ValueNeededOthers))
                        .Id,
                    EntityId = userGroup.Id
                },
                new()
                {
                    Value = BitConverter.GetBytes(Consts.TrueValue),
                    PermissionId = (await _permissionEntityService.GetByAliasTypeAsync(
                            Consts.PermissionAlias.UserGroupUpdate_Priority,
                            PermissionType.ValueNeededOwner))
                        .Id,
                    EntityId = userGroup.Id
                },
                new()
                {
                    Value = BitConverter.GetBytes(Consts.RootUserGroupValue),
                    PermissionId = (await _permissionEntityService.GetByAliasTypeAsync(
                            Consts.PermissionAlias.UserGroupUpdate_Priority,
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
                            Consts.PermissionAlias.UserGroupDelete,
                            PermissionType.ValueNeededOwner))
                        .Id,
                    EntityId = userGroup.Id
                },
                new()
                {
                    Value = BitConverter.GetBytes(Consts.RootUserGroupValue),
                    PermissionId = (await _permissionEntityService.GetByAliasTypeAsync(
                            Consts.PermissionAlias.UserGroupDelete,
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
                            Consts.PermissionAlias.UserGroupJoin,
                            PermissionType.ValueNeededOwner))
                        .Id,
                    EntityId = userGroup.Id
                },
                new()
                {
                    Value = BitConverter.GetBytes(Consts.RootUserGroupValue),
                    PermissionId = (await _permissionEntityService.GetByAliasTypeAsync(
                            Consts.PermissionAlias.UserGroupJoin,
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
                            Consts.PermissionAlias.UserGroupLeave,
                            PermissionType.ValueNeededOwner))
                        .Id,
                    EntityId = userGroup.Id
                },
                new()
                {
                    Value = BitConverter.GetBytes(Consts.RootUserGroupValue),
                    PermissionId = (await _permissionEntityService.GetByAliasTypeAsync(
                            Consts.PermissionAlias.UserGroupLeave,
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
                            Consts.PermissionAlias.UserGroupTransferRequestCreate,
                            PermissionType.Value))
                        .Id,
                    EntityId = userGroup.Id
                },
                new()
                {
                    Value = BitConverter.GetBytes(Consts.TrueValue),
                    PermissionId = (await _permissionEntityService.GetByAliasTypeAsync(
                            Consts.PermissionAlias.UserGroupTransferRequestCreate,
                            PermissionType.ValueNeededOwner))
                        .Id,
                    EntityId = userGroup.Id
                },
                new()
                {
                    Value = BitConverter.GetBytes(Consts.RootUserGroupValue),
                    PermissionId = (await _permissionEntityService.GetByAliasTypeAsync(
                            Consts.PermissionAlias.UserGroupTransferRequestCreate,
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
                            Consts.PermissionAlias.UserGroupTransferRequestUpdate,
                            PermissionType.Value))
                        .Id,
                    EntityId = userGroup.Id
                },
                new()
                {
                    Value = BitConverter.GetBytes(Consts.TrueValue),
                    PermissionId = (await _permissionEntityService.GetByAliasTypeAsync(
                            Consts.PermissionAlias.UserGroupTransferRequestUpdate,
                            PermissionType.ValueNeededOwner))
                        .Id,
                    EntityId = userGroup.Id
                },
                new()
                {
                    Value = BitConverter.GetBytes(Consts.RootUserGroupValue),
                    PermissionId = (await _permissionEntityService.GetByAliasTypeAsync(
                            Consts.PermissionAlias.UserGroupTransferRequestUpdate,
                            PermissionType.ValueNeededOthers))
                        .Id,
                    EntityId = userGroup.Id
                },

                #endregion
                
                #region TransferUserGroupRead

                new()
                {
                    Value = BitConverter.GetBytes(Consts.TrueValue),
                    PermissionId = (await _permissionEntityService.GetByAliasTypeAsync(
                            Consts.PermissionAlias.UserGroupTransferRequestRead,
                            PermissionType.Value))
                        .Id,
                    EntityId = userGroup.Id
                },
                new()
                {
                    Value = BitConverter.GetBytes(Consts.TrueValue),
                    PermissionId = (await _permissionEntityService.GetByAliasTypeAsync(
                            Consts.PermissionAlias.UserGroupTransferRequestRead,
                            PermissionType.ValueNeededOwner))
                        .Id,
                    EntityId = userGroup.Id
                },
                new()
                {
                    Value = BitConverter.GetBytes(Consts.RootUserGroupValue),
                    PermissionId = (await _permissionEntityService.GetByAliasTypeAsync(
                            Consts.PermissionAlias.UserGroupTransferRequestRead,
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
                            Consts.PermissionAlias.UserGroupInviteRequestCreate,
                            PermissionType.Value))
                        .Id,
                    EntityId = userGroup.Id
                },
                new()
                {
                    Value = BitConverter.GetBytes(Consts.TrueValue),
                    PermissionId = (await _permissionEntityService.GetByAliasTypeAsync(
                            Consts.PermissionAlias.UserGroupInviteRequestCreate,
                            PermissionType.ValueNeededOwner))
                        .Id,
                    EntityId = userGroup.Id
                },
                new()
                {
                    Value = BitConverter.GetBytes(Consts.RootUserGroupValue),
                    PermissionId = (await _permissionEntityService.GetByAliasTypeAsync(
                            Consts.PermissionAlias.UserGroupInviteRequestCreate,
                            PermissionType.ValueNeededOthers))
                        .Id,
                    EntityId = userGroup.Id
                },
                new()
                {
                    Value = BitConverter.GetBytes(Consts.TrueValue),
                    PermissionId = (await _permissionEntityService.GetByAliasTypeAsync(
                            Consts.PermissionAlias.UserGroupMemberInviteRequestCreate,
                            PermissionType.ValueNeededOwner))
                        .Id,
                    EntityId = userGroup.Id
                },
                new()
                {
                    Value = BitConverter.GetBytes(Consts.TrueValue),
                    PermissionId = (await _permissionEntityService.GetByAliasTypeAsync(
                            Consts.PermissionAlias.UserGroupMemberInviteRequestCreate,
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
                            Consts.PermissionAlias.UserGroupInviteRequestUpdate,
                            PermissionType.Value))
                        .Id,
                    EntityId = userGroup.Id
                },
                new()
                {
                    Value = BitConverter.GetBytes(Consts.TrueValue),
                    PermissionId = (await _permissionEntityService.GetByAliasTypeAsync(
                            Consts.PermissionAlias.UserGroupInviteRequestUpdate,
                            PermissionType.ValueNeededOwner))
                        .Id,
                    EntityId = userGroup.Id
                },
                new()
                {
                    Value = BitConverter.GetBytes(Consts.RootUserGroupValue),
                    PermissionId = (await _permissionEntityService.GetByAliasTypeAsync(
                            Consts.PermissionAlias.UserGroupInviteRequestUpdate,
                            PermissionType.ValueNeededOthers))
                        .Id,
                    EntityId = userGroup.Id
                },
                new()
                {
                    Value = BitConverter.GetBytes(Consts.TrueValue),
                    PermissionId = (await _permissionEntityService.GetByAliasTypeAsync(
                            Consts.PermissionAlias.UserGroupMemberInviteRequestUpdate,
                            PermissionType.ValueNeededOwner))
                        .Id,
                    EntityId = userGroup.Id
                },
                new()
                {
                    Value = BitConverter.GetBytes(Consts.TrueValue),
                    PermissionId = (await _permissionEntityService.GetByAliasTypeAsync(
                            Consts.PermissionAlias.UserGroupMemberInviteRequestUpdate,
                            PermissionType.ValueNeededOthers))
                        .Id,
                    EntityId = userGroup.Id
                },

                #endregion
                
                #region InviteUserRead

                new()
                {
                    Value = BitConverter.GetBytes(Consts.TrueValue),
                    PermissionId = (await _permissionEntityService.GetByAliasTypeAsync(
                            Consts.PermissionAlias.UserGroupInviteRequestRead,
                            PermissionType.Value))
                        .Id,
                    EntityId = userGroup.Id
                },
                new()
                {
                    Value = BitConverter.GetBytes(Consts.TrueValue),
                    PermissionId = (await _permissionEntityService.GetByAliasTypeAsync(
                            Consts.PermissionAlias.UserGroupInviteRequestRead,
                            PermissionType.ValueNeededOwner))
                        .Id,
                    EntityId = userGroup.Id
                },
                new()
                {
                    Value = BitConverter.GetBytes(Consts.RootUserGroupValue),
                    PermissionId = (await _permissionEntityService.GetByAliasTypeAsync(
                            Consts.PermissionAlias.UserGroupInviteRequestRead,
                            PermissionType.ValueNeededOthers))
                        .Id,
                    EntityId = userGroup.Id
                },
                new()
                {
                    Value = BitConverter.GetBytes(Consts.TrueValue),
                    PermissionId = (await _permissionEntityService.GetByAliasTypeAsync(
                            Consts.PermissionAlias.UserGroupMemberInviteRequestRead,
                            PermissionType.ValueNeededOwner))
                        .Id,
                    EntityId = userGroup.Id
                },
                new()
                {
                    Value = BitConverter.GetBytes(Consts.TrueValue),
                    PermissionId = (await _permissionEntityService.GetByAliasTypeAsync(
                            Consts.PermissionAlias.UserGroupMemberInviteRequestRead,
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
                            Consts.PermissionAlias.UserGroupKickUser,
                            PermissionType.Value))
                        .Id,
                    EntityId = userGroup.Id
                },
                new()
                {
                    Value = BitConverter.GetBytes(Consts.TrueValue),
                    PermissionId = (await _permissionEntityService.GetByAliasTypeAsync(
                            Consts.PermissionAlias.UserGroupKickUser,
                            PermissionType.ValueNeededOwner))
                        .Id,
                    EntityId = userGroup.Id
                },
                new()
                {
                    Value = BitConverter.GetBytes(Consts.RootUserGroupValue),
                    PermissionId = (await _permissionEntityService.GetByAliasTypeAsync(
                            Consts.PermissionAlias.UserGroupKickUser,
                            PermissionType.ValueNeededOthers))
                        .Id,
                    EntityId = userGroup.Id
                },
                new()
                {
                    Value = BitConverter.GetBytes(Consts.TrueValue),
                    PermissionId = (await _permissionEntityService.GetByAliasTypeAsync(
                            Consts.PermissionAlias.UserGroupMemberKickUser,
                            PermissionType.ValueNeededOwner))
                        .Id,
                    EntityId = userGroup.Id
                },
                new()
                {
                    Value = BitConverter.GetBytes(Consts.TrueValue),
                    PermissionId = (await _permissionEntityService.GetByAliasTypeAsync(
                            Consts.PermissionAlias.UserGroupMemberKickUser,
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
                            Consts.PermissionAlias.UserGroupAddUser,
                            PermissionType.ValueNeededOwner))
                        .Id,
                    EntityId = userGroup.Id
                },
                new()
                {
                    Value = BitConverter.GetBytes(Consts.RootUserGroupValue),
                    PermissionId = (await _permissionEntityService.GetByAliasTypeAsync(
                            Consts.PermissionAlias.UserGroupAddUser,
                            PermissionType.ValueNeededOthers))
                        .Id,
                    EntityId = userGroup.Id
                },

                #endregion

                #endregion
            };

            await _permissionValueEntityService.Save(userGroupPermissionValues, cancellationToken);

            //Create UserToUserGroupMappings for (Owner, Public, GroupMember) and save them
            var userToUserGroupMappingOwner = await _userToUserGroupMappingEntityService.Save(new UserToUserGroupMapping
            {
                EntityLeftId = userGroup.UserId,
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
                #region Public

                new()
                {
                    Value = BitConverter.GetBytes(Consts.FalseValue),
                    PermissionId = (await _permissionEntityService.GetByAliasTypeAsync(
                            Consts.PermissionAlias.UserGroupMemberInviteRequestCreate,
                            PermissionType.Value))
                        .Id,
                    EntityId = userToUserGroupMappingPublic.Id
                },
                new()
                {
                    Value = BitConverter.GetBytes(Consts.FalseValue),
                    PermissionId = (await _permissionEntityService.GetByAliasTypeAsync(
                            Consts.PermissionAlias.UserGroupMemberInviteRequestRead,
                            PermissionType.Value))
                        .Id,
                    EntityId = userToUserGroupMappingPublic.Id
                },
                new()
                {
                    Value = BitConverter.GetBytes(Consts.FalseValue),
                    PermissionId = (await _permissionEntityService.GetByAliasTypeAsync(
                            Consts.PermissionAlias.UserGroupMemberInviteRequestUpdate,
                            PermissionType.Value))
                        .Id,
                    EntityId = userToUserGroupMappingPublic.Id
                },
                new()
                {
                    Value = BitConverter.GetBytes(Consts.FalseValue),
                    PermissionId = (await _permissionEntityService.GetByAliasTypeAsync(
                            Consts.PermissionAlias.UserGroupMemberKickUser,
                            PermissionType.Value))
                        .Id,
                    EntityId = userToUserGroupMappingPublic.Id
                },

                #endregion

                #region GroupMember

                new()
                {
                    Value = BitConverter.GetBytes(Consts.FalseValue),
                    PermissionId = (await _permissionEntityService.GetByAliasTypeAsync(
                            Consts.PermissionAlias.UserGroupMemberInviteRequestCreate,
                            PermissionType.Value))
                        .Id,
                    EntityId = userToUserGroupMappingGroupMember.Id
                },
                new()
                {
                    Value = BitConverter.GetBytes(Consts.FalseValue),
                    PermissionId = (await _permissionEntityService.GetByAliasTypeAsync(
                            Consts.PermissionAlias.UserGroupMemberInviteRequestUpdate,
                            PermissionType.Value))
                        .Id,
                    EntityId = userToUserGroupMappingGroupMember.Id
                },
                new()
                {
                    Value = BitConverter.GetBytes(Consts.FalseValue),
                    PermissionId = (await _permissionEntityService.GetByAliasTypeAsync(
                            Consts.PermissionAlias.UserGroupMemberInviteRequestRead,
                            PermissionType.Value))
                        .Id,
                    EntityId = userToUserGroupMappingGroupMember.Id
                },
                new()
                {
                    Value = BitConverter.GetBytes(Consts.FalseValue),
                    PermissionId = (await _permissionEntityService.GetByAliasTypeAsync(
                            Consts.PermissionAlias.UserGroupMemberKickUser,
                            PermissionType.Value))
                        .Id,
                    EntityId = userToUserGroupMappingGroupMember.Id
                },

                #endregion
            };
            
            await _permissionValueEntityService.Save(userToUserGroupMappingPermissionValues, cancellationToken);
            
            //Delete Authorize cache when joining group (for Public, GroupMember users that joined)
            await _authorizeEntityService.PurgeByEntityIdAsync(userGroup.UserId, cancellationToken);
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

            var user = await _userAdvancedService.GetFromHttpContext(cancellationToken: cancellationToken);
            if (user == null)
                throw new HttpResponseException(StatusCodes.Status400BadRequest, ErrorType.HttpContext,
                    Localize.Error.UserNotFoundOrHttpContextMissingClaims);

            var userGroup = await _userGroupEntityService.GetByIdAsync(data.Id, cancellationToken);
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
                EntityLeftPermissionAlias = Consts.PermissionAlias.UserGroupRead,
                EntityRightTableName = _userGroupRepository.GetTableName(),
                EntityRightGroupsTableName = null,
                EntityRightEntityToEntityMappingsTableName = null,
                EntityRightId = userGroup.Id,
                EntityRightPermissionAlias = Consts.PermissionAlias.UserGroupRead,
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
    public async Task<IDtoResultBase> ReadCollection(
        UserGroupReadCollectionDto data,
        CancellationToken cancellationToken = default
    )
    {
        _logger.Log(LogLevel.Information, Localize.Log.MethodStart(GetType(), nameof(ReadCollection)));

        if (ValidateModel(data) is { } validationResult)
            return validationResult;

        try
        {
            await _appDbContextAction.BeginTransactionAsync();

            var user = await _userAdvancedService.GetFromHttpContext(cancellationToken: cancellationToken);
            if (user == null)
                throw new HttpResponseException(StatusCodes.Status400BadRequest, ErrorType.HttpContext,
                    Localize.Error.UserNotFoundOrHttpContextMissingClaims);

            var userGroups =
                await _userGroupEntityService.GetFiltered(data.FilterExpressionModel, data.FilterSortModel,
                    data.PageModel, new AuthorizeModel
                    {
                        EntityLeftTableName = _userRepository.GetTableName(),
                        EntityLeftGroupsTableName = _userGroupRepository.GetTableName(),
                        EntityLeftEntityToEntityMappingsTableName = _userToUserGroupMappingRepository.GetTableName(),
                        EntityLeftId = user.Id,
                        EntityLeftPermissionAlias = Consts.PermissionAlias.UserGroupRead,
                        EntityRightTableName = _userGroupRepository.GetTableName(),
                        EntityRightGroupsTableName = null,
                        EntityRightEntityToEntityMappingsTableName = null,
                        EntityRightIdRawSql = "\"Id\"",
                        EntityRightPermissionAlias = Consts.PermissionAlias.UserGroupRead,
                        SqlExpressionPermissionTypeValueNeededOwner = "T1.\"Id\" = T2.\"UserId\""
                    }, cancellationToken: cancellationToken);

            await _appDbContextAction.CommitTransactionAsync();

            _logger.Log(LogLevel.Information, Localize.Log.MethodEnd(GetType(), nameof(ReadCollection)));

            return new UserGroupReadCollectionDtoResultDto
            {
                Total = userGroups.total,
                Items = userGroups.entities.Select(_ =>
                    _mapper.ProjectTo<UserGroupReadCollectionItemResultDto>(new[] {_}.AsQueryable()).Single())
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

            var user = await _userAdvancedService.GetFromHttpContext(cancellationToken: cancellationToken);
            if (user == null)
                throw new HttpResponseException(StatusCodes.Status400BadRequest, ErrorType.HttpContext,
                    Localize.Error.UserNotFoundOrHttpContextMissingClaims);

            var userGroup = await _userGroupEntityService.GetByIdAsync(data.Id, cancellationToken);
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
                EntityLeftPermissionAlias = Consts.PermissionAlias.UserGroupUpdate,
                EntityRightTableName = _userGroupRepository.GetTableName(),
                EntityRightGroupsTableName = null,
                EntityRightEntityToEntityMappingsTableName = null,
                EntityRightId = userGroup.Id,
                EntityRightPermissionAlias = Consts.PermissionAlias.UserGroupUpdate,
                SqlExpressionPermissionTypeValueNeededOwner = "T1.\"Id\" = T2.\"UserId\""
            });

            if (!authorizeResult)
                throw new HttpResponseException(StatusCodes.Status403Forbidden, ErrorType.Permission,
                    Localize.Error.PermissionInsufficientPermissions);

            //Authorize against UserGroup: Automapper TODO: + against User (user that initiates action)
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
                                Consts.PermissionAlias.UserGroupCreate_Alias,
                            EntityRightTableName = _userRepository.GetTableName(),
                            EntityRightGroupsTableName = _userGroupRepository.GetTableName(),
                            EntityRightEntityToEntityMappingsTableName =
                                _userToUserGroupMappingRepository.GetTableName(),
                            EntityRightId = userGroup.UserId,
                            EntityRightPermissionAlias =
                                Consts.PermissionAlias.UserGroupCreate_Alias,
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
                                Consts.PermissionAlias.UserGroupCreate_Description,
                            EntityRightTableName = _userRepository.GetTableName(),
                            EntityRightGroupsTableName = _userGroupRepository.GetTableName(),
                            EntityRightEntityToEntityMappingsTableName =
                                _userToUserGroupMappingRepository.GetTableName(),
                            EntityRightId = userGroup.UserId,
                            EntityRightPermissionAlias =
                                Consts.PermissionAlias.UserGroupCreate_Description,
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
                                Consts.PermissionAlias.UserGroupCreate_Priority,
                            EntityRightTableName = _userRepository.GetTableName(),
                            EntityRightGroupsTableName = _userGroupRepository.GetTableName(),
                            EntityRightEntityToEntityMappingsTableName =
                                _userToUserGroupMappingRepository.GetTableName(),
                            EntityRightId = userGroup.UserId,
                            EntityRightPermissionAlias =
                                Consts.PermissionAlias.UserGroupCreate_Priority,
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

            var user = await _userAdvancedService.GetFromHttpContext(cancellationToken: cancellationToken);
            if (user == null)
                throw new HttpResponseException(StatusCodes.Status400BadRequest, ErrorType.HttpContext,
                    Localize.Error.UserNotFoundOrHttpContextMissingClaims);

            var userGroup = await _userGroupEntityService.GetByIdAsync(data.Id, cancellationToken);
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
                EntityLeftPermissionAlias = Consts.PermissionAlias.UserGroupDelete,
                EntityRightTableName = _userGroupRepository.GetTableName(),
                EntityRightGroupsTableName = null,
                EntityRightEntityToEntityMappingsTableName = null,
                EntityRightId = userGroup.Id,
                EntityRightPermissionAlias = Consts.PermissionAlias.UserGroupDelete,
                SqlExpressionPermissionTypeValueNeededOwner = "T1.\"Id\" = T2.\"UserId\""
            });

            if (!authorizeResult)
                throw new HttpResponseException(StatusCodes.Status403Forbidden, ErrorType.Permission,
                    Localize.Error.PermissionInsufficientPermissions);

            //Delete UserGroup PermissionValues associated with this group
            await _permissionValueEntityService.PurgeAsync(userGroup.Id, cancellationToken);
            
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
                    await _permissionValueEntityService.PurgeAsync(userToUserGroupMapping.Id, cancellationToken);

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