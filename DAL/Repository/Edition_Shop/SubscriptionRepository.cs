using System;
using DAL.Data;
using DAL.Repository.Base;
using Domain.Entities.Edition_Shop;

namespace DAL.Repository.Edition_Shop;

public interface ISubscriptionRepository : IRepositoryBase<Subscription, Guid>
{
}

public class SubscriptionRepository : RepositoryBase<Subscription, Guid>, ISubscriptionRepository
{
    public SubscriptionRepository(AppDbContext appDbContext) : base(appDbContext)
    {
    }
}