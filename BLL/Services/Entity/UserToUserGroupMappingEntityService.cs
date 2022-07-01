using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BLL.Services.Base;
using Common.Models;
using DAL.Data;
using DAL.Extensions;
using DAL.Repository;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BLL.Services.Entity;

/// <summary>
///     Service to work with UserToUserGroupMapping entity
/// </summary>
public interface IUserToUserGroupMappingEntityService : IEntityToEntityMappingServiceBase<UserToUserGroupMapping>
{
    Task<UserToUserGroupMapping> GetByUserIdUserGroupIdAsync(Guid userId, Guid userGroupId);
    Task<IReadOnlyCollection<UserToUserGroupMapping>> GetByUserGroupIdAsync(
        Guid userGroupId,
        PageModel pageModel,
        CancellationToken cancellationToken = default
    );
}

public class UserToUserGroupMappingEntityService : IUserToUserGroupMappingEntityService
{
    #region Ctor

    public UserToUserGroupMappingEntityService(
        ILogger<UserToUserGroupMappingEntityService> logger,
        IUserToUserGroupMappingRepository userToUserGroupMappingRepository,
        IAppDbContextAction appDbContextAction
    )
    {
        _logger = logger;
        _userToUserGroupMappingRepository = userToUserGroupMappingRepository;
        _appDbContextAction = appDbContextAction;
    }

    #endregion

    #region Fields

    private readonly ILogger<UserToUserGroupMappingEntityService> _logger;
    private readonly IUserToUserGroupMappingRepository _userToUserGroupMappingRepository;
    private readonly IAppDbContextAction _appDbContextAction;

    #endregion

    #region Methods

    public async Task<UserToUserGroupMapping> Save(
        UserToUserGroupMapping entity,
        CancellationToken cancellationToken = default
    )
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

        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        _userToUserGroupMappingRepository.Delete(entity);
        await _appDbContextAction.CommitAsync(cancellationToken);
    }

    public async Task<UserToUserGroupMapping> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _userToUserGroupMappingRepository.SingleOrDefaultAsync(_ => _.Id == id);

        _logger.Log(LogLevel.Information,
            Localize.Log.Method(GetType(), nameof(GetByIdAsync), $"{entity?.GetType().Name} {entity?.Id}"));

        return entity;
    }

    public async Task<UserToUserGroupMapping> GetByUserIdUserGroupIdAsync(Guid userId, Guid userGroupId)
    {
        var entity = await _userToUserGroupMappingRepository.SingleOrDefaultAsync(_ => _.EntityLeftId == userId && _.EntityRightId == userGroupId);

        _logger.Log(LogLevel.Information,
            Localize.Log.Method(GetType(), nameof(GetByUserIdUserGroupIdAsync), $"{entity?.GetType().Name} {entity?.Id}"));

        return entity;
    }

    public async Task<IReadOnlyCollection<UserToUserGroupMapping>> GetByUserGroupIdAsync(
        Guid userGroupId,
        PageModel pageModel,
        CancellationToken cancellationToken = default
    )
    {
        var result = await _userToUserGroupMappingRepository
            .QueryMany(_ => _.EntityRightId == userGroupId)
            .OrderBy(_ => _.CreatedAt)
            .GetPage(pageModel)
            .ToArrayAsync(cancellationToken);

        _logger.Log(LogLevel.Information,
            Localize.Log.Method(GetType(), nameof(GetByUserGroupIdAsync),
                $"{result.GetType().Name} {result.Length}"));

        return result;
    }

    #endregion
}