using System;
using System.Threading;
using System.Threading.Tasks;
using BLL.Services.Base;
using DAL.Data;
using DAL.Repository;
using Domain.Entities;
using Microsoft.Extensions.Logging;

namespace BLL.Services;

/// <summary>
/// Service to work with UserProfile entity
/// </summary>
public interface IUserProfileService : IEntityServiceBase<UserProfile>
{
    /// <summary>
    /// Gets entity with Username that equals given one
    /// </summary>
    /// <param name="username"></param>
    /// <returns></returns>
    Task<UserProfile> GetByUsernameAsync(string username);
    
    /// <summary>
    /// Gets entity with UserId that equals give one
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<UserProfile> GetByUserIdAsync(Guid userId);
}

public class UserProfileService : IUserProfileService
{
    #region Fields

    private readonly ILogger<UserProfileService> _logger;
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly IAppDbContextAction _appDbContextAction;

    #endregion

    #region Ctor

    public UserProfileService(
        ILogger<UserProfileService> logger,
        IUserProfileRepository userProfileRepository,
        IAppDbContextAction appDbContextAction
    )
    {
        _logger = logger;
        _userProfileRepository = userProfileRepository;
        _appDbContextAction = appDbContextAction;
    }

    #endregion

    #region Methods

    public async Task Save(UserProfile entity, CancellationToken cancellationToken = default)
    {
        _userProfileRepository.Save(entity);
        await _appDbContextAction.CommitAsync(cancellationToken);
    }

    public async Task Delete(UserProfile entity, CancellationToken cancellationToken = default)
    {
        _userProfileRepository.Delete(entity);
        await _appDbContextAction.CommitAsync(cancellationToken);
    }

    public async Task<UserProfile> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _userProfileRepository.SingleOrDefaultAsync(_ => _.Id == id);
    }

    public async Task<UserProfile> Create(UserProfile entity, CancellationToken cancellationToken = default)
    {
        await Save(entity, cancellationToken);
        return entity;
    }

    public async Task<UserProfile> GetByUsernameAsync(string username)
    {
        return await _userProfileRepository.SingleOrDefaultAsync(_ => _.Username == username);
    }

    public async Task<UserProfile> GetByUserIdAsync(Guid userId)
    {
        return await _userProfileRepository.SingleOrDefaultAsync(_ => _.UserId == userId);
    }

    #endregion
}