using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using API.Controllers.Base;
using BLL.Handlers;
using BLL.Services.Advanced;
using Common.Models;
using DTO.Models.Generic;
using DTO.Models.UserGroup;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class UserGroupController : CustomControllerBase
{
    #region Ctor

    public UserGroupController(
        IHttpContextAccessor httpContextAccessor,
        ILogger<UserGroupController> logger,
        IUserGroupHandler userGroupHandler,
        IWarningAdvancedService warningAdvancedService,
        IUserGroupActionsHandler userGroupActionsHandler
    ) : base(httpContextAccessor, warningAdvancedService)
    {
        _logger = logger;
        _userGroupHandler = userGroupHandler;
        _userGroupActionsHandler = userGroupActionsHandler;
    }

    #endregion

    #region Fields

    private readonly ILogger<UserGroupController> _logger;
    private readonly IUserGroupHandler _userGroupHandler;
    private readonly IUserGroupActionsHandler _userGroupActionsHandler;

    #endregion

    #region Methods

    [HttpPost]
    [Route(nameof(Create))]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.JsonWebToken)]
    [ProducesResponseType(typeof(UserGroupCreateResultDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Create(
        [Required] [FromBody] UserGroupCreateDto data,
        CancellationToken cancellationToken = default
    )
    {
        return ResponseWith(await _userGroupHandler.Create(data, cancellationToken));
    }

    [HttpGet]
    [Route(nameof(Read))]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.JsonWebToken)]
    [ProducesResponseType(typeof(UserGroupReadResultDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Read(
        [Required] [FromQuery] UserGroupReadDto data,
        CancellationToken cancellationToken = default
    )
    {
        return ResponseWith(await _userGroupHandler.Read(data, cancellationToken));
    }
    
    [HttpGet]
    [Route(nameof(ReadCollection))]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.JsonWebToken)]
    [ProducesResponseType(typeof(UserGroupReadResultDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> ReadCollection(
        [Required] [FromQuery] UserGroupReadCollectionDto data,
        CancellationToken cancellationToken = default
    )
    {
        return ResponseWith(await _userGroupHandler.ReadCollection(data, cancellationToken));
    }

    [HttpPut]
    [Route(nameof(Update))]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.JsonWebToken)]
    [ProducesResponseType(typeof(UserGroupUpdateResultDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Update(
        [Required] [FromBody] UserGroupUpdateDto data,
        CancellationToken cancellationToken = default
    )
    {
        return ResponseWith(await _userGroupHandler.Update(data, cancellationToken));
    }

    [HttpDelete]
    [Route(nameof(Delete))]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.JsonWebToken)]
    [ProducesResponseType(typeof(OkResultDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Delete(
        [Required] [FromQuery] UserGroupDeleteDto data,
        CancellationToken cancellationToken = default
    )
    {
        return ResponseWith(await _userGroupHandler.Delete(data, cancellationToken));
    }

    [HttpPost]
    [Route(nameof(UserGroupJoinUser))]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.JsonWebToken)]
    [ProducesResponseType(typeof(OkResultDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> UserGroupJoinUser(
        [Required] [FromBody] UserGroupJoinUserDto data,
        CancellationToken cancellationToken = default
    )
    {
        return ResponseWith(await _userGroupActionsHandler.UserGroupJoinUser(data, cancellationToken));
    }

    [HttpPost]
    [Route(nameof(UserGroupLeaveUser))]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.JsonWebToken)]
    [ProducesResponseType(typeof(OkResultDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> UserGroupLeaveUser(
        [Required] [FromBody] UserGroupLeaveUserDto data,
        CancellationToken cancellationToken = default
    )
    {
        return ResponseWith(await _userGroupActionsHandler.UserGroupLeaveUser(data, cancellationToken));
    }
    
    [HttpPost]
    [Route(nameof(UserGroupTransferRequestCreate))]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.JsonWebToken)]
    [ProducesResponseType(typeof(UserGroupTransferRequestCreateResultDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> UserGroupTransferRequestCreate(
        [Required] [FromBody] UserGroupTransferRequestCreateDto data,
        CancellationToken cancellationToken = default
    )
    {
        return ResponseWith(await _userGroupActionsHandler.UserGroupTransferRequestCreate(data, cancellationToken));
    }

    [HttpPut]
    [Route(nameof(UserGroupTransferRequestUpdate))]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.JsonWebToken)]
    [ProducesResponseType(typeof(OkResultDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> UserGroupTransferRequestUpdate(
        [Required] [FromBody] UserGroupTransferRequestUpdateDto data,
        CancellationToken cancellationToken = default
    )
    {
        return ResponseWith(await _userGroupActionsHandler.UserGroupTransferRequestUpdate(data, cancellationToken));
    }
    
    [HttpGet]
    [Route(nameof(UserGroupTransferRequestRead))]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.JsonWebToken)]
    [ProducesResponseType(typeof(UserGroupTransferRequestReadResultDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> UserGroupTransferRequestRead(
        [Required] [FromQuery] UserGroupTransferRequestReadDto data,
        CancellationToken cancellationToken = default
    )
    {
        return ResponseWith(await _userGroupActionsHandler.UserGroupTransferRequestRead(data, cancellationToken));
    }
    
    [HttpGet]
    [Route(nameof(UserGroupTransferRequestReceiverReadCollection))]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.JsonWebToken)]
    [ProducesResponseType(typeof(UserGroupTransferRequestReadCollectionResultDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> UserGroupTransferRequestReceiverReadCollection(
        [Required] [FromQuery] UserGroupTransferRequestReadCollectionDto data,
        CancellationToken cancellationToken = default
    )
    {
        return ResponseWith(await _userGroupActionsHandler.UserGroupTransferRequestReceiverReadCollection(data, cancellationToken));
    }
    
    [HttpGet]
    [Route(nameof(UserGroupTransferRequestSenderReadCollection))]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.JsonWebToken)]
    [ProducesResponseType(typeof(UserGroupTransferRequestReadCollectionResultDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> UserGroupTransferRequestSenderReadCollection(
        [Required] [FromQuery] UserGroupTransferRequestReadCollectionDto data,
        CancellationToken cancellationToken = default
    )
    {
        return ResponseWith(await _userGroupActionsHandler.UserGroupTransferRequestSenderReadCollection(data, cancellationToken));
    }

    [HttpPost]
    [Route(nameof(UserGroupInviteRequestCreate))]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.JsonWebToken)]
    [ProducesResponseType(typeof(UserGroupInviteRequestCreateResultDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> UserGroupInviteRequestCreate(
        [Required] [FromBody] UserGroupInviteRequestCreateDto data,
        CancellationToken cancellationToken = default
    )
    {
        return ResponseWith(await _userGroupActionsHandler.UserGroupInviteRequestCreate(data, cancellationToken));
    }

    [HttpPut]
    [Route(nameof(UserGroupInviteRequestUpdate))]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.JsonWebToken)]
    [ProducesResponseType(typeof(OkResultDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> UserGroupInviteRequestUpdate(
        [Required] [FromBody] UserGroupInviteRequestUpdateDto data,
        CancellationToken cancellationToken = default
    )
    {
        return ResponseWith(await _userGroupActionsHandler.UserGroupInviteRequestUpdate(data, cancellationToken));
    }

    [HttpGet]
    [Route(nameof(UserGroupInviteRequestRead))]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.JsonWebToken)]
    [ProducesResponseType(typeof(UserGroupInviteRequestReadResultDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> UserGroupInviteRequestRead(
        [Required] [FromQuery] UserGroupInviteRequestReadDto data,
        CancellationToken cancellationToken = default
    )
    {
        return ResponseWith(await _userGroupActionsHandler.UserGroupInviteRequestRead(data, cancellationToken));
    }
    
    [HttpGet]
    [Route(nameof(UserGroupInviteRequestReceiverReadCollection))]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.JsonWebToken)]
    [ProducesResponseType(typeof(UserGroupTransferRequestReadCollectionResultDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> UserGroupInviteRequestReceiverReadCollection(
        [Required] [FromQuery] UserGroupInviteRequestReceiverReadCollectionDto data,
        CancellationToken cancellationToken = default
    )
    {
        return ResponseWith(await _userGroupActionsHandler.UserGroupInviteRequestReceiverReadCollection(data, cancellationToken));
    }
    
    [HttpGet]
    [Route(nameof(UserGroupInviteRequestSenderReadCollection))]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.JsonWebToken)]
    [ProducesResponseType(typeof(UserGroupTransferRequestReadCollectionResultDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> UserGroupInviteRequestSenderReadCollection(
        [Required] [FromQuery] UserGroupInviteRequestSenderReadCollectionDto data,
        CancellationToken cancellationToken = default
    )
    {
        return ResponseWith(await _userGroupActionsHandler.UserGroupInviteRequestSenderReadCollection(data, cancellationToken));
    }

    [HttpPost]
    [Route(nameof(UserGroupAddUser))]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.JsonWebToken)]
    [ProducesResponseType(typeof(OkResultDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> UserGroupAddUser(
        [Required] [FromBody] UserGroupAddUserDto data,
        CancellationToken cancellationToken = default
    )
    {
        return ResponseWith(await _userGroupActionsHandler.UserGroupAddUser(data, cancellationToken));
    }
    
    [HttpPost]
    [Route(nameof(UserGroupDeleteUser))]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.JsonWebToken)]
    [ProducesResponseType(typeof(OkResultDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> UserGroupDeleteUser(
        [Required] [FromBody] UserGroupDeleteUserDto data,
        CancellationToken cancellationToken = default
    )
    {
        return ResponseWith(await _userGroupActionsHandler.UserGroupDeleteUser(data, cancellationToken));
    }

    #endregion
}