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

public interface IUserToCompanyMappingService : IEntityServiceBase<UserToCompanyMapping>
{
}

public class UserToCompanyMappingService : IUserToCompanyMappingService
{
    #region Fields

    private readonly ILogger<UserToCompanyMappingService> _logger;
    private readonly IUserToCompanyMappingRepository _userToCompanyMappingRepository;
    private readonly IAppDbContextAction _appDbContextAction;
    private readonly HttpContext _httpContext;

    #endregion

    #region Ctor

    public UserToCompanyMappingService(
        ILogger<UserToCompanyMappingService> logger,
        IUserToCompanyMappingRepository userToCompanyMappingRepository,
        IAppDbContextAction appDbContextAction,
        IHttpContextAccessor httpContextAccessor
    )
    {
        _logger = logger;
        _userToCompanyMappingRepository = userToCompanyMappingRepository;
        _appDbContextAction = appDbContextAction;
        _httpContext = httpContextAccessor.HttpContext;
    }

    #endregion

    #region Methods

    #endregion

    public async Task<UserToCompanyMapping> Save(UserToCompanyMapping entity, CancellationToken cancellationToken = default)
    {
        _userToCompanyMappingRepository.Save(entity);
        await _appDbContextAction.CommitAsync(cancellationToken);
        
        return entity;
    }

    public async Task Delete(UserToCompanyMapping entity, CancellationToken cancellationToken = default)
    {
        _userToCompanyMappingRepository.Delete(entity);
        await _appDbContextAction.CommitAsync(cancellationToken);
    }

    public async Task<UserToCompanyMapping> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _userToCompanyMappingRepository.GetByIdAsync(id);
    }
}