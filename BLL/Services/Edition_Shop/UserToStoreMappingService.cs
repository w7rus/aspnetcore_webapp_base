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

public interface IUserToStoreMappingService : IEntityServiceBase<UserToStoreMapping>
{
}

public class UserToStoreMappingService : IUserToStoreMappingService
{
    #region Fields

    private readonly ILogger<UserToStoreMappingService> _logger;
    private readonly IUserToStoreMappingRepository _userToStoreMappingRepository;
    private readonly IAppDbContextAction _appDbContextAction;
    private readonly HttpContext _httpContext;

    #endregion

    #region Ctor

    public UserToStoreMappingService(
        ILogger<UserToStoreMappingService> logger,
        IUserToStoreMappingRepository userToStoreMappingRepository,
        IAppDbContextAction appDbContextAction,
        IHttpContextAccessor httpContextAccessor
    )
    {
        _logger = logger;
        _userToStoreMappingRepository = userToStoreMappingRepository;
        _appDbContextAction = appDbContextAction;
        _httpContext = httpContextAccessor.HttpContext;
    }

    #endregion

    #region Methods

    #endregion

    public async Task<UserToStoreMapping> Save(UserToStoreMapping entity, CancellationToken cancellationToken = default)
    {
        _userToStoreMappingRepository.Save(entity);
        await _appDbContextAction.CommitAsync(cancellationToken);
        
        return entity;
    }

    public async Task Delete(UserToStoreMapping entity, CancellationToken cancellationToken = default)
    {
        _userToStoreMappingRepository.Delete(entity);
        await _appDbContextAction.CommitAsync(cancellationToken);
    }

    public async Task<UserToStoreMapping> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _userToStoreMappingRepository.GetByIdAsync(id);
    }
}