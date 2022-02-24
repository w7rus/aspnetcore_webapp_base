using System;
using DAL.Data;
using DAL.Repository.Base;
using Domain.Entities.Edition_Shop;

namespace DAL.Repository.Edition_Shop;

public interface IOrderItemRepository : IRepositoryBase<OrderItem, Guid>
{
}

public class OrderItemRepository : RepositoryBase<OrderItem, Guid>, IOrderItemRepository
{
    public OrderItemRepository(AppDbContext appDbContext) : base(appDbContext)
    {
    }
}