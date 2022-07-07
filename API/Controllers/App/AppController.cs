using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using API.Controllers.Base;
using API.Controllers.User;
using BLL.Handlers.App;
using BLL.Services.Advanced;
using DTO.Models.Application;
using DTO.Models.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;

namespace API.Controllers.App;

[ApiController]
[Route("[controller]")]
public class AppController : CustomControllerBase
{
    #region Ctor

    public AppController(
        IHttpContextAccessor httpContextAccessor,
        ILogger<UserController> logger,
        IAppHandler appHandler,
        IWarningAdvancedService warningAdvancedService
    ) : base(
        httpContextAccessor, warningAdvancedService)
    {
        _logger = logger;
        _appHandler = appHandler;
    }

    #endregion

    #region Methods

    [HttpPost]
    [AllowAnonymous]
    [Route(nameof(Setup))]
    [SwaggerOperation(Summary = "Setup", Description = "Setup")]
    [ProducesResponseType(typeof(OkResultDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Setup(
        [Required] [FromBody] ApplicationSetupDto data,
        CancellationToken cancellationToken = default
    )
    {
        return Ok(await _appHandler.Setup(data, cancellationToken));
    }

    #endregion

    #region Fields

    private readonly ILogger<UserController> _logger;
    private readonly IAppHandler _appHandler;

    #endregion
}