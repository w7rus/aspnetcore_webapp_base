using System;
using System.Threading;
using System.Threading.Tasks;
using Domain.Entities.Base;

namespace BLL.Services.Base;

public interface IEntityServiceBase<TEntity> where TEntity : EntityBase<Guid>
{
    Task Save(TEntity entity, CancellationToken cancellationToken);
    Task Delete(TEntity entity, CancellationToken cancellationToken);
    Task<TEntity> GetByIdAsync(Guid id);
}

public abstract class EntityServiceBase<TEntity> : IEntityServiceBase<TEntity> where TEntity : EntityBase<Guid>
{
    public abstract Task Save(TEntity entity, CancellationToken cancellationToken);
    public abstract Task Delete(TEntity entity, CancellationToken cancellationToken);
    public abstract Task<TEntity> GetByIdAsync(Guid id);
}