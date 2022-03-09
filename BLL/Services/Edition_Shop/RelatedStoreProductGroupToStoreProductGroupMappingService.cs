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
    IRelatedStoreProductGroupToStoreProductGroupMappingService : IEntityServiceBase<
        RelatedStoreProductGroupToStoreProductGroupMapping>
{
}

public class
    RelatedStoreProductGroupToStoreProductGroupMappingService :
        IRelatedStoreProductGroupToStoreProductGroupMappingService
{
    #region Fields

    private readonly ILogger<RelatedStoreProductGroupToStoreProductGroupMappingService> _logger;

    private readonly IRelatedStoreProductGroupToStoreProductGroupMappingRepository
        _relatedStoreProductGroupToStoreProductGroupMappingRepository;

    private readonly IAppDbContextAction _appDbContextAction;
    private readonly HttpContext _httpContext;

    #endregion

    #region Ctor

    public RelatedStoreProductGroupToStoreProductGroupMappingService(
        ILogger<RelatedStoreProductGroupToStoreProductGroupMappingService> logger,
        IRelatedStoreProductGroupToStoreProductGroupMappingRepository
            relatedStoreProductGroupToStoreProductGroupMappingRepository,
        IAppDbContextAction appDbContextAction,
        IHttpContextAccessor httpContextAccessor
    )
    {
        _logger = logger;
        _relatedStoreProductGroupToStoreProductGroupMappingRepository =
            relatedStoreProductGroupToStoreProductGroupMappingRepository;
        _appDbContextAction = appDbContextAction;
        _httpContext = httpContextAccessor.HttpContext;
    }

    #endregion

    #region Methods

    #endregion

    public async Task Save(
        RelatedStoreProductGroupToStoreProductGroupMapping entity,
        CancellationToken cancellationToken = default
    )
    {
        throw new NotImplementedException();
    }

    public async Task Delete(
        RelatedStoreProductGroupToStoreProductGroupMapping entity,
        CancellationToken cancellationToken = default
    )
    {
        throw new NotImplementedException();
    }

    public async Task<RelatedStoreProductGroupToStoreProductGroupMapping> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default
    )
    {
        throw new NotImplementedException();
    }

    public async Task<RelatedStoreProductGroupToStoreProductGroupMapping> Create(
        RelatedStoreProductGroupToStoreProductGroupMapping entity,
        CancellationToken cancellationToken = default
    )
    {
        throw new NotImplementedException();
    }
}