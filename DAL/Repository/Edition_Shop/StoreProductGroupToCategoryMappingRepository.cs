using System;
using DAL.Data;
using DAL.Repository.Base;
using Domain.Entities.Edition_Shop;

namespace DAL.Repository.Edition_Shop;

public interface
    IStoreProductGroupToCategoryMappingRepository : IRepositoryBase<StoreProductGroupToCategoryMapping, Guid>
{
}

public class StoreProductGroupToCategoryMappingRepository : RepositoryBase<StoreProductGroupToCategoryMapping, Guid>,
    IStoreProductGroupToCategoryMappingRepository
{
    public StoreProductGroupToCategoryMappingRepository(AppDbContext appDbContext) : base(appDbContext)
    {
    }
}