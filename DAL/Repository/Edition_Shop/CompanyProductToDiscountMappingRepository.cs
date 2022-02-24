using System;
using DAL.Data;
using DAL.Repository.Base;
using Domain.Entities.Edition_Shop;

namespace DAL.Repository.Edition_Shop;

public interface ICompanyProductToDiscountMappingRepository : IRepositoryBase<CompanyProductToDiscountMapping, Guid>
{
}

public class CompanyProductToDiscountMappingRepository : RepositoryBase<CompanyProductToDiscountMapping, Guid>,
    ICompanyProductToDiscountMappingRepository
{
    public CompanyProductToDiscountMappingRepository(AppDbContext appDbContext) : base(appDbContext)
    {
    }
}