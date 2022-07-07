using System.ComponentModel.DataAnnotations;
using API.Controllers.Base;
using BLL.Handlers.App;
using BLL.Services.Advanced;
using Common.Models;
using DTO.Models.DomainInfo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace API.Controllers.App;

[ApiController]
[Route("[controller]")]
public class DomainInfoController : CustomControllerBase
{
    #region Ctor

    public DomainInfoController(
        IHttpContextAccessor httpContextAccessor,
        ILogger<DomainInfoController> logger,
        IDomainInfoHandler domainInfoHandler,
        IWarningAdvancedService warningAdvancedService
    ) : base(httpContextAccessor, warningAdvancedService)
    {
        _logger = logger;
        _domainInfoHandler = domainInfoHandler;
    }

    #endregion

    #region Fields

    private readonly ILogger<DomainInfoController> _logger;
    private readonly IDomainInfoHandler _domainInfoHandler;

    #endregion

    #region Methods

    [HttpGet]
    [Route(nameof(Read))]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.JsonWebToken)]
    [ProducesResponseType(typeof(DomainInfoReadResultDto), StatusCodes.Status200OK)]
    public IActionResult Read(
        [Required] [FromQuery] DomainInfoReadDto data
    )
    {
        return ResponseWith(_domainInfoHandler.Read(data));
    }

    [HttpGet]
    [Route(nameof(ReadAssemblyQualifiedNames))]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.JsonWebToken)]
    [ProducesResponseType(typeof(DomainInfoReadAssemblyQualifiedNamesResultDto), StatusCodes.Status200OK)]
    public IActionResult ReadAssemblyQualifiedNames()
    {
        return ResponseWith(_domainInfoHandler.ReadAssemblyQualifiedNames());
    }

    #endregion
}