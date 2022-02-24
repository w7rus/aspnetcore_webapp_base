using System;
using DAL.Data;
using DAL.Repository.Base;
using Domain.Entities.Edition_Shop;

namespace DAL.Repository.Edition_Shop;

public interface
    IRelatedCompanyProductGroupToCompanyProductGroupMappingRepository : IRepositoryBase<
        RelatedCompanyProductGroupToCompanyProductGroupMapping, Guid>
{
}

public class RelatedCompanyProductGroupToCompanyProductGroupMappingRepository :
    RepositoryBase<RelatedCompanyProductGroupToCompanyProductGroupMapping, Guid>,
    IRelatedCompanyProductGroupToCompanyProductGroupMappingRepository
{
    public RelatedCompanyProductGroupToCompanyProductGroupMappingRepository(AppDbContext appDbContext) :
        base(appDbContext)
    {
    }
}