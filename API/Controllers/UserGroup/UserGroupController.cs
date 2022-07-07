using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using API.Controllers.Base;
using BLL.Handlers.UserGroup;
using BLL.Services.Advanced;
using Common.Models;
using DTO.Models.Generic;
using DTO.Models.UserGroup;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace API.Controllers.UserGroup;

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
    [ProducesResponseType(typeof(UserGroupReadCollectionResultDto), StatusCodes.Status200OK)]
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

    #endregion
}