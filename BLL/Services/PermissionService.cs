using System;
using System.Threading;
using System.Threading.Tasks;
using BLL.Services.Base;
using Common.Models;
using DAL.Repository;
using Domain.Entities;
using Domain.Enums;
using Microsoft.Extensions.Logging;

namespace BLL.Services;

/// <summary>
/// Service to work with Permission entity
/// Permissions are managed in AppDbContext.Seed
/// </summary>
public interface IPermissionService : IEntityServiceBase<Permission>
{
    /// <summary>
    /// Gets entity by equal Alias & equal Type
    /// </summary>
    /// <param name="alias"></param>
    /// <param name="permissionType"></param>
    /// <returns></returns>
    Task<Permission> GetByAliasAndTypeAsync(string alias, PermissionType permissionType);
}

public class PermissionService : IPermissionService
{
    #region Fields

    private readonly ILogger<PermissionService> _logger;
    private readonly IPermissionRepository _permissionRepository;

    #endregion

    #region Ctor

    public PermissionService(ILogger<PermissionService> logger, IPermissionRepository permissionRepository)
    {
        _logger = logger;
        _permissionRepository = permissionRepository;
    }

    #endregion

    #region Methods

    public Task<Permission> Save(Permission entity, CancellationToken cancellationToken = default)
    {
        throw new ApplicationException(Localize.Error.PermissionDynamicManagementNotAllowed);
    }

    public Task Delete(Permission entity, CancellationToken cancellationToken = default)
    {
        throw new ApplicationException(Localize.Error.PermissionDynamicManagementNotAllowed);
    }

    public async Task<Permission> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _permissionRepository.SingleOrDefaultAsync(_ => _.Id == id);

        _logger.Log(LogLevel.Information,
            Localize.Log.Method(GetType(), nameof(GetByIdAsync), $"{entity?.GetType().Name} {entity?.Id}"));

        return entity;
    }

    public async Task<Permission> GetByAliasAndTypeAsync(string alias, PermissionType permissionType)
    {
        var entity = await _permissionRepository.SingleOrDefaultAsync(_ => _.Alias == alias && _.Type == permissionType);

        _logger.Log(LogLevel.Information,
            Localize.Log.Method(GetType(), nameof(GetByAliasAndTypeAsync), $"{entity?.GetType().Name} {entity?.Id}"));

        return entity;
    }

    #endregion
}