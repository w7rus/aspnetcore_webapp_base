using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common.Models;
using Domain.Entities;
using Domain.Entities.Base;
using Domain.Enums;
using Microsoft.Extensions.Logging;

namespace BLL.Services.Advanced;

/// <summary>
/// Advanced Service for User and UserGroup management
/// </summary>
public interface IUserToUserGroupAdvancedService
{
    /// <summary>
    /// Authorizes PermissionValue[] from UserGroup[] given user is member of to another PermissionValue
    /// </summary>
    /// <param name="userComparable">Subject that initiates an action</param>
    /// <param name="userPermissionComparable">Permission of a Subject that initiates an action</param>
    /// <param name="entityPermissionValueCompared">PermissionValue against which authorization will run</param>
    /// <param name="cancellationToken"></param>
    /// <typeparam name="TEntityCompared"></typeparam>
    /// <returns></returns>
    Task<bool> AuthorizeUserPermissionToAnyPermissionValue<TEntityCompared>(
        User userComparable,
        Permission userPermissionComparable,
        EntityPermissionValueBase<TEntityCompared> entityPermissionValueCompared,
        CancellationToken cancellationToken = default
    ) where TEntityCompared : EntityBase<Guid>;
    
    /// <summary>
    /// Authorizes PermissionValue[] from UserGroup[] given user is member of to custom value
    /// </summary>
    /// <param name="userComparable">Subject that initiates an action</param>
    /// <param name="userPermissionComparable">Permission of a Subject that initiates an action</param>
    /// <param name="valueCompared">Value against which authorization will run</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> AuthorizeUserPermissionToCustomValue(
        User userComparable,
        Permission userPermissionComparable,
        byte[] valueCompared,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Returns PermissionValue with Type = PermissionType.ValueNeededSystem by given alias
    /// </summary>
    /// <param name="alias"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<UserGroupPermissionValue> GetSystemPermissionValueByAlias(
        string alias,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Authorizes PermissionValue[] from UserGroup[] to another PermissionValue[] from UserGroup[] user is member of
    /// </summary>
    /// <param name="userComparable">Subject that initiates an action</param>
    /// <param name="userPermissionComparable">Permission of a Subject that initiates an action</param>
    /// <param name="userCompared">Object's Subject over which the action is performed</param>
    /// <param name="userPermissionCompared">Permission of a Object's Subject over which the action is performed</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> AuthorizeUserPermissionToUserPermission(
        User userComparable,
        Permission userPermissionComparable,
        User userCompared,
        Permission userPermissionCompared,
        CancellationToken cancellationToken = default
    );
}

public class UserToUserGroupAdvancedService : IUserToUserGroupAdvancedService
{
    #region Fields

    private readonly ILogger<UserToUserGroupAdvancedService> _logger;
    private readonly IPermissionAdvancedService _permissionAdvancedService;
    private readonly IUserGroupPermissionValueService _userGroupPermissionValueService;
    private readonly IUserToUserGroupMappingService _userToUserGroupMappingService;
    private readonly IUserGroupService _userGroupService;
    private readonly IUserService _userService;
    private readonly IPermissionService _permissionService;

    #endregion

    #region Ctor

    public UserToUserGroupAdvancedService(
        ILogger<UserToUserGroupAdvancedService> logger,
        IPermissionAdvancedService permissionAdvancedService,
        IUserGroupPermissionValueService userGroupPermissionValueService,
        IUserToUserGroupMappingService userToUserGroupMappingService,
        IUserGroupService userGroupService,
        IUserService userService,
        IPermissionService permissionService
    )
    {
        _logger = logger;
        _permissionAdvancedService = permissionAdvancedService;
        _userGroupPermissionValueService = userGroupPermissionValueService;
        _userToUserGroupMappingService = userToUserGroupMappingService;
        _userGroupService = userGroupService;
        _userService = userService;
        _permissionService = permissionService;
    }

    #endregion

    #region Methods

