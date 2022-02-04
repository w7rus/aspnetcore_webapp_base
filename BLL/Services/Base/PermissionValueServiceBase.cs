using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Domain.Entities.Base;

namespace BLL.Services.Base;

public interface
    IEntityPermissionValueServiceBase<TEntityPermissionValue, TEntity> : IEntityServiceBase<TEntityPermissionValue>
    where TEntityPermissionValue : EntityPermissionValueBase<TEntity>
    where TEntity : EntityBase<Guid>
{
    Task<TEntityPermissionValue> Add(
        byte[] value,
        ulong grant,
        Guid permissionId,
        Guid entityId,
        CancellationToken cancellationToken = new()
    );

    Task<IReadOnlyCollection<TEntityPermissionValue>> GetByPermissionId(
        Guid permissionId,
        CancellationToken cancellationToken = new()
    );

    Task<IReadOnlyCollection<TEntityPermissionValue>> GetByEntityId(
        Guid entityId,
        CancellationToken cancellationToken = new()
    );

    Task<TEntityPermissionValue> GetByEntityIdPermissionId(
        Guid entityId,
        Guid permissionId,
        CancellationToken cancellationToken = new()
    );
}

public abstract class
    EntityPermissionValueServiceBaseBase<TEntityPermissionValue, TEntity> : IEntityPermissionValueServiceBase<
        TEntityPermissionValue, TEntity>
    where TEntityPermissionValue : EntityPermissionValueBase<TEntity> where TEntity : EntityBase<Guid>
{
    public abstract Task Save(TEntityPermissionValue entity, CancellationToken cancellationToken = new());

    public abstract Task<TEntityPermissionValue> Add(
        byte[] value,
        ulong grant,
        Guid permissionId,
        Guid entityId,
        CancellationToken cancellationToken = new()
    );

    public abstract Task Delete(TEntityPermissionValue entity, CancellationToken cancellationToken = new());
    public abstract Task<TEntityPermissionValue> GetByIdAsync(Guid id, CancellationToken cancellationToken = new());

    public abstract Task<IReadOnlyCollection<TEntityPermissionValue>> GetByPermissionId(
        Guid permissionId,
        CancellationToken cancellationToken = new()
    );

    public abstract Task<IReadOnlyCollection<TEntityPermissionValue>> GetByEntityId(
        Guid entityId,
        CancellationToken cancellationToken = new()
    );

    public abstract Task<TEntityPermissionValue> GetByEntityIdPermissionId(
        Guid entityId,
        Guid permissionId,
        CancellationToken cancellationToken = new()
    );
}