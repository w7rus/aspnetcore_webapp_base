using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using API.Controllers.Base;
using BLL.Handlers;
using BLL.Services.Advanced;
using Common.Models;
using DTO.Models.Auth;
using DTO.Models.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : CustomControllerBase
{
    #region Ctor

    public AuthController(
        ILogger<AuthController> logger,
        IAuthHandler authHandler,
        IHttpContextAccessor httpContextAccessor,
        IWarningAdvancedService warningAdvancedService
    ) : base(httpContextAccessor, warningAdvancedService)
    {
        _logger = logger;
        _authHandler = authHandler;
    }

    #endregion

    #region Fields

    private readonly ILogger<AuthController> _logger;
    private readonly IAuthHandler _authHandler;

    #endregion

    #region Endpoints

    [HttpPost]
    [AllowAnonymous]
    [Route(nameof(SignInAsGuest))]
    [SwaggerOperation(Summary = "Sign In as Guest",
        Description = "Sign In as Guest")]
    [ProducesResponseType(typeof(AuthSignUpResultDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> SignInAsGuest(
        [Required] [FromBody] AuthSignUpInAsGuestDto data,
        CancellationToken cancellationToken = default
    )
    {
        return ResponseWith(await _authHandler.SignInAsGuest(data, cancellationToken));
    }

    [HttpPost]
    [Route(nameof(SignUp))]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.JsonWebToken)]
    [SwaggerOperation(Summary = "Sign Up for creating a new User account",
        Description = "Sign Up for creating a new User account")]
    [ProducesResponseType(typeof(AuthSignUpResultDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> SignUp(
        [Required] [FromBody] AuthSignUpDto data,
        CancellationToken cancellationToken = default
    )
    {
        return ResponseWith(await _authHandler.SignUp(data, cancellationToken));
    }

    [HttpPost]
    [Route(nameof(SignIn))]
    [AllowAnonymous]
    [SwaggerOperation(Summary = "Sign In to an existing User account",
        Description = "Sign In to an existing User account")]
    [ProducesResponseType(typeof(AuthSignInResultDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> SignIn(
        [Required] [FromBody] AuthSignInDto data,
        CancellationToken cancellationToken = default
    )
    {
        return ResponseWith(await _authHandler.SignIn(data, cancellationToken));
    }

    [HttpPost]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.JsonWebTokenExpired)]
    [Route(nameof(Refresh))]
    [SwaggerOperation(Summary = "Refresh Authorization using Refresh Token",
        Description = "Refresh Authorization using Refresh Token")]
    [ProducesResponseType(typeof(AuthRefreshResultDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Refresh(
        [Required] [FromBody] AuthRefreshDto data,
        CancellationToken cancellationToken = default
    )
    {
        return ResponseWith(await _authHandler.Refresh(data, cancellationToken));
    }

    [HttpPost]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.JsonWebToken)]
    [Route(nameof(SignOut))]
    [SwaggerOperation(Summary = "Sign Out from an existing User account",
        Description = "Sign Out from an existing User account")]
    [ProducesResponseType(typeof(OkResultDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> SignOut(
        [Required] [FromBody] AuthSignOutDto data,
        CancellationToken cancellationToken = default
    )
    {
        return ResponseWith(await _authHandler.SignOut(data, cancellationToken));
    }

    #endregion
}