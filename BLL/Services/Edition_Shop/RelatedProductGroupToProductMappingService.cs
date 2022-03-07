using System;
using System.Threading;
using System.Threading.Tasks;
using BLL.Services.Base;
using DAL.Data;
using DAL.Repository.Edition_Shop;
using Domain.Entities.Edition_Shop;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace BLL.Services.Edition_Shop;

public interface IRelatedProductGroupToProductMappingService : IEntityServiceBase<RelatedProductGroupToProductMapping>
{
}

public class RelatedProductGroupToProductMappingService : IRelatedProductGroupToProductMappingService
{
    #region Fields

    private readonly ILogger<RelatedProductGroupToProductMappingService> _logger;
    private readonly IRelatedProductGroupToProductMappingRepository _relatedProductGroupToProductMappingRepository;
    private readonly IAppDbContextAction _appDbContextAction;
    private readonly HttpContext _httpContext;

    #endregion

    #region Ctor

    public RelatedProductGroupToProductMappingService(
        ILogger<RelatedProductGroupToProductMappingService> logger,
        IRelatedProductGroupToProductMappingRepository relatedProductGroupToProductMappingRepository,
        IAppDbContextAction appDbContextAction,
        HttpContext httpContext
    )
    {
        _logger = logger;
        _relatedProductGroupToProductMappingRepository = relatedProductGroupToProductMappingRepository;
        _appDbContextAction = appDbContextAction;
        _httpContext = httpContext;
    }

    #endregion

    #region Methods

    #endregion

    public async Task Save(RelatedProductGroupToProductMapping entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task Delete(RelatedProductGroupToProductMapping entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<RelatedProductGroupToProductMapping> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default
    )
    {
        throw new NotImplementedException();
    }

    public async Task<RelatedProductGroupToProductMapping> Create(
        RelatedProductGroupToProductMapping entity,
        CancellationToken cancellationToken = default
    )
    {
        throw new NotImplementedException();
    }
}