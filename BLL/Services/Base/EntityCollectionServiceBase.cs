using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Common.Models;
using Domain.Entities.Base;

namespace BLL.Services.Base;

public interface IEntityCollectionServiceBase<TEntity> where TEntity : EntityBase<Guid>
{
    Task<IReadOnlyCollection<TEntity>> Save(ICollection<TEntity> entities, CancellationToken cancellationToken = default);
    Task Delete(ICollection<TEntity> entities, CancellationToken cancellationToken = default);
    Task<(int total, IReadOnlyCollection<TEntity> entities)> GetFiltered(
        FilterExpressionModel filterExpressionModel,
        FilterSortModel filterSortModel,
        PageModel pageModel,
        AuthorizeModel authorizeModel,
        FilterExpressionModel systemFilterExpressionModel = null,
        CancellationToken cancellationToken = default
    );
}