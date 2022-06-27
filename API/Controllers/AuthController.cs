using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using API.Controllers.Base;
using BLL.Handlers;
using BLL.Services;
using BLL.Services.Advanced;
using Common.Attributes;
using Common.Models;
using DTO.Models.Auth;
using DTO.Models.Generic;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : CustomControllerBase
    {
        #region Fields

        private readonly ILogger<AuthController> _logger;
        private readonly IAuthHandler _authHandler;

        #endregion

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

        #region Endpoints
        
        [HttpPost]
        [AllowAnonymous]
        [Route("signinasguest")]
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
        [Route("signup")]
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
        [Route("signin")]
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
        [Route("refresh")]
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
        [Route("signout")]
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
}