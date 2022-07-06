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

public interface IUserGroupTransferRequestEntityService : IEntityServiceBase<UserGroupTransferRequest>, IEntityCollectionServiceBase<UserGroupTransferRequest>
{
    Task<IReadOnlyCollection<UserGroupTransferRequest>> GetByUserGroupIdAsync(
        Guid userGroupId,
        PageModel pageModel,
        CancellationToken cancellationToken = default
    );
    
    Task<IReadOnlyCollection<UserGroupTransferRequest>> GetBySrcUserIdAsync(
        Guid srcUserId,
        PageModel pageModel,
        CancellationToken cancellationToken = default
    );
    
    Task<IReadOnlyCollection<UserGroupTransferRequest>> GetByDestUserIdAsync(
        Guid destUserId,
        PageModel pageModel,
        CancellationToken cancellationToken = default
    );
}

public class UserGroupTransferRequestEntityService : IUserGroupTransferRequestEntityService
{
    #region Ctor

    public UserGroupTransferRequestEntityService(
        ILogger<UserGroupTransferRequestEntityService> logger,
        IUserGroupTransferRequestRepository userGroupTransferRequestRepository,
        IAppDbContextAction appDbContextAction
    )
    {
        _logger = logger;
        _userGroupTransferRequestRepository = userGroupTransferRequestRepository;
        _appDbContextAction = appDbContextAction;
    }

    #endregion

    #region Fields

    private readonly ILogger<UserGroupTransferRequestEntityService> _logger;
    private readonly IUserGroupTransferRequestRepository _userGroupTransferRequestRepository;
    private readonly IAppDbContextAction _appDbContextAction;

    #endregion

    #region Methods

    public async Task<UserGroupTransferRequest> Save(UserGroupTransferRequest entity, CancellationToken cancellationToken = default)
    {
        _logger.Log(LogLevel.Information,
            Localize.Log.Method(GetType(), nameof(Save), $"{entity?.GetType().Name} {entity?.Id}"));

        _userGroupTransferRequestRepository.Save(entity);
        await _appDbContextAction.CommitAsync(cancellationToken);

        return entity;
    }

    public async Task Delete(UserGroupTransferRequest entity, CancellationToken cancellationToken = default)
    {
        _logger.Log(LogLevel.Information,
            Localize.Log.Method(GetType(), nameof(Delete), $"{entity?.GetType().Name} {entity?.Id}"));

        _userGroupTransferRequestRepository.Delete(entity);
        await _appDbContextAction.CommitAsync(cancellationToken);
    }

    public async Task<UserGroupTransferRequest> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _userGroupTransferRequestRepository.SingleOrDefaultAsync(_ => _.Id == id);

        _logger.Log(LogLevel.Information,
            Localize.Log.Method(GetType(), nameof(GetByIdAsync), $"{entity?.GetType().Name} {entity?.Id}"));

        return entity;
    }
    
    public async Task<IReadOnlyCollection<UserGroupTransferRequest>> GetByUserGroupIdAsync(Guid userGroupId, PageModel pageModel, CancellationToken cancellationToken = default)
    {
        var result = await _userGroupTransferRequestRepository
            .QueryMany(_ => _.UserGroupId == userGroupId)
            .OrderBy(_ => _.CreatedAt)
            .GetPage(pageModel)
            .ToArrayAsync(cancellationToken);

        _logger.Log(LogLevel.Information,
            Localize.Log.Method(GetType(), nameof(GetByUserGroupIdAsync),
                $"{result.GetType().Name} {result.Length}"));

        return result;
    }

    public async Task<IReadOnlyCollection<UserGroupTransferRequest>> GetBySrcUserIdAsync(Guid srcUserId, PageModel pageModel, CancellationToken cancellationToken = default)
    {
        var result = await _userGroupTransferRequestRepository
            .QueryMany(_ => _.SrcUserId == srcUserId)
            .OrderBy(_ => _.CreatedAt)
            .GetPage(pageModel)
            .ToArrayAsync(cancellationToken);

        _logger.Log(LogLevel.Information,
            Localize.Log.Method(GetType(), nameof(GetByUserGroupIdAsync),
                $"{result.GetType().Name} {result.Length}"));

        return result;
    }

    public async Task<IReadOnlyCollection<UserGroupTransferRequest>> GetByDestUserIdAsync(Guid destUserId, PageModel pageModel, CancellationToken cancellationToken = default)
    {
        var result = await _userGroupTransferRequestRepository
            .QueryMany(_ => _.DestUserId == destUserId)
            .OrderBy(_ => _.CreatedAt)
            .GetPage(pageModel)
            .ToArrayAsync(cancellationToken);

        _logger.Log(LogLevel.Information,
            Localize.Log.Method(GetType(), nameof(GetByUserGroupIdAsync),
                $"{result.GetType().Name} {result.Length}"));

        return result;
    }

    #endregion

    public async Task<IReadOnlyCollection<UserGroupTransferRequest>> Save(ICollection<UserGroupTransferRequest> entities, CancellationToken cancellationToken = default)
    {
        _logger.Log(LogLevel.Information,
            Localize.Log.Method(GetType(), nameof(Save), $"{entities?.GetType().Name}"));

        _userGroupTransferRequestRepository.Save(entities);
        await _appDbContextAction.CommitAsync(cancellationToken);

        return entities as IReadOnlyCollection<UserGroupTransferRequest>;
    }

    public async Task Delete(ICollection<UserGroupTransferRequest> entities, CancellationToken cancellationToken = default)
    {
        _logger.Log(LogLevel.Information,
            Localize.Log.Method(GetType(), nameof(Delete), $"{entities?.GetType().Name}"));

        var entitiesEnumerated = entities as UserGroupTransferRequest[] ?? entities?.ToArray();
        
        _userGroupTransferRequestRepository.Delete(entitiesEnumerated);
        await _appDbContextAction.CommitAsync(cancellationToken);
    }

    public async Task<(int total, IReadOnlyCollection<UserGroupTransferRequest> entities)> GetFiltered(
        FilterExpressionModel filterExpressionModel,
        FilterSortModel filterSortModel,
        PageModel pageModel,
        AuthorizeModel authorizeModel,
        FilterExpressionModel systemFilterExpressionModel = null,
        CancellationToken cancellationToken = default
    )
    {
        var result =
            _userGroupTransferRequestRepository.GetFilteredSorted(filterExpressionModel, filterSortModel, authorizeModel,
                systemFilterExpressionModel);

        var total = result.Count();

        result = result.GetPage(pageModel);

        _logger.Log(LogLevel.Information,
            Localize.Log.Method(GetType(), nameof(GetFiltered),
                $"{result?.GetType().Name} {result?.Count()}"));

        return (total, await result?.ToArrayAsync(cancellationToken)!);
    }
}