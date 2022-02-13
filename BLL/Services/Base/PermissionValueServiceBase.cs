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
    Task<IReadOnlyCollection<TEntityPermissionValue>> GetByPermissionId(
        Guid permissionId,
        CancellationToken cancellationToken = default
    );

    Task<IReadOnlyCollection<TEntityPermissionValue>> GetByEntityId(
        Guid entityId,
        CancellationToken cancellationToken = default
    );

    Task<TEntityPermissionValue> GetByEntityIdPermissionId(
        Guid entityId,
        Guid permissionId,
        CancellationToken cancellationToken = default
    );
}