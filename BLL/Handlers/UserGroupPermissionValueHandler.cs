using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using BLL.Handlers.Base;
using BLL.Services;
using BLL.Services.Advanced;
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
    Task<DTOResultBase> Create(PermissionValueCreate data, CancellationToken cancellationToken = default);
    Task<DTOResultBase> Read(PermissionValueRead data, CancellationToken cancellationToken = default);

    Task<DTOResultBase> ReadFSPCollection(
        PermissionValueReadFSPCollection data,
        CancellationToken cancellationToken = default
    );

    Task<DTOResultBase> Update(PermissionValueUpdate data, CancellationToken cancellationToken = default);
    Task<DTOResultBase> Delete(PermissionValueDelete data, CancellationToken cancellationToken = default);
}

public class UserGroupPermissionValueHandler : HandlerBase, IUserGroupPermissionValueHandler
{
    #region Fields

    private readonly ILogger<HandlerBase> _logger;
    private readonly IAppDbContextAction _appDbContextAction;
    private readonly IPermissionValueService _permissionValueService;
    private readonly IMapper _mapper;
    private readonly IPermissionService _permissionService;
    private readonly IUserGroupService _userGroupService;
    private readonly IUserAdvancedService _userAdvancedService;
    private readonly IUserRepository _userRepository;
    private readonly IUserGroupRepository _userGroupRepository;
    private readonly IUserToUserGroupMappingRepository _userToUserGroupMappingRepository;
    private readonly AppDbContext _appDbContext;

    #endregion

    #region Ctor

    public UserGroupPermissionValueHandler(
        ILogger<HandlerBase> logger,
        IAppDbContextAction appDbContextAction,
        IPermissionValueService permissionValueService,
        IMapper mapper,
        IPermissionService permissionService,
        IUserGroupService userGroupService,
        IUserAdvancedService userAdvancedService,
        IUserRepository userRepository,
        IUserGroupRepository userGroupRepository,
        IUserToUserGroupMappingRepository userToUserGroupMappingRepository,
        AppDbContext appDbContext
    )
    {
        _logger = logger;
        _appDbContextAction = appDbContextAction;
        _permissionValueService = permissionValueService;
        _mapper = mapper;
        _permissionService = permissionService;
        _userGroupService = userGroupService;
        _userAdvancedService = userAdvancedService;
        _userRepository = userRepository;
        _userGroupRepository = userGroupRepository;
        _userToUserGroupMappingRepository = userToUserGroupMappingRepository;
        _appDbContext = appDbContext;
    }

    #endregion

    //TODO: Those are only for UserGroups for now
    #region Methods

