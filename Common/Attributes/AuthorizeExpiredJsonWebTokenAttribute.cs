using Common.Options;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;

namespace Common.Attributes;

/// <summary>
///     Attribute to mark Endpoint to verify that requester had provided valid JsonWebToken but expired.
///     Do not try to use this attribute on a controller since its not implemented yet!
/// </summary>
public class AuthorizeExpiredJsonWebTokenAttribute : AuthorizeAttribute, IAuthorizationFilter
{
    private readonly JsonWebTokenOptions _jsonWebTokenOptions;

    public AuthorizeExpiredJsonWebTokenAttribute(IOptions<JsonWebTokenOptions> jsonWebTokenOptions)
    {
        _jsonWebTokenOptions = jsonWebTokenOptions.Value;
    }

    public AuthorizeExpiredJsonWebTokenAttribute(string policy, IOptions<JsonWebTokenOptions> jsonWebTokenOptions) :
        base(policy)
    {
        _jsonWebTokenOptions = jsonWebTokenOptions.Value;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        // var httpRequest = context.HttpContext.Request;
        //
        // var authorizationBearerPayload = httpRequest.Headers["Authorization"].SingleOrDefault()?.Split(" ").Last();
        //
        // if (string.IsNullOrEmpty(authorizationBearerPayload))
        // {
        //     context.Result = new UnauthorizedResult();
        //     return;
        // }
        //
        // if (string.IsNullOrEmpty(Policy))
        // {
        //     context.Result = new UnauthorizedResult();
        //     return;
        // }
        //
        // var authenticationHeaderValue = new AuthenticationHeaderValue(Policy, authorizationBearerPayload);
        //
        // try
        // {
        //     var tokenHandler = new JwtSecurityTokenHandler();
        //     tokenHandler.ValidateToken(authenticationHeaderValue.Parameter,
        //         new TokenValidationParameters
        //         {
        //             ValidateIssuer = _jsonWebTokenOptions.ValidateIssuer,
        //             ValidateAudience = _jsonWebTokenOptions.ValidateAudience,
        //             ValidateLifetime = false,
        //             ValidateIssuerSigningKey = _jsonWebTokenOptions.ValidateIssuerSigningKey,
        //             IssuerSigningKey =
        //                 new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jsonWebTokenOptions.IssuerSigningKey))
        //         }, out var validatedToken);
        //
        //     var jwtToken = (JwtSecurityToken) validatedToken;
        //     
        //     context.HttpContext.User.Identities.FirstOrDefault()
        //         ?.AddClaims(jwtToken.Claims);
        // }
        // catch (Exception)
        // {
        //     context.Result = new UnauthorizedResult();
        // }
    }
}