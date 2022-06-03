using API.Controllers.Base;
using BLL.Handlers;
using Common.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : CustomControllerBase
{
    #region Fields

    private readonly ILogger<UserController> _logger;
    private readonly IUserHandler _userHandler;

    #endregion

    #region Ctor

    public UserController(
        IHttpContextAccessor httpContextAccessor,
        ILogger<UserController> logger,
        IUserHandler userHandler
    ) : base(
        httpContextAccessor)
    {
        _logger = logger;
        _userHandler = userHandler;
    }

    #endregion

    #region Methods

    #endregion
}