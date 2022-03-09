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

public interface
    IRelatedStoreProductGroupToStoreProductMappingService : IEntityServiceBase<
        RelatedStoreProductGroupToStoreProductMapping>
{
}

public class
    RelatedStoreProductGroupToStoreProductMappingService : IRelatedStoreProductGroupToStoreProductMappingService
{
    #region Fields

    private readonly ILogger<RelatedStoreProductGroupToStoreProductMappingService> _logger;

    private readonly IRelatedStoreProductGroupToStoreProductMappingRepository
        _relatedStoreProductGroupToStoreProductMappingRepository;

    private readonly IAppDbContextAction _appDbContextAction;
    private readonly HttpContext _httpContext;

    #endregion

    #region Ctor

    public RelatedStoreProductGroupToStoreProductMappingService(
        ILogger<RelatedStoreProductGroupToStoreProductMappingService> logger,
        IRelatedStoreProductGroupToStoreProductMappingRepository
            relatedStoreProductGroupToStoreProductMappingRepository,
        IAppDbContextAction appDbContextAction,
        IHttpContextAccessor httpContextAccessor
    )
    {
        _logger = logger;
        _relatedStoreProductGroupToStoreProductMappingRepository =
            relatedStoreProductGroupToStoreProductMappingRepository;
        _appDbContextAction = appDbContextAction;
        _httpContext = httpContextAccessor.HttpContext;
    }

    #endregion

    #region Methods

    #endregion

    public async Task Save(
        RelatedStoreProductGroupToStoreProductMapping entity,
        CancellationToken cancellationToken = default
    )
    {
        throw new NotImplementedException();
    }

    public async Task Delete(
        RelatedStoreProductGroupToStoreProductMapping entity,
        CancellationToken cancellationToken = default
    )
    {
        throw new NotImplementedException();
    }

    public async Task<RelatedStoreProductGroupToStoreProductMapping> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default
    )
    {
        throw new NotImplementedException();
    }

    public async Task<RelatedStoreProductGroupToStoreProductMapping> Create(
        RelatedStoreProductGroupToStoreProductMapping entity,
        CancellationToken cancellationToken = default
    )
    {
        throw new NotImplementedException();
    }
}