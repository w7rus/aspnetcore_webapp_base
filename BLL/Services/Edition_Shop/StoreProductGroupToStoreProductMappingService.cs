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

public interface
    IStoreProductGroupToStoreProductMappingService : IEntityServiceBase<StoreProductGroupToStoreProductMapping>
{
}

public class StoreProductGroupToStoreProductMappingService : IStoreProductGroupToStoreProductMappingService
{
    #region Fields

    private readonly ILogger<StoreProductGroupToStoreProductMappingService> _logger;

    private readonly IStoreProductGroupToStoreProductMappingRepository
        _storeProductGroupToStoreProductMappingRepository;

    private readonly IAppDbContextAction _appDbContextAction;
    private readonly HttpContext _httpContext;

    #endregion

    #region Ctor

    public StoreProductGroupToStoreProductMappingService(
        ILogger<StoreProductGroupToStoreProductMappingService> logger,
        IStoreProductGroupToStoreProductMappingRepository storeProductGroupToStoreProductMappingRepository,
        IAppDbContextAction appDbContextAction,
        HttpContext httpContext
    )
    {
        _logger = logger;
        _storeProductGroupToStoreProductMappingRepository = storeProductGroupToStoreProductMappingRepository;
        _appDbContextAction = appDbContextAction;
        _httpContext = httpContext;
    }

    #endregion

    #region Methods

    #endregion

    public async Task Save(StoreProductGroupToStoreProductMapping entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task Delete(
        StoreProductGroupToStoreProductMapping entity,
        CancellationToken cancellationToken = default
    )
    {
        throw new NotImplementedException();
    }

    public async Task<StoreProductGroupToStoreProductMapping> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default
    )
    {
        throw new NotImplementedException();
    }

    public async Task<StoreProductGroupToStoreProductMapping> Create(
        StoreProductGroupToStoreProductMapping entity,
        CancellationToken cancellationToken = default
    )
    {
        throw new NotImplementedException();
    }
}