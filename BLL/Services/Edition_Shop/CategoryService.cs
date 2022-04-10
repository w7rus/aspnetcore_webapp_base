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

public interface ICategoryService : IEntityServiceBase<Category>
{
}

public class CategoryService : ICategoryService
{
    #region Fields

    private readonly ILogger<CategoryService> _logger;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IAppDbContextAction _appDbContextAction;
    private readonly HttpContext _httpContext;

    #endregion

    #region Ctor

    public CategoryService(
        ILogger<CategoryService> logger,
        ICategoryRepository categoryRepository,
        IAppDbContextAction appDbContextAction,
        IHttpContextAccessor httpContextAccessor
    )
    {
        _logger = logger;
        _categoryRepository = categoryRepository;
        _appDbContextAction = appDbContextAction;
        _httpContext = httpContextAccessor.HttpContext;
    }

    #endregion

    #region Methods

    public async Task<Category> Save(Category entity, CancellationToken cancellationToken = default)
    {
        _categoryRepository.Save(entity);
        await _appDbContextAction.CommitAsync(cancellationToken);
        
        return entity;
    }

    public async Task Delete(Category entity, CancellationToken cancellationToken = default)
    {
        _categoryRepository.Delete(entity);
        await _appDbContextAction.CommitAsync(cancellationToken);
    }

    public async Task<Category> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _categoryRepository.GetByIdAsync(id);
    }

    #endregion
}