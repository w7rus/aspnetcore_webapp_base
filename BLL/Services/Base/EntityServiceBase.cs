using System;
using System.Threading;
using System.Threading.Tasks;
using Domain.Entities.Base;

namespace BLL.Services.Base;

/// <summary>
/// Base service every entity must implement
/// </summary>
/// <typeparam name="TEntity"></typeparam>
public interface IEntityServiceBase<TEntity> where TEntity : EntityBase<Guid>
{
    /// <summary>
    /// Saves entity
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<TEntity> Save(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes entity
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task Delete(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets entity by PK
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<TEntity> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    // /// <summary>
    // /// Creates entity and saves it
    // /// </summary>
    // /// <param name="entity"></param>
    // /// <param name="cancellationToken"></param>
    // /// <returns></returns>
    // Task<TEntity> Create(TEntity entity, CancellationToken cancellationToken = default);
}