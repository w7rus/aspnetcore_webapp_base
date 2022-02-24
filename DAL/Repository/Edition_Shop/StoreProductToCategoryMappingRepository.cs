using System;
using DAL.Data;
using DAL.Repository.Base;
using Domain.Entities.Edition_Shop;

namespace DAL.Repository.Edition_Shop;

public interface IStoreProductToCategoryMappingRepository : IRepositoryBase<StoreProductToCategoryMapping, Guid>
{
}

public class StoreProductToCategoryMappingRepository : RepositoryBase<StoreProductToCategoryMapping, Guid>,
    IStoreProductToCategoryMappingRepository
{
    public StoreProductToCategoryMappingRepository(AppDbContext appDbContext) : base(appDbContext)
    {
    }
}