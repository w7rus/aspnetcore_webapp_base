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

public interface IOrderItemService : IEntityServiceBase<OrderItem>
{
}

public class OrderItemService : IOrderItemService
{
    #region Fields

    private readonly ILogger<OrderItemService> _logger;
    private readonly IOrderItemRepository _orderItemRepository;
    private readonly IAppDbContextAction _appDbContextAction;
    private readonly HttpContext _httpContext;

    #endregion

    #region Ctor

    public OrderItemService(
        ILogger<OrderItemService> logger,
        IOrderItemRepository orderItemRepository,
        IAppDbContextAction appDbContextAction,
        HttpContext httpContext
    )
    {
        _logger = logger;
        _orderItemRepository = orderItemRepository;
        _appDbContextAction = appDbContextAction;
        _httpContext = httpContext;
    }

    #endregion

    #region Methods

    #endregion

    public async Task Save(OrderItem entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task Delete(OrderItem entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<OrderItem> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<OrderItem> Create(OrderItem entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}