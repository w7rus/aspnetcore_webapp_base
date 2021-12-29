using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using Common.Models;
using Common.Options;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Common.Attributes
{
    /// <summary>
    /// Attribute to mark Endpoint to verify that requester had provided valid JsonWebToken.
    /// Do not try to use this attribute on a controller since its not implemented yet!
    /// </summary>
    public class AuthorizeJsonWebTokenAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        private readonly JsonWebTokenOptions _jsonWebTokenOptions;

        public AuthorizeJsonWebTokenAttribute(IOptions<JsonWebTokenOptions> jsonWebTokenOptions)
        {
            _jsonWebTokenOptions = jsonWebTokenOptions.Value;
        }

        public AuthorizeJsonWebTokenAttribute(string policy, IOptions<JsonWebTokenOptions> jsonWebTokenOptions) :
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
            //             ValidateLifetime = true,
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
}