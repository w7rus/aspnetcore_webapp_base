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

public interface IProductGroupService : IEntityServiceBase<ProductGroup>
{
}

public class ProductGroupService : IProductGroupService
{
    #region Fields

    private readonly ILogger<ProductGroupService> _logger;
    private readonly IProductGroupRepository _productGroupRepository;
    private readonly IAppDbContextAction _appDbContextAction;
    private readonly HttpContext _httpContext;

    #endregion

    #region Ctor

    public ProductGroupService(
        ILogger<ProductGroupService> logger,
        IProductGroupRepository productGroupRepository,
        IAppDbContextAction appDbContextAction,
        IHttpContextAccessor httpContextAccessor
    )
    {
        _logger = logger;
        _productGroupRepository = productGroupRepository;
        _appDbContextAction = appDbContextAction;
        _httpContext = httpContextAccessor.HttpContext;
    }

    #endregion

    #region Methods

    #endregion

    public async Task<ProductGroup> Save(ProductGroup entity, CancellationToken cancellationToken = default)
    {
        _productGroupRepository.Save(entity);
        await _appDbContextAction.CommitAsync(cancellationToken);
        
        return entity;
    }

    public async Task Delete(ProductGroup entity, CancellationToken cancellationToken = default)
    {
        _productGroupRepository.Delete(entity);
        await _appDbContextAction.CommitAsync(cancellationToken);
    }

    public async Task<ProductGroup> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _productGroupRepository.GetByIdAsync(id);
    }
}