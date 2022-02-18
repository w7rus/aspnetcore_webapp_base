using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Entities.Base;
using Microsoft.Extensions.Logging;

namespace BLL.Services.Advanced;

/// <summary>
/// Advanced Service for User and UserGroup management
/// </summary>
public interface IUserToUserGroupService
{
    /// <summary>
    /// Authorizes PermissionValue.Value to another PermissionValue.Value from all groups given user is member of
    /// </summary>
    /// <param name="user"></param>
    /// <param name="permission"></param>
    /// <param name="entityPermissionValueCompared"></param>
    /// <param name="cancellationToken"></param>
    /// <typeparam name="TEntityCompared"></typeparam>
    /// <returns></returns>
    Task<bool> AuthorizePermission<TEntityCompared>(
        User user,
        Permission permission,
        EntityPermissionValueBase<TEntityCompared> entityPermissionValueCompared,
        CancellationToken cancellationToken = default
    ) where TEntityCompared : EntityBase<Guid>;
    
    /// <summary>
    /// Authorizes PermissionValue.Value to another PermissionValue.Value from all groups given user is member of
    /// </summary>
    /// <param name="user"></param>
    /// <param name="permission"></param>
    /// <param name="_valueCompared"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> AuthorizePermission(
        User user,
        Permission permission,
        byte[] _valueCompared,
        CancellationToken cancellationToken = default
    );
}

public class UserToUserGroupService : IUserToUserGroupService
{
    #region Fields

    private readonly ILogger<UserToUserGroupService> _logger;
    private readonly IAuthorizePermissionValueService _authorizePermissionValueService;
    private readonly IUserGroupPermissionValueService _userGroupPermissionValueService;
    private readonly IUserToUserGroupMappingService _userToUserGroupMappingService;
    private readonly IUserGroupService _userGroupService;
    private readonly IUserService _userService;

    #endregion

    #region Ctor

    public UserToUserGroupService(
        ILogger<UserToUserGroupService> logger,
        IAuthorizePermissionValueService authorizePermissionValueService,
        IUserGroupPermissionValueService userGroupPermissionValueService,
        IUserToUserGroupMappingService userToUserGroupMappingService,
        IUserGroupService userGroupService,
        IUserService userService
    )
    {
        _logger = logger;
        _authorizePermissionValueService = authorizePermissionValueService;
        _userGroupPermissionValueService = userGroupPermissionValueService;
        _userToUserGroupMappingService = userToUserGroupMappingService;
        _userGroupService = userGroupService;
        _userService = userService;
    }

    #endregion

    #region Methods

    public async Task<bool> AuthorizePermission<TEntityCompared>(
        User user,
        Permission permission,
        EntityPermissionValueBase<TEntityCompared> entityPermissionValueCompared,
        CancellationToken cancellationToken = default
    ) where TEntityCompared : EntityBase<Guid>
    {
        var userGroups = user.UserToUserGroupMappings.Select(_ => _.Group).ToArray();
        var result = false;
        foreach (var userGroup in userGroups)
        {
            if (await _userGroupPermissionValueService.GetByEntityIdPermissionId(userGroup.Id, permission.Id,
                    cancellationToken) is var permissionValue && permissionValue == null)
                continue;
            result |= _authorizePermissionValueService.Authorize(permissionValue,
                entityPermissionValueCompared);
        }

        return result;
    }

    public async Task<bool> AuthorizePermission(
        User user,
        Permission permission,
        byte[] _valueCompared,
        CancellationToken cancellationToken = default
    )
    {
        var userGroups = user.UserToUserGroupMappings.Select(_ => _.Group).ToArray();
        var result = false;
        foreach (var userGroup in userGroups)
        {
            if (await _userGroupPermissionValueService.GetByEntityIdPermissionId(userGroup.Id, permission.Id,
                    cancellationToken) is var permissionValue && permissionValue == null)
                continue;
            result |= _authorizePermissionValueService.Authorize(permissionValue,
                _valueCompared);
        }

        return result;
    }

    #endregion
}