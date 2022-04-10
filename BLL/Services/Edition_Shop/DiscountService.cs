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

public interface IDiscountService : IEntityServiceBase<Discount>
{
}

public class DiscountService : IDiscountService
{
    #region Fields

    private readonly ILogger<DiscountService> _logger;
    private readonly IDiscountRepository _discountRepository;
    private readonly IAppDbContextAction _appDbContextAction;
    private readonly HttpContext _httpContext;

    #endregion

    #region Ctor

    public DiscountService(
        ILogger<DiscountService> logger,
        IDiscountRepository discountRepository,
        IAppDbContextAction appDbContextAction,
        IHttpContextAccessor httpContextAccessor
    )
    {
        _logger = logger;
        _discountRepository = discountRepository;
        _appDbContextAction = appDbContextAction;
        _httpContext = httpContextAccessor.HttpContext;
    }

    #endregion

    #region Methods

    #endregion

    public async Task<Discount> Save(Discount entity, CancellationToken cancellationToken = default)
    {
        _discountRepository.Save(entity);
        await _appDbContextAction.CommitAsync(cancellationToken);
        
        return entity;
    }

    public async Task Delete(Discount entity, CancellationToken cancellationToken = default)
    {
        _discountRepository.Delete(entity);
        await _appDbContextAction.CommitAsync(cancellationToken);
    }

    public async Task<Discount> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _discountRepository.GetByIdAsync(id);
    }
}