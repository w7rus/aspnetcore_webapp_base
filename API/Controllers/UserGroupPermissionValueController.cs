using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using API.Controllers.Base;
using BLL.Handlers;
using BLL.Services;
using BLL.Services.Advanced;
using Common.Models;
using DTO.Models.PermissionValue;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class UserGroupPermissionValueController : CustomControllerBase
{
    #region Fields

    private readonly ILogger<UserGroupPermissionValueController> _logger;
    private readonly IPermissionValueHandler _permissionValueHandler;

    #endregion

    #region Ctor

    public UserGroupPermissionValueController(
        IHttpContextAccessor httpContextAccessor,
        ILogger<UserGroupPermissionValueController> logger,
        IPermissionValueHandler permissionValueHandler,
        IWarningAdvancedService warningAdvancedService
    ) : base(httpContextAccessor, warningAdvancedService)
    {
        _logger = logger;
        _permissionValueHandler = permissionValueHandler;
    }

    #endregion

    #region Methods

    [HttpPost]
    [SwaggerOperation(Summary = "Creates PermissionValue",
        Description = "Creates PermissionValue")]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.JsonWebToken)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create(
        [Required] [FromBody] PermissionValueCreate data,
        CancellationToken cancellationToken = default
    )
    {
        return ResponseWith(await _permissionValueHandler.Create(data, cancellationToken));
    }

    [HttpGet]
    [SwaggerOperation(Summary = "Reads PermissionValue",
        Description = "Reads PermissionValue")]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.JsonWebToken)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Read(
        [Required] [FromQuery] PermissionValueRead data,
        CancellationToken cancellationToken = default
    )
    {
        return ResponseWith(await _permissionValueHandler.Read(data, cancellationToken));
    }

    [HttpGet]
    [Route("FSPCollection")]
    [SwaggerOperation(Summary = "Reads PermissionValues",
        Description = "Reads PermissionValues")]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.JsonWebToken)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ReadFSPCollection(
        [Required] [FromQuery] PermissionValueReadFSPCollection data,
        CancellationToken cancellationToken = default
    )
    {
        return ResponseWith(await _permissionValueHandler.ReadFSPCollection(data, cancellationToken));
    }

    [HttpPut]
    [SwaggerOperation(Summary = "Updates PermissionValue",
        Description = "Updates PermissionValue")]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.JsonWebToken)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update(
        [Required] [FromBody] PermissionValueUpdate data,
        CancellationToken cancellationToken = default
    )
    {
        return ResponseWith(await _permissionValueHandler.Update(data, cancellationToken));
    }

    [HttpDelete]
    [SwaggerOperation(Summary = "Deletes PermissionValue",
        Description = "Deletes PermissionValue")]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.JsonWebToken)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete(
        [Required] [FromQuery] PermissionValueDelete data,
        CancellationToken cancellationToken = default
    )
    {
        return ResponseWith(await _permissionValueHandler.Delete(data, cancellationToken));
    }

    #endregion
}