using System;
using System.Linq;
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
using DTO.Models.PermissionValue;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace BLL.Handlers;

public interface IUserGroupPermissionValueHandler
{
    Task<IDtoResultBase> Create(PermissionValueCreateDto data, CancellationToken cancellationToken = default);
    Task<IDtoResultBase> Read(PermissionValueReadDto data, CancellationToken cancellationToken = default);

    Task<IDtoResultBase> ReadFSPCollection(
        PermissionValueReadEntityCollectionDto data,
        CancellationToken cancellationToken = default
    );

    Task<IDtoResultBase> Update(PermissionValueUpdateDto data, CancellationToken cancellationToken = default);
    Task<IDtoResultBase> Delete(PermissionValueDeleteDto data, CancellationToken cancellationToken = default);
}

public class UserGroupPermissionValueHandler : HandlerBase, IUserGroupPermissionValueHandler
{
    #region Ctor

    public UserGroupPermissionValueHandler(
        ILogger<HandlerBase> logger,
        IAppDbContextAction appDbContextAction,
        IPermissionValueEntityCollectionService permissionValueEntityCollectionService,
        IMapper mapper,
        IPermissionEntityService permissionEntityService,
        IUserGroupEntityService userGroupEntityService,
        IUserAdvancedService userAdvancedService,
        IUserRepository userRepository,
        IUserGroupRepository userGroupRepository,
        IUserToUserGroupMappingRepository userToUserGroupMappingRepository,
        AppDbContext appDbContext,
        IAuthorizeAdvancedService authorizeAdvancedService,
        IAuthorizeEntityService authorizeEntityService,
        IUserToUserGroupMappingEntityService userToUserGroupMappingEntityService
    )
    {
        _logger = logger;
        _appDbContextAction = appDbContextAction;
        _permissionValueEntityCollectionService = permissionValueEntityCollectionService;
        _mapper = mapper;
        _permissionEntityService = permissionEntityService;
        _userGroupEntityService = userGroupEntityService;
        _userAdvancedService = userAdvancedService;
        _userRepository = userRepository;
        _userGroupRepository = userGroupRepository;
        _userToUserGroupMappingRepository = userToUserGroupMappingRepository;
        _appDbContext = appDbContext;
        _authorizeAdvancedService = authorizeAdvancedService;
        _authorizeEntityService = authorizeEntityService;
        _userToUserGroupMappingEntityService = userToUserGroupMappingEntityService;
    }

    #endregion

    #region Fields

    private readonly ILogger<HandlerBase> _logger;
    private readonly IAppDbContextAction _appDbContextAction;
    private readonly IPermissionValueEntityCollectionService _permissionValueEntityCollectionService;
    private readonly IMapper _mapper;
    private readonly IPermissionEntityService _permissionEntityService;
    private readonly IUserGroupEntityService _userGroupEntityService;
    private readonly IUserAdvancedService _userAdvancedService;
    private readonly IUserRepository _userRepository;
    private readonly IUserGroupRepository _userGroupRepository;
    private readonly IUserToUserGroupMappingRepository _userToUserGroupMappingRepository;
    private readonly AppDbContext _appDbContext;
    private readonly IAuthorizeAdvancedService _authorizeAdvancedService;
    private readonly IAuthorizeEntityService _authorizeEntityService;
    private readonly IUserToUserGroupMappingEntityService _userToUserGroupMappingEntityService;

    #endregion

    #region Methods

    public async Task<IDtoResultBase> Create(
        PermissionValueCreateDto data,
        CancellationToken cancellationToken = default
    )
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

            var userGroup = await _userGroupEntityService.GetByIdAsync(data.EntityId, cancellationToken);
            if (userGroup == null)
                throw new HttpResponseException(StatusCodes.Status500InternalServerError, ErrorType.Generic,
                    Localize.Error.UserGroupNotFound);

