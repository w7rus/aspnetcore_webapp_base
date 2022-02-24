using System;
using DAL.Data;
using DAL.Repository.Base;
using Domain.Entities.Edition_Shop;

namespace DAL.Repository.Edition_Shop;

public interface IStoreRepository : IRepositoryBase<Store, Guid>
{
}

public class StoreRepository : RepositoryBase<Store, Guid>, IStoreRepository
{
    public StoreRepository(AppDbContext appDbContext) : base(appDbContext)
    {
    }
}