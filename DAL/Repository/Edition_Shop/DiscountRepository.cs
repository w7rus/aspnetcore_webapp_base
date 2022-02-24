using System;
using DAL.Data;
using DAL.Repository.Base;
using Domain.Entities.Edition_Shop;

namespace DAL.Repository.Edition_Shop;

public interface IDiscountRepository : IRepositoryBase<Discount, Guid>
{
}

public class DiscountRepository : RepositoryBase<Discount, Guid>, IDiscountRepository
{
    public DiscountRepository(AppDbContext appDbContext) : base(appDbContext)
    {
    }
}