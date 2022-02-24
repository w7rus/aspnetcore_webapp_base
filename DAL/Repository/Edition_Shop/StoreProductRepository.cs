using System;
using DAL.Data;
using DAL.Repository.Base;
using Domain.Entities.Edition_Shop;

namespace DAL.Repository.Edition_Shop;

public interface IStoreProductRepository : IRepositoryBase<StoreProduct, Guid>
{
}

public class StoreProductRepository : RepositoryBase<StoreProduct, Guid>, IStoreProductRepository
{
    public StoreProductRepository(AppDbContext appDbContext) : base(appDbContext)
    {
    }
}