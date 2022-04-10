using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using API.Controllers.Base;
using BLL.Handlers;
using BLL.Services.Advanced;
using Common.Attributes;
using Common.Models;
using DTO.Models.Auth;
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
    [ProducesResponseType(typeof(ErrorModelResult), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorModelResult), StatusCodes.Status400BadRequest)]
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
            IHttpContextAccessor httpContextAccessor
        ) : base(httpContextAccessor)
        {
            _logger = logger;
            _authHandler = authHandler;
        }

        #endregion

        #region Endpoints
        
        [HttpPost]
        [AllowAnonymous]
        [Route("signupinasguest")]
        [SwaggerOperation(Summary = "Sign Up In for creating a new Guest account",
            Description = "Sign Up for creating a new Guest account")]
        [ProducesResponseType(typeof(AuthSignUpResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> SignUpInAsGuest(
            [Required] [FromBody] AuthSignUpInAsGuest data,
            CancellationToken cancellationToken = default
        )
        {
            return ResponseWith(await _authHandler.SignUpInAsGuest(data, cancellationToken));
        }

        [HttpPost]
        [Route("signup")]
        [Authorize(AuthenticationSchemes = AuthenticationSchemes.JsonWebToken)]
        [SwaggerOperation(Summary = "Sign Up for creating a new User account",
            Description = "Sign Up for creating a new User account")]
        [ProducesResponseType(typeof(AuthSignUpResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> SignUp(
            [Required] [FromBody] AuthSignUp data,
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
        [ProducesResponseType(typeof(AuthSignInResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> SignIn(
            [Required] [FromBody] AuthSignIn data,
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
        [ProducesResponseType(typeof(AuthRefreshResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> Refresh(
            [Required] [FromBody] AuthRefresh data,
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
        [ProducesResponseType(typeof(AuthSignOutResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> SignOut(
            [Required] [FromBody] AuthSignOut data,
            CancellationToken cancellationToken = default
        )
        {
            return ResponseWith(await _authHandler.SignOut(data, cancellationToken));
        }

        #endregion
    }
}