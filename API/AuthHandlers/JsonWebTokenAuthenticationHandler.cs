using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using BLL.Services;
using Common.Models;
using Common.Options;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;

namespace API.AuthHandlers;

public class JsonWebTokenAuthenticationHandler : AuthenticationHandler<JsonWebTokenAuthenticationSchemeOptions>
{
    private readonly JsonWebTokenAuthenticationSchemeOptions _jsonWebTokenAuthenticationSchemeOptions;
    private readonly IJsonWebTokenService _jsonWebTokenService;

    public JsonWebTokenAuthenticationHandler(
        IOptionsMonitor<JsonWebTokenAuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock,
        IOptions<JsonWebTokenOptions> jsonWebTokenOptions,
        IJsonWebTokenService jsonWebTokenService
    ) : base(options, logger, encoder, clock)
    {
        _jsonWebTokenService = jsonWebTokenService;
        _jsonWebTokenAuthenticationSchemeOptions = options.Get(AuthenticationSchemes.JsonWebToken);
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var executingEndpoint = Context.GetEndpoint();

        if (executingEndpoint == null)
            return AuthenticateResult.Fail(new NullReferenceException(nameof(executingEndpoint)));

        if (executingEndpoint.Metadata.OfType<AllowAnonymousAttribute>().Any()
            || executingEndpoint.Metadata.OfType<AllowAnonymousAttribute>().Any())
            return AuthenticateResult.Success(new AuthenticationTicket(new ClaimsPrincipal(), Scheme.Name));

        var authorizationBearerPayload =
            Context.Request.Headers[HeaderNames.Authorization].SingleOrDefault()?.Split(" ").Last();

        if (string.IsNullOrEmpty(authorizationBearerPayload))
            Context.Request.Cookies.TryGetValue(CookieKey.JsonWebToken, out authorizationBearerPayload);

        if (string.IsNullOrEmpty(authorizationBearerPayload))
            return AuthenticateResult.Fail(Localize.Error.JsonWebTokenNotProvided);

        var claims = new List<Claim>();

        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            tokenHandler.ValidateToken(authorizationBearerPayload,
                _jsonWebTokenAuthenticationSchemeOptions.TokenValidationParameters,
                out var validatedToken);

            var jwtToken = (JwtSecurityToken) validatedToken;

            claims.AddRange(jwtToken.Claims);
        }
        catch (Exception e)
        {
            return AuthenticateResult.Fail(Localize.Error.JsonWebTokenValidationFailed);
        }

        var jsonWebToken = await _jsonWebTokenService.GetByTokenAsync(authorizationBearerPayload);
        if (jsonWebToken == null)
            return AuthenticateResult.Fail(Localize.Error.JsonWebTokenNotFound);

        if (jsonWebToken.ExpiresAt < DateTimeOffset.UtcNow)
        {
            return AuthenticateResult.Fail(Localize.Error.JsonWebTokenExpired);
        }

        claims.Add(new Claim(ClaimKey.JsonWebTokenId, jsonWebToken.Id.ToString(),
            ClaimValueTypes.String));

        var claimsIdentity = new ClaimsIdentity(claims, nameof(JsonWebTokenAuthenticationHandler));
        var ticket = new AuthenticationTicket(new ClaimsPrincipal(claimsIdentity), Scheme.Name);

        return AuthenticateResult.Success(ticket);
    }
}