            var authorizeResult = _authorizeAdvancedService.Authorize(new AuthorizeModel
            {
                EntityLeftTableName = _userRepository.GetTableName(),
                EntityLeftGroupsTableName = _userGroupRepository.GetTableName(),
                EntityLeftEntityToEntityMappingsTableName = _userToUserGroupMappingRepository.GetTableName(),
                EntityLeftId = user.Id,
                EntityLeftPermissionAlias = Consts.PermissionAlias.g_any_a_create_o_permissionvalue,
                EntityRightTableName = _userGroupRepository.GetTableName(),
                EntityRightGroupsTableName = null,
                EntityRightEntityToEntityMappingsTableName = null,
                EntityRightId = userGroup.Id,
                EntityRightPermissionAlias = Consts.PermissionAlias.g_any_a_create_o_permissionvalue,
                SqlExpressionPermissionTypeValueNeededOwner = "T1.\"Id\" = T2.\"UserId\""
            });

            if (!authorizeResult)
                throw new HttpResponseException(StatusCodes.Status403Forbidden, ErrorType.Permission,
                    Localize.Error.PermissionInsufficientPermissions);

            var permission = await _permissionEntityService.GetByIdAsync(data.PermissionId, cancellationToken);
            if (permission == null)
                throw new HttpResponseException(StatusCodes.Status500InternalServerError, ErrorType.Permission,
                    Localize.Error.PermissionNotFound);

            var permissionValue = _mapper.Map<PermissionValue>(data);

            permissionValue = await _permissionValueEntityCollectionService.Save(permissionValue, cancellationToken);

            await _authorizeEntityService.PurgeByEntityIdAsync(userGroup.Id, cancellationToken);

            for (var page = 1;; page += 1)
            {
                var entities = await _userToUserGroupMappingEntityService.GetByUserGroupIdAsync(userGroup.Id,
                    new PageModel
                    {
                        Page = page,
                        PageSize = 512
                    }, cancellationToken);

                foreach (var userToUserGroupMapping in entities)
                    await _authorizeEntityService.PurgeByEntityIdAsync(userToUserGroupMapping.EntityLeftId,
                        cancellationToken);

                await _appDbContextAction.CommitAsync(cancellationToken);

                if (entities.Count < 512)
                    break;
            }

            await _appDbContextAction.CommitTransactionAsync();

            _logger.Log(LogLevel.Information, Localize.Log.MethodEnd(GetType(), nameof(Create)));

