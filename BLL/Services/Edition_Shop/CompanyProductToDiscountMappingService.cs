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

public interface ICompanyProductToDiscountMappingService : IEntityServiceBase<CompanyProductToDiscountMapping>
{
}

public class CompanyProductToDiscountMappingService : ICompanyProductToDiscountMappingService
{
    #region Fields

    private readonly ILogger<CompanyProductToDiscountMappingService> _logger;
    private readonly ICompanyProductToDiscountMappingRepository _companyProductToDiscountMappingRepository;
    private readonly IAppDbContextAction _appDbContextAction;
    private readonly HttpContext _httpContext;

    #endregion

    #region Ctor

    public CompanyProductToDiscountMappingService(
        ILogger<CompanyProductToDiscountMappingService> logger,
        ICompanyProductToDiscountMappingRepository companyProductToDiscountMappingRepository,
        IAppDbContextAction appDbContextAction,
        IHttpContextAccessor httpContextAccessor
    )
    {
        _logger = logger;
        _companyProductToDiscountMappingRepository = companyProductToDiscountMappingRepository;
        _appDbContextAction = appDbContextAction;
        _httpContext = httpContextAccessor.HttpContext;
    }

    #endregion

    #region Methods

    #endregion

    public async Task Save(CompanyProductToDiscountMapping entity, CancellationToken cancellationToken = default)
    {
        _companyProductToDiscountMappingRepository.Save(entity);
        await _appDbContextAction.CommitAsync(cancellationToken);
    }

    public async Task Delete(CompanyProductToDiscountMapping entity, CancellationToken cancellationToken = default)
    {
        _companyProductToDiscountMappingRepository.Delete(entity);
        await _appDbContextAction.CommitAsync(cancellationToken);
    }

    public async Task<CompanyProductToDiscountMapping> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default
    )
    {
        return await _companyProductToDiscountMappingRepository.GetByIdAsync(id);
    }

    public async Task<CompanyProductToDiscountMapping> Create(
        CompanyProductToDiscountMapping entity,
        CancellationToken cancellationToken = default
    )
    {
        await Save(entity, cancellationToken);
        return entity;
    }
}