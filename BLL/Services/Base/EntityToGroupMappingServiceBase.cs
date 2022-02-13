using System;
using System.Threading;
using System.Threading.Tasks;
using Domain.Entities.Base;

namespace BLL.Services.Base;

public interface IEntityToGroupMappingServiceBase<TEntity> : IEntityServiceBase<TEntity>
    where TEntity : EntityBase<Guid>
{
}