using System;
using DAL.Data;
using DAL.Repository.Base;
using Domain.Entities.Edition_Shop;

namespace DAL.Repository.Edition_Shop;

public interface IOperatingTimeRepository : IRepositoryBase<OperatingTime, Guid>
{
}

public class OperatingTimeRepository : RepositoryBase<OperatingTime, Guid>, IOperatingTimeRepository
{
    public OperatingTimeRepository(AppDbContext appDbContext) : base(appDbContext)
    {
    }
}