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

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class UserPermissionValueController : CustomControllerBase
{
    #region Ctor

    public UserPermissionValueController(
        IHttpContextAccessor httpContextAccessor,
        ILogger<UserPermissionValueController> logger,
        IUserPermissionValueHandler userPermissionValueHandler,
        IWarningAdvancedService warningAdvancedService
    ) : base(httpContextAccessor, warningAdvancedService)
    {
        _logger = logger;
        _userPermissionValueHandler = userPermissionValueHandler;
    }

    #endregion

    #region Fields

    private readonly ILogger<UserPermissionValueController> _logger;
    private readonly IUserPermissionValueHandler _userPermissionValueHandler;

    #endregion

    #region Methods
    
    [HttpPost]
    [SwaggerOperation(Summary = "Creates PermissionValue",
        Description = "Creates PermissionValue")]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.JsonWebToken)]
    [ProducesResponseType(typeof(PermissionValueCreateResultDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Create(
        [Required] [FromBody] PermissionValueCreateDto data,
        CancellationToken cancellationToken = default
    )
    {
        return ResponseWith(await _userPermissionValueHandler.Create(data, cancellationToken));
    }

    [HttpGet]
    [SwaggerOperation(Summary = "Reads PermissionValue",
        Description = "Reads PermissionValue")]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.JsonWebToken)]
    [ProducesResponseType(typeof(PermissionValueReadResultDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Read(
        [Required] [FromQuery] PermissionValueReadDto data,
        CancellationToken cancellationToken = default
    )
    {
        return ResponseWith(await _userPermissionValueHandler.Read(data, cancellationToken));
    }

    [HttpGet]
    [Route("FSPCollection")]
    [SwaggerOperation(Summary = "Reads PermissionValues",
        Description = "Reads PermissionValues")]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.JsonWebToken)]
    [ProducesResponseType(typeof(PermissionValueReadFSPCollectionResultDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> ReadFSPCollection(
        [Required] [FromQuery] PermissionValueReadEntityCollectionDto data,
        CancellationToken cancellationToken = default
    )
    {
        return ResponseWith(await _userPermissionValueHandler.ReadFSPCollection(data, cancellationToken));
    }

    [HttpPut]
    [SwaggerOperation(Summary = "Updates PermissionValue",
        Description = "Updates PermissionValue")]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.JsonWebToken)]
    [ProducesResponseType(typeof(PermissionValueUpdateResultDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Update(
        [Required] [FromBody] PermissionValueUpdateDto data,
        CancellationToken cancellationToken = default
    )
    {
        return ResponseWith(await _userPermissionValueHandler.Update(data, cancellationToken));
    }

    [HttpDelete]
    [SwaggerOperation(Summary = "Deletes PermissionValue",
        Description = "Deletes PermissionValue")]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.JsonWebToken)]
    [ProducesResponseType(typeof(OkResultDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Delete(
        [Required] [FromQuery] PermissionValueDeleteDto data,
        CancellationToken cancellationToken = default
    )
    {
        return ResponseWith(await _userPermissionValueHandler.Delete(data, cancellationToken));
    }

    #endregion
}