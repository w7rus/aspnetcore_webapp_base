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

public interface ICompanyProductGroupToCategoryMappingService : IEntityServiceBase<CompanyProductGroupToCategoryMapping>
{
}

public class CompanyProductGroupToCategoryMappingService : ICompanyProductGroupToCategoryMappingService
{
    #region Fields

    private readonly ILogger<CompanyProductGroupToCategoryMappingService> _logger;
    private readonly ICompanyProductGroupToCategoryMappingRepository _companyProductGroupToCategoryMappingRepository;
    private readonly IAppDbContextAction _appDbContextAction;
    private readonly HttpContext _httpContext;

    #endregion

    #region Ctor

    public CompanyProductGroupToCategoryMappingService(
        ILogger<CompanyProductGroupToCategoryMappingService> logger,
        ICompanyProductGroupToCategoryMappingRepository companyProductGroupToCategoryMappingRepository,
        IAppDbContextAction appDbContextAction,
        HttpContext httpContext
    )
    {
        _logger = logger;
        _companyProductGroupToCategoryMappingRepository = companyProductGroupToCategoryMappingRepository;
        _appDbContextAction = appDbContextAction;
        _httpContext = httpContext;
    }

    #endregion

    #region Methods

    #endregion

    public async Task Save(CompanyProductGroupToCategoryMapping entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task Delete(CompanyProductGroupToCategoryMapping entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<CompanyProductGroupToCategoryMapping> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default
    )
    {
        throw new NotImplementedException();
    }

    public async Task<CompanyProductGroupToCategoryMapping> Create(
        CompanyProductGroupToCategoryMapping entity,
        CancellationToken cancellationToken = default
    )
    {
        throw new NotImplementedException();
    }
}