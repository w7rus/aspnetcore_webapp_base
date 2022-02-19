using System;
using DAL.Data;
using DAL.Repository.Base;
using Domain.Entities;

namespace DAL.Repository
{
    public interface IUserToUserGroupMappingRepository : IRepositoryBase<UserToUserEntityMapping, Guid>
    {
    }

    public class UserToUserGroupMappingRepository : RepositoryBase<UserToUserEntityMapping, Guid>,
        IUserToUserGroupMappingRepository
    {
        public UserToUserGroupMappingRepository(AppDbContext appDbContext) : base(appDbContext)
        {
        }
    }
}