using System;
using DAL.Data;
using DAL.Repository.Base;
using Domain.Entities.Edition_Shop;

namespace DAL.Repository.Edition_Shop;

public interface IProductGroupRepository : IRepositoryBase<ProductGroup, Guid>
{
}

public class ProductGroupRepository : RepositoryBase<ProductGroup, Guid>, IProductGroupRepository
{
    public ProductGroupRepository(AppDbContext appDbContext) : base(appDbContext)
    {
    }
}