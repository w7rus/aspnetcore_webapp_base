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
/// Service to work with UserGroup entity
/// </summary>
public interface IUserGroupService : IEntityServiceBase<UserGroup>
{
    Task<UserGroup> Add(string alias, string description, CancellationToken cancellationToken = new());
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

    public async Task Save(UserGroup entity, CancellationToken cancellationToken = new())
    {
        _userGroupRepository.Save(entity);
        await _appDbContextAction.CommitAsync(cancellationToken);
    }

    public async Task Delete(UserGroup entity, CancellationToken cancellationToken = new())
    {
        _userGroupRepository.Delete(entity);
        await _appDbContextAction.CommitAsync(cancellationToken);
    }

    public async Task<UserGroup> GetByIdAsync(Guid id, CancellationToken cancellationToken = new())
    {
        return await _userGroupRepository.SingleOrDefaultAsync(_ => _.Id == id);
    }

    public async Task<UserGroup> GetByAliasAsync(string alias)
    {
        return await _userGroupRepository.SingleOrDefaultAsync(_ => _.Alias == alias);
    }

    public async Task<UserGroup> Add(string alias, string description, CancellationToken cancellationToken = new())
    {
        var entity = new UserGroup
        {
            Alias = alias,
            Description = description
        };

        await Save(entity, cancellationToken);
        return entity;
    }

    #endregion
}