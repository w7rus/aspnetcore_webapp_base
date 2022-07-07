using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using API.Controllers.Base;
using BLL.Handlers.Permission;
using BLL.Services.Advanced;
using Common.Models;
using DTO.Models.Permission;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace API.Controllers.Permission;

[ApiController]
[Route("[controller]")]
public class PermissionController : CustomControllerBase
{
    #region Ctor

    public PermissionController(
        IHttpContextAccessor httpContextAccessor,
        ILogger<PermissionController> logger,
        IPermissionHandler permissionHandler,
        IWarningAdvancedService warningAdvancedService
    ) : base(
        httpContextAccessor, warningAdvancedService)
    {
        _logger = logger;
        _permissionHandler = permissionHandler;
    }

    #endregion

    #region Fields

    private readonly ILogger<PermissionController> _logger;
    private readonly IPermissionHandler _permissionHandler;

    #endregion

    #region Methods

    [HttpGet]
    [Route(nameof(Read))]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.JsonWebToken)]
    [ProducesResponseType(typeof(PermissionReadResultDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Read(
        [Required] [FromQuery] PermissionReadDto data,
        CancellationToken cancellationToken = default
    )
    {
        return ResponseWith(await _permissionHandler.Read(data, cancellationToken));
    }

    [HttpGet]
    [Route(nameof(ReadCollection))]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.JsonWebToken)]
    [ProducesResponseType(typeof(PermissionReadCollectionResultDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> ReadCollection(
        [Required] [FromQuery] PermissionReadCollectionDto data,
        CancellationToken cancellationToken = default
    )
    {
        return ResponseWith(await _permissionHandler.ReadCollection(data, cancellationToken));
    }

    #endregion
}