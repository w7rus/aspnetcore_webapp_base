using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using API.Controllers.Base;
using BLL.Handlers;
using BLL.Services.Advanced;
using DTO.Models.Generic;
using DTO.Models.PermissionValue;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using Common.Models;
using DTO.Models.UserProfile;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class UserProfileController : CustomControllerBase
{
    #region Ctor

    public UserProfileController(
        IHttpContextAccessor httpContextAccessor,
        ILogger<UserProfileController> logger,
        IUserProfileHandler userProfileHandler,
        IWarningAdvancedService warningAdvancedService
    ) : base(httpContextAccessor, warningAdvancedService)
    {
        _logger = logger;
        _userProfileHandler = userProfileHandler;
    }

    #endregion

    #region Fields

    private readonly ILogger<UserProfileController> _logger;
    private readonly IUserProfileHandler _userProfileHandler;

    #endregion

    #region Methods

    [HttpPost]
    [Route(nameof(Create))]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.JsonWebToken)]
    [ProducesResponseType(typeof(UserProfileCreateResultDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Create(
        [Required] [FromBody] UserProfileCreateDto data,
        CancellationToken cancellationToken = default
    )
    {
        return ResponseWith(await _userProfileHandler.Create(data, cancellationToken));
    }

    [HttpGet]
    [Route(nameof(Read))]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.JsonWebToken)]
    [ProducesResponseType(typeof(UserProfileReadResultDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Read(
        [Required] [FromQuery] UserProfileReadDto data,
        CancellationToken cancellationToken = default
    )
    {
        return ResponseWith(await _userProfileHandler.Read(data, cancellationToken));
    }

    [HttpPut]
    [Route(nameof(Update))]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.JsonWebToken)]
    [ProducesResponseType(typeof(UserProfileUpdateResultDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Update(
        [Required] [FromBody] UserProfileUpdateDto data,
        CancellationToken cancellationToken = default
    )
    {
        return ResponseWith(await _userProfileHandler.Update(data, cancellationToken));
    }

    [HttpDelete]
    [Route(nameof(Delete))]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.JsonWebToken)]
    [ProducesResponseType(typeof(OkResultDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Delete(
        [Required] [FromQuery] UserProfileDeleteDto data,
        CancellationToken cancellationToken = default
    )
    {
        return ResponseWith(await _userProfileHandler.Delete(data, cancellationToken));
    }

    #endregion
}