using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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
using DTO.Models.Generic;
using DTO.Models.PermissionValue;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace BLL.Handlers;

public interface IUserToUserGroupMappingPermissionValueHandler
{
    Task<IDtoResultBase> Create(PermissionValueCreateDto data, CancellationToken cancellationToken = default);
    Task<IDtoResultBase> Read(PermissionValueReadDto data, CancellationToken cancellationToken = default);

    Task<IDtoResultBase> ReadCollection(
        PermissionValueReadCollectionDto data,
        CancellationToken cancellationToken = default
    );

    Task<IDtoResultBase> Update(PermissionValueUpdateDto data, CancellationToken cancellationToken = default);
    Task<IDtoResultBase> Delete(PermissionValueDeleteDto data, CancellationToken cancellationToken = default);
}

public class UserToUserGroupMappingPermissionValueHandler : HandlerBase, IUserToUserGroupMappingPermissionValueHandler
{
    #region Ctor

    public UserToUserGroupMappingPermissionValueHandler(
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
        IAuthorizeAdvancedService authorizeAdvancedService,
        IAuthorizeEntityService authorizeEntityService,
        IUserToUserGroupMappingEntityService userToUserGroupMappingEntityService
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
        _authorizeAdvancedService = authorizeAdvancedService;
        _authorizeEntityService = authorizeEntityService;
        _userToUserGroupMappingEntityService = userToUserGroupMappingEntityService;
    }

    #endregion

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
    private readonly IAuthorizeAdvancedService _authorizeAdvancedService;
    private readonly IAuthorizeEntityService _authorizeEntityService;
    private readonly IUserToUserGroupMappingEntityService _userToUserGroupMappingEntityService;

    private static readonly string UserToUserGroupMappingEntityDiscriminatorHash =
        typeof(UserToUserGroupMapping).EntityAqnHash();

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

            var user = await _userAdvancedService.GetFromHttpContext(cancellationToken: cancellationToken);
            if (user == null)
                throw new HttpResponseException(StatusCodes.Status400BadRequest, ErrorType.HttpContext,
                    Localize.Error.UserNotFoundOrHttpContextMissingClaims);

            var userToUserGroupMapping =
                await _userToUserGroupMappingEntityService.GetByIdAsync(data.EntityId, cancellationToken);
            if (userToUserGroupMapping == null)
                throw new HttpResponseException(StatusCodes.Status404NotFound, ErrorType.Generic,
                    Localize.Error.UserToUserGroupMappingNotFound);

            var userGroup = await _userGroupEntityService.GetByIdAsync(userToUserGroupMapping.EntityRightId, cancellationToken);
            if (userGroup == null)
                throw new HttpResponseException(StatusCodes.Status404NotFound, ErrorType.Generic,
                    Localize.Error.UserGroupNotFound);

