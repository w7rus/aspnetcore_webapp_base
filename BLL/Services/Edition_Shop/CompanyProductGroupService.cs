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

public interface ICompanyProductGroupService : IEntityServiceBase<CompanyProductGroup>
{
}

public class CompanyProductGroupService : ICompanyProductGroupService
{
    #region Fields

    private readonly ILogger<CompanyProductGroupService> _logger;
    private readonly ICompanyProductGroupRepository _companyProductGroupRepository;
    private readonly IAppDbContextAction _appDbContextAction;
    private readonly HttpContext _httpContext;

    #endregion

    #region Ctor

    public CompanyProductGroupService(
        ILogger<CompanyProductGroupService> logger,
        ICompanyProductGroupRepository companyProductGroupRepository,
        IAppDbContextAction appDbContextAction,
        IHttpContextAccessor httpContextAccessor
    )
    {
        _logger = logger;
        _companyProductGroupRepository = companyProductGroupRepository;
        _appDbContextAction = appDbContextAction;
        _httpContext = httpContextAccessor.HttpContext;
    }

    #endregion

    #region Methods

    #endregion

    public async Task Save(CompanyProductGroup entity, CancellationToken cancellationToken = default)
    {
        _companyProductGroupRepository.Save(entity);
        await _appDbContextAction.CommitAsync(cancellationToken);
    }

    public async Task Delete(CompanyProductGroup entity, CancellationToken cancellationToken = default)
    {
        _companyProductGroupRepository.Delete(entity);
        await _appDbContextAction.CommitAsync(cancellationToken);
    }

    public async Task<CompanyProductGroup> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _companyProductGroupRepository.GetByIdAsync(id);
    }

    public async Task<CompanyProductGroup> Create(
        CompanyProductGroup entity,
        CancellationToken cancellationToken = default
    )
    {
        await Save(entity, cancellationToken);
        return entity;
    }
}