using System;
using DAL.Data;
using DAL.Repository.Base;
using Domain.Entities;

namespace DAL.Repository
{
    public interface IRefreshTokenRepository : IRepositoryBase<RefreshToken, Guid>
    {
    }

    public class RefreshTokenRepository : RepositoryBase<RefreshToken, Guid>, IRefreshTokenRepository
    {
        public RefreshTokenRepository(AppDbContext appDbContext) : base(appDbContext)
        {
        }
    }
}