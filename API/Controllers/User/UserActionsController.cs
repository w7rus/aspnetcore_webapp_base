using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using API.Controllers.Base;
using BLL.Handlers.User;
using BLL.Services.Advanced;
using Common.Models;
using DTO.Models.Generic;
using DTO.Models.UserActions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace API.Controllers.User;

[ApiController]
[Route("[controller]")]
public class UserActionsController : CustomControllerBase
{
    #region Ctor

    public UserActionsController(
        IHttpContextAccessor httpContextAccessor,
        ILogger<UserActionsController> logger,
        IUserActionsHandler userActionsHandler,
        IWarningAdvancedService warningAdvancedService
    ) : base(httpContextAccessor, warningAdvancedService)
    {
        _logger = logger;
        _userActionsHandler = userActionsHandler;
    }

    #endregion

    #region Fields

    private readonly ILogger<UserActionsController> _logger;
    private readonly IUserActionsHandler _userActionsHandler;

    #endregion

    #region Methods
    
    [HttpPost]
    [Route(nameof(UserActionJoinUserGroup))]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.JsonWebToken)]
    [ProducesResponseType(typeof(OkResultDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> UserActionJoinUserGroup(
        [Required] [FromBody] UserActionJoinUserGroupDto data,
        CancellationToken cancellationToken = default
    )
    {
        return ResponseWith(await _userActionsHandler.UserActionJoinUserGroup(data, cancellationToken));
    }

    [HttpPost]
    [Route(nameof(UserActionLeaveUserGroup))]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.JsonWebToken)]
    [ProducesResponseType(typeof(OkResultDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> UserActionLeaveUserGroup(
        [Required] [FromBody] UserActionLeaveUserGroupDto data,
        CancellationToken cancellationToken = default
    )
    {
        return ResponseWith(await _userActionsHandler.UserActionLeaveUserGroup(data, cancellationToken));
    }

    #endregion
}