    public async Task<DTOResultBase> Create(PermissionValueCreate data, CancellationToken cancellationToken = default)
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
                    Localize.Error.UserDoesNotExistOrHttpContextMissingClaims);

            var userGroup = await _userGroupService.GetByIdAsync(data.EntityId, cancellationToken);
            if (userGroup == null)
                throw new HttpResponseException(StatusCodes.Status500InternalServerError, ErrorType.Generic,
                    Localize.Error.UserGroupNotFound);

            //Authorize permissionValue creation
            var authorizeResult = _appDbContext.Set<AuthorizeResult>()
                .FromSqlRaw((new AuthorizeModel
                {
                    EntityLeftTableName = $"'{_userRepository.GetTableName()}'",
                    EntityLeftGroupsTableName = $"'{_userGroupRepository.GetTableName()}'",
                    EntityLeftEntityToEntityMappingsTableName = $"'{_userToUserGroupMappingRepository.GetTableName()}'",
                    EntityLeftId = $"'{user.Id.ToString()}'",
                    EntityLeftPermissionAlias = "'g_any_a_create_o_permissionvalue'",
                    EntityRightTableName = $"'{_userGroupRepository.GetTableName()}'",
                    EntityRightGroupsTableName = null,
                    EntityRightEntityToEntityMappingsTableName = null,
                    EntityRightId = $"'{userGroup.Id.ToString()}'",
                    EntityRightPermissionAlias = "'g_any_a_create_o_permissionvalue'",
                    SQLExpressionPermissionTypeValueNeededOwner = "'T1.\"Id\" = T2.\"OwnerUserId\"'"
                }).GetRawSql()).ToList().SingleOrDefault();
            
            if (authorizeResult?.Result != null && !authorizeResult.Result)
                throw new HttpResponseException(StatusCodes.Status403Forbidden, ErrorType.Permission,
                    Localize.Error.PermissionInsufficientPermissions);

            var permission = await _permissionService.GetByIdAsync(data.PermissionId, cancellationToken);
            if (permission == null)
                throw new HttpResponseException(StatusCodes.Status500InternalServerError, ErrorType.Permission,
                    Localize.Error.PermissionNotFound);

            var permissionValue = _mapper.Map<PermissionValue>(data);
            
            _logger.Log(LogLevel.Information,
                Localize.Log.Method(GetType(), nameof(Create),
                    $"{data.GetType().Name} mapped to {permissionValue.GetType().Name}"));

            permissionValue = await _permissionValueService.Save(permissionValue, cancellationToken);
            
            await _appDbContextAction.CommitTransactionAsync();

            _logger.Log(LogLevel.Information, Localize.Log.MethodEnd(GetType(), nameof(Create)));

            return _mapper.Map<PermissionValueCreateResult>(permissionValue);
        }
        catch (Exception)
        {
            await _appDbContextAction.RollbackTransactionAsync();

            throw;
        }
    }

    public async Task<DTOResultBase> Read(PermissionValueRead data, CancellationToken cancellationToken = default)
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
                    Localize.Error.UserDoesNotExistOrHttpContextMissingClaims);

            var permissionValue = await _permissionValueService.GetByIdAsync(data.Id, cancellationToken);
            if (permissionValue == null)
                throw new HttpResponseException(StatusCodes.Status404NotFound, ErrorType.Generic,
                    Localize.Error.PermissionValueNotFound);

            var userGroup = await _userGroupService.GetByIdAsync(permissionValue.EntityId, cancellationToken);
            if (userGroup == null)
                throw new HttpResponseException(StatusCodes.Status404NotFound, ErrorType.Generic,
                    Localize.Error.UserGroupNotFound);
            
            //Authorize permissionValue reading
            var authorizeResult = _appDbContext.Set<AuthorizeResult>()
                .FromSqlRaw((new AuthorizeModel
            {
                EntityLeftTableName = $"'{_userRepository.GetTableName()}'",
                EntityLeftGroupsTableName = $"'{_userGroupRepository.GetTableName()}'",
                EntityLeftEntityToEntityMappingsTableName = $"'{_userToUserGroupMappingRepository.GetTableName()}'",
                EntityLeftId = $"'{user.Id.ToString()}'",
                EntityLeftPermissionAlias = "'g_any_a_read_o_permissionvalue'",
                EntityRightTableName = $"'{_userGroupRepository.GetTableName()}'",
                EntityRightGroupsTableName = null,
                EntityRightEntityToEntityMappingsTableName = null,
                EntityRightId = $"'{userGroup.Id.ToString()}'",
                EntityRightPermissionAlias = "'g_any_a_read_o_permissionvalue'",
                SQLExpressionPermissionTypeValueNeededOwner = "'T1.\"Id\" = T2.\"OwnerUserId\"'"
            }).GetRawSql()).ToList().SingleOrDefault();
            
            if (authorizeResult?.Result != null && !authorizeResult.Result)
                throw new HttpResponseException(StatusCodes.Status403Forbidden, ErrorType.Permission,
                    Localize.Error.PermissionInsufficientPermissions);
            
            await _appDbContextAction.CommitTransactionAsync();
            
            _logger.Log(LogLevel.Information, Localize.Log.MethodEnd(GetType(), nameof(Read)));
            
            return _mapper.Map<PermissionValueReadResult>(permissionValue);
        }
        catch (Exception)
        {
            await _appDbContextAction.RollbackTransactionAsync();

            throw;
        }
    }
    
    public async Task<DTOResultBase> ReadFSPCollection(PermissionValueReadFSPCollection data, CancellationToken cancellationToken = default)
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
                    Localize.Error.UserDoesNotExistOrHttpContextMissingClaims);

            var permissionValues =
                (await _permissionValueService.GetFilteredSortedPaged(data.FilterExpressionModel,
                    data.FilterSortModel, data.PageModel, new AuthorizeModel
                    {
                        EntityLeftTableName = $"'{_userRepository.GetTableName()}'",
                        EntityLeftGroupsTableName = $"'{_userGroupRepository.GetTableName()}'",
                        EntityLeftEntityToEntityMappingsTableName = $"'{_userToUserGroupMappingRepository.GetTableName()}'",
                        EntityLeftId = $"'{user.Id.ToString()}'",
                        EntityLeftPermissionAlias = "'g_any_a_read_o_permissionvalue'",
                        EntityRightTableName = $"'{_userGroupRepository.GetTableName()}'",
                        EntityRightGroupsTableName = null,
                        EntityRightEntityToEntityMappingsTableName = null,
                        EntityRightId = "\"EntityId\"",
                        EntityRightPermissionAlias = "'g_any_a_read_o_permissionvalue'",
                        SQLExpressionPermissionTypeValueNeededOwner = "'T1.\"Id\" = T2.\"OwnerUserId\"'"
                    }, cancellationToken));

            await _appDbContextAction.CommitTransactionAsync();

            _logger.Log(LogLevel.Information, Localize.Log.MethodEnd(GetType(), nameof(ReadFSPCollection)));

            return new PermissionValueReadFSPCollectionResult()
            {
                Total = permissionValues.total,
                Items = permissionValues.entities.Select(_ =>
                    _mapper.ProjectTo<PermissionValueReadFSPCollectionItemResult>(new[] {_}.AsQueryable()).Single())
            };
        }
        catch (Exception)
        {
            await _appDbContextAction.RollbackTransactionAsync();

            throw;
        }
    }

    public async Task<DTOResultBase> Update(PermissionValueUpdate data, CancellationToken cancellationToken = default)
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
                    Localize.Error.UserDoesNotExistOrHttpContextMissingClaims);

            var permissionValue = await _permissionValueService.GetByIdAsync(data.Id, cancellationToken);
            if (permissionValue == null)
                throw new HttpResponseException(StatusCodes.Status404NotFound, ErrorType.Generic,
                    Localize.Error.PermissionValueNotFound);
            
            var userGroup = await _userGroupService.GetByIdAsync(permissionValue.EntityId, cancellationToken);
            if (userGroup == null)
                throw new HttpResponseException(StatusCodes.Status404NotFound, ErrorType.Generic,
                    Localize.Error.UserGroupNotFound);
            
            //Authorize permissionValue update
            var authorizeResult = _appDbContext.Set<AuthorizeResult>()
                .FromSqlRaw((new AuthorizeModel
                {
                    EntityLeftTableName = $"'{_userRepository.GetTableName()}'",
                    EntityLeftGroupsTableName = $"'{_userGroupRepository.GetTableName()}'",
                    EntityLeftEntityToEntityMappingsTableName = $"'{_userToUserGroupMappingRepository.GetTableName()}'",
                    EntityLeftId = $"'{user.Id.ToString()}'",
                    EntityLeftPermissionAlias = "'g_any_a_update_o_permissionvalue'",
                    EntityRightTableName = $"'{_userGroupRepository.GetTableName()}'",
                    EntityRightGroupsTableName = null,
                    EntityRightEntityToEntityMappingsTableName = null,
                    EntityRightId = $"'{userGroup.Id.ToString()}'",
                    EntityRightPermissionAlias = "'g_any_a_update_o_permissionvalue'",
                    SQLExpressionPermissionTypeValueNeededOwner = "'T1.\"Id\" = T2.\"OwnerUserId\"'"
                }).GetRawSql()).ToList().SingleOrDefault();
            
            if (authorizeResult?.Result != null && !authorizeResult.Result)
                throw new HttpResponseException(StatusCodes.Status403Forbidden, ErrorType.Permission,
                    Localize.Error.PermissionInsufficientPermissions);

            _mapper.Map(data, permissionValue);
            
            _logger.Log(LogLevel.Information,
                Localize.Log.Method(GetType(), nameof(Update),
                    $"{data.GetType().Name} mapped to {permissionValue.GetType().Name}"));

            await _permissionValueService.Save(permissionValue, cancellationToken);
            
            await _appDbContextAction.CommitTransactionAsync();

            _logger.Log(LogLevel.Information, Localize.Log.MethodEnd(GetType(), nameof(Update)));
            
            return _mapper.Map<PermissionValueUpdateResult>(permissionValue);
        }
        catch (Exception)
        {
            await _appDbContextAction.RollbackTransactionAsync();

            throw;
        }
    }

    public async Task<DTOResultBase> Delete(PermissionValueDelete data, CancellationToken cancellationToken = default)
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
                    Localize.Error.UserDoesNotExistOrHttpContextMissingClaims);

            var permissionValue = await _permissionValueService.GetByIdAsync(data.Id, cancellationToken);
            if (permissionValue == null)
                throw new HttpResponseException(StatusCodes.Status404NotFound, ErrorType.Generic,
                    Localize.Error.PermissionValueNotFound);
            
            var userGroup = await _userGroupService.GetByIdAsync(permissionValue.EntityId, cancellationToken);
            if (userGroup == null)
                throw new HttpResponseException(StatusCodes.Status404NotFound, ErrorType.Generic,
                    Localize.Error.UserGroupNotFound);
            
            //Authorize permissionValue update
            var authorizeResult = _appDbContext.Set<AuthorizeResult>()
                .FromSqlRaw((new AuthorizeModel
                {
                    EntityLeftTableName = $"'{_userRepository.GetTableName()}'",
                    EntityLeftGroupsTableName = $"'{_userGroupRepository.GetTableName()}'",
                    EntityLeftEntityToEntityMappingsTableName = $"'{_userToUserGroupMappingRepository.GetTableName()}'",
                    EntityLeftId = $"'{user.Id.ToString()}'",
                    EntityLeftPermissionAlias = "'g_any_a_delete_o_permissionvalue'",
                    EntityRightTableName = $"'{_userGroupRepository.GetTableName()}'",
                    EntityRightGroupsTableName = null,
                    EntityRightEntityToEntityMappingsTableName = null,
                    EntityRightId = $"'{userGroup.Id.ToString()}'",
                    EntityRightPermissionAlias = "'g_any_a_delete_o_permissionvalue'",
                    SQLExpressionPermissionTypeValueNeededOwner = "'T1.\"Id\" = T2.\"OwnerUserId\"'"
                }).GetRawSql()).ToList().SingleOrDefault();
            
            if (authorizeResult?.Result != null && !authorizeResult.Result)
                throw new HttpResponseException(StatusCodes.Status403Forbidden, ErrorType.Permission,
                    Localize.Error.PermissionInsufficientPermissions);

            await _permissionValueService.Delete(permissionValue, cancellationToken);
            
            await _appDbContextAction.CommitTransactionAsync();

            _logger.Log(LogLevel.Information, Localize.Log.MethodEnd(GetType(), nameof(Delete)));

            return new PermissionValueDeleteResult();
        }
        catch (Exception)
        {
            await _appDbContextAction.RollbackTransactionAsync();

            throw;
        }
    }

    #endregion
}