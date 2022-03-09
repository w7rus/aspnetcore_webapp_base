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

public interface IProductToCategoryMappingService : IEntityServiceBase<ProductToCategoryMapping>
{
}

public class ProductToCategoryMappingService : IProductToCategoryMappingService
{
    #region Fields

    private readonly ILogger<ProductToCategoryMappingService> _logger;
    private readonly IProductToCategoryMappingRepository _productToCategoryMappingRepository;
    private readonly IAppDbContextAction _appDbContextAction;
    private readonly HttpContext _httpContext;

    #endregion

    #region Ctor

    public ProductToCategoryMappingService(
        ILogger<ProductToCategoryMappingService> logger,
        IProductToCategoryMappingRepository productToCategoryMappingRepository,
        IAppDbContextAction appDbContextAction,
        IHttpContextAccessor httpContextAccessor
    )
    {
        _logger = logger;
        _productToCategoryMappingRepository = productToCategoryMappingRepository;
        _appDbContextAction = appDbContextAction;
        _httpContext = httpContextAccessor.HttpContext;
    }

    #endregion

    #region Methods

    #endregion

    public async Task Save(ProductToCategoryMapping entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task Delete(ProductToCategoryMapping entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<ProductToCategoryMapping> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<ProductToCategoryMapping> Create(
        ProductToCategoryMapping entity,
        CancellationToken cancellationToken = default
    )
    {
        throw new NotImplementedException();
    }
}