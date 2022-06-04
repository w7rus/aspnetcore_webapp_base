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
using Domain.Entities;
using Domain.Enums;
using DTO.Models.PermissionValue;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace BLL.Handlers;

public interface IUserGroupPermissionValueHandler
{
    Task<DTOResultBase> Create(PermissionValueCreate data, CancellationToken cancellationToken = default);
    Task<DTOResultBase> Read(PermissionValueRead data, CancellationToken cancellationToken = default);
    Task<DTOResultBase> ReadByEntity(PermissionValueReadByEntity data, CancellationToken cancellationToken = default);

    Task<DTOResultBase> ReadByPermission(
        PermissionValueReadByPermission data,
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
    private readonly IUserGroupPermissionValueService _userGroupPermissionValueService;
    private readonly IMapper _mapper;
    private readonly IPermissionService _permissionService;
    private readonly IUserGroupService _userGroupService;
    private readonly IUserToUserGroupAdvancedService _userToUserGroupAdvancedService;
    private readonly IUserAdvancedService _userAdvancedService;

    #endregion

    #region Ctor

    public UserGroupPermissionValueHandler(
        ILogger<HandlerBase> logger,
        IAppDbContextAction appDbContextAction,
        IUserGroupPermissionValueService userGroupPermissionValueService,
        IMapper mapper,
        IPermissionService permissionService,
        IUserGroupService userGroupService,
        IUserToUserGroupAdvancedService userToUserGroupAdvancedService,
        IUserAdvancedService userAdvancedService
    )
    {
        _logger = logger;
        _appDbContextAction = appDbContextAction;
        _userGroupPermissionValueService = userGroupPermissionValueService;
        _mapper = mapper;
        _permissionService = permissionService;
        _userGroupService = userGroupService;
        _userToUserGroupAdvancedService = userToUserGroupAdvancedService;
        _userAdvancedService = userAdvancedService;
    }

    #endregion

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
                throw new HttpResponseException(StatusCodes.Status500InternalServerError, ErrorType.HttpContext,
                    Localize.Error.UserGroupDoesNotExist);

            //Authorize permissionValue creation
            if (!await _userToUserGroupAdvancedService.AuthorizePermissionToPermission(user,
                    await _permissionService.GetByAliasAndTypeAsync("g_any_a_create_o_permissionvalue",
                        PermissionType.Value), userGroup,
                    await _permissionService.GetByAliasAndTypeAsync("g_any_a_create_o_permissionvalue",
                        userGroup.OwnerUser == user
                            ? PermissionType.ValueNeededOwner
                            : PermissionType.ValueNeededOthers), cancellationToken))
                throw new HttpResponseException(StatusCodes.Status403Forbidden, ErrorType.Permission,
                    Localize.Error.PermissionInsufficientPermissions);

            var permission = await _permissionService.GetByIdAsync(data.PermissionId, cancellationToken);
            if (permission == null)
                throw new HttpResponseException(StatusCodes.Status500InternalServerError, ErrorType.HttpContext,
                    Localize.Error.PermissionDoesNotExist);

            var permissionValue = _mapper.Map<UserGroupPermissionValue>(data);
            
            _logger.Log(LogLevel.Information,
                Localize.Log.Method(GetType(), nameof(Create),
                    $"{data.GetType().Name} mapped to {permissionValue.GetType().Name}"));

            permissionValue = await _userGroupPermissionValueService.Save(permissionValue, cancellationToken);
            
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

            var permissionValue = await _userGroupPermissionValueService.GetByIdAsync(data.Id, cancellationToken);
            if (permissionValue == null)
                throw new HttpResponseException(StatusCodes.Status500InternalServerError, ErrorType.HttpContext,
                    Localize.Error.PermissionValueDoesNotExist);

            var userGroup = await _userGroupService.GetByIdAsync(permissionValue.EntityId, cancellationToken);
            if (userGroup == null)
                throw new HttpResponseException(StatusCodes.Status500InternalServerError, ErrorType.HttpContext,
                    Localize.Error.UserGroupDoesNotExist);
            
            //Authorize permissionValue reading
            if (!await _userToUserGroupAdvancedService.AuthorizePermissionToPermission(user,
                    await _permissionService.GetByAliasAndTypeAsync("g_any_a_read_o_permissionvalue",
                        PermissionType.Value), userGroup,
                    await _permissionService.GetByAliasAndTypeAsync("g_any_a_read_o_permissionvalue",
                        userGroup.OwnerUser == user
                            ? PermissionType.ValueNeededOwner
                            : PermissionType.ValueNeededOthers), cancellationToken))
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

    //TODO: Rename DTO (Filtered) + inherit PageModel + Add Sorting Model
    public async Task<DTOResultBase> ReadByEntity(
        PermissionValueReadByEntity data,
        CancellationToken cancellationToken = default
    )
    {
        _logger.Log(LogLevel.Information, Localize.Log.MethodStart(GetType(), nameof(ReadByEntity)));

        if (ValidateModel(data) is { } validationResult)
            return validationResult;

        try
        {
            await _appDbContextAction.BeginTransactionAsync();
            
            var user = await _userAdvancedService.GetFromHttpContext(cancellationToken);
            if (user == null)
                throw new HttpResponseException(StatusCodes.Status500InternalServerError, ErrorType.HttpContext,
                    Localize.Error.UserDoesNotExistOrHttpContextMissingClaims);
            
            var permissionValuesGrouped = (await _userGroupPermissionValueService.GetByEntityId(data.EntityId, cancellationToken)).GroupBy(_ => _.Entity);

            var readablePermissionValues = new List<UserGroupPermissionValue>();

            foreach (var permissionValuesGroup in permissionValuesGrouped)
            {
                var permissionValue = permissionValuesGroup.First();
                
                var userGroup = await _userGroupService.GetByIdAsync(permissionValue.EntityId, cancellationToken);
                if (userGroup == null)
                    throw new HttpResponseException(StatusCodes.Status500InternalServerError, ErrorType.HttpContext,
                        Localize.Error.UserGroupDoesNotExist);
                
                //Authorize permissionValue reading
                if (!await _userToUserGroupAdvancedService.AuthorizePermissionToPermission(user,
                    await _permissionService.GetByAliasAndTypeAsync("g_any_a_read_o_permissionvalue",
                        PermissionType.Value), userGroup,
                    await _permissionService.GetByAliasAndTypeAsync("g_any_a_read_o_permissionvalue",
                        userGroup.OwnerUser == user
                            ? PermissionType.ValueNeededOwner
                            : PermissionType.ValueNeededOthers), cancellationToken))
                    continue;
                
                readablePermissionValues.AddRange(permissionValuesGroup);
            }

            await _appDbContextAction.CommitTransactionAsync();

            _logger.Log(LogLevel.Information, Localize.Log.MethodEnd(GetType(), nameof(ReadByEntity)));

            return new PermissionValueReadByEntityResult
            {
                Total = readablePermissionValues.Count,
                Items = readablePermissionValues.Select(_ => _mapper.ProjectTo<PermissionValueReadResult>(new [] {_}.AsQueryable()).Single())
            };
        }
        catch (Exception)
        {
            await _appDbContextAction.RollbackTransactionAsync();

            throw;
        }
    }

    //TODO: Rename DTO (Filtered) + inherit PageModel + Add Sorting Model
    public async Task<DTOResultBase> ReadByPermission(
        PermissionValueReadByPermission data,
        CancellationToken cancellationToken = default
    )
    {
        _logger.Log(LogLevel.Information, Localize.Log.MethodStart(GetType(), nameof(ReadByPermission)));

        if (ValidateModel(data) is { } validationResult)
            return validationResult;

        try
        {
            await _appDbContextAction.BeginTransactionAsync();
            
            var user = await _userAdvancedService.GetFromHttpContext(cancellationToken);
            if (user == null)
                throw new HttpResponseException(StatusCodes.Status500InternalServerError, ErrorType.HttpContext,
                    Localize.Error.UserDoesNotExistOrHttpContextMissingClaims);
            
            var permissionValuesGrouped = (await _userGroupPermissionValueService.GetByPermissionId(data.PermissionId, cancellationToken)).GroupBy(_ => _.Entity);

            var readablePermissionValues = new List<UserGroupPermissionValue>();

            foreach (var permissionValuesGroup in permissionValuesGrouped)
            {
                var permissionValue = permissionValuesGroup.First();
                
                var userGroup = await _userGroupService.GetByIdAsync(permissionValue.EntityId, cancellationToken);
                if (userGroup == null)
                    throw new HttpResponseException(StatusCodes.Status500InternalServerError, ErrorType.HttpContext,
                        Localize.Error.UserGroupDoesNotExist);
                
                //Authorize permissionValue reading
                if (!await _userToUserGroupAdvancedService.AuthorizePermissionToPermission(user,
                    await _permissionService.GetByAliasAndTypeAsync("g_any_a_read_o_permissionvalue",
                        PermissionType.Value), userGroup,
                    await _permissionService.GetByAliasAndTypeAsync("g_any_a_read_o_permissionvalue",
                        userGroup.OwnerUser == user
                            ? PermissionType.ValueNeededOwner
                            : PermissionType.ValueNeededOthers), cancellationToken))
                    continue;
                
                readablePermissionValues.AddRange(permissionValuesGroup);
            }
            
            await _appDbContextAction.CommitTransactionAsync();

            _logger.Log(LogLevel.Information, Localize.Log.MethodEnd(GetType(), nameof(ReadByPermission)));

            return new PermissionValueReadByEntityResult
            {
                Total = readablePermissionValues.Count,
                Items = readablePermissionValues.Select(_ => _mapper.ProjectTo<PermissionValueReadResult>(new [] {_}.AsQueryable()).Single())
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

            var permissionValue = await _userGroupPermissionValueService.GetByIdAsync(data.Id, cancellationToken);
            if (permissionValue == null)
                throw new HttpResponseException(StatusCodes.Status500InternalServerError, ErrorType.HttpContext,
                    Localize.Error.PermissionValueDoesNotExist);
            
            var userGroup = await _userGroupService.GetByIdAsync(permissionValue.EntityId, cancellationToken);
            if (userGroup == null)
                throw new HttpResponseException(StatusCodes.Status500InternalServerError, ErrorType.HttpContext,
                    Localize.Error.UserGroupDoesNotExist);
            
            //Authorize permissionValue update
            if (!await _userToUserGroupAdvancedService.AuthorizePermissionToPermission(user,
                    await _permissionService.GetByAliasAndTypeAsync("g_any_a_update_o_permissionvalue",
                        PermissionType.Value), userGroup,
                    await _permissionService.GetByAliasAndTypeAsync("g_any_a_update_o_permissionvalue",
                        userGroup.OwnerUser == user
                            ? PermissionType.ValueNeededOwner
                            : PermissionType.ValueNeededOthers), cancellationToken))
                throw new HttpResponseException(StatusCodes.Status403Forbidden, ErrorType.Permission,
                    Localize.Error.PermissionInsufficientPermissions);

            _mapper.Map(data, permissionValue);
            
            _logger.Log(LogLevel.Information,
                Localize.Log.Method(GetType(), nameof(Update),
                    $"{data.GetType().Name} mapped to {permissionValue.GetType().Name}"));

            await _userGroupPermissionValueService.Save(permissionValue, cancellationToken);
            
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

            var permissionValue = await _userGroupPermissionValueService.GetByIdAsync(data.Id, cancellationToken);
            if (permissionValue == null)
                throw new HttpResponseException(StatusCodes.Status500InternalServerError, ErrorType.HttpContext,
                    Localize.Error.PermissionValueDoesNotExist);
            
            var userGroup = await _userGroupService.GetByIdAsync(permissionValue.EntityId, cancellationToken);
            if (userGroup == null)
                throw new HttpResponseException(StatusCodes.Status500InternalServerError, ErrorType.HttpContext,
                    Localize.Error.UserGroupDoesNotExist);
            
            //Authorize permissionValue update
            if (!await _userToUserGroupAdvancedService.AuthorizePermissionToPermission(user,
                    await _permissionService.GetByAliasAndTypeAsync("g_any_a_delete_o_permissionvalue",
                        PermissionType.Value), userGroup,
                    await _permissionService.GetByAliasAndTypeAsync("g_any_a_delete_o_permissionvalue",
                        userGroup.OwnerUser == user
                            ? PermissionType.ValueNeededOwner
                            : PermissionType.ValueNeededOthers), cancellationToken))
                throw new HttpResponseException(StatusCodes.Status403Forbidden, ErrorType.Permission,
                    Localize.Error.PermissionInsufficientPermissions);

            await _userGroupPermissionValueService.Delete(permissionValue, cancellationToken);
            
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