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
    private readonly IDiscountToUserMappingRepository _discountToUserMappingRepository;
    private readonly IAppDbContextAction _appDbContextAction;
    private readonly HttpContext _httpContext;

    #endregion

    #region Ctor

    public DiscountToUserMappingService(
        ILogger<DiscountToUserMappingService> logger,
        IDiscountToUserMappingRepository discountToUserMappingRepository,
        IAppDbContextAction appDbContextAction,
        IHttpContextAccessor httpContextAccessor
    )
    {
        _logger = logger;
        _discountToUserMappingRepository = discountToUserMappingRepository;
        _appDbContextAction = appDbContextAction;
        _httpContext = httpContextAccessor.HttpContext;
    }

    #endregion

    #region Methods

    #endregion

    public async Task<DiscountToUserMapping> Save(DiscountToUserMapping entity, CancellationToken cancellationToken = default)
    {
        _discountToUserMappingRepository.Save(entity);
        await _appDbContextAction.CommitAsync(cancellationToken);
        
        return entity;
    }

    public async Task Delete(DiscountToUserMapping entity, CancellationToken cancellationToken = default)
    {
        _discountToUserMappingRepository.Delete(entity);
        await _appDbContextAction.CommitAsync(cancellationToken);
    }

    public async Task<DiscountToUserMapping> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _discountToUserMappingRepository.GetByIdAsync(id);
    }
}