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

public interface IStoreProductToDiscountMappingService : IEntityServiceBase<StoreProductToDiscountMapping>
{
}

public class StoreProductToDiscountMappingService : IStoreProductToDiscountMappingService
{
    #region Fields

    private readonly ILogger<StoreProductToDiscountMappingService> _logger;
    private readonly IStoreProductToDiscountMappingRepository _storeProductToDiscountMappingRepository;
    private readonly IAppDbContextAction _appDbContextAction;
    private readonly HttpContext _httpContext;

    #endregion

    #region Ctor

    public StoreProductToDiscountMappingService(
        ILogger<StoreProductToDiscountMappingService> logger,
        IStoreProductToDiscountMappingRepository storeProductToDiscountMappingRepository,
        IAppDbContextAction appDbContextAction,
        HttpContext httpContext
    )
    {
        _logger = logger;
        _storeProductToDiscountMappingRepository = storeProductToDiscountMappingRepository;
        _appDbContextAction = appDbContextAction;
        _httpContext = httpContext;
    }

    #endregion

    #region Methods

    #endregion

    public async Task Save(StoreProductToDiscountMapping entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task Delete(StoreProductToDiscountMapping entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<StoreProductToDiscountMapping> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default
    )
    {
        throw new NotImplementedException();
    }

    public async Task<StoreProductToDiscountMapping> Create(
        StoreProductToDiscountMapping entity,
        CancellationToken cancellationToken = default
    )
    {
        throw new NotImplementedException();
    }
}