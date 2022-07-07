using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using API.Controllers.Base;
using BLL.Handlers;
using BLL.Services.Advanced;
using Common.Models;
using DTO.Models.Generic;
using DTO.Models.PermissionValue;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class UserToUserGroupMappingPermissionValueController : CustomControllerBase
{
    #region Ctor

    public UserToUserGroupMappingPermissionValueController(
        IHttpContextAccessor httpContextAccessor,
        ILogger<UserToUserGroupMappingPermissionValueController> logger,
        IUserToUserGroupMappingPermissionValueHandler userToUserGroupMappingPermissionValueHandler,
        IWarningAdvancedService warningAdvancedService
    ) : base(httpContextAccessor, warningAdvancedService)
    {
        _logger = logger;
        _userToUserGroupMappingPermissionValueHandler = userToUserGroupMappingPermissionValueHandler;
    }

    #endregion

    #region Fields

    private readonly ILogger<UserToUserGroupMappingPermissionValueController> _logger;
    private readonly IUserToUserGroupMappingPermissionValueHandler _userToUserGroupMappingPermissionValueHandler;

    #endregion

    #region Methods

    [HttpPost]
    [Route(nameof(Create))]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.JsonWebToken)]
    [ProducesResponseType(typeof(PermissionValueCreateResultDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Create(
        [Required] [FromBody] PermissionValueCreateDto data,
        CancellationToken cancellationToken = default
    )
    {
        return ResponseWith(await _userToUserGroupMappingPermissionValueHandler.Create(data, cancellationToken));
    }

    [HttpGet]
    [Route(nameof(Read))]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.JsonWebToken)]
    [ProducesResponseType(typeof(PermissionValueReadResultDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Read(
        [Required] [FromQuery] PermissionValueReadDto data,
        CancellationToken cancellationToken = default
    )
    {
        return ResponseWith(await _userToUserGroupMappingPermissionValueHandler.Read(data, cancellationToken));
    }

    [HttpGet]
    [Route(nameof(ReadCollection))]
    [SwaggerOperation(Summary = "Reads PermissionValues",
        Description = "Reads PermissionValues")]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.JsonWebToken)]
    [ProducesResponseType(typeof(PermissionValueReadCollectionResultDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> ReadCollection(
        [Required] [FromQuery] PermissionValueReadCollectionDto data,
        CancellationToken cancellationToken = default
    )
    {
        return ResponseWith(
            await _userToUserGroupMappingPermissionValueHandler.ReadCollection(data, cancellationToken));
    }

    [HttpPut]
    [Route(nameof(Update))]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.JsonWebToken)]
    [ProducesResponseType(typeof(PermissionValueUpdateResultDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Update(
        [Required] [FromBody] PermissionValueUpdateDto data,
        CancellationToken cancellationToken = default
    )
    {
        return ResponseWith(await _userToUserGroupMappingPermissionValueHandler.Update(data, cancellationToken));
    }

    [HttpDelete]
    [Route(nameof(Delete))]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.JsonWebToken)]
    [ProducesResponseType(typeof(OkResultDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Delete(
        [Required] [FromQuery] PermissionValueDeleteDto data,
        CancellationToken cancellationToken = default
    )
    {
        return ResponseWith(await _userToUserGroupMappingPermissionValueHandler.Delete(data, cancellationToken));
    }

    #endregion
}