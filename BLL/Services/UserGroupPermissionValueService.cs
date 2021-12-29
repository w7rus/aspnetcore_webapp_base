using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BLL.Services.Base;
using DAL.Data;
using DAL.Repository;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BLL.Services;

public interface
    IUserGroupPermissionValueService : IEntityPermissionValueServiceBase<UserGroupPermissionValue, UserGroup>
{
}

public class UserGroupPermissionValueService : IUserGroupPermissionValueService
{
    #region Fields

    private readonly ILogger<UserGroupPermissionValueService> _logger;
    private readonly IUserGroupPermissionValueRepository _userGroupPermissionValueRepository;
    private readonly IAppDbContextAction _appDbContextAction;

    #endregion

    #region Ctor

    public UserGroupPermissionValueService(
        ILogger<UserGroupPermissionValueService> logger,
        IUserGroupPermissionValueRepository userGroupPermissionValueRepository,
        IAppDbContextAction appDbContextAction
    )
    {
        _logger = logger;
        _userGroupPermissionValueRepository = userGroupPermissionValueRepository;
        _appDbContextAction = appDbContextAction;
    }

    #endregion

    #region Methods

    public async Task Save(UserGroupPermissionValue entityPermissionValue, CancellationToken cancellationToken)
    {
        _userGroupPermissionValueRepository.Save(entityPermissionValue);
        await _appDbContextAction.CommitAsync(cancellationToken);
    }

    public async Task<UserGroupPermissionValue> Add(
        byte[] value,
        ulong grant,
        Guid permissionId,
        Guid entityId,
        CancellationToken cancellationToken
    )
    {
        var entity = new UserGroupPermissionValue
        {
            Value = value,
            Grant = grant,
            PermissionId = permissionId,
            EntityId = entityId,
        };

        await Save(entity, cancellationToken);
        return entity;
    }

    public async Task Delete(UserGroupPermissionValue entityPermissionValue, CancellationToken cancellationToken)
    {
        _userGroupPermissionValueRepository.Delete(entityPermissionValue);
        await _appDbContextAction.CommitAsync(cancellationToken);
    }

    public async Task<UserGroupPermissionValue> GetByIdAsync(Guid id)
    {
        return await _userGroupPermissionValueRepository.SingleOrDefaultAsync(_ => _.Id == id);
    }

    public async Task<IReadOnlyCollection<UserGroupPermissionValue>> GetByPermissionId(
        Guid permissionId,
        CancellationToken cancellationToken
    )
    {
        return await _userGroupPermissionValueRepository.QueryMany(_ => _.PermissionId == permissionId)
            .ToArrayAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<UserGroupPermissionValue>> GetByEntityId(
        Guid entityId,
        CancellationToken cancellationToken
    )
    {
        return await _userGroupPermissionValueRepository.QueryMany(_ => _.EntityId == entityId)
            .ToArrayAsync(cancellationToken);
    }

    #endregion
}