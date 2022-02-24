using System;
using DAL.Data;
using DAL.Repository.Base;
using Domain.Entities.Edition_Shop;

namespace DAL.Repository.Edition_Shop;

public interface IStoreProductGroupRepository : IRepositoryBase<StoreProductGroup, Guid>
{
}

public class StoreProductGroupRepository : RepositoryBase<StoreProductGroup, Guid>, IStoreProductGroupRepository
{
    public StoreProductGroupRepository(AppDbContext appDbContext) : base(appDbContext)
    {
    }
}