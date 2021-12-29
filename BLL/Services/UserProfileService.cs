using System;
using System.Threading;
using System.Threading.Tasks;
using BLL.Services.Base;
using DAL.Data;
using DAL.Repository;
using Domain.Entities;
using Microsoft.Extensions.Logging;

namespace BLL.Services;

public interface IUserProfileService : IEntityServiceBase<UserProfile>
{
    new Task Save(UserProfile userProfile, CancellationToken cancellationToken);

    Task<UserProfile> Add(
        string username,
        string firstname,
        string lastname,
        string description,
        Guid userId,
        CancellationToken cancellationToken
    );

    new Task Delete(UserProfile userProfile, CancellationToken cancellationToken);
    new Task<UserProfile> GetByIdAsync(Guid id);
    Task<UserProfile> GetByUsernameAsync(string username);
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

    public async Task Save(UserProfile userProfile, CancellationToken cancellationToken)
    {
        _userProfileRepository.Save(userProfile);
        await _appDbContextAction.CommitAsync(cancellationToken);
    }

    public async Task<UserProfile> Add(
        string username,
        string firstname,
        string lastname,
        string description,
        Guid userId,
        CancellationToken cancellationToken
    )
    {
        var entity = new UserProfile
        {
            Username = username,
            FirstName = firstname,
            LastName = lastname,
            Description = description,
            UserId = userId,
        };

        await Save(entity, cancellationToken);
        return entity;
    }

    public async Task Delete(UserProfile userProfile, CancellationToken cancellationToken)
    {
        _userProfileRepository.Delete(userProfile);
        await _appDbContextAction.CommitAsync(cancellationToken);
    }

    public async Task<UserProfile> GetByIdAsync(Guid id)
    {
        return await _userProfileRepository.SingleOrDefaultAsync(_ => _.Id == id);
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