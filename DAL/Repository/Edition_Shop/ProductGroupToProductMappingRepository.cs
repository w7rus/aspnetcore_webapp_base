using System;
using DAL.Data;
using DAL.Repository.Base;
using Domain.Entities.Edition_Shop;

namespace DAL.Repository.Edition_Shop;

public interface IProductGroupToProductMappingRepository : IRepositoryBase<ProductGroupToProductMapping, Guid>
{
}

public class ProductGroupToProductMappingRepository : RepositoryBase<ProductGroupToProductMapping, Guid>,
    IProductGroupToProductMappingRepository
{
    public ProductGroupToProductMappingRepository(AppDbContext appDbContext) : base(appDbContext)
    {
    }
}