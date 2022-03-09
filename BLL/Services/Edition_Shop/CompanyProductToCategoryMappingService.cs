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

public interface ICompanyProductToCategoryMappingService : IEntityServiceBase<CompanyProductToCategoryMapping>
{
}

public class CompanyProductToCategoryMappingService : ICompanyProductToCategoryMappingService
{
    #region Fields

    private readonly ILogger<CompanyProductToCategoryMappingService> _logger;
    private readonly ICompanyProductToCategoryMappingRepository _companyProductToCategoryMappingRepository;
    private readonly IAppDbContextAction _appDbContextAction;
    private readonly HttpContext _httpContext;

    #endregion

    #region Ctor

    public CompanyProductToCategoryMappingService(
        ILogger<CompanyProductToCategoryMappingService> logger,
        ICompanyProductToCategoryMappingRepository companyProductToCategoryMappingRepository,
        IAppDbContextAction appDbContextAction,
        IHttpContextAccessor httpContextAccessor
    )
    {
        _logger = logger;
        _companyProductToCategoryMappingRepository = companyProductToCategoryMappingRepository;
        _appDbContextAction = appDbContextAction;
        _httpContext = httpContextAccessor.HttpContext;
    }

    #endregion

    #region Methods

    #endregion

    public async Task Save(CompanyProductToCategoryMapping entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task Delete(CompanyProductToCategoryMapping entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<CompanyProductToCategoryMapping> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default
    )
    {
        throw new NotImplementedException();
    }

    public async Task<CompanyProductToCategoryMapping> Create(
        CompanyProductToCategoryMapping entity,
        CancellationToken cancellationToken = default
    )
    {
        throw new NotImplementedException();
    }
}