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
    IRelatedCompanyProductGroupToCompanyProductGroupMappingService : IEntityServiceBase<
        RelatedCompanyProductGroupToCompanyProductGroupMapping>
{
}

public class
    RelatedCompanyProductGroupToCompanyProductGroupMappingService :
        IRelatedCompanyProductGroupToCompanyProductGroupMappingService
{
    #region Fields

    private readonly ILogger<RelatedCompanyProductGroupToCompanyProductGroupMappingService> _logger;

    private readonly IRelatedCompanyProductGroupToCompanyProductGroupMappingRepository
        _relatedCompanyProductGroupToCompanyProductGroupMappingRepository;

    private readonly IAppDbContextAction _appDbContextAction;
    private readonly HttpContext _httpContext;

    #endregion

    #region Ctor

    public RelatedCompanyProductGroupToCompanyProductGroupMappingService(
        ILogger<RelatedCompanyProductGroupToCompanyProductGroupMappingService> logger,
        IRelatedCompanyProductGroupToCompanyProductGroupMappingRepository
            relatedCompanyProductGroupToCompanyProductGroupMappingRepository,
        IAppDbContextAction appDbContextAction,
        HttpContext httpContext
    )
    {
        _logger = logger;
        _relatedCompanyProductGroupToCompanyProductGroupMappingRepository =
            relatedCompanyProductGroupToCompanyProductGroupMappingRepository;
        _appDbContextAction = appDbContextAction;
        _httpContext = httpContext;
    }

    #endregion

    #region Methods

    #endregion

    public async Task Save(
        RelatedCompanyProductGroupToCompanyProductGroupMapping entity,
        CancellationToken cancellationToken = default
    )
    {
        throw new NotImplementedException();
    }

    public async Task Delete(
        RelatedCompanyProductGroupToCompanyProductGroupMapping entity,
        CancellationToken cancellationToken = default
    )
    {
        throw new NotImplementedException();
    }

    public async Task<RelatedCompanyProductGroupToCompanyProductGroupMapping> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default
    )
    {
        throw new NotImplementedException();
    }

    public async Task<RelatedCompanyProductGroupToCompanyProductGroupMapping> Create(
        RelatedCompanyProductGroupToCompanyProductGroupMapping entity,
        CancellationToken cancellationToken = default
    )
    {
        throw new NotImplementedException();
    }
}