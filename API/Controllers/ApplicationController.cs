using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using API.Controllers.Base;
using BLL.Handlers;
using BLL.Services.Advanced;
using Common.Models;
using DTO.Models.Application;
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
public class ApplicationController : CustomControllerBase
{
    #region Fields

    private readonly ILogger<UserController> _logger;
    private readonly IApplicationHandler _applicationHandler;

    #endregion

    #region Ctor

    public ApplicationController(
        IHttpContextAccessor httpContextAccessor,
        ILogger<UserController> logger,
        IApplicationHandler applicationHandler,
        IWarningAdvancedService warningAdvancedService
    ) : base(
        httpContextAccessor, warningAdvancedService)
    {
        _logger = logger;
        _applicationHandler = applicationHandler;
    }

    #endregion

    #region Methods

    [HttpPost]
    [AllowAnonymous]
    [Route("setup")]
    [SwaggerOperation(Summary = "Setup", Description = "Setup")]
    [ProducesResponseType(typeof(OkResultDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Setup(
        [Required] [FromBody] ApplicationSetupDto data,
        CancellationToken cancellationToken = default
    )
    {
        return Ok(await _applicationHandler.Setup(data, cancellationToken));
    }

    #endregion
}