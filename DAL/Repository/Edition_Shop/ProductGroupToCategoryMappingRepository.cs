using System;
using DAL.Data;
using DAL.Repository.Base;
using Domain.Entities.Edition_Shop;

namespace DAL.Repository.Edition_Shop;

public interface IProductGroupToCategoryMappingRepository : IRepositoryBase<ProductGroupToCategoryMapping, Guid>
{
}

public class ProductGroupToCategoryMappingRepository : RepositoryBase<ProductGroupToCategoryMapping, Guid>,
    IProductGroupToCategoryMappingRepository
{
    public ProductGroupToCategoryMappingRepository(AppDbContext appDbContext) : base(appDbContext)
    {
    }
}