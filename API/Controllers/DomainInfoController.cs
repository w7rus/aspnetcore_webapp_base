using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using API.Controllers.Base;
using BLL.Handlers;
using BLL.Services;
using BLL.Services.Advanced;
using DTO.Models.DomainInfo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using Common.Models;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class DomainInfoController : CustomControllerBase
{
    #region Fields

    private readonly ILogger<DomainInfoController> _logger;
    private readonly IDomainInfoHandler _domainInfoHandler;

    #endregion

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

    #region Methods
    
    [HttpGet]
    [SwaggerOperation(Summary = "Reads Domain Info (Assembly properties with value types)",
        Description = "Reads Domain Info (Assembly properties with value types)")]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.JsonWebToken)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult Read(
        [Required] [FromQuery] DomainInfoReadDto data
    )
    {
        return ResponseWith(_domainInfoHandler.Read(data));
    }
    
    [HttpGet]
    [Route("assemblyQualifiedNames")]
    [SwaggerOperation(Summary = "Reads Domain Info (Available assembly qualified names under Domain.Entities)",
        Description = "Reads Domain Info (Available assembly qualified names under Domain.Entities)")]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.JsonWebToken)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult ReadAssemblyQualifiedNames()
    {
        return ResponseWith(_domainInfoHandler.ReadAssemblyQualifiedNames());
    }

    #endregion
}