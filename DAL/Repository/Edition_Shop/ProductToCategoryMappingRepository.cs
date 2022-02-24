using System;
using DAL.Data;
using DAL.Repository.Base;
using Domain.Entities.Edition_Shop;

namespace DAL.Repository.Edition_Shop;

public interface IProductToCategoryMappingRepository : IRepositoryBase<ProductToCategoryMapping, Guid>
{
}

public class ProductToCategoryMappingRepository : RepositoryBase<ProductToCategoryMapping, Guid>,
    IProductToCategoryMappingRepository
{
    public ProductToCategoryMappingRepository(AppDbContext appDbContext) : base(appDbContext)
    {
    }
}