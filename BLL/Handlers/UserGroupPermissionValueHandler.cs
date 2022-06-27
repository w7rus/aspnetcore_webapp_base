using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using BLL.Handlers.Base;
using BLL.Services;
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
using DTO.Models.PermissionValue;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BLL.Handlers;

public interface IUserGroupPermissionValueHandler
{
    Task<DTOResultBase> Create(PermissionValueCreateDto data, CancellationToken cancellationToken = default);
    Task<DTOResultBase> Read(PermissionValueReadDto data, CancellationToken cancellationToken = default);

    Task<DTOResultBase> ReadFSPCollection(
        PermissionValueReadFSPCollectionDto data,
        CancellationToken cancellationToken = default
    );

    Task<DTOResultBase> Update(PermissionValueUpdateDto data, CancellationToken cancellationToken = default);
    Task<DTOResultBase> Delete(PermissionValueDeleteDto data, CancellationToken cancellationToken = default);
}

public class UserGroupPermissionValueHandler : HandlerBase, IUserGroupPermissionValueHandler
{
    #region Fields

    private readonly ILogger<HandlerBase> _logger;
    private readonly IAppDbContextAction _appDbContextAction;
    private readonly IPermissionValueEntityService _permissionValueEntityService;
    private readonly IMapper _mapper;
    private readonly IPermissionEntityService _permissionEntityService;
    private readonly IUserGroupEntityService _userGroupEntityService;
    private readonly IUserAdvancedService _userAdvancedService;
    private readonly IUserRepository _userRepository;
    private readonly IUserGroupRepository _userGroupRepository;
    private readonly IUserToUserGroupMappingRepository _userToUserGroupMappingRepository;
    private readonly AppDbContext _appDbContext;
    private readonly IAuthorizeAdvancedService _authorizeAdvancedService;

    #endregion

    #region Ctor

    public UserGroupPermissionValueHandler(
        ILogger<HandlerBase> logger,
        IAppDbContextAction appDbContextAction,
        IPermissionValueEntityService permissionValueEntityService,
        IMapper mapper,
        IPermissionEntityService permissionEntityService,
        IUserGroupEntityService userGroupEntityService,
        IUserAdvancedService userAdvancedService,
        IUserRepository userRepository,
        IUserGroupRepository userGroupRepository,
        IUserToUserGroupMappingRepository userToUserGroupMappingRepository,
        AppDbContext appDbContext,
        IAuthorizeAdvancedService authorizeAdvancedService
    )
    {
        _logger = logger;
        _appDbContextAction = appDbContextAction;
        _permissionValueEntityService = permissionValueEntityService;
        _mapper = mapper;
        _permissionEntityService = permissionEntityService;
        _userGroupEntityService = userGroupEntityService;
        _userAdvancedService = userAdvancedService;
        _userRepository = userRepository;
        _userGroupRepository = userGroupRepository;
        _userToUserGroupMappingRepository = userToUserGroupMappingRepository;
        _appDbContext = appDbContext;
        _authorizeAdvancedService = authorizeAdvancedService;
    }

    #endregion
    
    #region Methods

    public async Task<DTOResultBase> Create(PermissionValueCreateDto data, CancellationToken cancellationToken = default)
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
                SqlExpressionPermissionTypeValueNeededOwner = "T1.\"Id\" = T2.\"OwnerUserId\""
            });
            
            if (!authorizeResult)
                throw new HttpResponseException(StatusCodes.Status403Forbidden, ErrorType.Permission,
                    Localize.Error.PermissionInsufficientPermissions);

            var permission = await _permissionEntityService.GetByIdAsync(data.PermissionId, cancellationToken);
            if (permission == null)
                throw new HttpResponseException(StatusCodes.Status500InternalServerError, ErrorType.Permission,
                    Localize.Error.PermissionNotFound);

            var permissionValue = _mapper.Map<PermissionValue>(data);
            
            _logger.Log(LogLevel.Information,
                Localize.Log.Method(GetType(), nameof(Create),
                    $"{data.GetType().Name} mapped to {permissionValue.GetType().Name}"));

            permissionValue = await _permissionValueEntityService.Save(permissionValue, cancellationToken);
            
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

    public async Task<DTOResultBase> Read(PermissionValueReadDto data, CancellationToken cancellationToken = default)
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

            var permissionValue = await _permissionValueEntityService.GetByIdAsync(data.Id, cancellationToken);
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
                SqlExpressionPermissionTypeValueNeededOwner = "T1.\"Id\" = T2.\"OwnerUserId\""
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
    
    public async Task<DTOResultBase> ReadFSPCollection(PermissionValueReadFSPCollectionDto data, CancellationToken cancellationToken = default)
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
                (await _permissionValueEntityService.GetFilteredSortedPaged(data.FilterExpressionModel,
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
                        SqlExpressionPermissionTypeValueNeededOwner = "T1.\"Id\" = T2.\"OwnerUserId\""
                    }, cancellationToken));

            await _appDbContextAction.CommitTransactionAsync();

            _logger.Log(LogLevel.Information, Localize.Log.MethodEnd(GetType(), nameof(ReadFSPCollection)));

            return new PermissionValueReadFSPCollectionResultDto()
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

    public async Task<DTOResultBase> Update(PermissionValueUpdateDto data, CancellationToken cancellationToken = default)
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

            var permissionValue = await _permissionValueEntityService.GetByIdAsync(data.Id, cancellationToken);
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
                SqlExpressionPermissionTypeValueNeededOwner = "T1.\"Id\" = T2.\"OwnerUserId\""
            });
            
            if (!authorizeResult)
                throw new HttpResponseException(StatusCodes.Status403Forbidden, ErrorType.Permission,
                    Localize.Error.PermissionInsufficientPermissions);

            _mapper.Map(data, permissionValue);
            
            _logger.Log(LogLevel.Information,
                Localize.Log.Method(GetType(), nameof(Update),
                    $"{data.GetType().Name} mapped to {permissionValue.GetType().Name}"));

            await _permissionValueEntityService.Save(permissionValue, cancellationToken);
            
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

    public async Task<DTOResultBase> Delete(PermissionValueDeleteDto data, CancellationToken cancellationToken = default)
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

            var permissionValue = await _permissionValueEntityService.GetByIdAsync(data.Id, cancellationToken);
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
                SqlExpressionPermissionTypeValueNeededOwner = "T1.\"Id\" = T2.\"OwnerUserId\""
            });
            
            if (!authorizeResult)
                throw new HttpResponseException(StatusCodes.Status403Forbidden, ErrorType.Permission,
                    Localize.Error.PermissionInsufficientPermissions);

            await _permissionValueEntityService.Delete(permissionValue, cancellationToken);
            
            await _appDbContextAction.CommitTransactionAsync();

            _logger.Log(LogLevel.Information, Localize.Log.MethodEnd(GetType(), nameof(Delete)));

            return new PermissionValueDeleteResultDto();
        }
        catch (Exception)
        {
            await _appDbContextAction.RollbackTransactionAsync();

            throw;
        }
    }

    #endregion
}