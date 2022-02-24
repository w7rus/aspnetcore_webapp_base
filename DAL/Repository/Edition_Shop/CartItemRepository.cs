using System;
using DAL.Data;
using DAL.Repository.Base;
using Domain.Entities.Edition_Shop;

namespace DAL.Repository.Edition_Shop;

public interface ICartItemRepository : IRepositoryBase<CartItem, Guid>
{
}

public class CartItemRepository : RepositoryBase<CartItem, Guid>, ICartItemRepository
{
    public CartItemRepository(AppDbContext appDbContext) : base(appDbContext)
    {
    }
}