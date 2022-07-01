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
        IWarningAdvancedService warningAdvancedService
    ) : base(httpContextAccessor, warningAdvancedService)
    {
        _logger = logger;
        _userGroupHandler = userGroupHandler;
    }

    #endregion

    #region Fields

    private readonly ILogger<UserGroupController> _logger;
    private readonly IUserGroupHandler _userGroupHandler;

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
        [Required] [FromBody] UserGroupJoinDto data,
        CancellationToken cancellationToken = default
    )
    {
        return ResponseWith(await _userGroupHandler.Join(data, cancellationToken));
    }

    [HttpPost]
    [Route("leave")]
    [SwaggerOperation(Summary = "Leave UserGroup",
        Description = "Leave UserGroup")]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.JsonWebToken)]
    [ProducesResponseType(typeof(OkResultDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Leave(
        [Required] [FromBody] UserGroupLeaveDto data,
        CancellationToken cancellationToken = default
    )
    {
        return ResponseWith(await _userGroupHandler.Leave(data, cancellationToken));
    }

    #endregion
}