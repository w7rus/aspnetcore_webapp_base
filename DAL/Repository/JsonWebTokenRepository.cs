using System;
using DAL.Data;
using DAL.Repository.Base;
using Domain.Entities;

namespace DAL.Repository
{
    public interface IJsonWebTokenRepository : IRepositoryBase<JsonWebToken, Guid>
    {
    }

    public class JsonWebTokenRepository : RepositoryBase<JsonWebToken, Guid>, IJsonWebTokenRepository
    {
        public JsonWebTokenRepository(AppDbContext appDbContext) : base(appDbContext)
        {
        }
    }
}