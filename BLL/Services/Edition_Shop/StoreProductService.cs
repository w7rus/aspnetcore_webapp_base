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

public interface IStoreProductService : IEntityServiceBase<StoreProduct>
{
}

public class StoreProductService : IStoreProductService
{
    #region Fields

    private readonly ILogger<StoreProductService> _logger;
    private readonly IStoreProductRepository _storeProductRepository;
    private readonly IAppDbContextAction _appDbContextAction;
    private readonly HttpContext _httpContext;

    #endregion

    #region Ctor

    public StoreProductService(
        ILogger<StoreProductService> logger,
        IStoreProductRepository storeProductRepository,
        IAppDbContextAction appDbContextAction,
        IHttpContextAccessor httpContextAccessor
    )
    {
        _logger = logger;
        _storeProductRepository = storeProductRepository;
        _appDbContextAction = appDbContextAction;
        _httpContext = httpContextAccessor.HttpContext;
    }

    #endregion

    #region Methods

    #endregion

    public async Task Save(StoreProduct entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task Delete(StoreProduct entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<StoreProduct> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<StoreProduct> Create(StoreProduct entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}