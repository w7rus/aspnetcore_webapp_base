using System;
using DAL.Data;
using DAL.Repository.Base;
using Domain.Entities.Edition_Shop;

namespace DAL.Repository.Edition_Shop;

public interface
    ICompanyProductGroupToCategoryMappingRepository : IRepositoryBase<CompanyProductGroupToCategoryMapping, Guid>
{
}

public class CompanyProductGroupToCategoryMappingRepository :
    RepositoryBase<CompanyProductGroupToCategoryMapping, Guid>, ICompanyProductGroupToCategoryMappingRepository
{
    public CompanyProductGroupToCategoryMappingRepository(AppDbContext appDbContext) : base(appDbContext)
    {
    }
}