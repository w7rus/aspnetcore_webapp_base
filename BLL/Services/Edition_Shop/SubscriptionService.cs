using System;
using System.Threading;
using System.Threading.Tasks;
using BLL.Services.Base;
using DAL.Data;
using DAL.Repository.Edition_Shop;
using Domain.Entities.Edition_Shop;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace BLL.Services.Edition_Shop;

public interface ISubscriptionService : IEntityServiceBase<Subscription>
{
}

public class SubscriptionService : ISubscriptionService
{
    #region Fields

    private readonly ILogger<SubscriptionService> _logger;
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly IAppDbContextAction _appDbContextAction;
    private readonly HttpContext _httpContext;

    #endregion

    #region Ctor

    public SubscriptionService(
        ILogger<SubscriptionService> logger,
        ISubscriptionRepository subscriptionRepository,
        IAppDbContextAction appDbContextAction,
        IHttpContextAccessor httpContextAccessor
    )
    {
        _logger = logger;
        _subscriptionRepository = subscriptionRepository;
        _appDbContextAction = appDbContextAction;
        _httpContext = httpContextAccessor.HttpContext;
    }

    #endregion

    #region Methods

    #endregion

    public async Task<Subscription> Save(Subscription entity, CancellationToken cancellationToken = default)
    {
        _subscriptionRepository.Save(entity);
        await _appDbContextAction.CommitAsync(cancellationToken);
        
        return entity;
    }

    public async Task Delete(Subscription entity, CancellationToken cancellationToken = default)
    {
        _subscriptionRepository.Delete(entity);
        await _appDbContextAction.CommitAsync(cancellationToken);
    }

    public async Task<Subscription> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _subscriptionRepository.GetByIdAsync(id);
    }
}