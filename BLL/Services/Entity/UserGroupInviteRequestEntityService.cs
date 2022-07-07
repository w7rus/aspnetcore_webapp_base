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

public interface IUserGroupInviteRequestEntityService : IEntityServiceBase<UserGroupInviteRequest>,
    IEntityCollectionServiceBase<UserGroupInviteRequest>
{
    Task<IReadOnlyCollection<UserGroupInviteRequest>> GetByUserGroupIdAsync(
        Guid userGroupId,
        PageModel pageModel,
        CancellationToken cancellationToken = default
    );

    Task<IReadOnlyCollection<UserGroupInviteRequest>> GetBySrcUserIdAsync(
        Guid srcUserId,
        PageModel pageModel,
        CancellationToken cancellationToken = default
    );

    Task<IReadOnlyCollection<UserGroupInviteRequest>> GetByDestUserIdAsync(
        Guid destUserId,
        PageModel pageModel,
        CancellationToken cancellationToken = default
    );

    Task<UserGroupInviteRequest> GetByUserGroupIdDestUserIdAsync(
        Guid userGroupId,
        Guid destUserId,
        CancellationToken cancellationToken = default
    );
}

public class UserGroupInviteRequestEntityService : IUserGroupInviteRequestEntityService
{
    #region Ctor

    public UserGroupInviteRequestEntityService(
        ILogger<UserGroupInviteRequestEntityService> logger,
        IUserGroupInviteRequestRepository userGroupInviteRequestRepository,
        IAppDbContextAction appDbContextAction
    )
    {
        _logger = logger;
        _userGroupInviteRequestRepository = userGroupInviteRequestRepository;
        _appDbContextAction = appDbContextAction;
    }

    #endregion

    public async Task<IReadOnlyCollection<UserGroupInviteRequest>> Save(
        ICollection<UserGroupInviteRequest> entities,
        CancellationToken cancellationToken = default
    )
    {
        _logger.Log(LogLevel.Information,
            Localize.Log.Method(GetType(), nameof(Save), $"{entities?.GetType().Name}"));

        _userGroupInviteRequestRepository.Save(entities);
        await _appDbContextAction.CommitAsync(cancellationToken);

        return entities as IReadOnlyCollection<UserGroupInviteRequest>;
    }

    public async Task Delete(
        ICollection<UserGroupInviteRequest> entities,
        CancellationToken cancellationToken = default
    )
    {
        _logger.Log(LogLevel.Information,
            Localize.Log.Method(GetType(), nameof(Delete), $"{entities?.GetType().Name}"));

        var entitiesEnumerated = entities as UserGroupInviteRequest[] ?? entities?.ToArray();

        _userGroupInviteRequestRepository.Delete(entitiesEnumerated);
        await _appDbContextAction.CommitAsync(cancellationToken);
    }

    public async Task<(int total, IReadOnlyCollection<UserGroupInviteRequest> entities)> GetFiltered(
        FilterExpressionModel filterExpressionModel,
        FilterSortModel filterSortModel,
        PageModel pageModel,
        AuthorizeModel authorizeModel,
        FilterExpressionModel systemFilterExpressionModel = null,
        CancellationToken cancellationToken = default
    )
    {
        var result =
            _userGroupInviteRequestRepository.GetFilteredSorted(filterExpressionModel, filterSortModel, authorizeModel,
                systemFilterExpressionModel);

        var total = result.Count();

        result = result.GetPage(pageModel);

        _logger.Log(LogLevel.Information,
            Localize.Log.Method(GetType(), nameof(GetFiltered),
                $"{result?.GetType().Name} {result?.Count()}"));

        return (total, await result?.ToArrayAsync(cancellationToken)!);
    }

    #region Fields

    private readonly ILogger<UserGroupInviteRequestEntityService> _logger;
    private readonly IUserGroupInviteRequestRepository _userGroupInviteRequestRepository;
    private readonly IAppDbContextAction _appDbContextAction;

