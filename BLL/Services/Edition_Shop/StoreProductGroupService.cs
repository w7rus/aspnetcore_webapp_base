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

public interface IStoreProductGroupService : IEntityServiceBase<StoreProductGroup>
{
}

public class StoreProductGroupService : IStoreProductGroupService
{
    #region Fields

    private readonly ILogger<StoreProductGroupService> _logger;
    private readonly IStoreProductGroupRepository _storeProductGroupRepository;
    private readonly IAppDbContextAction _appDbContextAction;
    private readonly HttpContext _httpContext;

    #endregion

    #region Ctor

    public StoreProductGroupService(
        ILogger<StoreProductGroupService> logger,
        IStoreProductGroupRepository storeProductGroupRepository,
        IAppDbContextAction appDbContextAction,
        IHttpContextAccessor httpContextAccessor
    )
    {
        _logger = logger;
        _storeProductGroupRepository = storeProductGroupRepository;
        _appDbContextAction = appDbContextAction;
        _httpContext = httpContextAccessor.HttpContext;
    }

    #endregion

    #region Methods

    #endregion

    public async Task Save(StoreProductGroup entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task Delete(StoreProductGroup entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<StoreProductGroup> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<StoreProductGroup> Create(StoreProductGroup entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}