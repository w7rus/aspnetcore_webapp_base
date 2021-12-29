using System;
using DAL.Data;
using DAL.Repository.Base;
using Domain.Entities;

namespace DAL.Repository
{
    public interface IUserToGroupMappingRepository : IRepositoryBase<UserToGroupMapping, Guid>
    {
    }

    public class UserToGroupMappingRepository : RepositoryBase<UserToGroupMapping, Guid>,
        IUserToGroupMappingRepository
    {
        public UserToGroupMappingRepository(AppDbContext appDbContext) : base(appDbContext)
        {
        }
    }
}