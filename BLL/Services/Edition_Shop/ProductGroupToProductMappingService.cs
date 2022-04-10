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

public interface IProductGroupToProductMappingService : IEntityServiceBase<ProductGroupToProductMapping>
{
}

public class ProductGroupToProductMappingService : IProductGroupToProductMappingService
{
    #region Fields

    private readonly ILogger<ProductGroupToProductMappingService> _logger;
    private readonly IProductGroupToProductMappingRepository _productGroupToProductMappingRepository;
    private readonly IAppDbContextAction _appDbContextAction;
    private readonly HttpContext _httpContext;

    #endregion

    #region Ctor

    public ProductGroupToProductMappingService(
        ILogger<ProductGroupToProductMappingService> logger,
        IProductGroupToProductMappingRepository productGroupToProductMappingRepository,
        IAppDbContextAction appDbContextAction,
        IHttpContextAccessor httpContextAccessor
    )
    {
        _logger = logger;
        _productGroupToProductMappingRepository = productGroupToProductMappingRepository;
        _appDbContextAction = appDbContextAction;
        _httpContext = httpContextAccessor.HttpContext;
    }

    #endregion

    #region Methods

    #endregion

    public async Task<ProductGroupToProductMapping> Save(ProductGroupToProductMapping entity, CancellationToken cancellationToken = default)
    {
        _productGroupToProductMappingRepository.Save(entity);
        await _appDbContextAction.CommitAsync(cancellationToken);
        
        return entity;
    }

    public async Task Delete(ProductGroupToProductMapping entity, CancellationToken cancellationToken = default)
    {
        _productGroupToProductMappingRepository.Delete(entity);
        await _appDbContextAction.CommitAsync(cancellationToken);
    }

    public async Task<ProductGroupToProductMapping> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _productGroupToProductMappingRepository.GetByIdAsync(id);
    }
}