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

public interface ICartItemService : IEntityServiceBase<CartItem>
{
}

public class CartItemService : ICartItemService
{
    #region Fields

    private readonly ILogger<CartItemService> _logger;
    private readonly ICartItemRepository _cartItemRepository;
    private readonly IAppDbContextAction _appDbContextAction;
    private readonly HttpContext _httpContext;

    #endregion

    #region Ctor

    public CartItemService(
        ILogger<CartItemService> logger,
        ICartItemRepository cartItemRepository,
        IAppDbContextAction appDbContextAction,
        IHttpContextAccessor httpContextAccessor
    )
    {
        _logger = logger;
        _cartItemRepository = cartItemRepository;
        _appDbContextAction = appDbContextAction;
        _httpContext = httpContextAccessor.HttpContext;
    }

    #endregion

    #region Methods

    public async Task Save(CartItem entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task Delete(CartItem entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<CartItem> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<CartItem> Create(CartItem entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    #endregion
}