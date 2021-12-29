using System;
using DAL.Data;
using DAL.Repository.Base;
using Domain.Entities;

namespace DAL.Repository
{
    public interface IUserGroupPermissionValueRepository : IRepositoryBase<UserGroupPermissionValue, Guid>
    {
    }

    public class UserGroupPermissionValueRepository : RepositoryBase<UserGroupPermissionValue, Guid>,
        IUserGroupPermissionValueRepository
    {
        public UserGroupPermissionValueRepository(AppDbContext appDbContext) : base(appDbContext)
        {
        }
    }
}