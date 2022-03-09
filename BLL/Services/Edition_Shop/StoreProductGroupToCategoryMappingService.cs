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

public interface IStoreProductGroupToCategoryMappingService : IEntityServiceBase<StoreProductGroupToCategoryMapping>
{
}

public class StoreProductGroupToCategoryMappingService : IStoreProductGroupToCategoryMappingService
{
    #region Fields

    private readonly ILogger<StoreProductGroupToCategoryMappingService> _logger;
    private readonly IStoreProductGroupToCategoryMappingRepository _storeProductGroupToCategoryMappingRepository;
    private readonly IAppDbContextAction _appDbContextAction;
    private readonly HttpContext _httpContext;

    #endregion

    #region Ctor

    public StoreProductGroupToCategoryMappingService(
        ILogger<StoreProductGroupToCategoryMappingService> logger,
        IStoreProductGroupToCategoryMappingRepository storeProductGroupToCategoryMappingRepository,
        IAppDbContextAction appDbContextAction,
        IHttpContextAccessor httpContextAccessor
    )
    {
        _logger = logger;
        _storeProductGroupToCategoryMappingRepository = storeProductGroupToCategoryMappingRepository;
        _appDbContextAction = appDbContextAction;
        _httpContext = httpContextAccessor.HttpContext;
    }

    #endregion

    #region Methods

    #endregion

    public async Task Save(StoreProductGroupToCategoryMapping entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task Delete(StoreProductGroupToCategoryMapping entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<StoreProductGroupToCategoryMapping> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default
    )
    {
        throw new NotImplementedException();
    }

    public async Task<StoreProductGroupToCategoryMapping> Create(
        StoreProductGroupToCategoryMapping entity,
        CancellationToken cancellationToken = default
    )
    {
        throw new NotImplementedException();
    }
}