using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Common.Models;
using Domain.Entities.Base;

namespace BLL.Services.Base;

public interface IEntityFSPServiceBase<TEntity> where TEntity : EntityBase<Guid>
{
    Task<(int total, IReadOnlyCollection<TEntity> entities)> GetFilteredSortedPaged(
        FilterExpressionModel filterExpressionModel,
        FilterSortModel filterSortModel,
        PageModel pageModel,
        AuthorizeModel authorizeModel,
        CancellationToken cancellationToken = default
    );
}