using System;
using DAL.Data;
using DAL.Repository.Base;
using Domain.Entities.Edition_Shop;

namespace DAL.Repository.Edition_Shop;

public interface ICompanyRepository : IRepositoryBase<Company, Guid>
{
}

public class CompanyRepository : RepositoryBase<Company, Guid>, ICompanyRepository
{
    public CompanyRepository(AppDbContext appDbContext) : base(appDbContext)
    {
    }
}