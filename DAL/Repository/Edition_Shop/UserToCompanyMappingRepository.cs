using System;
using DAL.Data;
using DAL.Repository.Base;
using Domain.Entities.Edition_Shop;

namespace DAL.Repository.Edition_Shop;

public interface IUserToCompanyMappingRepository : IRepositoryBase<UserToCompanyMapping, Guid>
{
}

public class UserToCompanyMappingRepository : RepositoryBase<UserToCompanyMapping, Guid>,
    IUserToCompanyMappingRepository
{
    public UserToCompanyMappingRepository(AppDbContext appDbContext) : base(appDbContext)
    {
    }
}