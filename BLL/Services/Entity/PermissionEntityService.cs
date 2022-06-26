using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BLL.Services.Base;
using Common.Models;
using DAL.Extensions;
using DAL.Repository;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BLL.Services.Entity;

/// <summary>
/// Service to work with Permission entity
/// Permissions are managed in AppDbContext.Seed
/// </summary>
public interface IPermissionEntityService : IEntityServiceBase<Permission>
{
    Task<(int total, IReadOnlyCollection<Permission> entities)> GetFilteredSortedPaged(
        FilterExpressionModel filterExpressionModel,
        FilterSortModel filterSortModel,
        PageModel pageModel,
        CancellationToken cancellationToken = default
    );
}

public class PermissionEntityService : IPermissionEntityService
{
    #region Fields

    private readonly ILogger<PermissionEntityService> _logger;
    private readonly IPermissionRepository _permissionRepository;

    #endregion

    #region Ctor

    public PermissionEntityService(ILogger<PermissionEntityService> logger, IPermissionRepository permissionRepository)
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

    public async Task<(int total, IReadOnlyCollection<Permission> entities)> GetFilteredSortedPaged(
        FilterExpressionModel filterExpressionModel,
        FilterSortModel filterSortModel,
        PageModel pageModel,
        CancellationToken cancellationToken = default
    )
    {
        var result = _permissionRepository.GetFilteredSorted(filterExpressionModel, filterSortModel, null);
        
        var total = result.Count();

        result = result.GetPage(pageModel);

        _logger.Log(LogLevel.Information,
            Localize.Log.Method(GetType(), nameof(GetFilteredSortedPaged),
                $"{result?.GetType().Name} {result?.Count()}"));

        return (total, await result?.ToArrayAsync(cancellationToken)!);
    }

    #endregion
}