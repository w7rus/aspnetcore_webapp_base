using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using BLL.Handlers.Base;
using BLL.Services.Advanced;
using BLL.Services.Entity;
using Common.Enums;
using Common.Exceptions;
using Common.Models;
using Common.Models.Base;
using DAL.Data;
using DAL.Repository;
using Domain.Entities;
using DTO.Models.Generic;
using DTO.Models.UserActions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace BLL.Handlers.User;

public interface IUserActionsHandler
{
    Task<IDtoResultBase> UserActionJoinUserGroup(
        UserActionJoinUserGroupDto data,
        CancellationToken cancellationToken = default
    );

    Task<IDtoResultBase> UserActionLeaveUserGroup(
        UserActionLeaveUserGroupDto data,
        CancellationToken cancellationToken = default
    );
}

public class UserActionsHandler : HandlerBase, IUserActionsHandler
{
    #region Ctor

    public UserActionsHandler(
        ILogger<HandlerBase> logger,
        IAppDbContextAction appDbContextAction,
        IMapper mapper,
        IUserAdvancedService userAdvancedService,
        IAuthorizeAdvancedService authorizeAdvancedService,
        IUserRepository userRepository,
        IUserGroupRepository userGroupRepository,
        IUserToUserGroupMappingRepository userToUserGroupMappingRepository,
        IUserGroupEntityService userGroupEntityService,
        IUserToUserGroupMappingEntityService userToUserGroupMappingEntityService,
        IPermissionValueEntityService permissionValueEntityService,
        IAuthorizeEntityService authorizeEntityService
    )
    {
        _logger = logger;
        _appDbContextAction = appDbContextAction;
        _userAdvancedService = userAdvancedService;
        _authorizeAdvancedService = authorizeAdvancedService;
        _userRepository = userRepository;
        _userGroupRepository = userGroupRepository;
        _userToUserGroupMappingRepository = userToUserGroupMappingRepository;
        _userGroupEntityService = userGroupEntityService;
        _userToUserGroupMappingEntityService = userToUserGroupMappingEntityService;
        _permissionValueEntityService = permissionValueEntityService;
        _authorizeEntityService = authorizeEntityService;
    }

    #endregion

    #region Fields

    private readonly ILogger<HandlerBase> _logger;
    private readonly IAppDbContextAction _appDbContextAction;
    private readonly IUserAdvancedService _userAdvancedService;
    private readonly IAuthorizeAdvancedService _authorizeAdvancedService;
    private readonly IUserRepository _userRepository;
    private readonly IUserGroupRepository _userGroupRepository;
    private readonly IUserToUserGroupMappingRepository _userToUserGroupMappingRepository;
    private readonly IUserGroupEntityService _userGroupEntityService;
    private readonly IUserToUserGroupMappingEntityService _userToUserGroupMappingEntityService;
    private readonly IPermissionValueEntityService _permissionValueEntityService;
    private readonly IAuthorizeEntityService _authorizeEntityService;

    #endregion

    #region Methods

    public async Task<IDtoResultBase> UserActionJoinUserGroup(
        UserActionJoinUserGroupDto data,
        CancellationToken cancellationToken = default
    )
    {
        _logger.Log(LogLevel.Information, Localize.Log.MethodStart(GetType(), nameof(UserActionJoinUserGroup)));

        if (ValidateModel(data) is { } validationResult)
            return validationResult;

        try
        {
            await _appDbContextAction.BeginTransactionAsync();

            var user = await _userAdvancedService.GetFromHttpContext(cancellationToken: cancellationToken);
            if (user == null)
                throw new HttpResponseException(StatusCodes.Status400BadRequest, ErrorType.HttpContext,
                    Localize.Error.UserNotFoundOrHttpContextMissingClaims);

            var userGroup = await _userGroupEntityService.GetByIdAsync(data.UserGroupId, cancellationToken);
            if (userGroup == null)
                throw new HttpResponseException(StatusCodes.Status404NotFound, ErrorType.Generic,
                    Localize.Error.UserGroupNotFound);

            //Authorize against UserGroup TODO: + against User (user that initiates action)
            var authorizeResult = _authorizeAdvancedService.Authorize(new AuthorizeModel
            {
                EntityLeftTableName = _userRepository.GetTableName(),
                EntityLeftGroupsTableName = _userGroupRepository.GetTableName(),
                EntityLeftEntityToEntityMappingsTableName = _userToUserGroupMappingRepository.GetTableName(),
                EntityLeftId = user.Id,
                EntityLeftPermissionAlias = Consts.PermissionAlias.UserGroupJoin,
                EntityRightTableName = _userGroupRepository.GetTableName(),
                EntityRightGroupsTableName = null,
                EntityRightEntityToEntityMappingsTableName = null,
                EntityRightId = userGroup.Id,
                EntityRightPermissionAlias = Consts.PermissionAlias.UserGroupJoin,
                SqlExpressionPermissionTypeValueNeededOwner = "T1.\"Id\" = T2.\"UserId\""
            });

            if (!authorizeResult)
                throw new HttpResponseException(StatusCodes.Status403Forbidden, ErrorType.Permission,
                    Localize.Error.PermissionInsufficientPermissions);

            //Do not allow to join User associated with UserGroup
            if (await _userToUserGroupMappingEntityService.GetByUserIdUserGroupIdAsync(user.Id, userGroup.Id) != null)
                throw new HttpResponseException(StatusCodes.Status400BadRequest, ErrorType.Generic,
                    Localize.Error.UserToUserGroupMappingAlreadyExists);

            await _userToUserGroupMappingEntityService.Save(new UserToUserGroupMapping
            {
                EntityLeftId = user.Id,
                EntityRightId = userGroup.Id
            }, cancellationToken);

            //Delete Authorize cache when joining group (for the user that joined)
            await _authorizeEntityService.PurgeByEntityIdAsync(user.Id, cancellationToken);

            await _appDbContextAction.CommitTransactionAsync();

            _logger.Log(LogLevel.Information, Localize.Log.MethodEnd(GetType(), nameof(UserActionJoinUserGroup)));

            return new OkResultDto();
        }
        catch (Exception)
        {
            await _appDbContextAction.RollbackTransactionAsync();

            throw;
        }
    }

