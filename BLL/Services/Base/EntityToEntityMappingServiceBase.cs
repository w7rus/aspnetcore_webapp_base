using System;
using System.Threading;
using System.Threading.Tasks;
using Domain.Entities.Base;

namespace BLL.Services.Base;

/// <summary>
/// Base service every entity that utilizes grouping must implement
/// </summary>
/// <typeparam name="TEntity"></typeparam>
public interface IEntityToEntityMappingServiceBase<TEntity> : IEntityServiceBase<TEntity>
    where TEntity : EntityBase<Guid>
{
}