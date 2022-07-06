using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BLL.Services.Base;
using Common.Exceptions;
using Common.Models;
using DAL.Data;
using DAL.Extensions;
using DAL.Repository;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BLL.Services.Entity;

/// <summary>
///     Service to work with UserGroup entity
/// </summary>
public interface IUserGroupEntityService : IEntityServiceBase<UserGroup>, IEntityCollectionServiceBase<UserGroup>
{
    Task<UserGroup> GetByAliasAsync(string alias);
    Task<bool> GetIsPriorityClaimed(long priority);
}

public class UserGroupEntityService : IUserGroupEntityService
{
    #region Fields

    #endregion

    #region Ctor

    private readonly ILogger<UserGroupEntityService> _logger;
    private readonly IUserGroupRepository _userGroupRepository;
    private readonly IAppDbContextAction _appDbContextAction;

    public UserGroupEntityService(
        ILogger<UserGroupEntityService> logger,
        IUserGroupRepository userGroupRepository,
        IAppDbContextAction appDbContextAction
    )
    {
        _logger = logger;
        _userGroupRepository = userGroupRepository;
        _appDbContextAction = appDbContextAction;
    }

    #endregion

    #region Methods

    public async Task<UserGroup> Save(UserGroup entity, CancellationToken cancellationToken = default)
    {
        _logger.Log(LogLevel.Information,
            Localize.Log.Method(GetType(), nameof(Save), $"{entity?.GetType().Name} {entity?.Id}"));

        if (entity.IsSystem)
            throw new CustomException(Localize.Error.UserGroupIsSystemManagementNotAllowed);

        _userGroupRepository.Save(entity);
        await _appDbContextAction.CommitAsync(cancellationToken);

        return entity;
    }

    public async Task Delete(UserGroup entity, CancellationToken cancellationToken = default)
    {
        _logger.Log(LogLevel.Information,
            Localize.Log.Method(GetType(), nameof(Delete), $"{entity?.GetType().Name} {entity?.Id}"));

        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        if (entity.IsSystem)
            throw new CustomException(Localize.Error.UserGroupIsSystemManagementNotAllowed);

        _userGroupRepository.Delete(entity);
        await _appDbContextAction.CommitAsync(cancellationToken);
    }

    public async Task<UserGroup> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _userGroupRepository.SingleOrDefaultAsync(_ => _.Id == id);

        _logger.Log(LogLevel.Information,
            Localize.Log.Method(GetType(), nameof(GetByIdAsync), $"{entity?.GetType().Name} {entity?.Id}"));

        return entity;
    }

    public async Task<UserGroup> GetByAliasAsync(string alias)
    {
        var entity = await _userGroupRepository.SingleOrDefaultAsync(_ => _.Alias == alias);

        _logger.Log(LogLevel.Information,
            Localize.Log.Method(GetType(), nameof(GetByAliasAsync), $"{entity?.GetType().Name} {entity?.Id}"));

        return entity;
    }

    public async Task<bool> GetIsPriorityClaimed(long priority)
    {
        var result = await _userGroupRepository.SingleOrDefaultAsync(_ => _.Priority == priority);

        _logger.Log(LogLevel.Information,
            Localize.Log.Method(GetType(), nameof(GetIsPriorityClaimed), $"{result?.GetType().Name} {result}"));

        return result != null;
    }

    public async Task<IReadOnlyCollection<UserGroup>> Save(ICollection<UserGroup> entities, CancellationToken cancellationToken = default)
    {
        _logger.Log(LogLevel.Information,
            Localize.Log.Method(GetType(), nameof(Save), $"{entities?.GetType().Name}"));

        _userGroupRepository.Save(entities);
        await _appDbContextAction.CommitAsync(cancellationToken);

        return entities as IReadOnlyCollection<UserGroup>;
    }

    public async Task Delete(ICollection<UserGroup> entities, CancellationToken cancellationToken = default)
    {
        _logger.Log(LogLevel.Information,
            Localize.Log.Method(GetType(), nameof(Delete), $"{entities?.GetType().Name}"));

        var entitiesEnumerated = entities as UserGroup[] ?? entities?.ToArray();
        
        _userGroupRepository.Delete(entitiesEnumerated);
        await _appDbContextAction.CommitAsync(cancellationToken);
    }

    public async Task<(int total, IReadOnlyCollection<UserGroup> entities)> GetFiltered(
        FilterExpressionModel filterExpressionModel,
        FilterSortModel filterSortModel,
        PageModel pageModel,
        AuthorizeModel authorizeModel,
        FilterExpressionModel systemFilterExpressionModel = null,
        CancellationToken cancellationToken = default
    )
    {
        var result =
            _userGroupRepository.GetFilteredSorted(filterExpressionModel, filterSortModel, authorizeModel,
                systemFilterExpressionModel);

        var total = result.Count();

        result = result.GetPage(pageModel);

        _logger.Log(LogLevel.Information,
            Localize.Log.Method(GetType(), nameof(GetFiltered),
                $"{result?.GetType().Name} {result?.Count()}"));

        return (total, await result?.ToArrayAsync(cancellationToken)!);
    }

    #endregion
}