using System;
using DAL.Data;
using DAL.Repository.Base;
using Domain.Entities;

namespace DAL.Repository;

public interface IUserToUserGroupMappingRepository : IRepositoryBase<UserToUserGroupMapping, Guid>
{
}

public class UserToUserGroupMappingRepository : RepositoryBase<UserToUserGroupMapping, Guid>,
    IUserToUserGroupMappingRepository
{
    public UserToUserGroupMappingRepository(AppDbContext appDbContext) : base(appDbContext)
    {
    }
}