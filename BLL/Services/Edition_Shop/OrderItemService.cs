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
        IHttpContextAccessor httpContextAccessor
    )
    {
        _logger = logger;
        _orderItemRepository = orderItemRepository;
        _appDbContextAction = appDbContextAction;
        _httpContext = httpContextAccessor.HttpContext;
    }

    #endregion

    #region Methods

    #endregion

    public async Task<OrderItem> Save(OrderItem entity, CancellationToken cancellationToken = default)
    {
        _orderItemRepository.Save(entity);
        await _appDbContextAction.CommitAsync(cancellationToken);
        
        return entity;
    }

    public async Task Delete(OrderItem entity, CancellationToken cancellationToken = default)
    {
        _orderItemRepository.Delete(entity);
        await _appDbContextAction.CommitAsync(cancellationToken);
    }

    public async Task<OrderItem> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _orderItemRepository.GetByIdAsync(id);
    }
}