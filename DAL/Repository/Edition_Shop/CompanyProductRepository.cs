using System;
using DAL.Data;
using DAL.Repository.Base;
using Domain.Entities.Edition_Shop;

namespace DAL.Repository.Edition_Shop;

public interface ICompanyProductRepository : IRepositoryBase<CompanyProduct, Guid>
{
}

public class CompanyProductRepository : RepositoryBase<CompanyProduct, Guid>, ICompanyProductRepository
{
    public CompanyProductRepository(AppDbContext appDbContext) : base(appDbContext)
    {
    }
}