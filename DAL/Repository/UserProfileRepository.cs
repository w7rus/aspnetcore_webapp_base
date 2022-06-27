using System;
using DAL.Data;
using DAL.Repository.Base;
using Domain.Entities;

namespace DAL.Repository;

public interface IUserProfileRepository : IRepositoryBase<UserProfile, Guid>
{
}

public sealed class UserProfileRepository : RepositoryBase<UserProfile, Guid>, IUserProfileRepository
{
    public UserProfileRepository(AppDbContext appDbContext) : base(appDbContext)
    {
    }
}