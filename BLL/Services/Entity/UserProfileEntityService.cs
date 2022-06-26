using System;
using System.Threading;
using System.Threading.Tasks;
using BLL.Services.Base;
using Common.Models;
using DAL.Data;
using DAL.Repository;
using Domain.Entities;
using Microsoft.Extensions.Logging;

namespace BLL.Services.Entity;

/// <summary>
/// Service to work with UserProfile entity
/// </summary>
public interface IUserProfileEntityService : IEntityServiceBase<UserProfile>
{
    Task<UserProfile> GetByUsernameAsync(string username);
    Task<UserProfile> GetByUserIdAsync(Guid userId);
}

public class UserProfileEntityService : IUserProfileEntityService
{
    #region Fields

    private readonly ILogger<UserProfileEntityService> _logger;
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly IAppDbContextAction _appDbContextAction;

    #endregion

    #region Ctor

    public UserProfileEntityService(
        ILogger<UserProfileEntityService> logger,
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

    public async Task<UserProfile> Save(UserProfile entity, CancellationToken cancellationToken = default)
    {
        _logger.Log(LogLevel.Information,
            Localize.Log.Method(GetType(), nameof(Save), $"{entity?.GetType().Name} {entity?.Id}"));

        _userProfileRepository.Save(entity);
        await _appDbContextAction.CommitAsync(cancellationToken);

        return entity;
    }

    public async Task Delete(UserProfile entity, CancellationToken cancellationToken = default)
    {
        _logger.Log(LogLevel.Information,
            Localize.Log.Method(GetType(), nameof(Delete), $"{entity?.GetType().Name} {entity?.Id}"));

        _userProfileRepository.Delete(entity);
        await _appDbContextAction.CommitAsync(cancellationToken);
    }

    public async Task<UserProfile> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _userProfileRepository.SingleOrDefaultAsync(_ => _.Id == id);

        _logger.Log(LogLevel.Information,
            Localize.Log.Method(GetType(), nameof(GetByIdAsync), $"{entity?.GetType().Name} {entity?.Id}"));

        return entity;
    }

    public async Task<UserProfile> GetByUsernameAsync(string username)
    {
        var entity = await _userProfileRepository.SingleOrDefaultAsync(_ => _.Username == username);

        _logger.Log(LogLevel.Information,
            Localize.Log.Method(GetType(), nameof(GetByUsernameAsync), $"{entity?.GetType().Name} {entity?.Id}"));

        return entity;
    }

    public async Task<UserProfile> GetByUserIdAsync(Guid userId)
    {
        var entity = await _userProfileRepository.SingleOrDefaultAsync(_ => _.UserId == userId);

        _logger.Log(LogLevel.Information,
            Localize.Log.Method(GetType(), nameof(GetByUsernameAsync), $"{entity?.GetType().Name} {entity?.Id}"));

        return entity;
    }

    #endregion
}