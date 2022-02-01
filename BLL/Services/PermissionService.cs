using System;
using System.Threading.Tasks;
using BLL.Services.Base;
using DAL.Repository;
using Domain.Entities;
using Microsoft.Extensions.Logging;

namespace BLL.Services;

/// <summary>
/// Service to work with Permission entity
/// Permissions are managed in AppDbContext.Seed
/// </summary>
public interface IPermissionService
{
    Task<Permission> GetByIdAsync(Guid id);
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

    public async Task<Permission> GetByIdAsync(Guid id)
    {
        return await _permissionRepository.SingleOrDefaultAsync(_ => _.Id == id);
    }

    public async Task<Permission> GetByAliasAsync(string alias)
    {
        return await _permissionRepository.SingleOrDefaultAsync(_ => _.Alias == alias);
    }

    #endregion
}