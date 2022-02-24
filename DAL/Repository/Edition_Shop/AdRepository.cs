using System;
using DAL.Data;
using DAL.Repository.Base;
using Domain.Entities.Edition_Shop;

namespace DAL.Repository.Edition_Shop;

public interface IAdRepository : IRepositoryBase<Ad, Guid>
{
}

public class AdRepository : RepositoryBase<Ad, Guid>, IAdRepository
{
    public AdRepository(AppDbContext appDbContext) : base(appDbContext)
    {
    }
}