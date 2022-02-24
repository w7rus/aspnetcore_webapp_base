using System;
using DAL.Data;
using DAL.Repository.Base;
using Domain.Entities.Edition_Shop;

namespace DAL.Repository.Edition_Shop;

public interface ICompanyProductGroupRepository : IRepositoryBase<CompanyProductGroup, Guid>
{
}

public class CompanyProductGroupRepository : RepositoryBase<CompanyProductGroup, Guid>, ICompanyProductGroupRepository
{
    public CompanyProductGroupRepository(AppDbContext appDbContext) : base(appDbContext)
    {
    }
}