using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BLL.Services.Base;
using Common.Models;
using DAL.Data;
using DAL.Repository;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BLL.Services;

/// <summary>
/// Service to work with UserGroupPermissionValue entity
/// </summary>
public interface
    IUserGroupPermissionValueService : IEntityPermissionValueServiceBase<UserGroupPermissionValue, UserGroup>
{
}

public class UserGroupPermissionValueService : IUserGroupPermissionValueService
{
    #region Fields

    private readonly ILogger<UserGroupPermissionValueService> _logger;
    private readonly IUserGroupPermissionValueRepository _userGroupPermissionValueRepository;
    private readonly IAppDbContextAction _appDbContextAction;

    #endregion

    #region Ctor

    public UserGroupPermissionValueService(
        ILogger<UserGroupPermissionValueService> logger,
        IUserGroupPermissionValueRepository userGroupPermissionValueRepository,
        IAppDbContextAction appDbContextAction
    )
    {
        _logger = logger;
        _userGroupPermissionValueRepository = userGroupPermissionValueRepository;
        _appDbContextAction = appDbContextAction;
    }

    #endregion

    #region Methods

    public async Task<UserGroupPermissionValue> Save(UserGroupPermissionValue entity, CancellationToken cancellationToken = default)
    {
        _logger.Log(LogLevel.Information,
            Localize.Log.Method(GetType(), nameof(Save), $"{entity?.GetType().Name} {entity?.Id}"));

        _userGroupPermissionValueRepository.Save(entity);
        await _appDbContextAction.CommitAsync(cancellationToken);
        
        return entity;
    }

    public async Task Delete(UserGroupPermissionValue entity, CancellationToken cancellationToken = default)
    {
        _logger.Log(LogLevel.Information,
            Localize.Log.Method(GetType(), nameof(Delete), $"{entity?.GetType().Name} {entity?.Id}"));

        _userGroupPermissionValueRepository.Delete(entity);
        await _appDbContextAction.CommitAsync(cancellationToken);
    }

    public async Task<UserGroupPermissionValue> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _userGroupPermissionValueRepository.SingleOrDefaultAsync(_ => _.Id == id);

        _logger.Log(LogLevel.Information,
            Localize.Log.Method(GetType(), nameof(GetByIdAsync), $"{entity?.GetType().Name} {entity?.Id}"));

        return entity;
    }

    public async Task<IReadOnlyCollection<UserGroupPermissionValue>> GetByPermissionId(
        Guid permissionId,
        CancellationToken cancellationToken = default
    )
    {
        var result = await _userGroupPermissionValueRepository.QueryMany(_ => _.PermissionId == permissionId)
            .ToArrayAsync(cancellationToken);

        _logger.Log(LogLevel.Information,
            Localize.Log.Method(GetType(), nameof(GetByPermissionId), $"{result?.GetType().Name} {result?.Length}"));

        return result;
    }

    public async Task<IReadOnlyCollection<UserGroupPermissionValue>> GetByEntityId(
        Guid entityId,
        CancellationToken cancellationToken = default
    )
    {
        var result = await _userGroupPermissionValueRepository.QueryMany(_ => _.EntityId == entityId)
            .ToArrayAsync(cancellationToken);

        _logger.Log(LogLevel.Information,
            Localize.Log.Method(GetType(), nameof(GetByEntityId), $"{result?.GetType().Name} {result?.Length}"));

        return result;
    }

    public async Task<UserGroupPermissionValue> GetByEntityIdPermissionId(
        Guid entityId,
        Guid permissionId,
        CancellationToken cancellationToken = default
    )
    {
        var entity = await _userGroupPermissionValueRepository.SingleOrDefaultAsync(_ =>
            _.EntityId == entityId && _.PermissionId == permissionId);

        _logger.Log(LogLevel.Information,
            Localize.Log.Method(GetType(), nameof(GetByEntityIdPermissionId), $"{entity?.GetType().Name} {entity?.Id}"));

        return entity;
    }

    #endregion
}