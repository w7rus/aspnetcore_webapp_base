using API.Controllers.Base;
using BLL.Handlers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class UserProfileController : CustomControllerBase
{
    #region Fields

    private readonly ILogger<UserProfileController> _logger;
    private readonly IUserProfileHandler _userProfileHandler;

    #endregion

    #region Ctor

    public UserProfileController(
        IHttpContextAccessor httpContextAccessor,
        ILogger<UserProfileController> logger,
        IUserProfileHandler userProfileHandler
    ) : base(httpContextAccessor)
    {
        _logger = logger;
        _userProfileHandler = userProfileHandler;
    }

    #endregion

    #region Methods

    #endregion
}