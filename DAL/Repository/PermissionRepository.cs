using System;
using DAL.Data;
using DAL.Repository.Base;
using Domain.Entities;

namespace DAL.Repository;

public interface IPermissionRepository : IRepositoryBase<Permission, Guid>
{
}

public class PermissionRepository : RepositoryBase<Permission, Guid>,
    IPermissionRepository
{
    public PermissionRepository(AppDbContext appDbContext) : base(appDbContext)
    {
    }
}