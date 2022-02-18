using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
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
    /// Gets entities with PermissionId that equals given one
    /// </summary>
    /// <param name="permissionId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IReadOnlyCollection<TEntityPermissionValue>> GetByPermissionId(
        Guid permissionId,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Gets entities with EntityId that equals given one
    /// </summary>
    /// <param name="entityId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IReadOnlyCollection<TEntityPermissionValue>> GetByEntityId(
        Guid entityId,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Gets entity with PermissionId & EntityId that equals given one
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
}