    public async Task<IDtoResultBase> UserActionLeaveUserGroup(
        UserActionLeaveUserGroupDto data,
        CancellationToken cancellationToken = default
    )
    {
        _logger.Log(LogLevel.Information, Localize.Log.MethodStart(GetType(), nameof(UserActionLeaveUserGroup)));

        if (ValidateModel(data) is { } validationResult)
            return validationResult;

        try
        {
            await _appDbContextAction.BeginTransactionAsync();

            var user = await _userAdvancedService.GetFromHttpContext(cancellationToken: cancellationToken);
            if (user == null)
                throw new HttpResponseException(StatusCodes.Status400BadRequest, ErrorType.HttpContext,
                    Localize.Error.UserNotFoundOrHttpContextMissingClaims);

            var userGroup = await _userGroupEntityService.GetByIdAsync(data.UserGroupId, cancellationToken);
            if (userGroup == null)
                throw new HttpResponseException(StatusCodes.Status404NotFound, ErrorType.Generic,
                    Localize.Error.UserGroupNotFound);

            //Authorize against UserGroup TODO: + against User (user that initiates action)
            var authorizeResult = _authorizeAdvancedService.Authorize(new AuthorizeModel
            {
                EntityLeftTableName = _userRepository.GetTableName(),
                EntityLeftGroupsTableName = _userGroupRepository.GetTableName(),
                EntityLeftEntityToEntityMappingsTableName = _userToUserGroupMappingRepository.GetTableName(),
                EntityLeftId = user.Id,
                EntityLeftPermissionAlias = Consts.PermissionAlias.UserGroupLeave,
                EntityRightTableName = _userGroupRepository.GetTableName(),
                EntityRightGroupsTableName = null,
                EntityRightEntityToEntityMappingsTableName = null,
                EntityRightId = userGroup.Id,
                EntityRightPermissionAlias = Consts.PermissionAlias.UserGroupLeave,
                SqlExpressionPermissionTypeValueNeededOwner = "T1.\"Id\" = T2.\"UserId\""
            });

            if (!authorizeResult)
                throw new HttpResponseException(StatusCodes.Status403Forbidden, ErrorType.Permission,
                    Localize.Error.PermissionInsufficientPermissions);

            var userToUserGroupMapping =
                await _userToUserGroupMappingEntityService.GetByUserIdUserGroupIdAsync(user.Id, userGroup.Id);
            if (userToUserGroupMapping == null)
                throw new HttpResponseException(StatusCodes.Status404NotFound, ErrorType.Generic,
                    Localize.Error.UserToUserGroupMappingNotFound);

            await _permissionValueEntityService.PurgeAsync(userToUserGroupMapping.Id, cancellationToken);

            //Delete Authorize cache when leaving group (for the user that left)
            await _authorizeEntityService.PurgeByEntityIdAsync(user.Id, cancellationToken);
            await _authorizeEntityService.PurgeByEntityIdAsync(userToUserGroupMapping.Id, cancellationToken);

            await _userToUserGroupMappingEntityService.Delete(userToUserGroupMapping, cancellationToken);

            await _appDbContextAction.CommitTransactionAsync();

            _logger.Log(LogLevel.Information, Localize.Log.MethodEnd(GetType(), nameof(UserActionLeaveUserGroup)));

            return new OkResultDto();
        }
        catch (Exception)
        {
            await _appDbContextAction.RollbackTransactionAsync();

            throw;
        }
    }

    #endregion
}