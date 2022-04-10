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

public interface IStoreService : IEntityServiceBase<Store>
{
}

public class StoreService : IStoreService
{
    #region Fields

    private readonly ILogger<StoreService> _logger;
    private readonly IStoreRepository _storeRepository;
    private readonly IAppDbContextAction _appDbContextAction;
    private readonly HttpContext _httpContext;

    #endregion

    #region Ctor

    public StoreService(
        ILogger<StoreService> logger,
        IStoreRepository storeRepository,
        IAppDbContextAction appDbContextAction,
        IHttpContextAccessor httpContextAccessor
    )
    {
        _logger = logger;
        _storeRepository = storeRepository;
        _appDbContextAction = appDbContextAction;
        _httpContext = httpContextAccessor.HttpContext;
    }

    #endregion

    #region Methods

    #endregion

    public async Task<Store> Save(Store entity, CancellationToken cancellationToken = default)
    {
        _storeRepository.Save(entity);
        await _appDbContextAction.CommitAsync(cancellationToken);
        
        return entity;
    }

    public async Task Delete(Store entity, CancellationToken cancellationToken = default)
    {
        _storeRepository.Delete(entity);
        await _appDbContextAction.CommitAsync(cancellationToken);
    }

    public async Task<Store> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _storeRepository.GetByIdAsync(id);
    }
}