using System;
using DAL.Data;
using DAL.Repository.Base;
using Domain.Entities.Edition_Shop;

namespace DAL.Repository.Edition_Shop;

public interface IUserToStoreMappingRepository : IRepositoryBase<UserToStoreMapping, Guid>
{
}

public class UserToStoreMappingRepository : RepositoryBase<UserToStoreMapping, Guid>,
    IUserToStoreMappingRepository
{
    public UserToStoreMappingRepository(AppDbContext appDbContext) : base(appDbContext)
    {
    }
}