            return _mapper.Map<PermissionValueCreateResultDto>(permissionValue);
        }
        catch (Exception)
        {
            await _appDbContextAction.RollbackTransactionAsync();

            throw;
        }
    }

    public async Task<IDtoResultBase> Read(PermissionValueReadDto data, CancellationToken cancellationToken = default)
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

            var permissionValue = await _permissionValueEntityCollectionService.GetByIdAsync(data.Id, cancellationToken);
            if (permissionValue == null)
                throw new HttpResponseException(StatusCodes.Status404NotFound, ErrorType.Generic,
                    Localize.Error.PermissionValueNotFound);

            var userGroup = await _userGroupEntityService.GetByIdAsync(permissionValue.EntityId, cancellationToken);
            if (userGroup == null)
                throw new HttpResponseException(StatusCodes.Status404NotFound, ErrorType.Generic,
                    Localize.Error.UserGroupNotFound);

            var authorizeResult = _authorizeAdvancedService.Authorize(new AuthorizeModel
            {
                EntityLeftTableName = _userRepository.GetTableName(),
                EntityLeftGroupsTableName = _userGroupRepository.GetTableName(),
                EntityLeftEntityToEntityMappingsTableName = _userToUserGroupMappingRepository.GetTableName(),
                EntityLeftId = user.Id,
                EntityLeftPermissionAlias = Consts.PermissionAlias.g_any_a_read_o_permissionvalue,
                EntityRightTableName = _userGroupRepository.GetTableName(),
                EntityRightGroupsTableName = null,
                EntityRightEntityToEntityMappingsTableName = null,
                EntityRightId = userGroup.Id,
                EntityRightPermissionAlias = Consts.PermissionAlias.g_any_a_read_o_permissionvalue,
                SqlExpressionPermissionTypeValueNeededOwner = "T1.\"Id\" = T2.\"UserId\""
            });

            if (!authorizeResult)
                throw new HttpResponseException(StatusCodes.Status403Forbidden, ErrorType.Permission,
                    Localize.Error.PermissionInsufficientPermissions);

            await _appDbContextAction.CommitTransactionAsync();

            _logger.Log(LogLevel.Information, Localize.Log.MethodEnd(GetType(), nameof(Read)));

            return _mapper.Map<PermissionValueReadResultDto>(permissionValue);
        }
        catch (Exception)
        {
            await _appDbContextAction.RollbackTransactionAsync();

            throw;
        }
    }

    public async Task<IDtoResultBase> ReadFSPCollection(
        PermissionValueReadEntityCollectionDto data,
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
                    Localize.Error.UserDoesNotFoundOrHttpContextMissingClaims);

            var permissionValues =
                await _permissionValueEntityCollectionService.GetFilteredSortedPaged(data.FilterExpressionModel,
                    data.FilterSortModel, data.PageModel, new AuthorizeModel
                    {
                        EntityLeftTableName = _userRepository.GetTableName(),
                        EntityLeftGroupsTableName = _userGroupRepository.GetTableName(),
                        EntityLeftEntityToEntityMappingsTableName = _userToUserGroupMappingRepository.GetTableName(),
                        EntityLeftId = user.Id,
                        EntityLeftPermissionAlias = Consts.PermissionAlias.g_any_a_read_o_permissionvalue,
                        EntityRightTableName = _userGroupRepository.GetTableName(),
                        EntityRightGroupsTableName = null,
                        EntityRightEntityToEntityMappingsTableName = null,
                        EntityRightIdRawSql = "\"EntityId\"",
                        EntityRightPermissionAlias = Consts.PermissionAlias.g_any_a_read_o_permissionvalue,
                        SqlExpressionPermissionTypeValueNeededOwner = "T1.\"Id\" = T2.\"UserId\""
                    }, cancellationToken);

            await _appDbContextAction.CommitTransactionAsync();

            _logger.Log(LogLevel.Information, Localize.Log.MethodEnd(GetType(), nameof(ReadFSPCollection)));

            return new PermissionValueReadFSPCollectionResultDto
            {
                Total = permissionValues.total,
                Items = permissionValues.entities.Select(_ =>
                    _mapper.ProjectTo<PermissionValueReadFSPCollectionItemResultDto>(new[] {_}.AsQueryable()).Single())
            };
        }
        catch (Exception)
        {
            await _appDbContextAction.RollbackTransactionAsync();

            throw;
        }
    }

    public async Task<IDtoResultBase> Update(
        PermissionValueUpdateDto data,
        CancellationToken cancellationToken = default
    )
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

            var permissionValue = await _permissionValueEntityCollectionService.GetByIdAsync(data.Id, cancellationToken);
            if (permissionValue == null)
                throw new HttpResponseException(StatusCodes.Status404NotFound, ErrorType.Generic,
                    Localize.Error.PermissionValueNotFound);

            var userGroup = await _userGroupEntityService.GetByIdAsync(permissionValue.EntityId, cancellationToken);
            if (userGroup == null)
                throw new HttpResponseException(StatusCodes.Status404NotFound, ErrorType.Generic,
                    Localize.Error.UserGroupNotFound);

            var authorizeResult = _authorizeAdvancedService.Authorize(new AuthorizeModel
            {
                EntityLeftTableName = _userRepository.GetTableName(),
                EntityLeftGroupsTableName = _userGroupRepository.GetTableName(),
                EntityLeftEntityToEntityMappingsTableName = _userToUserGroupMappingRepository.GetTableName(),
                EntityLeftId = user.Id,
                EntityLeftPermissionAlias = Consts.PermissionAlias.g_any_a_update_o_permissionvalue,
                EntityRightTableName = _userGroupRepository.GetTableName(),
                EntityRightGroupsTableName = null,
                EntityRightEntityToEntityMappingsTableName = null,
                EntityRightId = userGroup.Id,
                EntityRightPermissionAlias = Consts.PermissionAlias.g_any_a_update_o_permissionvalue,
                SqlExpressionPermissionTypeValueNeededOwner = "T1.\"Id\" = T2.\"UserId\""
            });

            if (!authorizeResult)
                throw new HttpResponseException(StatusCodes.Status403Forbidden, ErrorType.Permission,
                    Localize.Error.PermissionInsufficientPermissions);

            _mapper.Map(data, permissionValue);

            await _permissionValueEntityCollectionService.Save(permissionValue, cancellationToken);

            await _authorizeEntityService.PurgeByEntityIdAsync(userGroup.Id, cancellationToken);

            for (var page = 1;; page += 1)
            {
                var entities = await _userToUserGroupMappingEntityService.GetByUserGroupIdAsync(userGroup.Id,
                    new PageModel
                    {
                        Page = page,
                        PageSize = 512
                    }, cancellationToken);

                foreach (var userToUserGroupMapping in entities)
                    await _authorizeEntityService.PurgeByEntityIdAsync(userToUserGroupMapping.EntityLeftId,
                        cancellationToken);

                await _appDbContextAction.CommitAsync(cancellationToken);

                if (entities.Count < 512)
                    break;
            }

            await _appDbContextAction.CommitTransactionAsync();

            _logger.Log(LogLevel.Information, Localize.Log.MethodEnd(GetType(), nameof(Update)));

            return _mapper.Map<PermissionValueUpdateResultDto>(permissionValue);
        }
        catch (Exception)
        {
            await _appDbContextAction.RollbackTransactionAsync();

            throw;
        }
    }

    public async Task<IDtoResultBase> Delete(
        PermissionValueDeleteDto data,
        CancellationToken cancellationToken = default
    )
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

            var permissionValue = await _permissionValueEntityCollectionService.GetByIdAsync(data.Id, cancellationToken);
            if (permissionValue == null)
                throw new HttpResponseException(StatusCodes.Status404NotFound, ErrorType.Generic,
                    Localize.Error.PermissionValueNotFound);

            var userGroup = await _userGroupEntityService.GetByIdAsync(permissionValue.EntityId, cancellationToken);
            if (userGroup == null)
                throw new HttpResponseException(StatusCodes.Status404NotFound, ErrorType.Generic,
                    Localize.Error.UserGroupNotFound);

            var authorizeResult = _authorizeAdvancedService.Authorize(new AuthorizeModel
            {
                EntityLeftTableName = _userRepository.GetTableName(),
                EntityLeftGroupsTableName = _userGroupRepository.GetTableName(),
                EntityLeftEntityToEntityMappingsTableName = _userToUserGroupMappingRepository.GetTableName(),
                EntityLeftId = user.Id,
                EntityLeftPermissionAlias = Consts.PermissionAlias.g_any_a_delete_o_permissionvalue,
                EntityRightTableName = _userGroupRepository.GetTableName(),
                EntityRightGroupsTableName = null,
                EntityRightEntityToEntityMappingsTableName = null,
                EntityRightId = userGroup.Id,
                EntityRightPermissionAlias = Consts.PermissionAlias.g_any_a_delete_o_permissionvalue,
                SqlExpressionPermissionTypeValueNeededOwner = "T1.\"Id\" = T2.\"UserId\""
            });

            if (!authorizeResult)
                throw new HttpResponseException(StatusCodes.Status403Forbidden, ErrorType.Permission,
                    Localize.Error.PermissionInsufficientPermissions);

            await _permissionValueEntityCollectionService.Delete(permissionValue, cancellationToken);

            await _authorizeEntityService.PurgeByEntityIdAsync(userGroup.Id, cancellationToken);

            for (var page = 1;; page += 1)
            {
                var entities = await _userToUserGroupMappingEntityService.GetByUserGroupIdAsync(userGroup.Id,
                    new PageModel
                    {
                        Page = page,
                        PageSize = 512
                    }, cancellationToken);

                foreach (var userToUserGroupMapping in entities)
                    await _authorizeEntityService.PurgeByEntityIdAsync(userToUserGroupMapping.EntityLeftId,
                        cancellationToken);

                await _appDbContextAction.CommitAsync(cancellationToken);

                if (entities.Count < 512)
                    break;
            }

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