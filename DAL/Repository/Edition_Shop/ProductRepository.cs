using System;
using DAL.Data;
using DAL.Repository.Base;
using Domain.Entities.Edition_Shop;

namespace DAL.Repository.Edition_Shop;

public interface IProductRepository : IRepositoryBase<Product, Guid>
{
}

public class ProductRepository : RepositoryBase<Product, Guid>, IProductRepository
{
    public ProductRepository(AppDbContext appDbContext) : base(appDbContext)
    {
    }
}