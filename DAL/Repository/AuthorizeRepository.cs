using System;
using System.IO;
using DAL.Data;
using DAL.Repository.Base;
using Domain.Entities;
using File = Domain.Entities.File;

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