using System;
using System.Threading;
using System.Threading.Tasks;
using BLL.Services.Base;
using Common.Exceptions;
using Common.Models;
using DAL.Data;
using DAL.Repository;
using Domain.Entities;
using Microsoft.Extensions.Logging;

namespace BLL.Services;

/// <summary>
/// Service to work with UserGroup entity
/// </summary>
public interface IUserGroupService : IEntityServiceBase<UserGroup>
{
    /// <summary>
    /// Gets entity with equal Alias
    /// </summary>
    /// <param name="alias"></param>
    /// <returns></returns>
    Task<UserGroup> GetByAliasAsync(string alias);
}

public class UserGroupService : IUserGroupService
{
    #region Fields

    #endregion

    #region Ctor

    private readonly ILogger<UserGroupService> _logger;
    private readonly IUserGroupRepository _userGroupRepository;
    private readonly IAppDbContextAction _appDbContextAction;

    public UserGroupService(
        ILogger<UserGroupService> logger,
        IUserGroupRepository userGroupRepository,
        IAppDbContextAction appDbContextAction
    )
    {
        _logger = logger;
        _userGroupRepository = userGroupRepository;
        _appDbContextAction = appDbContextAction;
    }

    #endregion

    #region Methods

    public async Task<UserGroup> Save(UserGroup entity, CancellationToken cancellationToken = default)
    {
        _logger.Log(LogLevel.Information,
            Localize.Log.Method(GetType(), nameof(Save), $"{entity.GetType().Name} {entity.Id}"));

        if (entity.IsSystem)
            throw new CustomException(Localize.Error.UserGroupIsSystemManagementNotAllowed);

        _userGroupRepository.Save(entity);
        await _appDbContextAction.CommitAsync(cancellationToken);
        
        return entity;
    }

    public async Task Delete(UserGroup entity, CancellationToken cancellationToken = default)
    {
        _logger.Log(LogLevel.Information,
            Localize.Log.Method(GetType(), nameof(Delete), $"{entity.GetType().Name} {entity.Id}"));

        if (entity.IsSystem)
            throw new CustomException(Localize.Error.UserGroupIsSystemManagementNotAllowed);

        _userGroupRepository.Delete(entity);
        await _appDbContextAction.CommitAsync(cancellationToken);
    }

    public async Task<UserGroup> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _userGroupRepository.SingleOrDefaultAsync(_ => _.Id == id);

        _logger.Log(LogLevel.Information,
            Localize.Log.Method(GetType(), nameof(GetByIdAsync), $"{entity.GetType().Name} {entity.Id}"));

        return entity;
    }

    public async Task<UserGroup> GetByAliasAsync(string alias)
    {
        var entity = await _userGroupRepository.SingleOrDefaultAsync(_ => _.Alias == alias);

        _logger.Log(LogLevel.Information,
            Localize.Log.Method(GetType(), nameof(GetByAliasAsync), $"{entity.GetType().Name} {entity.Id}"));

        return entity;
    }

    #endregion
}