using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Entities.Base;
using Microsoft.Extensions.Logging;

namespace BLL.Services.Advanced;

public interface IUserToGroupService
{
    ICollection<UserGroup> GetUserGroupsByUser(User user);
    ICollection<User> GetUsersByUserGroup(UserGroup userGroup);

    Task<bool> AuthorizePermission<TEntityCompared>(
        User user,
        Permission permission,
        Permission permissionCompared,
        EntityPermissionValueBase<TEntityCompared> entityPermissionValueCompared,
        CancellationToken cancellationToken
    ) where TEntityCompared : EntityBase<Guid>;
}

public class UserToGroupService : IUserToGroupService
{
    #region Fields

    private readonly ILogger<UserToGroupService> _logger;
    private readonly IPermissionToPermissionValueService _permissionToPermissionValueService;
    private readonly IUserGroupPermissionValueService _userGroupPermissionValueService;

    #endregion

    #region Ctor

    public UserToGroupService(
        ILogger<UserToGroupService> logger,
        IPermissionToPermissionValueService permissionToPermissionValueService,
        IUserGroupPermissionValueService userGroupPermissionValueService
    )
    {
        _logger = logger;
        _permissionToPermissionValueService = permissionToPermissionValueService;
        _userGroupPermissionValueService = userGroupPermissionValueService;
    }

    #endregion

    #region Methods

    public ICollection<UserGroup> GetUserGroupsByUser(User user)
    {
        return user.UserToGroupMappings.Select(_ => _.Group).ToArray();
    }

    public ICollection<User> GetUsersByUserGroup(UserGroup userGroup)
    {
        return userGroup.GroupToEntityMappings.Select(_ => _.Entity).ToArray();
    }

    public async Task<bool> AuthorizePermission<TEntityCompared>(
        User user,
        Permission permission,
        Permission permissionCompared,
        EntityPermissionValueBase<TEntityCompared> entityPermissionValueCompared,
        CancellationToken cancellationToken
    ) where TEntityCompared : EntityBase<Guid>
    {
        var userGroups = GetUserGroupsByUser(user);
        var result = false;
        foreach (var userGroup in userGroups)
        {
            if (await _userGroupPermissionValueService.GetByEntityIdPermissionId(userGroup.Id, permission.Id,
                    cancellationToken) is var permissionValue && permissionValue == null)
                continue;
            result |= _permissionToPermissionValueService.Authorize(permission, permissionValue, permissionCompared,
                entityPermissionValueCompared);
        }

        return result;
    }

    #endregion
}