    public async Task<bool> AuthorizeUserPermissionToAnyPermissionValue<TEntityCompared>(
        User userComparable,
        Permission userPermissionComparable,
        EntityPermissionValueBase<TEntityCompared> entityPermissionValueCompared,
        CancellationToken cancellationToken = default
    ) where TEntityCompared : EntityBase<Guid>
    {
        var userGroups = userComparable.UserToUserGroupMappings.Select(_ => _.EntityRight).ToArray();
        Array.Sort(userGroups, (userGroupA, userGroupB) => userGroupA.Priority.CompareTo(userGroupB.Priority));
        
        var result = false;

        foreach (var userGroup in userGroups)
        {
            if (await _userGroupPermissionValueService.GetByEntityIdPermissionId(userGroup.Id, userPermissionComparable.Id,
                    cancellationToken) is var permissionValue && permissionValue == null)
                continue;

            result = _permissionAdvancedService.Authorize(permissionValue,
                entityPermissionValueCompared);
        }

        _logger.Log(LogLevel.Information,
            Localize.Log.Method(GetType(),
                $"{nameof(AuthorizeUserPermissionToAnyPermissionValue)}<{typeof(TEntityCompared).Name}>",
                $"{entityPermissionValueCompared.Permission.Alias} authorized {userPermissionComparable.Alias} as {result}"));

        return result;
    }

    public async Task<bool> AuthorizeUserPermissionToCustomValue(
        User userComparable,
        Permission userPermissionComparable,
        byte[] valueCompared,
        CancellationToken cancellationToken = default
    )
    {
        var userGroups = userComparable.UserToUserGroupMappings.Select(_ => _.EntityRight).ToArray();
        Array.Sort(userGroups, (userGroupA, userGroupB) => userGroupA.Priority.CompareTo(userGroupB.Priority));
        
        var result = false;

        foreach (var userGroup in userGroups)
        {
            if (await _userGroupPermissionValueService.GetByEntityIdPermissionId(userGroup.Id, userPermissionComparable.Id,
                    cancellationToken) is var permissionValue && permissionValue == null)
                continue;

            result = _permissionAdvancedService.Authorize(permissionValue,
                valueCompared);
        }

        _logger.Log(LogLevel.Information,
            Localize.Log.Method(GetType(), $"{nameof(AuthorizeUserPermissionToCustomValue)}",
                $"{Convert.ToBase64String(valueCompared)} authorized {userPermissionComparable.Alias} as {result}"));

        return result;
    }

    public async Task<UserGroupPermissionValue> GetSystemPermissionValueByAlias(
        string alias,
        CancellationToken cancellationToken = default
    )
    {
        var userGroup = await _userGroupService.GetByAliasAsync("Root");
        var permission = await _permissionService.GetByAliasAndTypeAsync(alias, PermissionType.ValueNeededSystem);
        var permissionValue = await _userGroupPermissionValueService.GetByEntityIdPermissionId(userGroup.Id,
            permission.Id,
            cancellationToken);

        _logger.Log(LogLevel.Information,
            Localize.Log.Method(GetType(), nameof(GetSystemPermissionValueByAlias),
                $"{permissionValue.GetType().Name} {permissionValue.Id}"));

        return permissionValue;
    }

    public async Task<bool> AuthorizeUserPermissionToUserPermission(
        User userComparable,
        Permission userPermissionComparable,
        User userCompared,
        Permission userPermissionCompared,
        CancellationToken cancellationToken = default
    )
    {
        var userGroups = userComparable.UserToUserGroupMappings.Select(_ => _.EntityRight).ToArray();
        Array.Sort(userGroups, (userGroupA, userGroupB) => userGroupA.Priority.CompareTo(userGroupB.Priority));
        
        var result = false;

        foreach (var userGroup in userGroups)
        {
            if (await _userGroupPermissionValueService.GetByEntityIdPermissionId(userGroup.Id,
                    userPermissionCompared.Id,
                    cancellationToken) is var permissionValue && permissionValue == null)
                continue;

            result = await AuthorizeUserPermissionToAnyPermissionValue(userCompared, userPermissionComparable, permissionValue,
                cancellationToken);
        }

        _logger.Log(LogLevel.Information,
            Localize.Log.Method(GetType(),
                $"{nameof(AuthorizeUserPermissionToUserPermission)}",
                $"{userPermissionCompared.Alias} authorized {userPermissionComparable.Alias} as {result}"));

        return result;
    }

    #endregion
}