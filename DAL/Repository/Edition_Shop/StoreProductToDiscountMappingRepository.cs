using System;
using DAL.Data;
using DAL.Repository.Base;
using Domain.Entities.Edition_Shop;

namespace DAL.Repository.Edition_Shop;

public interface IStoreProductToDiscountMappingRepository : IRepositoryBase<StoreProductToDiscountMapping, Guid>
{
}

public class StoreProductToDiscountMappingRepository : RepositoryBase<StoreProductToDiscountMapping, Guid>,
    IStoreProductToDiscountMappingRepository
{
    public StoreProductToDiscountMappingRepository(AppDbContext appDbContext) : base(appDbContext)
    {
    }
}