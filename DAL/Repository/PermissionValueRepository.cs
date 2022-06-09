using System;
using DAL.Data;
using DAL.Repository.Base;
using Domain.Entities;

namespace DAL.Repository
{
    public interface IPermissionValueRepository : IRepositoryBase<PermissionValue, Guid>
    {
    }

    public class PermissionValueRepository : RepositoryBase<PermissionValue, Guid>,
        IPermissionValueRepository
    {
        public PermissionValueRepository(AppDbContext appDbContext) : base(appDbContext)
        {
        }
    }
}