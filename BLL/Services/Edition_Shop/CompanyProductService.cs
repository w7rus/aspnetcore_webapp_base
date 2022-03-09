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

public interface ICompanyProductService : IEntityServiceBase<CompanyProduct>
{
}

public class CompanyProductService : ICompanyProductService
{
    #region Fields

    private readonly ILogger<CompanyProductService> _logger;
    private readonly ICompanyProductRepository _companyProductRepository;
    private readonly IAppDbContextAction _appDbContextAction;
    private readonly HttpContext _httpContext;

    #endregion

    #region Ctor

    public CompanyProductService(
        ILogger<CompanyProductService> logger,
        ICompanyProductRepository companyProductRepository,
        IAppDbContextAction appDbContextAction,
        IHttpContextAccessor httpContextAccessor
    )
    {
        _logger = logger;
        _companyProductRepository = companyProductRepository;
        _appDbContextAction = appDbContextAction;
        _httpContext = httpContextAccessor.HttpContext;
    }

    #endregion

    #region Methods

    #endregion

    public async Task Save(CompanyProduct entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task Delete(CompanyProduct entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<CompanyProduct> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<CompanyProduct> Create(CompanyProduct entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}