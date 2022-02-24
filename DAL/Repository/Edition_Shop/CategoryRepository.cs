using System;
using DAL.Data;
using DAL.Repository.Base;
using Domain.Entities.Edition_Shop;

namespace DAL.Repository.Edition_Shop;

public interface ICategoryRepository : IRepositoryBase<Category, Guid>
{
}

public class CategoryRepository : RepositoryBase<Category, Guid>, ICategoryRepository
{
    public CategoryRepository(AppDbContext appDbContext) : base(appDbContext)
    {
    }
}