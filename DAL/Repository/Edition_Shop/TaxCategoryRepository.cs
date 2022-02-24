using System;
using DAL.Data;
using DAL.Repository.Base;
using Domain.Entities.Edition_Shop;

namespace DAL.Repository.Edition_Shop;

public interface ITaxCategoryRepository : IRepositoryBase<TaxCategory, Guid>
{
}

public class TaxCategoryRepository : RepositoryBase<TaxCategory, Guid>, ITaxCategoryRepository
{
    public TaxCategoryRepository(AppDbContext appDbContext) : base(appDbContext)
    {
    }
}