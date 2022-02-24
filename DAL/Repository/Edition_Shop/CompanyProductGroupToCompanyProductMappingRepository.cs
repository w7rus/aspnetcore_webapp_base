using System;
using DAL.Data;
using DAL.Repository.Base;
using Domain.Entities.Edition_Shop;

namespace DAL.Repository.Edition_Shop;

public interface
    ICompanyProductGroupToCompanyProductMappingRepository : IRepositoryBase<CompanyProductGroupToCompanyProductMapping,
        Guid>
{
}

public class CompanyProductGroupToCompanyProductMappingRepository :
    RepositoryBase<CompanyProductGroupToCompanyProductMapping, Guid>,
    ICompanyProductGroupToCompanyProductMappingRepository
{
    public CompanyProductGroupToCompanyProductMappingRepository(AppDbContext appDbContext) : base(appDbContext)
    {
    }
}