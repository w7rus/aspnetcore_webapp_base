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
    IRelatedCompanyProductGroupToCompanyProductMappingService : IEntityServiceBase<
        RelatedCompanyProductGroupToCompanyProductMapping>
{
}

public class
    RelatedCompanyProductGroupToCompanyProductMappingService : IRelatedCompanyProductGroupToCompanyProductMappingService
{
    #region Fields

    private readonly ILogger<RelatedCompanyProductGroupToCompanyProductMappingService> _logger;

    private readonly IRelatedCompanyProductGroupToCompanyProductMappingRepository
        _relatedCompanyProductGroupToCompanyProductMappingRepository;

    private readonly IAppDbContextAction _appDbContextAction;
    private readonly HttpContext _httpContext;

    #endregion

    #region Ctor

    public RelatedCompanyProductGroupToCompanyProductMappingService(
        ILogger<RelatedCompanyProductGroupToCompanyProductMappingService> logger,
        IRelatedCompanyProductGroupToCompanyProductMappingRepository
            relatedCompanyProductGroupToCompanyProductMappingRepository,
        IAppDbContextAction appDbContextAction,
        IHttpContextAccessor httpContextAccessor
    )
    {
        _logger = logger;
        _relatedCompanyProductGroupToCompanyProductMappingRepository =
            relatedCompanyProductGroupToCompanyProductMappingRepository;
        _appDbContextAction = appDbContextAction;
        _httpContext = httpContextAccessor.HttpContext;
    }

    #endregion

    #region Methods

    #endregion

    public async Task Save(
        RelatedCompanyProductGroupToCompanyProductMapping entity,
        CancellationToken cancellationToken = default
    )
    {
        throw new NotImplementedException();
    }

    public async Task Delete(
        RelatedCompanyProductGroupToCompanyProductMapping entity,
        CancellationToken cancellationToken = default
    )
    {
        throw new NotImplementedException();
    }

    public async Task<RelatedCompanyProductGroupToCompanyProductMapping> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default
    )
    {
        throw new NotImplementedException();
    }

    public async Task<RelatedCompanyProductGroupToCompanyProductMapping> Create(
        RelatedCompanyProductGroupToCompanyProductMapping entity,
        CancellationToken cancellationToken = default
    )
    {
        throw new NotImplementedException();
    }
}