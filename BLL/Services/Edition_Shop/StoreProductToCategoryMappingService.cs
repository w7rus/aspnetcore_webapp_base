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

public interface IStoreProductToCategoryMappingService : IEntityServiceBase<StoreProductToCategoryMapping>
{
}

public class StoreProductToCategoryMappingService : IStoreProductToCategoryMappingService
{
    #region Fields

    private readonly ILogger<StoreProductToCategoryMappingService> _logger;
    private readonly IStoreProductToCategoryMappingRepository _storeProductToCategoryMappingRepository;
    private readonly IAppDbContextAction _appDbContextAction;
    private readonly HttpContext _httpContext;

    #endregion

    #region Ctor

    public StoreProductToCategoryMappingService(
        ILogger<StoreProductToCategoryMappingService> logger,
        IStoreProductToCategoryMappingRepository storeProductToCategoryMappingRepository,
        IAppDbContextAction appDbContextAction,
        IHttpContextAccessor httpContextAccessor
    )
    {
        _logger = logger;
        _storeProductToCategoryMappingRepository = storeProductToCategoryMappingRepository;
        _appDbContextAction = appDbContextAction;
        _httpContext = httpContextAccessor.HttpContext;
    }

    #endregion

    #region Methods

    #endregion

    public async Task<StoreProductToCategoryMapping> Save(StoreProductToCategoryMapping entity, CancellationToken cancellationToken = default)
    {
        _storeProductToCategoryMappingRepository.Save(entity);
        await _appDbContextAction.CommitAsync(cancellationToken);
        
        return entity;
    }

    public async Task Delete(StoreProductToCategoryMapping entity, CancellationToken cancellationToken = default)
    {
        _storeProductToCategoryMappingRepository.Delete(entity);
        await _appDbContextAction.CommitAsync(cancellationToken);
    }

    public async Task<StoreProductToCategoryMapping> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default
    )
    {
        return await _storeProductToCategoryMappingRepository.GetByIdAsync(id);
    }
}