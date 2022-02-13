using System;
using System.Threading;
using System.Threading.Tasks;
using BLL.Services.Base;
using Common.Models;
using DAL.Repository;
using Domain.Entities;
using Microsoft.Extensions.Logging;

namespace BLL.Services;

/// <summary>
/// Service to work with Permission entity
/// Permissions are managed in AppDbContext.Seed
/// </summary>
public interface IPermissionService : IEntityServiceBase<Permission>
{
    Task<Permission> GetByAliasAsync(string alias);
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

    public Task Save(Permission entity, CancellationToken cancellationToken = default)
    {
        throw new ApplicationException(Localize.Error.PermissionDynamicManagementNotAllowed);
    }

    public Task Delete(Permission entity, CancellationToken cancellationToken = default)
    {
        throw new ApplicationException(Localize.Error.PermissionDynamicManagementNotAllowed);
    }

    public async Task<Permission> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _permissionRepository.SingleOrDefaultAsync(_ => _.Id == id);
    }

    public Task<Permission> Create(Permission entity, CancellationToken cancellationToken = default)
    {
        throw new ApplicationException(Localize.Error.PermissionDynamicManagementNotAllowed);
    }

    public async Task<Permission> GetByAliasAsync(string alias)
    {
        return await _permissionRepository.SingleOrDefaultAsync(_ => _.Alias == alias);
    }

    #endregion
}