using System;
using DAL.Data;
using DAL.Repository.Base;
using Domain.Entities.Edition_Shop;

namespace DAL.Repository.Edition_Shop;

public interface
    IRelatedCompanyProductGroupToCompanyProductMappingRepository : IRepositoryBase<
        RelatedCompanyProductGroupToCompanyProductMapping, Guid>
{
}

public class RelatedCompanyProductGroupToCompanyProductMappingRepository :
    RepositoryBase<RelatedCompanyProductGroupToCompanyProductMapping, Guid>,
    IRelatedCompanyProductGroupToCompanyProductMappingRepository
{
    public RelatedCompanyProductGroupToCompanyProductMappingRepository(AppDbContext appDbContext) : base(appDbContext)
    {
    }
}