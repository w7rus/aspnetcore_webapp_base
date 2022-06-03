using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using API.Controllers.Base;
using BLL.Handlers;
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
public class PermissionValueController : CustomControllerBase
{
    #region Fields

    private readonly ILogger<PermissionValueController> _logger;
    private readonly IUserGroupPermissionValueHandler _userGroupPermissionValueHandler;

    #endregion

    #region Ctor

    public PermissionValueController(
        IHttpContextAccessor httpContextAccessor,
        ILogger<PermissionValueController> logger,
        IUserGroupPermissionValueHandler userGroupPermissionValueHandler
    ) : base(httpContextAccessor)
    {
        _logger = logger;
        _userGroupPermissionValueHandler = userGroupPermissionValueHandler;
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
        return ResponseWith(await _userGroupPermissionValueHandler.Create(data, cancellationToken));
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
        return ResponseWith(await _userGroupPermissionValueHandler.Read(data, cancellationToken));
    }
    
    [HttpGet]
    [Route("readByEntity")]
    [SwaggerOperation(Summary = "Reads PermissionValues by Entity",
        Description = "Reads PermissionValues by Entity")]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.JsonWebToken)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ReadByEntity(
        [Required] [FromQuery] PermissionValueReadByEntity data,
        CancellationToken cancellationToken = default
    )
    {
        return ResponseWith(await _userGroupPermissionValueHandler.ReadByEntity(data, cancellationToken));
    }
    
    [HttpGet]
    [Route("readByPermission")]
    [SwaggerOperation(Summary = "Reads PermissionValues by Permission",
        Description = "Reads PermissionValues by Permission")]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.JsonWebToken)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ReadByPermission(
        [Required] [FromQuery] PermissionValueReadByPermission data,
        CancellationToken cancellationToken = default
    )
    {
        return ResponseWith(await _userGroupPermissionValueHandler.ReadByPermission(data, cancellationToken));
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
        return ResponseWith(await _userGroupPermissionValueHandler.Update(data, cancellationToken));
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
        return ResponseWith(await _userGroupPermissionValueHandler.Delete(data, cancellationToken));
    }

    #endregion
}