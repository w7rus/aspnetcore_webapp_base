using System;
using DAL.Data;
using DAL.Repository.Base;
using Domain.Entities.Edition_Shop;

namespace DAL.Repository.Edition_Shop;

public interface
    IRelatedStoreProductGroupToStoreProductMappingRepository : IRepositoryBase<
        RelatedStoreProductGroupToStoreProductMapping, Guid>
{
}

public class RelatedStoreProductGroupToStoreProductMappingRepository :
    RepositoryBase<RelatedStoreProductGroupToStoreProductMapping, Guid>,
    IRelatedStoreProductGroupToStoreProductMappingRepository
{
    public RelatedStoreProductGroupToStoreProductMappingRepository(AppDbContext appDbContext) : base(appDbContext)
    {
    }
}