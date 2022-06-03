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

    public async Task<UserToUserGroupMapping> Save(UserToUserGroupMapping entity, CancellationToken cancellationToken = default)
    {
        _logger.Log(LogLevel.Information,
            Localize.Log.Method(GetType(), nameof(Save), $"{entity?.GetType().Name} {entity?.Id}"));

        _userToUserGroupMappingRepository.Save(entity);
        await _appDbContextAction.CommitAsync(cancellationToken);
        
        return entity;
    }

    public async Task Delete(UserToUserGroupMapping entity, CancellationToken cancellationToken = default)
    {
        _logger.Log(LogLevel.Information,
            Localize.Log.Method(GetType(), nameof(Delete), $"{entity?.GetType().Name} {entity?.Id}"));

        _userToUserGroupMappingRepository.Delete(entity);
        await _appDbContextAction.CommitAsync(cancellationToken);
    }

    public async Task<UserToUserGroupMapping> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _userToUserGroupMappingRepository.SingleOrDefaultAsync(_ => _.Id == id);

        _logger.Log(LogLevel.Information,
            Localize.Log.Method(GetType(), nameof(Delete), $"{entity?.GetType().Name} {entity?.Id}"));

        return entity;
    }

    #endregion
}