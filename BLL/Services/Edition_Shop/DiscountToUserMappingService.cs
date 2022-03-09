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

public interface IDiscountToUserMappingService : IEntityServiceBase<DiscountToUserMapping>
{
}

public class DiscountToUserMappingService : IDiscountToUserMappingService
{
    #region Fields

    private readonly ILogger<DiscountToUserMappingService> _logger;
    private readonly IDiscountRepository _discountRepository;
    private readonly IAppDbContextAction _appDbContextAction;
    private readonly HttpContext _httpContext;

    #endregion

    #region Ctor

    public DiscountToUserMappingService(
        ILogger<DiscountToUserMappingService> logger,
        IDiscountRepository discountRepository,
        IAppDbContextAction appDbContextAction,
        IHttpContextAccessor httpContextAccessor
    )
    {
        _logger = logger;
        _discountRepository = discountRepository;
        _appDbContextAction = appDbContextAction;
        _httpContext = httpContextAccessor.HttpContext;
    }

    #endregion

    #region Methods

    #endregion

    public async Task Save(DiscountToUserMapping entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task Delete(DiscountToUserMapping entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<DiscountToUserMapping> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<DiscountToUserMapping> Create(DiscountToUserMapping entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}