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

public interface IProductGroupToCategoryMappingService : IEntityServiceBase<ProductGroupToCategoryMapping>
{
}

public class ProductGroupToCategoryMappingService : IProductGroupToCategoryMappingService
{
    #region Fields

    private readonly ILogger<ProductGroupToCategoryMappingService> _logger;
    private readonly IProductGroupToCategoryMappingRepository _productGroupToCategoryMappingRepository;
    private readonly IAppDbContextAction _appDbContextAction;
    private readonly HttpContext _httpContext;

    #endregion

    #region Ctor

    public ProductGroupToCategoryMappingService(
        ILogger<ProductGroupToCategoryMappingService> logger,
        IProductGroupToCategoryMappingRepository productGroupToCategoryMappingRepository,
        IAppDbContextAction appDbContextAction,
        IHttpContextAccessor httpContextAccessor
    )
    {
        _logger = logger;
        _productGroupToCategoryMappingRepository = productGroupToCategoryMappingRepository;
        _appDbContextAction = appDbContextAction;
        _httpContext = httpContextAccessor.HttpContext;
    }

    #endregion

    #region Methods

    #endregion

    public async Task Save(ProductGroupToCategoryMapping entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task Delete(ProductGroupToCategoryMapping entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<ProductGroupToCategoryMapping> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default
    )
    {
        throw new NotImplementedException();
    }

    public async Task<ProductGroupToCategoryMapping> Create(
        ProductGroupToCategoryMapping entity,
        CancellationToken cancellationToken = default
    )
    {
        throw new NotImplementedException();
    }
}