using System;
using System.Collections.Generic;
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
    Task<DTOResultBase> Create(UserGroupCreateDto data, CancellationToken cancellationToken = default);
    Task<DTOResultBase> Read(UserGroupReadDto data, CancellationToken cancellationToken = default);
    Task<DTOResultBase> Update(UserGroupUpdateDto data, CancellationToken cancellationToken = default);
    Task<DTOResultBase> Delete(UserGroupDeleteDto data, CancellationToken cancellationToken = default);
    Task<DTOResultBase> Join(UserGroupJoinDto data, CancellationToken cancellationToken = default);
    Task<DTOResultBase> Leave(UserGroupLeaveDto data, CancellationToken cancellationToken = default);
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
        IPermissionEntityService permissionEntityService
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
    }

    #endregion

    #region Methods

    public async Task<DTOResultBase> Create(UserGroupCreateDto data, CancellationToken cancellationToken = default)
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
                    Localize.Error.UserDoesNotFoundOrHttpContextMissingClaims);

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

            var permissionValues = new PermissionValue[]
            {
                #region Create

                new()
                {
                    Value = BitConverter.GetBytes(long.MinValue),
                    PermissionId = (await _permissionEntityService.GetByAliasTypeAsync(
                            Consts.PermissionAlias.g_group_a_create_o_usergroup,
                            PermissionType.ValueNeededOwner))
                        .Id,
                    EntityId = userGroup.Id
                },
                new()
                {
                    Value = BitConverter.GetBytes(long.MinValue),
                    PermissionId = (await _permissionEntityService.GetByAliasTypeAsync(
                            Consts.PermissionAlias.g_group_a_create_o_usergroup_o_alias_l_automapper,
                            PermissionType.ValueNeededOwner))
                        .Id,
                    EntityId = userGroup.Id
                },
                new()
                {
                    Value = BitConverter.GetBytes(long.MinValue),
                    PermissionId = (await _permissionEntityService.GetByAliasTypeAsync(
                            Consts.PermissionAlias.g_group_a_create_o_usergroup_o_description_l_automapper,
                            PermissionType.ValueNeededOwner))
                        .Id,
                    EntityId = userGroup.Id
                },

                #endregion

                #region Read

                new()
                {
                    Value = BitConverter.GetBytes(long.MinValue),
                    PermissionId = (await _permissionEntityService.GetByAliasTypeAsync(
                            Consts.PermissionAlias.g_group_a_read_o_usergroup,
                            PermissionType.ValueNeededOwner))
                        .Id,
                    EntityId = userGroup.Id
                },

                #endregion

                #region Update

                new()
                {
                    Value = BitConverter.GetBytes(long.MinValue),
                    PermissionId = (await _permissionEntityService.GetByAliasTypeAsync(
                            Consts.PermissionAlias.g_group_a_update_o_usergroup,
                            PermissionType.ValueNeededOwner))
                        .Id,
                    EntityId = userGroup.Id
                },
                new()
                {
                    Value = BitConverter.GetBytes(long.MinValue),
                    PermissionId = (await _permissionEntityService.GetByAliasTypeAsync(
                            Consts.PermissionAlias.g_group_a_update_o_usergroup_o_alias_l_automapper,
                            PermissionType.ValueNeededOwner))
                        .Id,
                    EntityId = userGroup.Id
                },
                new()
                {
                    Value = BitConverter.GetBytes(long.MinValue),
                    PermissionId = (await _permissionEntityService.GetByAliasTypeAsync(
                            Consts.PermissionAlias.g_group_a_update_o_usergroup_o_description_l_automapper,
                            PermissionType.ValueNeededOwner))
                        .Id,
                    EntityId = userGroup.Id
                },
                new()
                {
                    Value = BitConverter.GetBytes(long.MinValue),
                    PermissionId = (await _permissionEntityService.GetByAliasTypeAsync(
                            Consts.PermissionAlias.g_group_a_update_o_usergroup_o_priority_l_automapper,
                            PermissionType.ValueNeededOwner))
                        .Id,
                    EntityId = userGroup.Id
                },

                #endregion

                #region Delete

                new()
                {
                    Value = BitConverter.GetBytes(long.MinValue),
                    PermissionId = (await _permissionEntityService.GetByAliasTypeAsync(
                            Consts.PermissionAlias.g_group_a_delete_o_usergroup,
                            PermissionType.ValueNeededOwner))
                        .Id,
                    EntityId = userGroup.Id
                },

                #endregion

                #region Join

                new()
                {
                    Value = BitConverter.GetBytes(long.MinValue),
                    PermissionId = (await _permissionEntityService.GetByAliasTypeAsync(
                            Consts.PermissionAlias.g_group_a_join_o_usergroup,
                            PermissionType.ValueNeededOwner))
                        .Id,
                    EntityId = userGroup.Id
                },

                #endregion

                #region Leave

                new()
                {
                    Value = BitConverter.GetBytes(long.MinValue),
                    PermissionId = (await _permissionEntityService.GetByAliasTypeAsync(
                            Consts.PermissionAlias.g_group_a_leave_o_usergroup,
                            PermissionType.ValueNeededOwner))
                        .Id,
                    EntityId = userGroup.Id
                }

                #endregion
            };

            await _permissionValueEntityService.Save(permissionValues, cancellationToken);

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

    public async Task<DTOResultBase> Read(UserGroupReadDto data, CancellationToken cancellationToken = default)
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
                    Localize.Error.UserDoesNotFoundOrHttpContextMissingClaims);

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

    public async Task<DTOResultBase> Update(UserGroupUpdateDto data, CancellationToken cancellationToken = default)
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
                    Localize.Error.UserDoesNotFoundOrHttpContextMissingClaims);

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

    public async Task<DTOResultBase> Delete(UserGroupDeleteDto data, CancellationToken cancellationToken = default)
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
                    Localize.Error.UserDoesNotFoundOrHttpContextMissingClaims);

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

            await _permissionValueEntityService.PurgeAsync(userGroup.Id, cancellationToken);

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

    public async Task<DTOResultBase> Join(UserGroupJoinDto data, CancellationToken cancellationToken = default)
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
                    Localize.Error.UserDoesNotFoundOrHttpContextMissingClaims);

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

            await _appDbContextAction.CommitTransactionAsync();

            await _userToUserGroupMappingEntityService.Save(new UserToUserGroupMapping
            {
                EntityLeftId = user.Id,
                EntityRightId = userGroup.Id
            }, cancellationToken);

            _logger.Log(LogLevel.Information, Localize.Log.MethodEnd(GetType(), nameof(Join)));

            return new OkResultDto();
        }
        catch (Exception)
        {
            await _appDbContextAction.RollbackTransactionAsync();

            throw;
        }
    }

    public async Task<DTOResultBase> Leave(UserGroupLeaveDto data, CancellationToken cancellationToken = default)
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
                    Localize.Error.UserDoesNotFoundOrHttpContextMissingClaims);

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

            await _appDbContextAction.CommitTransactionAsync();

            var userToUserGroupMapping =
                await _userToUserGroupMappingEntityService.GetByUserIdUserGroupIdAsync(user.Id, userGroup.Id);
            if (userToUserGroupMapping == null)
                throw new HttpResponseException(StatusCodes.Status500InternalServerError, ErrorType.Generic,
                    Localize.Error.UserToUserGroupMappingNotFound);

            await _userToUserGroupMappingEntityService.Delete(userToUserGroupMapping, cancellationToken);

            _logger.Log(LogLevel.Information, Localize.Log.MethodEnd(GetType(), nameof(Leave)));

            return new OkResultDto();
        }
        catch (Exception)
        {
            await _appDbContextAction.RollbackTransactionAsync();

            throw;
        }
    }

    //TODO: Task<DTOResultBase> TransferOwnership(... data, CancellationToken cancellationToken = default)
    //TODO: g_group_a_transferownership_o_usergroup

    //TODO: Task<DTOResultBase> InviteUser(... data, CancellationToken cancellationToken = default)
    //TODO: g_group_a_manage_o_usergroup_a_invite_o_user

    //TODO: Task<DTOResultBase> AddUser(... data, CancellationToken cancellationToken = default)
    //TODO: g_group_a_manage_o_usergroup_a_add_o_user

    //TODO: Task<DTOResultBase> DeleteUser(... data, CancellationToken cancellationToken = default)
    //TODO: g_group_a_manage_o_usergroup_a_delete_o_user

    //TODO: Task<DTOResultBase> ReadFSPCollection(... data, CancellationToken cancellationToken = default)

    //TODO: Create PermissionValues for UserToUserGroupMapping to handle per user permissions inside group (boolean), invite, kick and more, those permissions are compared against g_any_true or g_any_false

    #endregion
}