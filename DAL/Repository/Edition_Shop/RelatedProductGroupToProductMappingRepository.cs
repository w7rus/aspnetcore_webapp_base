using System;
using DAL.Data;
using DAL.Repository.Base;
using Domain.Entities.Edition_Shop;

namespace DAL.Repository.Edition_Shop;

public interface
    IRelatedProductGroupToProductMappingRepository : IRepositoryBase<RelatedProductGroupToProductMapping, Guid>
{
}

public class RelatedProductGroupToProductMappingRepository : RepositoryBase<RelatedProductGroupToProductMapping, Guid>,
    IRelatedProductGroupToProductMappingRepository
{
    public RelatedProductGroupToProductMappingRepository(AppDbContext appDbContext) : base(appDbContext)
    {
    }
}