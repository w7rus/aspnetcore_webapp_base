using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BLL.Services.Base;
using Common.Models;
using DAL.Data;
using DAL.Extensions;
using DAL.Repository;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BLL.Services.Entity;

/// <summary>
/// Service to work with PermissionValue entity
/// </summary>
public interface IPermissionValueEntityService : IEntityServiceBase<PermissionValue>
{
    Task<PermissionValue> GetByEntityIdPermissionId(
        Guid entityId,
        Guid permissionId,
        CancellationToken cancellationToken = default
    );

    Task<(int total, IReadOnlyCollection<PermissionValue> entities)> GetFilteredSortedPaged(
        FilterExpressionModel filterExpressionModel,
        FilterSortModel filterSortModel,
        PageModel pageModel,
        AuthorizeModel authorizeModel,
        CancellationToken cancellationToken = default
    );
}

public class PermissionValueEntityService : IPermissionValueEntityService
{
    #region Fields

    private readonly ILogger<PermissionValueEntityService> _logger;
    private readonly IPermissionValueRepository _permissionValueRepository;
    private readonly IAppDbContextAction _appDbContextAction;

    #endregion

    #region Ctor

    public PermissionValueEntityService(
        ILogger<PermissionValueEntityService> logger,
        IPermissionValueRepository permissionValueRepository,
        IAppDbContextAction appDbContextAction
    )
    {
        _logger = logger;
        _permissionValueRepository = permissionValueRepository;
        _appDbContextAction = appDbContextAction;
    }

    #endregion

    #region Methods

    public async Task<PermissionValue> Save(PermissionValue entity, CancellationToken cancellationToken = default)
    {
        _logger.Log(LogLevel.Information,
            Localize.Log.Method(GetType(), nameof(Save), $"{entity?.GetType().Name} {entity?.Id}"));

        _permissionValueRepository.Save(entity);
        await _appDbContextAction.CommitAsync(cancellationToken);

        return entity;
    }

    public async Task Delete(PermissionValue entity, CancellationToken cancellationToken = default)
    {
        _logger.Log(LogLevel.Information,
            Localize.Log.Method(GetType(), nameof(Delete), $"{entity?.GetType().Name} {entity?.Id}"));

        _permissionValueRepository.Delete(entity);
        await _appDbContextAction.CommitAsync(cancellationToken);
    }

    public async Task<PermissionValue> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _permissionValueRepository.SingleOrDefaultAsync(_ => _.Id == id);

        _logger.Log(LogLevel.Information,
            Localize.Log.Method(GetType(), nameof(GetByIdAsync), $"{entity?.GetType().Name} {entity?.Id}"));

        return entity;
    }

    public async Task<PermissionValue> GetByEntityIdPermissionId(
        Guid entityId,
        Guid permissionId,
        CancellationToken cancellationToken = default
    )
    {
        var entity = await _permissionValueRepository.SingleOrDefaultAsync(_ =>
            _.EntityId == entityId && _.PermissionId == permissionId);

        _logger.Log(LogLevel.Information,
            Localize.Log.Method(GetType(), nameof(GetByEntityIdPermissionId),
                $"{entity?.GetType().Name} {entity?.Id}"));

        return entity;
    }

    public async Task<(int total, IReadOnlyCollection<PermissionValue> entities)> GetFilteredSortedPaged(
        FilterExpressionModel filterExpressionModel,
        FilterSortModel filterSortModel,
        PageModel pageModel,
        AuthorizeModel authorizeModel,
        CancellationToken cancellationToken = default
    )
    {
        var result =
            _permissionValueRepository.GetFilteredSorted(filterExpressionModel, filterSortModel, authorizeModel);

        var total = result.Count();

        result = result.GetPage(pageModel);

        _logger.Log(LogLevel.Information,
            Localize.Log.Method(GetType(), nameof(GetFilteredSortedPaged),
                $"{result?.GetType().Name} {result?.Count()}"));

        return (total, await result?.ToArrayAsync(cancellationToken)!);
    }

    #endregion
}