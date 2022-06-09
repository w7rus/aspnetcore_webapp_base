using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Common.Models;
using Domain.Entities.Base;

namespace BLL.Services.Base;

/// <summary>
/// Base service every entity that utilizes permissions must implement
/// </summary>
/// <typeparam name="TEntityPermissionValue"></typeparam>
/// <typeparam name="TEntity"></typeparam>
public interface
    IEntityPermissionValueServiceBase<TEntityPermissionValue, TEntity> : IEntityServiceBase<TEntityPermissionValue>
    where TEntityPermissionValue : EntityPermissionValueBase<TEntity>
    where TEntity : EntityBase<Guid>
{
    /// <summary>
    /// Gets entities with equal PermissionId
    /// </summary>
    /// <param name="permissionId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IReadOnlyCollection<TEntityPermissionValue>> GetByPermissionId(
        Guid permissionId,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Gets entities with equal EntityId
    /// </summary>
    /// <param name="entityId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IReadOnlyCollection<TEntityPermissionValue>> GetByEntityId(
        Guid entityId,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Gets entity with equal PermissionId & equal EntityId
    /// </summary>
    /// <param name="entityId"></param>
    /// <param name="permissionId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<TEntityPermissionValue> GetByEntityIdPermissionId(
        Guid entityId,
        Guid permissionId,
        CancellationToken cancellationToken = default
    );

    Task<(int total, IReadOnlyCollection<TEntityPermissionValue> entities)> GetFilteredSortedPaged(
        FilterExpressionModel filterExpressionModel,
        FilterSortModel filterSortModel,
        PageModel pageModel,
        CancellationToken cancellationToken = default
    );
}