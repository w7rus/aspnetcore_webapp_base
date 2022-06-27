using API.Controllers.Base;
using BLL.Handlers;
using BLL.Services.Advanced;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class UserProfileController : CustomControllerBase
{
    #region Ctor

    public UserProfileController(
        IHttpContextAccessor httpContextAccessor,
        ILogger<UserProfileController> logger,
        IUserProfileHandler userProfileHandler,
        IWarningAdvancedService warningAdvancedService
    ) : base(httpContextAccessor, warningAdvancedService)
    {
        _logger = logger;
        _userProfileHandler = userProfileHandler;
    }

    #endregion

    #region Fields

    private readonly ILogger<UserProfileController> _logger;
    private readonly IUserProfileHandler _userProfileHandler;

    #endregion

    #region Methods
    
    //TODO: Endpoints

    #endregion
}