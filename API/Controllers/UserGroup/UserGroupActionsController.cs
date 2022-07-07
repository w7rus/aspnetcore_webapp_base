using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using API.Controllers.Base;
using BLL.Handlers.UserGroup;
using BLL.Services.Advanced;
using Common.Models;
using DTO.Models.Generic;
using DTO.Models.UserGroup;
using DTO.Models.UserGroupActions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace API.Controllers.UserGroup;

[ApiController]
[Route("[controller]")]
public class UserGroupActionsController : CustomControllerBase
{
    #region Ctor

    public UserGroupActionsController(
        IHttpContextAccessor httpContextAccessor,
        ILogger<UserGroupActionsController> logger,
        IUserGroupActionsHandler userGroupActionsHandler,
        IWarningAdvancedService warningAdvancedService
    ) : base(httpContextAccessor, warningAdvancedService)
    {
        _logger = logger;
        _userGroupActionsHandler = userGroupActionsHandler;
    }

    #endregion

    #region Fields

    private readonly ILogger<UserGroupActionsController> _logger;
    private readonly IUserGroupActionsHandler _userGroupActionsHandler;

    #endregion

    #region Methods

    [HttpPost]
    [Route(nameof(UserGroupTransferRequestCreate))]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.JsonWebToken)]
    [ProducesResponseType(typeof(UserGroupActionTransferRequestCreateResultDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> UserGroupTransferRequestCreate(
        [Required] [FromBody] UserGroupActionTransferRequestCreateDto data,
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
        [Required] [FromBody] UserGroupActionTransferRequestUpdateDto data,
        CancellationToken cancellationToken = default
    )
    {
        return ResponseWith(await _userGroupActionsHandler.UserGroupTransferRequestUpdate(data, cancellationToken));
    }

    [HttpGet]
    [Route(nameof(UserGroupTransferRequestRead))]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.JsonWebToken)]
    [ProducesResponseType(typeof(UserGroupActionTransferRequestReadResultDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> UserGroupTransferRequestRead(
        [Required] [FromQuery] UserGroupActionTransferRequestReadDto data,
        CancellationToken cancellationToken = default
    )
    {
        return ResponseWith(await _userGroupActionsHandler.UserGroupTransferRequestRead(data, cancellationToken));
    }

    [HttpGet]
    [Route(nameof(UserGroupTransferRequestReceiverReadCollection))]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.JsonWebToken)]
    [ProducesResponseType(typeof(UserGroupActionTransferRequestReadCollectionResultDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> UserGroupTransferRequestReceiverReadCollection(
        [Required] [FromQuery] UserGroupActionTransferRequestReadCollectionDto data,
        CancellationToken cancellationToken = default
    )
    {
        return ResponseWith(
            await _userGroupActionsHandler.UserGroupTransferRequestReceiverReadCollection(data, cancellationToken));
    }

    [HttpGet]
    [Route(nameof(UserGroupTransferRequestSenderReadCollection))]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.JsonWebToken)]
    [ProducesResponseType(typeof(UserGroupActionTransferRequestReadCollectionResultDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> UserGroupTransferRequestSenderReadCollection(
        [Required] [FromQuery] UserGroupActionTransferRequestReadCollectionDto data,
        CancellationToken cancellationToken = default
    )
    {
        return ResponseWith(
            await _userGroupActionsHandler.UserGroupTransferRequestSenderReadCollection(data, cancellationToken));
    }

    [HttpPost]
    [Route(nameof(UserGroupInviteRequestCreate))]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.JsonWebToken)]
    [ProducesResponseType(typeof(UserGroupActionInviteRequestCreateResultDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> UserGroupInviteRequestCreate(
        [Required] [FromBody] UserGroupActionInviteRequestCreateDto data,
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
        [Required] [FromBody] UserGroupActionInviteRequestUpdateDto data,
        CancellationToken cancellationToken = default
    )
    {
        return ResponseWith(await _userGroupActionsHandler.UserGroupInviteRequestUpdate(data, cancellationToken));
    }

    [HttpGet]
    [Route(nameof(UserGroupInviteRequestRead))]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.JsonWebToken)]
    [ProducesResponseType(typeof(UserGroupActionInviteRequestReadResultDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> UserGroupInviteRequestRead(
        [Required] [FromQuery] UserGroupActionInviteRequestReadDto data,
        CancellationToken cancellationToken = default
    )
    {
        return ResponseWith(await _userGroupActionsHandler.UserGroupInviteRequestRead(data, cancellationToken));
    }

    [HttpGet]
    [Route(nameof(UserGroupInviteRequestReceiverReadCollection))]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.JsonWebToken)]
    [ProducesResponseType(typeof(UserGroupActionTransferRequestReadCollectionResultDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> UserGroupInviteRequestReceiverReadCollection(
        [Required] [FromQuery] UserGroupActionInviteRequestReceiverReadCollectionDto data,
        CancellationToken cancellationToken = default
    )
    {
        return ResponseWith(
            await _userGroupActionsHandler.UserGroupInviteRequestReceiverReadCollection(data, cancellationToken));
    }

    [HttpGet]
    [Route(nameof(UserGroupInviteRequestSenderReadCollection))]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.JsonWebToken)]
    [ProducesResponseType(typeof(UserGroupActionTransferRequestReadCollectionResultDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> UserGroupInviteRequestSenderReadCollection(
        [Required] [FromQuery] UserGroupActionInviteRequestSenderReadCollectionDto data,
        CancellationToken cancellationToken = default
    )
    {
        return ResponseWith(
            await _userGroupActionsHandler.UserGroupInviteRequestSenderReadCollection(data, cancellationToken));
    }

    [HttpPost]
    [Route(nameof(UserGroupAddUser))]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.JsonWebToken)]
    [ProducesResponseType(typeof(OkResultDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> UserGroupAddUser(
        [Required] [FromBody] UserGroupActionAddUserDto data,
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