using System;
using DAL.Data;
using DAL.Repository.Base;
using Domain.Entities;

namespace DAL.Repository;

public interface IUserGroupTransferRequestRepository : IRepositoryBase<UserGroupTransferRequest, Guid>
{
}

public class UserGroupTransferRequestRepository : RepositoryBase<UserGroupTransferRequest, Guid>, IUserGroupTransferRequestRepository
{
    public UserGroupTransferRequestRepository(AppDbContext appDbContext) : base(appDbContext)
    {
    }
}