    #endregion

    #region Methods

    public async Task<UserGroupInviteRequest> Save(
        UserGroupInviteRequest entity,
        CancellationToken cancellationToken = default
    )
    {
        _logger.Log(LogLevel.Information,
            Localize.Log.Method(GetType(), nameof(Save), $"{entity?.GetType().Name} {entity?.Id}"));

        _userGroupInviteRequestRepository.Save(entity);
        await _appDbContextAction.CommitAsync(cancellationToken);

        return entity;
    }

    public async Task Delete(UserGroupInviteRequest entity, CancellationToken cancellationToken = default)
    {
        _logger.Log(LogLevel.Information,
            Localize.Log.Method(GetType(), nameof(Delete), $"{entity?.GetType().Name} {entity?.Id}"));

        _userGroupInviteRequestRepository.Delete(entity);
        await _appDbContextAction.CommitAsync(cancellationToken);
    }

    public async Task<UserGroupInviteRequest> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _userGroupInviteRequestRepository.SingleOrDefaultAsync(_ => _.Id == id);

        _logger.Log(LogLevel.Information,
            Localize.Log.Method(GetType(), nameof(GetByIdAsync), $"{entity?.GetType().Name} {entity?.Id}"));

        return entity;
    }

    public async Task<IReadOnlyCollection<UserGroupInviteRequest>> GetByUserGroupIdAsync(
        Guid userGroupId,
        PageModel pageModel,
        CancellationToken cancellationToken = default
    )
    {
        var result = await _userGroupInviteRequestRepository
            .QueryMany(_ => _.UserGroupId == userGroupId)
            .OrderBy(_ => _.CreatedAt)
            .GetPage(pageModel)
            .ToArrayAsync(cancellationToken);

        _logger.Log(LogLevel.Information,
            Localize.Log.Method(GetType(), nameof(GetByUserGroupIdAsync),
                $"{result.GetType().Name} {result.Length}"));

        return result;
    }

    public async Task<IReadOnlyCollection<UserGroupInviteRequest>> GetBySrcUserIdAsync(
        Guid srcUserId,
        PageModel pageModel,
        CancellationToken cancellationToken = default
    )
    {
        var result = await _userGroupInviteRequestRepository
            .QueryMany(_ => _.SrcUserId == srcUserId)
            .OrderBy(_ => _.CreatedAt)
            .GetPage(pageModel)
            .ToArrayAsync(cancellationToken);

        _logger.Log(LogLevel.Information,
            Localize.Log.Method(GetType(), nameof(GetByUserGroupIdAsync),
                $"{result.GetType().Name} {result.Length}"));

        return result;
    }

    public async Task<IReadOnlyCollection<UserGroupInviteRequest>> GetByDestUserIdAsync(
        Guid destUserId,
        PageModel pageModel,
        CancellationToken cancellationToken = default
    )
    {
        var result = await _userGroupInviteRequestRepository
            .QueryMany(_ => _.DestUserId == destUserId)
            .OrderBy(_ => _.CreatedAt)
            .GetPage(pageModel)
            .ToArrayAsync(cancellationToken);

        _logger.Log(LogLevel.Information,
            Localize.Log.Method(GetType(), nameof(GetByUserGroupIdAsync),
                $"{result.GetType().Name} {result.Length}"));

        return result;
    }

    public async Task<UserGroupInviteRequest> GetByUserGroupIdDestUserIdAsync(
        Guid userGroupId,
        Guid destUserId,
        CancellationToken cancellationToken = default
    )
    {
        var entity =
            await _userGroupInviteRequestRepository.SingleOrDefaultAsync(_ =>
                _.UserGroupId == userGroupId && _.DestUserId == destUserId);

        _logger.Log(LogLevel.Information,
            Localize.Log.Method(GetType(), nameof(GetByIdAsync), $"{entity?.GetType().Name} {entity?.Id}"));

        return entity;
    }

    #endregion
}