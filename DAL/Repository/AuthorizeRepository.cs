using System;
using DAL.Data;
using DAL.Repository.Base;
using Domain.Entities;

namespace DAL.Repository;

public interface IAuthorizeRepository : IRepositoryBase<Authorize, Guid>
{
}

public class AuthorizeRepository : RepositoryBase<Authorize, Guid>, IAuthorizeRepository
{
    public AuthorizeRepository(AppDbContext appDbContext) : base(appDbContext)
    {
    }
}