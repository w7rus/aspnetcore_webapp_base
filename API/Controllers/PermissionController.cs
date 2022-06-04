using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using API.Controllers.Base;
using BLL.Handlers;
using DTO.Models.Permission;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using Common.Models;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class PermissionController : CustomControllerBase
{
    #region Fields

    private readonly ILogger<PermissionController> _logger;
    private readonly IPermissionHandler _permissionHandler;

    #endregion

    #region Ctor

    public PermissionController(
        IHttpContextAccessor httpContextAccessor,
        ILogger<PermissionController> logger,
        IPermissionHandler permissionHandler
    ) : base(
        httpContextAccessor)
    {
        _logger = logger;
        _permissionHandler = permissionHandler;
    }

    #endregion

    #region Methods

    [HttpGet]
    [SwaggerOperation(Summary = "Reads Permission",
        Description = "Reads Permission")]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.JsonWebToken)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Read(
        [Required] [FromQuery] PermissionRead data,
        CancellationToken cancellationToken = default
    )
    {
        return ResponseWith(await _permissionHandler.Read(data, cancellationToken));
    }
    
    [HttpGet]
    [Route("fsp")]
    [SwaggerOperation(Summary = "Reads Permission[]",
        Description = "Reads Permission[]")]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.JsonWebToken)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ReadFilteredSortedPaged(
        [Required] [FromQuery] PermissionReadCollection data,
        CancellationToken cancellationToken = default
    )
    {
        return ResponseWith(await _permissionHandler.ReadFilteredSortedPaged(data, cancellationToken));
    }

    #endregion
}