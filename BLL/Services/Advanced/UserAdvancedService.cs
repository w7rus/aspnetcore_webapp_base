using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common.Exceptions;
using Common.Models;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace BLL.Services.Advanced;

public interface IUserAdvancedService
{
    /// <summary>
    /// Gets entity by HttpContext authorization data
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<User> GetFromHttpContext(CancellationToken cancellationToken = default);
}

public class UserAdvancedService : IUserAdvancedService
{
    #region Fields

    private readonly ILogger<UserAdvancedService> _logger;
    private readonly IUserService _userService;
    private readonly HttpContext _httpContext;

    #endregion

    #region Ctor

    public UserAdvancedService(
        ILogger<UserAdvancedService> logger,
        IUserService userService,
        IHttpContextAccessor httpContextAccessor
    )
    {
        _logger = logger;
        _userService = userService;
        _httpContext = httpContextAccessor.HttpContext;
    }

    #endregion

    #region Methods

    public async Task<User> GetFromHttpContext(CancellationToken cancellationToken = default)
    {
        if (!Guid.TryParse(_httpContext.User.Claims.SingleOrDefault(_ => _.Type == ClaimKey.UserId)?.Value,
                out var userId))
            return null;

        var entity = await _userService.GetByIdAsync(userId, cancellationToken);

        _logger.Log(LogLevel.Information,
            Localize.Log.Method(GetType(), nameof(GetFromHttpContext), $"{entity.GetType().Name} {entity.Id}"));

        return entity;
    }

    #endregion
}