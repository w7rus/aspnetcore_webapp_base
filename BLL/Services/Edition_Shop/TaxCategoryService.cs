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

public interface ITaxCategoryService : IEntityServiceBase<TaxCategory>
{
}

public class TaxCategoryService : ITaxCategoryService
{
    #region Fields

    private readonly ILogger<TaxCategoryService> _logger;
    private readonly ITaxCategoryRepository _taxCategoryRepository;
    private readonly IAppDbContextAction _appDbContextAction;
    private readonly HttpContext _httpContext;

    #endregion

    #region Ctor

    public TaxCategoryService(
        ILogger<TaxCategoryService> logger,
        ITaxCategoryRepository taxCategoryRepository,
        IAppDbContextAction appDbContextAction,
        IHttpContextAccessor httpContextAccessor
    )
    {
        _logger = logger;
        _taxCategoryRepository = taxCategoryRepository;
        _appDbContextAction = appDbContextAction;
        _httpContext = httpContextAccessor.HttpContext;
    }

    #endregion

    #region Methods

    #endregion

    public async Task<TaxCategory> Save(TaxCategory entity, CancellationToken cancellationToken = default)
    {
        _taxCategoryRepository.Save(entity);
        await _appDbContextAction.CommitAsync(cancellationToken);
        
        return entity;
    }

    public async Task Delete(TaxCategory entity, CancellationToken cancellationToken = default)
    {
        _taxCategoryRepository.Delete(entity);
        await _appDbContextAction.CommitAsync(cancellationToken);
    }

    public async Task<TaxCategory> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _taxCategoryRepository.GetByIdAsync(id);
    }
}