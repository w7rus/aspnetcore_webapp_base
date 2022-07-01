using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Domain.Entities.Base;

namespace BLL.Services.Base;

public interface IEntityServiceBaseAdditional<TEntity> where TEntity : EntityBase<Guid>
{
    /// <summary>
    ///     Saves entity
    /// </summary>
    /// <param name="entities"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IReadOnlyCollection<TEntity>> Save(ICollection<TEntity> entities, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Deletes entity
    /// </summary>
    /// <param name="entities"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task Delete(ICollection<TEntity> entities, CancellationToken cancellationToken = default);
}