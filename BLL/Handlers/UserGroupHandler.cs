using System;
using System.Collections.Generic;
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
        IUserGroupEntityService userGroupEntityService
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
    }

    #endregion
    
    //TODO: Place DataAnnotation attribs for DTOs

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
                throw new HttpResponseException(StatusCodes.Status500InternalServerError, ErrorType.HttpContext,
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

            var userGroup = _mapper.Map<UserGroup>(data,
                opts => { opts.Items[Consts.AutoMapperModelAuthorizeDataKey] = autoMapperModelAuthorizeData; });

            await _userGroupEntityService.Save(userGroup, cancellationToken);

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
                throw new HttpResponseException(StatusCodes.Status500InternalServerError, ErrorType.HttpContext,
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
            
            var userOwner = await _userEntityService.GetByIdAsync(data.UserId, cancellationToken);
            if (userOwner == null)
                throw new HttpResponseException(StatusCodes.Status500InternalServerError, ErrorType.HttpContext,
                    Localize.Error.UserNotFound);
            
            var userGroup = await _userGroupEntityService.GetByIdAsync(data.Id, cancellationToken);
            if (userGroup == null)
                throw new HttpResponseException(StatusCodes.Status500InternalServerError, ErrorType.HttpContext,
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
                    
                    //TODO: g_group_a_transferownership_o_usergroup_l_automapper
                }
            };

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
                throw new HttpResponseException(StatusCodes.Status500InternalServerError, ErrorType.HttpContext,
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

            await _appDbContextAction.CommitTransactionAsync();

            _logger.Log(LogLevel.Information, Localize.Log.MethodEnd(GetType(), nameof(Join)));

            return;
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

            await _appDbContextAction.CommitTransactionAsync();

            _logger.Log(LogLevel.Information, Localize.Log.MethodEnd(GetType(), nameof(Leave)));

            return;
        }
        catch (Exception)
        {
            await _appDbContextAction.RollbackTransactionAsync();

            throw;
        }
    }

    //TODO: Create PermissionValues for UserToUserGroupMapping to handle per user permissions inside group (boolean), invite, kick and more, those permissions are compared against g_any_true or g_any_false

    #endregion
}