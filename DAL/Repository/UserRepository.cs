using System;
using DAL.Data;
using DAL.Repository.Base;
using Domain.Entities;

namespace DAL.Repository
{
    public interface IUserRepository : IRepositoryBase<User, Guid>
    {
    }

    public sealed class UserRepository : RepositoryBase<User, Guid>, IUserRepository
    {
        public UserRepository(AppDbContext appDbContext) : base(appDbContext)
        {
        }
    }
}