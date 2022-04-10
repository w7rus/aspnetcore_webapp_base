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

    public async Task<RelatedStoreProductGroupToStoreProductGroupMapping> Save(
        RelatedStoreProductGroupToStoreProductGroupMapping entity,
        CancellationToken cancellationToken = default
    )
    {
        _relatedStoreProductGroupToStoreProductGroupMappingRepository.Save(entity);
        await _appDbContextAction.CommitAsync(cancellationToken);
        
        return entity;
    }

    public async Task Delete(
        RelatedStoreProductGroupToStoreProductGroupMapping entity,
        CancellationToken cancellationToken = default
    )
    {
        _relatedStoreProductGroupToStoreProductGroupMappingRepository.Delete(entity);
        await _appDbContextAction.CommitAsync(cancellationToken);
    }

    public async Task<RelatedStoreProductGroupToStoreProductGroupMapping> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default
    )
    {
        return await _relatedStoreProductGroupToStoreProductGroupMappingRepository.GetByIdAsync(id);
    }
}