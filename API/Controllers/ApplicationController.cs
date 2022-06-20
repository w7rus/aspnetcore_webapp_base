using API.Controllers.Base;
using BLL.Handlers;
using BLL.Services.Advanced;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

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
    
    //TODO: Root user account claim

    #endregion
}