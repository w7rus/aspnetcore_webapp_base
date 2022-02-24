using System;
using DAL.Data;
using DAL.Repository.Base;
using Domain.Entities.Edition_Shop;

namespace DAL.Repository.Edition_Shop;

public interface IOrderRepository : IRepositoryBase<Order, Guid>
{
}

public class OrderRepository : RepositoryBase<Order, Guid>, IOrderRepository
{
    public OrderRepository(AppDbContext appDbContext) : base(appDbContext)
    {
    }
}