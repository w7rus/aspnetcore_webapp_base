using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BLL.Services;
using Common.Attributes;
using Common.Enums;
using Common.Models;
using Common.Options;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace API.Middleware
{
    public class AuthMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(
            HttpContext context,
            ILogger<AuthMiddleware> logger,
            IJsonWebTokenService jsonWebTokenService,
            IOptions<JsonWebTokenOptions> jsonWebTokenOptions
        )
        {
            logger.Log(LogLevel.Information, Localize.Log.MiddlewareForwardStart(GetType().AssemblyQualifiedName));

            var executingEnpoint = context.GetEndpoint();

            if (executingEnpoint == null)
                return;

            /*
             * Add authentication modes attribute checks here
             * Edit AuthMode enum to add custom auth modes
             */
            var authModeDictionary = new Dictionary<AuthMode, bool>()
            {
                {AuthMode.None, false},
                {
                    AuthMode.Default, executingEnpoint.Metadata.OfType<AuthorizeAttribute>().Any()
                                      || executingEnpoint.Metadata.OfType<AuthorizeAttribute>().Any()
                },
                {
                    AuthMode.JsonWebToken, executingEnpoint.Metadata.OfType<WrapperTypeFilterAttribute>()
                                               .Any(_ => _.TypeInfo == typeof(AuthorizeJsonWebTokenAttribute))
                                           || executingEnpoint.Metadata.OfType<WrapperTypeFilterAttribute>()
                                               .Any(_ => _.TypeInfo == typeof(AuthorizeJsonWebTokenAttribute))
                },
                {
                    AuthMode.JsonWebTokenExpired, executingEnpoint.Metadata.OfType<WrapperTypeFilterAttribute>()
                                                      .Any(_ => _.TypeInfo ==
                                                                typeof(AuthorizeExpiredJsonWebTokenAttribute))
                                                  || executingEnpoint.Metadata.OfType<WrapperTypeFilterAttribute>()
                                                      .Any(_ => _.TypeInfo ==
                                                                typeof(AuthorizeExpiredJsonWebTokenAttribute))
                }
            };

            // Strictly force only one auth mode to be used on executing endpoint
            if (authModeDictionary.Count(_ => _.Value) > 1)
            {
                context.Response.ContentType = System.Net.Mime.MediaTypeNames.Application.Json;
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsJsonAsync(new ErrorModelResult
                {
                    Errors = new List<KeyValuePair<string, string>>()
                    {
                        new(Localize.ErrorType.AuthMiddleware, Localize.Error.IncorrectAttributeConfiguration)
                    }
                });
                return;
            }

            // Get auth mode
            var authModeKeyValuePair = authModeDictionary.FirstOrDefault(_ => _.Value);
            var authMode = AuthMode.None;

            if (!authModeKeyValuePair.Equals(default(KeyValuePair<AuthMode, bool>)))
                authMode = authModeKeyValuePair.Key;

            // Skip if there's no auth
            if (authMode != AuthMode.None)
            {
                /*
                 * JsonWebToken | JsonWebTokenExpired
                 */
                if (authMode is AuthMode.JsonWebToken or AuthMode.JsonWebTokenExpired)
                {
                    var authorizationBearerPayload =
                        context.Request.Headers["Authorization"].SingleOrDefault()?.Split(" ").Last();

                    if (string.IsNullOrEmpty(authorizationBearerPayload))
                        context.Request.Cookies.TryGetValue(CookieKey.JsonWebToken, out authorizationBearerPayload);

                    if (string.IsNullOrEmpty(authorizationBearerPayload))
                    {
                        context.Response.ContentType = System.Net.Mime.MediaTypeNames.Application.Json;
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        await context.Response.WriteAsJsonAsync(new ErrorModelResult
                        {
                            Errors = new List<KeyValuePair<string, string>>()
                            {
                                new(Localize.ErrorType.Auth, Localize.Error.JsonWebTokenNotProvided)
                            }
                        });
                        return;
                    }

                    // Validate token itself using JwtSecurityTokenHandler
                    switch (authMode)
                    {
                        case AuthMode.JsonWebToken:
                        {
                            try
                            {
                                var tokenHandler = new JwtSecurityTokenHandler();
                                tokenHandler.ValidateToken(authorizationBearerPayload,
                                    new TokenValidationParameters
                                    {
                                        ValidateIssuer = jsonWebTokenOptions.Value.ValidateIssuer,
                                        ValidateAudience = jsonWebTokenOptions.Value.ValidateAudience,
                                        ValidateLifetime = jsonWebTokenOptions.Value.ValidateLifetime,
                                        ValidateIssuerSigningKey = jsonWebTokenOptions.Value.ValidateIssuerSigningKey,
                                        IssuerSigningKey =
                                            new SymmetricSecurityKey(
                                                Encoding.UTF8.GetBytes(jsonWebTokenOptions.Value.IssuerSigningKey))
                                    }, out var validatedToken);

                                var jwtToken = (JwtSecurityToken) validatedToken;

                                context.User.Identities.FirstOrDefault()
                                    ?.AddClaims(jwtToken.Claims);
                            }
                            catch (Exception)
                            {
                                context.Response.ContentType = System.Net.Mime.MediaTypeNames.Application.Json;
                                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                                await context.Response.WriteAsJsonAsync(new ErrorModelResult
                                {
                                    Errors = new List<KeyValuePair<string, string>>()
                                    {
                                        new(Localize.ErrorType.Auth, Localize.Error.JsonWebTokenValidationFailed)
                                    }
                                });
                                return;
                            }
                        }
                            break;
                        case AuthMode.JsonWebTokenExpired:
                        {
                            try
                            {
                                var tokenHandler = new JwtSecurityTokenHandler();
                                tokenHandler.ValidateToken(authorizationBearerPayload,
                                    new TokenValidationParameters
                                    {
                                        ValidateIssuer = jsonWebTokenOptions.Value.ValidateIssuer,
                                        ValidateAudience = jsonWebTokenOptions.Value.ValidateAudience,
                                        ValidateLifetime = false,
                                        ValidateIssuerSigningKey = jsonWebTokenOptions.Value.ValidateIssuerSigningKey,
                                        IssuerSigningKey =
                                            new SymmetricSecurityKey(
                                                Encoding.UTF8.GetBytes(jsonWebTokenOptions.Value.IssuerSigningKey))
                                    }, out var validatedToken);

                                var jwtToken = (JwtSecurityToken) validatedToken;

                                context.User.Identities.FirstOrDefault()
                                    ?.AddClaims(jwtToken.Claims);
                            }
                            catch (Exception)
                            {
                                context.Response.ContentType = System.Net.Mime.MediaTypeNames.Application.Json;
                                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                                await context.Response.WriteAsJsonAsync(new ErrorModelResult
                                {
                                    Errors = new List<KeyValuePair<string, string>>()
                                    {
                                        new(Localize.ErrorType.Auth, Localize.Error.JsonWebTokenValidationFailed)
                                    }
                                });
                                return;
                            }
                        }
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    var jsonWebToken = await jsonWebTokenService.GetByTokenAsync(authorizationBearerPayload);
                    if (jsonWebToken == null)
                    {
                        context.Response.ContentType = System.Net.Mime.MediaTypeNames.Application.Json;
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        await context.Response.WriteAsJsonAsync(new ErrorModelResult
                        {
                            Errors = new List<KeyValuePair<string, string>>()
                            {
                                new(Localize.ErrorType.Auth, Localize.Error.JsonWebTokenNotFound)
                            }
                        });
                        return;
                    }

                    // Validate token expiry from database (skip expired)
                    if (authMode == AuthMode.JsonWebToken)
                    {
                        if (jsonWebToken.ExpiresAt < DateTimeOffset.UtcNow)
                        {
                            context.Response.ContentType = System.Net.Mime.MediaTypeNames.Application.Json;
                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            await context.Response.WriteAsJsonAsync(new ErrorModelResult
                            {
                                Errors = new List<KeyValuePair<string, string>>()
                                {
                                    new(Localize.ErrorType.Auth, Localize.Error.JsonWebTokenExpired)
                                }
                            });
                            return;
                        }
                    }

                    context.User.Identities.FirstOrDefault()
                        ?.AddClaim(new Claim(ClaimKey.JsonWebTokenId, jsonWebToken.Id.ToString(),
                            ClaimValueTypes.String));
                }
            }

            logger.Log(LogLevel.Information, Localize.Log.MiddlewareForwardEnd(GetType().AssemblyQualifiedName));

            await _next(context);

            logger.Log(LogLevel.Information, Localize.Log.MiddlewareBackwardStart(GetType().AssemblyQualifiedName));
            logger.Log(LogLevel.Information, Localize.Log.MiddlewareBackwardEnd(GetType().AssemblyQualifiedName));
        }
    }
}