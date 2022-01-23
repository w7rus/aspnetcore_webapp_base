using System;
using System.Threading;
using System.Threading.Tasks;
using BLL.Services.Base;
using DAL.Data;
using DAL.Repository;
using Domain.Entities;
using Microsoft.Extensions.Logging;

namespace BLL.Services;

public interface IUserGroupService : IEntityServiceBase<UserGroup>
{
    new Task Save(UserGroup userGroup, CancellationToken cancellationToken);
    Task<UserGroup> Add(string alias, string description, CancellationToken cancellationToken);
    new Task Delete(UserGroup userGroup, CancellationToken cancellationToken);
    new Task<UserGroup> GetByIdAsync(Guid id);
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

    public async Task Save(UserGroup userGroup, CancellationToken cancellationToken)
    {
        _userGroupRepository.Save(userGroup);
        await _appDbContextAction.CommitAsync(cancellationToken);
    }

    public async Task Delete(UserGroup userGroup, CancellationToken cancellationToken)
    {
        _userGroupRepository.Delete(userGroup);
        await _appDbContextAction.CommitAsync(cancellationToken);
    }

    public async Task<UserGroup> GetByIdAsync(Guid id)
    {
        return await _userGroupRepository.SingleOrDefaultAsync(_ => _.Id == id);
    }

    public async Task<UserGroup> GetByAliasAsync(string alias)
    {
        return await _userGroupRepository.SingleOrDefaultAsync(_ => _.Alias == alias);
    }

    public async Task<UserGroup> Add(string alias, string description, CancellationToken cancellationToken)
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