using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BLL.Services.Base;
using Castle.Core.Internal;
using Common.Attributes;
using Common.Enums;
using Common.Exceptions;
using Common.Models;
using DAL.Extensions;
using DAL.Repository;
using Domain.Entities;
using Domain.Enums;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using IComparable = System.IComparable;
using ValueType = Common.Enums.ValueType;

namespace BLL.Services;

/// <summary>
/// Service to work with Permission entity
/// Permissions are managed in AppDbContext.Seed
/// </summary>
public interface IPermissionService : IEntityServiceBase<Permission>
{
    Task<Permission> GetByAliasAndTypeAsync(string alias, PermissionType permissionType);

    Task<(int total, IReadOnlyCollection<Permission> entities)> GetFilteredSortedPaged(
        FilterExpressionModel filterExpressionModel,
        FilterSortModel filterSortModel,
        PageModel pageModel,
        CancellationToken cancellationToken = default
    );
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
        var entity =
            await _permissionRepository.SingleOrDefaultAsync(_ => _.Alias == alias && _.Type == permissionType);

        _logger.Log(LogLevel.Information,
            Localize.Log.Method(GetType(), nameof(GetByAliasAndTypeAsync), $"{entity?.GetType().Name} {entity?.Id}"));

        return entity;
    }

    public async Task<(int total, IReadOnlyCollection<Permission> entities)> GetFilteredSortedPaged(
        FilterExpressionModel filterExpressionModel,
        FilterSortModel filterSortModel,
        PageModel pageModel,
        CancellationToken cancellationToken = default
    )
    {
        var result = _permissionRepository.GetFilteredSortedPaged(filterExpressionModel, filterSortModel, pageModel, null);

        _logger.Log(LogLevel.Information,
            Localize.Log.Method(GetType(), nameof(GetFilteredSortedPaged),
                $"{result.entities?.GetType().Name} {result.entities?.Count()}"));

        return (result.total, await result.entities?.ToArrayAsync(cancellationToken)!);
    }

    #endregion
}