            var authorizeResult = _authorizeAdvancedService.Authorize(new AuthorizeModel
            {
                EntityLeftTableName = _userRepository.GetTableName(),
                EntityLeftGroupsTableName = _userGroupRepository.GetTableName(),
                EntityLeftEntityToEntityMappingsTableName = _userToUserGroupMappingRepository.GetTableName(),
                EntityLeftId = user.Id,
                EntityLeftPermissionAlias = Consts.PermissionAlias.PermissionValueCreate,
                EntityRightTableName = _userGroupRepository.GetTableName(),
                EntityRightGroupsTableName = null,
                EntityRightEntityToEntityMappingsTableName = null,
                EntityRightId = userGroup.Id,
                EntityRightPermissionAlias = Consts.PermissionAlias.PermissionValueCreate,
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

            permissionValue.EntityDiscriminator = UserToUserGroupMappingEntityDiscriminatorHash;

            permissionValue = await _permissionValueEntityService.Save(permissionValue, cancellationToken);

            await _authorizeEntityService.PurgeByEntityIdAsync(userToUserGroupMapping.Id, cancellationToken);

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

            var user = await _userAdvancedService.GetFromHttpContext(cancellationToken: cancellationToken);
            if (user == null)
                throw new HttpResponseException(StatusCodes.Status400BadRequest, ErrorType.HttpContext,
                    Localize.Error.UserNotFoundOrHttpContextMissingClaims);

            var permissionValue =
                await _permissionValueEntityService.GetByIdAsync(data.Id, cancellationToken);
            if (permissionValue == null)
                throw new HttpResponseException(StatusCodes.Status404NotFound, ErrorType.Generic,
                    Localize.Error.PermissionValueNotFound);
            
            var userToUserGroupMapping =
                await _userToUserGroupMappingEntityService.GetByIdAsync(permissionValue.EntityId, cancellationToken);
            if (userToUserGroupMapping == null)
                throw new HttpResponseException(StatusCodes.Status404NotFound, ErrorType.Generic,
                    Localize.Error.UserToUserGroupMappingNotFound);

            var userGroup = await _userGroupEntityService.GetByIdAsync(userToUserGroupMapping.EntityRightId, cancellationToken);
            if (userGroup == null)
                throw new HttpResponseException(StatusCodes.Status404NotFound, ErrorType.Generic,
                    Localize.Error.UserGroupNotFound);

            var authorizeResult = _authorizeAdvancedService.Authorize(new AuthorizeModel
            {
                EntityLeftTableName = _userRepository.GetTableName(),
                EntityLeftGroupsTableName = _userGroupRepository.GetTableName(),
                EntityLeftEntityToEntityMappingsTableName = _userToUserGroupMappingRepository.GetTableName(),
                EntityLeftId = user.Id,
                EntityLeftPermissionAlias = Consts.PermissionAlias.PermissionValueRead,
                EntityRightTableName = _userGroupRepository.GetTableName(),
                EntityRightGroupsTableName = null,
                EntityRightEntityToEntityMappingsTableName = null,
                EntityRightId = userGroup.Id,
                EntityRightPermissionAlias = Consts.PermissionAlias.PermissionValueRead,
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
    
    public async Task<IDtoResultBase> ReadCollection(
        PermissionValueReadCollectionDto data,
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

            var permissionValues =
                await _permissionValueEntityService.GetFiltered(data.FilterExpressionModel,
                    data.FilterSortModel, data.PageModel, new AuthorizeModel
                    {
                        EntityLeftTableName = _userRepository.GetTableName(),
                        EntityLeftGroupsTableName = _userGroupRepository.GetTableName(),
                        EntityLeftEntityToEntityMappingsTableName = _userToUserGroupMappingRepository.GetTableName(),
                        EntityLeftId = user.Id,
                        EntityLeftPermissionAlias = Consts.PermissionAlias.PermissionValueRead,
                        EntityRightTableName = _userGroupRepository.GetTableName(),
                        EntityRightGroupsTableName = null,
                        EntityRightEntityToEntityMappingsTableName = null,
                        EntityRightIdRawSql = $"(SELECT \"EntityRightId\" FROM {_userToUserGroupMappingRepository.GetTableName()}) WHERE \"Id\" = \"EntityId\" LIMIT 1)", // EntityId = UserToUserGroupMappingId
                        EntityRightPermissionAlias = Consts.PermissionAlias.PermissionValueRead,
                        SqlExpressionPermissionTypeValueNeededOwner = "T1.\"Id\" = T2.\"UserId\""
                    }, new FilterExpressionModel
                    {
                        ExpressionLogicalOperation = ExpressionLogicalOperation.None,
                        Items = new List<FilterExpressionModelItem>
                        {
                            new FilterExpressionModelItemExpression
                            {
                                ExpressionLogicalOperation = ExpressionLogicalOperation.None,
                                Key = "EntityDiscriminator",
                                Value = Encoding.UTF8.GetBytes(UserToUserGroupMappingEntityDiscriminatorHash),
                                FilterMatchOperation = FilterMatchOperation.Equal
                            }
                        }
                    }, cancellationToken);

            await _appDbContextAction.CommitTransactionAsync();

            _logger.Log(LogLevel.Information, Localize.Log.MethodEnd(GetType(), nameof(ReadCollection)));

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

            var user = await _userAdvancedService.GetFromHttpContext(cancellationToken: cancellationToken);
            if (user == null)
                throw new HttpResponseException(StatusCodes.Status400BadRequest, ErrorType.HttpContext,
                    Localize.Error.UserNotFoundOrHttpContextMissingClaims);

            var permissionValue =
                await _permissionValueEntityService.GetByIdAsync(data.Id, cancellationToken);
            if (permissionValue == null)
                throw new HttpResponseException(StatusCodes.Status404NotFound, ErrorType.Generic,
                    Localize.Error.PermissionValueNotFound);
            
            var userToUserGroupMapping =
                await _userToUserGroupMappingEntityService.GetByIdAsync(permissionValue.EntityId, cancellationToken);
            if (userToUserGroupMapping == null)
                throw new HttpResponseException(StatusCodes.Status404NotFound, ErrorType.Generic,
                    Localize.Error.UserToUserGroupMappingNotFound);

            var userGroup = await _userGroupEntityService.GetByIdAsync(userToUserGroupMapping.EntityRightId, cancellationToken);
            if (userGroup == null)
                throw new HttpResponseException(StatusCodes.Status404NotFound, ErrorType.Generic,
                    Localize.Error.UserGroupNotFound);

            var authorizeResult = _authorizeAdvancedService.Authorize(new AuthorizeModel
            {
                EntityLeftTableName = _userRepository.GetTableName(),
                EntityLeftGroupsTableName = _userGroupRepository.GetTableName(),
                EntityLeftEntityToEntityMappingsTableName = _userToUserGroupMappingRepository.GetTableName(),
                EntityLeftId = user.Id,
                EntityLeftPermissionAlias = Consts.PermissionAlias.PermissionValueUpdate,
                EntityRightTableName = _userGroupRepository.GetTableName(),
                EntityRightGroupsTableName = null,
                EntityRightEntityToEntityMappingsTableName = null,
                EntityRightId = userGroup.Id,
                EntityRightPermissionAlias = Consts.PermissionAlias.PermissionValueUpdate,
                SqlExpressionPermissionTypeValueNeededOwner = "T1.\"Id\" = T2.\"UserId\""
            });

            if (!authorizeResult)
                throw new HttpResponseException(StatusCodes.Status403Forbidden, ErrorType.Permission,
                    Localize.Error.PermissionInsufficientPermissions);

            _mapper.Map(data, permissionValue);

            await _permissionValueEntityService.Save(permissionValue, cancellationToken);

            await _authorizeEntityService.PurgeByEntityIdAsync(userToUserGroupMapping.Id, cancellationToken);

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

            var user = await _userAdvancedService.GetFromHttpContext(cancellationToken: cancellationToken);
            if (user == null)
                throw new HttpResponseException(StatusCodes.Status400BadRequest, ErrorType.HttpContext,
                    Localize.Error.UserNotFoundOrHttpContextMissingClaims);

            var permissionValue =
                await _permissionValueEntityService.GetByIdAsync(data.Id, cancellationToken);
            if (permissionValue == null)
                throw new HttpResponseException(StatusCodes.Status404NotFound, ErrorType.Generic,
                    Localize.Error.PermissionValueNotFound);
            
            var userToUserGroupMapping =
                await _userToUserGroupMappingEntityService.GetByIdAsync(permissionValue.EntityId, cancellationToken);
            if (userToUserGroupMapping == null)
                throw new HttpResponseException(StatusCodes.Status404NotFound, ErrorType.Generic,
                    Localize.Error.UserToUserGroupMappingNotFound);

            var userGroup = await _userGroupEntityService.GetByIdAsync(userToUserGroupMapping.EntityRightId, cancellationToken);
            if (userGroup == null)
                throw new HttpResponseException(StatusCodes.Status404NotFound, ErrorType.Generic,
                    Localize.Error.UserGroupNotFound);

            var authorizeResult = _authorizeAdvancedService.Authorize(new AuthorizeModel
            {
                EntityLeftTableName = _userRepository.GetTableName(),
                EntityLeftGroupsTableName = _userGroupRepository.GetTableName(),
                EntityLeftEntityToEntityMappingsTableName = _userToUserGroupMappingRepository.GetTableName(),
                EntityLeftId = user.Id,
                EntityLeftPermissionAlias = Consts.PermissionAlias.PermissionValueDelete,
                EntityRightTableName = _userGroupRepository.GetTableName(),
                EntityRightGroupsTableName = null,
                EntityRightEntityToEntityMappingsTableName = null,
                EntityRightId = userGroup.Id,
                EntityRightPermissionAlias = Consts.PermissionAlias.PermissionValueDelete,
                SqlExpressionPermissionTypeValueNeededOwner = "T1.\"Id\" = T2.\"UserId\""
            });

            if (!authorizeResult)
                throw new HttpResponseException(StatusCodes.Status403Forbidden, ErrorType.Permission,
                    Localize.Error.PermissionInsufficientPermissions);

            await _permissionValueEntityService.Delete(permissionValue, cancellationToken);

            await _authorizeEntityService.PurgeByEntityIdAsync(userToUserGroupMapping.Id, cancellationToken);

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