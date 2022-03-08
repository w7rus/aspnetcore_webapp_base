using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common.Models;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace BLL.Services.Advanced;

public interface IJsonWebTokenAdvancedService
{
    /// <summary>
    /// Gets entity by HttpContext authorization data
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<JsonWebToken> GetFromHttpContext(CancellationToken cancellationToken = default);
}

public class JsonWebTokenAdvancedService : IJsonWebTokenAdvancedService
{
    #region Fields

    private readonly ILogger<JsonWebTokenAdvancedService> _logger;
    private readonly IJsonWebTokenService _jsonWebTokenService;
    private readonly HttpContext _httpContext;

    #endregion

    #region Ctor

    public JsonWebTokenAdvancedService(
        ILogger<JsonWebTokenAdvancedService> logger,
        IJsonWebTokenService jsonWebTokenService,
        IHttpContextAccessor httpContextAccessor
    )
    {
        _logger = logger;
        _jsonWebTokenService = jsonWebTokenService;
        _httpContext = httpContextAccessor.HttpContext;
    }

    #endregion

    #region Methods

    public async Task<JsonWebToken> GetFromHttpContext(CancellationToken cancellationToken = default)
    {
        if (!Guid.TryParse(_httpContext.User.Claims.SingleOrDefault(_ => _.Type == ClaimKey.JsonWebTokenId)?.Value,
                out var jsonWebTokenId))
            throw new ApplicationException(Localize.Error.JsonWebTokenIdRetrievalFailed);

        return await _jsonWebTokenService.GetByIdAsync(jsonWebTokenId, cancellationToken);
    }

    #endregion
}