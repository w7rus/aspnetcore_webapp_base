using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using API.Controllers.Base;
using BLL.Handlers.User;
using BLL.Services.Advanced;
using Common.Models;
using DTO.Models.Generic;
using DTO.Models.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace API.Controllers.User;

[ApiController]
[Route("[controller]")]
public class UserController : CustomControllerBase
{
    #region Ctor

    public UserController(
        IHttpContextAccessor httpContextAccessor,
        ILogger<UserController> logger,
        IUserHandler userHandler,
        IWarningAdvancedService warningAdvancedService
    ) : base(
        httpContextAccessor, warningAdvancedService)
    {
        _logger = logger;
        _userHandler = userHandler;
    }

    #endregion

    #region Fields

    private readonly ILogger<UserController> _logger;
    private readonly IUserHandler _userHandler;

    #endregion

    #region Methods

    [HttpGet]
    [Route(nameof(Read))]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.JsonWebToken)]
    [ProducesResponseType(typeof(UserReadResultDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Read(
        [Required] [FromQuery] UserReadDto data,
        CancellationToken cancellationToken = default
    )
    {
        return ResponseWith(await _userHandler.Read(data, cancellationToken));
    }

    [HttpGet]
    [Route(nameof(ReadCollection))]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.JsonWebToken)]
    [ProducesResponseType(typeof(UserReadCollectionResultDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> ReadCollection(
        [Required] [FromQuery] UserReadCollectionDto data,
        CancellationToken cancellationToken = default
    )
    {
        return ResponseWith(await _userHandler.ReadCollection(data, cancellationToken));
    }

    [HttpPut]
    [Route(nameof(Update))]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.JsonWebToken)]
    [ProducesResponseType(typeof(UserUpdateResultDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Update(
        [Required] [FromBody] UserUpdateDto data,
        CancellationToken cancellationToken = default
    )
    {
        return ResponseWith(await _userHandler.Update(data, cancellationToken));
    }

    [HttpDelete]
    [Route(nameof(Delete))]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.JsonWebToken)]
    [ProducesResponseType(typeof(OkResultDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Delete(
        [Required] [FromQuery] UserDeleteDto data,
        CancellationToken cancellationToken = default
    )
    {
        return ResponseWith(await _userHandler.Delete(data, cancellationToken));
    }

    #endregion
}