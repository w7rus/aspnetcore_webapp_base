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
    IRelatedProductGroupToProductGroupMappingService : IEntityServiceBase<RelatedProductGroupToProductGroupMapping>
{
}

public class RelatedProductGroupToProductGroupMappingService : IRelatedProductGroupToProductGroupMappingService
{
    #region Fields

    private readonly ILogger<RelatedProductGroupToProductGroupMappingService> _logger;

    private readonly IRelatedProductGroupToProductGroupMappingRepository
        _relatedProductGroupToProductGroupMappingRepository;

    private readonly IAppDbContextAction _appDbContextAction;
    private readonly HttpContext _httpContext;

    #endregion

    #region Ctor

    public RelatedProductGroupToProductGroupMappingService(
        ILogger<RelatedProductGroupToProductGroupMappingService> logger,
        IRelatedProductGroupToProductGroupMappingRepository relatedProductGroupToProductGroupMappingRepository,
        IAppDbContextAction appDbContextAction,
        IHttpContextAccessor httpContextAccessor
    )
    {
        _logger = logger;
        _relatedProductGroupToProductGroupMappingRepository = relatedProductGroupToProductGroupMappingRepository;
        _appDbContextAction = appDbContextAction;
        _httpContext = httpContextAccessor.HttpContext;
    }

    #endregion

    #region Methods

    #endregion

    public async Task Save(
        RelatedProductGroupToProductGroupMapping entity,
        CancellationToken cancellationToken = default
    )
    {
        throw new NotImplementedException();
    }

    public async Task Delete(
        RelatedProductGroupToProductGroupMapping entity,
        CancellationToken cancellationToken = default
    )
    {
        throw new NotImplementedException();
    }

    public async Task<RelatedProductGroupToProductGroupMapping> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default
    )
    {
        throw new NotImplementedException();
    }

    public async Task<RelatedProductGroupToProductGroupMapping> Create(
        RelatedProductGroupToProductGroupMapping entity,
        CancellationToken cancellationToken = default
    )
    {
        throw new NotImplementedException();
    }
}