using API.Controllers.Base;
using BLL.Handlers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class UserGroupController : CustomControllerBase
{
    #region Fields

    private readonly ILogger<UserGroupController> _logger;
    private readonly IUserGroupHandler _userGroupHandler;

    #endregion

    #region Ctor

    public UserGroupController(
        IHttpContextAccessor httpContextAccessor,
        ILogger<UserGroupController> logger,
        IUserGroupHandler userGroupHandler
    ) : base(httpContextAccessor)
    {
        _logger = logger;
        _userGroupHandler = userGroupHandler;
    }

    #endregion

    #region Methods

    #endregion
}