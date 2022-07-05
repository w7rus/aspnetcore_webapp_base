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
    [SwaggerOperation(Summary = "Creates UserGroup",
        Description = "Creates UserGroup")]
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
    [SwaggerOperation(Summary = "Reads UserGroup",
        Description = "Reads UserGroup")]
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
    [Route("FSPCollection")]
    [SwaggerOperation(Summary = "Reads UserGroups",
        Description = "Reads UserGroups")]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.JsonWebToken)]
    [ProducesResponseType(typeof(UserGroupReadResultDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> ReadFSPCollection(
        [Required] [FromQuery] UserGroupReadEntityCollectionDto data,
        CancellationToken cancellationToken = default
    )
    {
        return ResponseWith(await _userGroupHandler.ReadFSPCollection(data, cancellationToken));
    }

    [HttpPut]
    [SwaggerOperation(Summary = "Updates UserGroup",
        Description = "Updates UserGroup")]
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
    [SwaggerOperation(Summary = "Deletes UserGroup",
        Description = "Deletes UserGroup")]
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
    [Route("join")]
    [SwaggerOperation(Summary = "Join UserGroup",
        Description = "Join UserGroup")]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.JsonWebToken)]
    [ProducesResponseType(typeof(OkResultDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Join(
        [Required] [FromBody] UserGroupJoinUserDto data,
        CancellationToken cancellationToken = default
    )
    {
        return ResponseWith(await _userGroupActionsHandler.UserGroupJoinUser(data, cancellationToken));
    }

    [HttpPost]
    [Route("leave")]
    [SwaggerOperation(Summary = "Leave UserGroup",
        Description = "Leave UserGroup")]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.JsonWebToken)]
    [ProducesResponseType(typeof(OkResultDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Leave(
        [Required] [FromBody] UserGroupLeaveUserDto data,
        CancellationToken cancellationToken = default
    )
    {
        return ResponseWith(await _userGroupActionsHandler.UserGroupLeaveUser(data, cancellationToken));
    }
    
    [HttpPost]
    [Route("initTransfer")]
    [SwaggerOperation(Summary = "InitTransfer UserGroup",
        Description = "InitTransfer UserGroup")]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.JsonWebToken)]
    [ProducesResponseType(typeof(UserGroupTransferInitResultDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> InitTransfer(
        [Required] [FromBody] UserGroupCreateTransferDto data,
        CancellationToken cancellationToken = default
    )
    {
        return ResponseWith(await _userGroupActionsHandler.UserGroupCreateTransfer(data, cancellationToken));
    }

    [HttpPut]
    [Route("manageTransfer")]
    [SwaggerOperation(Summary = "ManageTransfer UserGroup",
        Description = "ManageTransfer UserGroup")]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.JsonWebToken)]
    [ProducesResponseType(typeof(OkResultDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> ManageTransfer(
        [Required] [FromBody] UserGroupUpdateTransferDto data,
        CancellationToken cancellationToken = default
    )
    {
        return ResponseWith(await _userGroupActionsHandler.UserGroupUpdateTransfer(data, cancellationToken));
    }

    [HttpPost]
    [Route("initInviteUser")]
    [SwaggerOperation(Summary = "InitInviteUser UserGroup",
        Description = "InitInviteUser UserGroup")]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.JsonWebToken)]
    [ProducesResponseType(typeof(UserGroupInitInviteUserResultDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> InitInviteUser(
        [Required] [FromBody] UserGroupCreateInviteDto data,
        CancellationToken cancellationToken = default
    )
    {
        return ResponseWith(await _userGroupActionsHandler.UserGroupCreateInvite(data, cancellationToken));
    }

    [HttpPut]
    [Route("manageInviteUser")]
    [SwaggerOperation(Summary = "ManageInviteUser UserGroup",
        Description = "ManageInviteUser UserGroup")]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.JsonWebToken)]
    [ProducesResponseType(typeof(OkResultDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> ManageInviteUser(
        [Required] [FromBody] UserGroupUpdateInviteDto data,
        CancellationToken cancellationToken = default
    )
    {
        return ResponseWith(await _userGroupActionsHandler.UserGroupUpdateInvite(data, cancellationToken));
    }
    
    [HttpPost]
    [Route("addUser")]
    [SwaggerOperation(Summary = "AddUser UserGroup",
        Description = "AddUser UserGroup")]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.JsonWebToken)]
    [ProducesResponseType(typeof(OkResultDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> AddUser(
        [Required] [FromBody] UserGroupAddUserDto data,
        CancellationToken cancellationToken = default
    )
    {
        return ResponseWith(await _userGroupActionsHandler.UserGroupAddUser(data, cancellationToken));
    }
    
    [HttpPost]
    [Route("kickUser")]
    [SwaggerOperation(Summary = "KickUser UserGroup",
        Description = "KickUser UserGroup")]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.JsonWebToken)]
    [ProducesResponseType(typeof(OkResultDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> KickUser(
        [Required] [FromBody] UserGroupDeleteUserDto data,
        CancellationToken cancellationToken = default
    )
    {
        return ResponseWith(await _userGroupActionsHandler.UserGroupDeleteUser(data, cancellationToken));
    }

    #endregion
}