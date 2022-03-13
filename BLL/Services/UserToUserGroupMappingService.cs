using System;
using System.Threading;
using System.Threading.Tasks;
using BLL.Services.Base;
using Common.Models;
using DAL.Data;
using DAL.Repository;
using Domain.Entities;
using Microsoft.Extensions.Logging;

namespace BLL.Services;

/// <summary>
/// Service to work with UserToUserGroupMapping entity
/// </summary>
public interface IUserToUserGroupMappingService : IEntityToEntityMappingServiceBase<UserToUserGroupMapping>
{
}

public class UserToUserGroupMappingService : IUserToUserGroupMappingService
{
    #region Fields

    private readonly ILogger<UserToUserGroupMappingService> _logger;
    private readonly IUserToUserGroupMappingRepository _userToUserGroupMappingRepository;
    private readonly IAppDbContextAction _appDbContextAction;

    #endregion

    #region Ctor

    public UserToUserGroupMappingService(
        ILogger<UserToUserGroupMappingService> logger,
        IUserToUserGroupMappingRepository userToUserGroupMappingRepository,
        IAppDbContextAction appDbContextAction
    )
    {
        _logger = logger;
        _userToUserGroupMappingRepository = userToUserGroupMappingRepository;
        _appDbContextAction = appDbContextAction;
    }

    #endregion

    #region Methods

    public async Task Save(UserToUserGroupMapping group, CancellationToken cancellationToken = default)
    {
        _logger.Log(LogLevel.Information,
            Localize.Log.Method(GetType(), nameof(Save), $"{group.GetType().Name} {group.Id}"));

        _userToUserGroupMappingRepository.Save(group);
        await _appDbContextAction.CommitAsync(cancellationToken);
    }

    public async Task Delete(UserToUserGroupMapping group, CancellationToken cancellationToken = default)
    {
        _logger.Log(LogLevel.Information,
            Localize.Log.Method(GetType(), nameof(Delete), $"{group.GetType().Name} {group.Id}"));

        _userToUserGroupMappingRepository.Delete(group);
        await _appDbContextAction.CommitAsync(cancellationToken);
    }

    public async Task<UserToUserGroupMapping> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _userToUserGroupMappingRepository.SingleOrDefaultAsync(_ => _.Id == id);

        _logger.Log(LogLevel.Information,
            Localize.Log.Method(GetType(), nameof(Delete), $"{entity.GetType().Name} {entity.Id}"));

        return entity;
    }

    public async Task<UserToUserGroupMapping> Create(
        UserToUserGroupMapping group,
        CancellationToken cancellationToken = default
    )
    {
        await Save(group, cancellationToken);

        _logger.Log(LogLevel.Information,
            Localize.Log.Method(GetType(), nameof(Create), $"{group.GetType().Name} {group.Id}"));

        return group;
    }

    #endregion
}