using System;
using DAL.Data;
using DAL.Repository.Base;
using Domain.Entities.Edition_Shop;

namespace DAL.Repository.Edition_Shop;

public interface
    IRelatedProductGroupToProductGroupMappingRepository : IRepositoryBase<RelatedProductGroupToProductGroupMapping,
        Guid>
{
}

public class RelatedProductGroupToProductGroupMappingRepository :
    RepositoryBase<RelatedProductGroupToProductGroupMapping, Guid>, IRelatedProductGroupToProductGroupMappingRepository
{
    public RelatedProductGroupToProductGroupMappingRepository(AppDbContext appDbContext) : base(appDbContext)
    {
    }
}