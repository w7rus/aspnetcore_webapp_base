using System;
using DAL.Data;
using DAL.Repository.Base;
using Domain.Entities.Edition_Shop;

namespace DAL.Repository.Edition_Shop;

public interface
    IStoreProductGroupToStoreProductMappingRepository : IRepositoryBase<StoreProductGroupToStoreProductMapping, Guid>
{
}

public class StoreProductGroupToStoreProductMappingRepository :
    RepositoryBase<StoreProductGroupToStoreProductMapping, Guid>, IStoreProductGroupToStoreProductMappingRepository
{
    public StoreProductGroupToStoreProductMappingRepository(AppDbContext appDbContext) : base(appDbContext)
    {
    }
}