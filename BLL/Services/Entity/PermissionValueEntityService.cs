using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BLL.Services.Base;
using Common.Enums;
using Common.Models;
using DAL.Data;
using DAL.Extensions;
using DAL.Repository;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BLL.Services.Entity;

public interface IPermissionValueEntityCollectionService : IEntityServiceBase<PermissionValue>, IEntityCollectionServiceBase<PermissionValue>
{
    Task PurgeAsync(Guid entityId, CancellationToken cancellationToken = default);
}

public class PermissionValueEntityCollectionService : IPermissionValueEntityCollectionService
{
    #region Ctor

    public PermissionValueEntityCollectionService(
        ILogger<PermissionValueEntityCollectionService> logger,
        IPermissionValueRepository permissionValueRepository,
        IAppDbContextAction appDbContextAction
    )
    {
        _logger = logger;
        _permissionValueRepository = permissionValueRepository;
        _appDbContextAction = appDbContextAction;
    }

    #endregion

    #region Fields

    private readonly ILogger<PermissionValueEntityCollectionService> _logger;
    private readonly IPermissionValueRepository _permissionValueRepository;
    private readonly IAppDbContextAction _appDbContextAction;

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
    
    public async Task PurgeAsync(Guid entityId, CancellationToken cancellationToken = default)
    {
        _logger.Log(LogLevel.Information,
            Localize.Log.Method(GetType(), nameof(PurgeAsync), null));
        
        var query = _permissionValueRepository
            .QueryMany(_ => _.EntityId == entityId)
            .OrderBy(_ => _.CreatedAt);

        for (var page = 1;; page += 1)
        {
            var entities = await query.GetPage(new PageModel
            {
                Page = page,
                PageSize = 512
            }).ToArrayAsync(cancellationToken);

            _permissionValueRepository.Delete(entities);
            await _appDbContextAction.CommitAsync(cancellationToken);

            if (entities.Length < 512)
                break;
        }
    }

    public async Task<(int total, IReadOnlyCollection<PermissionValue> entities)> GetFilteredSortedPaged(
        FilterExpressionModel filterExpressionModel,
        FilterSortModel filterSortModel,
        PageModel pageModel,
        AuthorizeModel authorizeModel,
        FilterExpressionModel systemFilterExpressionModel = null,
        CancellationToken cancellationToken = default
    )
    {
        var result =
            _permissionValueRepository.GetFilteredSorted(filterExpressionModel, filterSortModel, authorizeModel,
                systemFilterExpressionModel);

        var total = result.Count();

        result = result.GetPage(pageModel);

        _logger.Log(LogLevel.Information,
            Localize.Log.Method(GetType(), nameof(GetFilteredSortedPaged),
                $"{result?.GetType().Name} {result?.Count()}"));

        return (total, await result?.ToArrayAsync(cancellationToken)!);
    }
    
    public async Task<IReadOnlyCollection<PermissionValue>> Save(ICollection<PermissionValue> entities, CancellationToken cancellationToken = default)
    {
        _logger.Log(LogLevel.Information,
            Localize.Log.Method(GetType(), nameof(Save), $"{entities?.GetType().Name}"));

        _permissionValueRepository.Save(entities);
        await _appDbContextAction.CommitAsync(cancellationToken);

        return entities as IReadOnlyCollection<PermissionValue>;
    }

    public async Task Delete(ICollection<PermissionValue> entities, CancellationToken cancellationToken = default)
    {
        _logger.Log(LogLevel.Information,
            Localize.Log.Method(GetType(), nameof(Delete), $"{entities?.GetType().Name}"));

        var entitiesEnumerated = entities as PermissionValue[] ?? entities?.ToArray();
        
        _permissionValueRepository.Delete(entitiesEnumerated);
        await _appDbContextAction.CommitAsync(cancellationToken);
    }

    #endregion
}