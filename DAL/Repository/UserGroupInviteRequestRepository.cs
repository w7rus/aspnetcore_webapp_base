using System;
using DAL.Data;
using DAL.Repository.Base;
using Domain.Entities;

namespace DAL.Repository;

public interface IUserGroupInviteRequestRepository : IRepositoryBase<UserGroupInviteRequest, Guid>
{
}

public class UserGroupInviteRequestRepository : RepositoryBase<UserGroupInviteRequest, Guid>,
    IUserGroupInviteRequestRepository
{
    public UserGroupInviteRequestRepository(AppDbContext appDbContext) : base(appDbContext)
    {
    }
}