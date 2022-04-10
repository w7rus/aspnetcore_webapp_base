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
    ICompanyProductGroupToCompanyProductMappingService : IEntityServiceBase<CompanyProductGroupToCompanyProductMapping>
{
}

public class CompanyProductGroupToCompanyProductMappingService : ICompanyProductGroupToCompanyProductMappingService
{
    #region Fields

    private readonly ILogger<CompanyProductGroupToCompanyProductMappingService> _logger;

    private readonly ICompanyProductGroupToCompanyProductMappingRepository
        _companyProductGroupToCompanyProductMappingRepository;

    private readonly IAppDbContextAction _appDbContextAction;
    private readonly HttpContext _httpContext;

    #endregion

    #region Ctor

    public CompanyProductGroupToCompanyProductMappingService(
        ILogger<CompanyProductGroupToCompanyProductMappingService> logger,
        ICompanyProductGroupToCompanyProductMappingRepository companyProductGroupToCompanyProductMappingRepository,
        IAppDbContextAction appDbContextAction,
        IHttpContextAccessor httpContextAccessor
    )
    {
        _logger = logger;
        _companyProductGroupToCompanyProductMappingRepository = companyProductGroupToCompanyProductMappingRepository;
        _appDbContextAction = appDbContextAction;
        _httpContext = httpContextAccessor.HttpContext;
    }

    #endregion

    #region Methods

    #endregion

    public async Task<CompanyProductGroupToCompanyProductMapping> Save(
        CompanyProductGroupToCompanyProductMapping entity,
        CancellationToken cancellationToken = default
    )
    {
        _companyProductGroupToCompanyProductMappingRepository.Save(entity);
        await _appDbContextAction.CommitAsync(cancellationToken);
        
        return entity;
    }

    public async Task Delete(
        CompanyProductGroupToCompanyProductMapping entity,
        CancellationToken cancellationToken = default
    )
    {
        _companyProductGroupToCompanyProductMappingRepository.Save(entity);
        await _appDbContextAction.CommitAsync(cancellationToken);
    }

    public async Task<CompanyProductGroupToCompanyProductMapping> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default
    )
    {
        return await _companyProductGroupToCompanyProductMappingRepository.GetByIdAsync(id);
    }
}