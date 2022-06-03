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
    //TODO: Fix
    /// <summary>
    /// Authorizes PermissionValue[] from UserGroup[] given user is member of to another PermissionValue
    /// </summary>
    /// <param name="userLeft">Equation left-side subject</param>
    /// <param name="permissionLeft">Equation left-side subject Permission</param>
    /// <param name="entityPermissionValueRight">Equation right-side PermissionValue</param>
    /// <param name="cancellationToken"></param>
    /// <typeparam name="TEntityCompared"></typeparam>
    /// <returns></returns>
    Task<bool> AuthorizePermissionToPermissionValue<TEntityCompared>(
        User userLeft,
        Permission permissionLeft,
        EntityPermissionValueBase<TEntityCompared> entityPermissionValueRight,
        CancellationToken cancellationToken = default
    ) where TEntityCompared : EntityBase<Guid>;
    
    /// <summary>
    /// Authorizes left-side User to custom value
    /// </summary>
    /// <param name="userLeft">Equation left-side subject</param>
    /// <param name="permissionLeft">Equation left-side subject Permission</param>
    /// <param name="valueRight">Equation right-side value</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> AuthorizePermissionToCustomValue(
        User userLeft,
        Permission permissionLeft,
        byte[] valueRight,
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
    /// Authorizes left-side User to right-side User
    /// </summary>
    /// <param name="userLeft">Equation left-side subject</param>
    /// <param name="permissionLeft">Equation left-side subject Permission</param>
    /// <param name="userRight">Equation right-side subject</param>
    /// <param name="permissionRight">Equation right-side subject Permission</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> AuthorizePermissionToPermission(
        User userLeft,
        Permission permissionLeft,
        User userRight,
        Permission permissionRight,
        CancellationToken cancellationToken = default
    );
    
    /// <summary>
    /// Authorizes left-side User to right-side UserGroup
    /// </summary>
    /// <param name="userLeft">Equation left-side subject</param>
    /// <param name="permissionLeft">Equation left-side subject Permission</param>
    /// <param name="userGroupRight">Equation right-side subject</param>
    /// <param name="permissionRight">Equation right-side subject Permission</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> AuthorizePermissionToPermission(
        User userLeft,
        Permission permissionLeft,
        UserGroup userGroupRight,
        Permission permissionRight,
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

    public async Task<bool> AuthorizePermissionToPermissionValue<TEntityCompared>(
        User userLeft,
        Permission permissionLeft,
        EntityPermissionValueBase<TEntityCompared> entityPermissionValueRight,
        CancellationToken cancellationToken = default
    ) where TEntityCompared : EntityBase<Guid>
    {
        var userGroups = userLeft.UserToUserGroupMappings.Select(_ => _.EntityRight).ToArray();
        Array.Sort(userGroups, (userGroupA, userGroupB) => userGroupA.Priority.CompareTo(userGroupB.Priority));
        
        var result = false;

        foreach (var userGroup in userGroups)
        {
            if (await _userGroupPermissionValueService.GetByEntityIdPermissionId(userGroup.Id, permissionLeft.Id,
                    cancellationToken) is var entityPermissionValueLeft && entityPermissionValueLeft == null)
                continue;

            result = _permissionAdvancedService.Authorize(entityPermissionValueLeft,
                entityPermissionValueRight);
        }

        _logger.Log(LogLevel.Information,
            Localize.Log.Method(GetType(),
                $"{nameof(AuthorizePermissionToPermissionValue)}<{typeof(TEntityCompared).Name}>",
                $"{entityPermissionValueRight.Permission.Alias} authorized {permissionLeft.Alias} as {result}"));

        return result;
    }

    public async Task<bool> AuthorizePermissionToCustomValue(
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
            Localize.Log.Method(GetType(), $"{nameof(AuthorizePermissionToCustomValue)}",
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

    public async Task<bool> AuthorizePermissionToPermission(
        User userLeft,
        Permission permissionLeft,
        User userRight,
        Permission permissionRight,
        CancellationToken cancellationToken = default
    )
    {
        var userGroups = userRight.UserToUserGroupMappings.Select(_ => _.EntityRight).ToArray();
        Array.Sort(userGroups, (userGroupA, userGroupB) => userGroupA.Priority.CompareTo(userGroupB.Priority));
        
        var result = false;

        foreach (var userGroup in userGroups)
        {
            if (await _userGroupPermissionValueService.GetByEntityIdPermissionId(userGroup.Id,
                    permissionRight.Id,
                    cancellationToken) is var entityPermissionValueRight && entityPermissionValueRight == null)
                continue;

            result = await AuthorizePermissionToPermissionValue(userLeft, permissionLeft, entityPermissionValueRight,
                cancellationToken);
        }

        _logger.Log(LogLevel.Information,
            Localize.Log.Method(GetType(),
                $"{nameof(AuthorizePermissionToPermission)}",
                $"{permissionRight.Alias} authorized {permissionLeft.Alias} as {result}"));

        return result;
    }
    
    public async Task<bool> AuthorizePermissionToPermission(
        User userLeft,
        Permission permissionLeft,
        UserGroup userGroupRight,
        Permission permissionRight,
        CancellationToken cancellationToken = default
    )
    {
        if (await _userGroupPermissionValueService.GetByEntityIdPermissionId(userGroupRight.Id,
                permissionRight.Id,
                cancellationToken) is var entityPermissionValueRight && entityPermissionValueRight == null)
            return false;

        var result = await AuthorizePermissionToPermissionValue(userLeft, permissionLeft, entityPermissionValueRight,
            cancellationToken);

        _logger.Log(LogLevel.Information,
            Localize.Log.Method(GetType(),
                $"{nameof(AuthorizePermissionToPermission)}",
                $"{permissionRight.Alias} authorized {permissionLeft.Alias} as {result}"));

        return result;
    }

    #endregion
}