using System;
using DAL.Data;
using DAL.Repository.Base;
using Domain.Entities.Edition_Shop;

namespace DAL.Repository.Edition_Shop;

public interface ICompanyProductToCategoryMappingRepository : IRepositoryBase<CompanyProductToCategoryMapping, Guid>
{
}

public class CompanyProductToCategoryMappingRepository : RepositoryBase<CompanyProductToCategoryMapping, Guid>,
    ICompanyProductToCategoryMappingRepository
{
    public CompanyProductToCategoryMappingRepository(AppDbContext appDbContext) : base(appDbContext)
    {
    }
}