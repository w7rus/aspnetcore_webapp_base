using System;
using DAL.Data;
using DAL.Repository.Base;
using Domain.Entities;

namespace DAL.Repository;

public interface IUserGroupRepository : IRepositoryBase<UserGroup, Guid>
{
}

public class UserGroupRepository : RepositoryBase<UserGroup, Guid>, IUserGroupRepository
{
    public UserGroupRepository(AppDbContext appDbContext) : base(appDbContext)
    {
    }
}