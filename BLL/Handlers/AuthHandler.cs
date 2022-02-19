using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using BLL.Handlers.Base;
using BLL.Services;
using BLL.Services.Advanced;
using Common.Attributes;
using Common.Exceptions;
using Common.Helpers;
using Common.Models;
using Common.Models.Base;
using Common.Options;
using DAL.Data;
using Domain.Entities;
using DTO.Models.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BLL.Handlers;

public interface IAuthHandler
{
    Task<DTOResultBase> SignUp(AuthSignUp data, CancellationToken cancellationToken = default);
    Task<DTOResultBase> SignInViaEmail(AuthSignInViaEmail data, CancellationToken cancellationToken = default);
    Task<DTOResultBase> Refresh(AuthRefresh data, CancellationToken cancellationToken = default);
    Task<DTOResultBase> SignOut(AuthSignOut data, CancellationToken cancellationToken = default);
}

public class AuthHandler : HandlerBase, IAuthHandler
{
    #region Fields

    private readonly string _fullName;
    private readonly ILogger<AuthHandler> _logger;
    private readonly IAppDbContextAction _appDbContextAction;
    private readonly IJsonWebTokenService _jsonWebTokenService;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IUserService _userService;
    private readonly RefreshTokenOptions _refreshTokenOptions;
    private readonly JsonWebTokenOptions _jsonWebTokenOptions;
    private readonly HttpContext _httpContext;
    private readonly MiscOptions _miscOptions;
    private readonly IUserToUserGroupMappingService _userToUserGroupMappingService;
    private readonly IUserGroupService _userGroupService;

    #endregion

    #region Ctor

    public AuthHandler(
        ILogger<AuthHandler> logger,
        IAppDbContextAction appDbContextAction,
        IJsonWebTokenService jsonWebTokenService,
        IRefreshTokenService refreshTokenService,
        IUserService userService,
        IOptions<RefreshTokenOptions> refreshTokenOptions,
        IOptions<JsonWebTokenOptions> jsonWebTokenOptions,
        IHttpContextAccessor httpContextAccessor,
        IOptions<MiscOptions> miscOptions,
        IUserToUserGroupMappingService userToUserGroupMappingService,
        IUserGroupService userGroupService
    )
    {
        _fullName = GetType().FullName;
        _logger = logger;
        _appDbContextAction = appDbContextAction;
        _jsonWebTokenService = jsonWebTokenService;
        _refreshTokenService = refreshTokenService;
        _userService = userService;
        _userToUserGroupMappingService = userToUserGroupMappingService;
        _userGroupService = userGroupService;
        _refreshTokenOptions = refreshTokenOptions.Value;
        _jsonWebTokenOptions = jsonWebTokenOptions.Value;
        _httpContext = httpContextAccessor.HttpContext;
        _miscOptions = miscOptions.Value;
    }

    #endregion

    #region Methods

    public async Task<DTOResultBase> SignUp(AuthSignUp data, CancellationToken cancellationToken = default)
    {
        _logger.Log(LogLevel.Information, Localize.Log.MethodStart(_fullName, nameof(SignUp)));

        if (ValidateModel(data) is { } validationResult)
            return validationResult;

        try
        {
            await _appDbContextAction.BeginTransactionAsync();

            var customPasswordHasher = new CustomPasswordHasher();

            var passwordHashed = customPasswordHasher.HashPassword(data.Password);

            var user = await _userService.Create(new User
            {
                Email = data.Email,
                Password = passwordHashed
            }, cancellationToken);
            
            _logger.Log(LogLevel.Information, Localize.Log.Method(_fullName, nameof(SignUp), $"Staged creation of {user.GetType().Name} {user.Id}"));

            var guestUserGroup = await _userGroupService.GetByAliasAsync("Member");

            var userToUserGroupMapping = await _userToUserGroupMappingService.Create(new UserToUserGroupMapping
            {
                EntityId = user.Id,
                GroupId = guestUserGroup.Id,
            }, cancellationToken);
            
            _logger.Log(LogLevel.Information, Localize.Log.Method(_fullName, nameof(SignUp), $"Staged creation of {userToUserGroupMapping.GetType().Name} {userToUserGroupMapping.Id}"));

            await _appDbContextAction.CommitTransactionAsync();

            _logger.Log(LogLevel.Information, Localize.Log.MethodEnd(_fullName, nameof(SignUp)));

            return new AuthSignUpResult
            {
                UserId = user.Id
            };
        }
        catch (Exception e)
        {
            await _appDbContextAction.RollbackTransactionAsync();
            _logger.Log(LogLevel.Error, Localize.Log.MethodError(_fullName, nameof(SignUp), e.Message));

            var errorModelResult = new ErrorModelResult
            {
                Errors = new List<KeyValuePair<string, string>>
                {
                    new(Localize.ErrorType.Auth, Localize.Error.AuthSignUpFailed)
                }
            };

            if (e is CustomException)
                errorModelResult.Errors.Add(new KeyValuePair<string, string>(Localize.ErrorType.Auth, e.Message));

            return errorModelResult;
        }
    }

    public async Task<DTOResultBase> SignInViaEmail(
        AuthSignInViaEmail data,
        CancellationToken cancellationToken = default
    )
    {
        _logger.Log(LogLevel.Information, Localize.Log.MethodStart(_fullName, nameof(SignInViaEmail)));

        if (ValidateModel(data) is { } validationResult)
            return validationResult;

        var warnings = new List<KeyValuePair<string, string>>();

        try
        {
            await _appDbContextAction.BeginTransactionAsync();

            var customPasswordHasher = new CustomPasswordHasher();

            var user = await _userService.GetByEmailAsync(data.Email);
            if (user == null || !customPasswordHasher.VerifyPassword(user.Password, data.Password))
                throw new CustomException();
            
            _logger.Log(LogLevel.Information, Localize.Log.Method(_fullName, nameof(SignInViaEmail), $"Received {user.GetType().Name} {user.Id}"));

            var refreshTokenString = Utilities.GenerateRandomBase64String(256);
            var refreshTokenExpiresAt =
                data.RefreshTokenExpireAt ??
                DateTimeOffset.UtcNow.AddSeconds(_refreshTokenOptions.DefaultExpirySeconds);
            var refreshToken = await _refreshTokenService.Create(new RefreshToken
            {
                Token = refreshTokenString,
                ExpiresAt = refreshTokenExpiresAt,
                UserId = user.Id
            }, cancellationToken);
            
            _logger.Log(LogLevel.Information, Localize.Log.Method(_fullName, nameof(SignInViaEmail), $"Staged creation of {refreshToken.GetType().Name} {refreshToken.Id}"));

            var jsonWebTokenExpiresAt = DateTimeOffset.UtcNow.AddSeconds(_jsonWebTokenOptions.ExpirySeconds);
            var jsonWebTokenString = _jsonWebTokenService.CreateWithClaims(_jsonWebTokenOptions.IssuerSigningKey,
                _jsonWebTokenOptions.Issuer, _jsonWebTokenOptions.Audience, new List<Claim>
                {
                    new(ClaimKey.UserId, user.Id.ToString(), ClaimValueTypes.String),
                }, jsonWebTokenExpiresAt.UtcDateTime);
            var jsonWebToken = await _jsonWebTokenService.Create(new JsonWebToken
                {
                    Token = jsonWebTokenString,
                    ExpiresAt = jsonWebTokenExpiresAt,
                    DeleteAfter = refreshTokenExpiresAt,
                    UserId = user.Id
                },
                cancellationToken);
            
            _logger.Log(LogLevel.Information, Localize.Log.Method(_fullName, nameof(SignInViaEmail), $"Staged creation of {jsonWebToken.GetType().Name} {jsonWebToken.Id}"));

            if (data.UseCookies)
            {
                _logger.Log(LogLevel.Information, Localize.Log.Method(_fullName, nameof(SignInViaEmail), $"Client requested to use cookies"));
                
                var refreshTokenCookieOptions = new CookieOptions
                {
                    Expires = refreshTokenExpiresAt,
                    Secure = _miscOptions.SecureCookies,
                    SameSite = SameSiteMode.Strict,
                    HttpOnly = true,
                };
                var jsonWebTokenCookieOptions = new CookieOptions
                {
                    Expires = refreshTokenExpiresAt,
                    Secure = _miscOptions.SecureCookies,
                    SameSite = SameSiteMode.Strict,
                    HttpOnly = true,
                };

                _httpContext.Response.Cookies.Append(CookieKey.RefreshToken, refreshTokenString,
                    refreshTokenCookieOptions);
                _httpContext.Response.Cookies.Append(CookieKey.JsonWebToken, jsonWebTokenString,
                    jsonWebTokenCookieOptions);
            }
            else
            {
                warnings.Add(new KeyValuePair<string, string>(Localize.WarningType.Auth, Localize.Warning.XssVulnerable));
            }

            await _appDbContextAction.CommitTransactionAsync();

            _logger.Log(LogLevel.Information, Localize.Log.MethodEnd(_fullName, nameof(SignInViaEmail)));

            return new AuthSignInResult
            {
                UserId = user.Id,
                JsonWebToken = !data.UseCookies ? jsonWebTokenString : null,
                JsonWebTokenExpiresAt = jsonWebTokenExpiresAt,
                RefreshToken = !data.UseCookies ? refreshTokenString : null,
                RefreshTokenExpiresAt = refreshTokenExpiresAt,
                Warnings = warnings
            };
        }
        catch (Exception e)
        {
            await _appDbContextAction.RollbackTransactionAsync();
            _logger.Log(LogLevel.Error, Localize.Log.MethodError(_fullName, nameof(SignInViaEmail), e.Message));

            var errorModelResult = new ErrorModelResult
            {
                Errors = new List<KeyValuePair<string, string>>
                {
                    new(Localize.ErrorType.Auth, Localize.Error.AuthSignInFailed)
                }
            };

            if (e is CustomException)
                errorModelResult.Errors.Add(new KeyValuePair<string, string>(Localize.ErrorType.Auth, e.Message));

            return errorModelResult;
        }
    }

    public async Task<DTOResultBase> Refresh(AuthRefresh data, CancellationToken cancellationToken = default)
    {
        _logger.Log(LogLevel.Information, Localize.Log.MethodStart(_fullName, nameof(Refresh)));

        if (ValidateModel(data) is { } validationResult)
            return validationResult;
        
        var warnings = new List<KeyValuePair<string, string>>();

        try
        {
            await _appDbContextAction.BeginTransactionAsync();

            var useCookies = data.RefreshToken == null;
            
            if (useCookies)
                _logger.Log(LogLevel.Information, Localize.Log.Method(_fullName, nameof(Refresh), $"Client requested to use cookies"));

            data.RefreshToken ??=
                _httpContext.Request.Cookies.SingleOrDefault(_ => _.Key == CookieKey.RefreshToken).Value;
            if (data.RefreshToken == null)
                throw new CustomException(Localize.Error.RefreshTokenNotProvided);
            
            _logger.Log(LogLevel.Information, Localize.Log.Method(_fullName, nameof(Refresh), $"{nameof(data.RefreshToken)} presented {data.RefreshToken}"));

            var refreshToken = await _refreshTokenService.GetByTokenAsync(data.RefreshToken);
            if (refreshToken == null)
                throw new CustomException(Localize.Error.RefreshTokenNotFound);
            
            _logger.Log(LogLevel.Information, Localize.Log.Method(_fullName, nameof(Refresh), $"Received {refreshToken.GetType().Name} {refreshToken.Id}"));

            if (refreshToken.ExpiresAt < DateTimeOffset.UtcNow)
                throw new CustomException(Localize.Error.RefreshTokenExpired);
            
            _logger.Log(LogLevel.Information, Localize.Log.Method(_fullName, nameof(Refresh), $"{refreshToken.GetType().Name} is valid until {refreshToken.ExpiresAt}"));

            _logger.Log(LogLevel.Information, Localize.Log.Method(_fullName, nameof(Refresh), $"Staging deletion of {refreshToken.GetType().Name} {refreshToken.Id}"));
            await _refreshTokenService.Delete(refreshToken, cancellationToken);

            var jsonWebToken = await _jsonWebTokenService.GetFromHttpContext(cancellationToken);
            if (jsonWebToken == null)
                throw new CustomException(Localize.Error.JsonWebTokenNotFound);
            
            _logger.Log(LogLevel.Information, Localize.Log.Method(_fullName, nameof(Refresh), $"Received {jsonWebToken.GetType().Name} {jsonWebToken.Id}"));

            _logger.Log(LogLevel.Information, Localize.Log.Method(_fullName, nameof(Refresh), $"Staging deletion of {jsonWebToken.GetType().Name} {jsonWebToken.Id}"));
            await _jsonWebTokenService.Delete(jsonWebToken, cancellationToken);

            var user = await _userService.GetFromHttpContext(cancellationToken);
            if (user == null)
                throw new CustomException();
            
            _logger.Log(LogLevel.Information, Localize.Log.Method(_fullName, nameof(Refresh), $"Received {user.GetType().Name} {user.Id}"));
            
            var refreshTokenString = Utilities.GenerateRandomBase64String(256);
            var refreshTokenExpiresAt =
                data.RefreshTokenExpireAt ??
                DateTimeOffset.UtcNow.AddSeconds(_refreshTokenOptions.DefaultExpirySeconds);
            refreshToken = await _refreshTokenService.Create(new RefreshToken
            {
                Token = refreshTokenString,
                ExpiresAt = refreshTokenExpiresAt,
                UserId = user.Id
            }, cancellationToken);
            
            _logger.Log(LogLevel.Information, Localize.Log.Method(_fullName, nameof(Refresh), $"Staged creation of {refreshToken.GetType().Name} {refreshToken.Id}"));

            var jsonWebTokenExpiresAt = DateTimeOffset.UtcNow.AddSeconds(_jsonWebTokenOptions.ExpirySeconds);
            var jsonWebTokenString = _jsonWebTokenService.CreateWithClaims(_jsonWebTokenOptions.IssuerSigningKey,
                _jsonWebTokenOptions.Issuer, _jsonWebTokenOptions.Audience, new List<Claim>
                {
                    new(ClaimKey.UserId, user.Id.ToString(), ClaimValueTypes.String),
                }, jsonWebTokenExpiresAt.UtcDateTime);
            jsonWebToken = await _jsonWebTokenService.Create(new JsonWebToken
                {
                    Token = jsonWebTokenString,
                    ExpiresAt = jsonWebTokenExpiresAt,
                    DeleteAfter = refreshTokenExpiresAt,
                    UserId = user.Id
                },
                cancellationToken);
            
            _logger.Log(LogLevel.Information, Localize.Log.Method(_fullName, nameof(Refresh), $"Staged creation of {jsonWebToken.GetType().Name} {jsonWebToken.Id}"));

            if (useCookies)
            {
                var refreshTokenCookieOptions = new CookieOptions
                {
                    Expires = refreshTokenExpiresAt,
                    Secure = _miscOptions.SecureCookies,
                    SameSite = SameSiteMode.Strict,
                    HttpOnly = true,
                };
                var jsonWebTokenCookieOptions = new CookieOptions
                {
                    Expires = refreshTokenExpiresAt,
                    Secure = _miscOptions.SecureCookies,
                    SameSite = SameSiteMode.Strict,
                    HttpOnly = true,
                };

                _httpContext.Response.Cookies.Append(CookieKey.RefreshToken, refreshTokenString,
                    refreshTokenCookieOptions);
                _httpContext.Response.Cookies.Append(CookieKey.JsonWebToken, jsonWebTokenString,
                    jsonWebTokenCookieOptions);
            }
            else
            {
                warnings.Add(new KeyValuePair<string, string>(Localize.WarningType.Auth, Localize.Warning.XssVulnerable));
            }

            await _appDbContextAction.CommitTransactionAsync();

            _logger.Log(LogLevel.Information, Localize.Log.MethodEnd(_fullName, nameof(Refresh)));

            return new AuthRefreshResult()
            {
                UserId = user.Id,
                JsonWebToken = !useCookies ? jsonWebTokenString : null,
                JsonWebTokenExpiresAt = jsonWebTokenExpiresAt,
                RefreshToken = !useCookies ? refreshTokenString : null,
                RefreshTokenExpiresAt = refreshTokenExpiresAt,
                Warnings = warnings
            };
        }
        catch (Exception e)
        {
            await _appDbContextAction.RollbackTransactionAsync();
            _logger.Log(LogLevel.Error, Localize.Log.MethodError(_fullName, nameof(Refresh), e.Message));

            var errorModelResult = new ErrorModelResult
            {
                Errors = new List<KeyValuePair<string, string>>
                {
                    new(Localize.ErrorType.Auth, Localize.Error.AuthRefreshFailed)
                }
            };

            if (e is CustomException)
                errorModelResult.Errors.Add(new KeyValuePair<string, string>(Localize.ErrorType.Auth, e.Message));

            return errorModelResult;
        }
    }

    public async Task<DTOResultBase> SignOut(AuthSignOut data, CancellationToken cancellationToken = default)
    {
        _logger.Log(LogLevel.Information, Localize.Log.MethodStart(_fullName, nameof(SignOut)));

        if (ValidateModel(data) is { } validationResult)
            return validationResult;

        try
        {
            await _appDbContextAction.BeginTransactionAsync();

            var useCookies = data.RefreshToken == null;
            
            if (useCookies)
                _logger.Log(LogLevel.Information, Localize.Log.Method(_fullName, nameof(SignOut), $"Client requested to use cookies"));

            data.RefreshToken ??=
                _httpContext.Request.Cookies.SingleOrDefault(_ => _.Key == CookieKey.RefreshToken).Value;
            if (data.RefreshToken == null)
                throw new CustomException(Localize.Error.RefreshTokenNotProvided);
            
            _logger.Log(LogLevel.Information, Localize.Log.Method(_fullName, nameof(SignOut), $"RefreshToken presented {data.RefreshToken}"));

            var refreshToken = await _refreshTokenService.GetByTokenAsync(data.RefreshToken);
            if (refreshToken == null)
                throw new CustomException(Localize.Error.RefreshTokenNotFound);
            
            _logger.Log(LogLevel.Information, Localize.Log.Method(_fullName, nameof(SignOut), $"Received RefreshToken {refreshToken.Id}"));

            if (refreshToken.ExpiresAt < DateTimeOffset.UtcNow)
                throw new CustomException(Localize.Error.RefreshTokenExpired);
            
            _logger.Log(LogLevel.Information, Localize.Log.Method(_fullName, nameof(SignOut), $"RefreshToken is valid until {refreshToken.ExpiresAt}"));

            _logger.Log(LogLevel.Information, Localize.Log.Method(_fullName, nameof(SignOut), $"Staging deletion of RefreshToken {refreshToken.Id}"));
            await _refreshTokenService.Delete(refreshToken, cancellationToken);

            var jsonWebToken = await _jsonWebTokenService.GetFromHttpContext();
            if (jsonWebToken == null)
                throw new CustomException(Localize.Error.JsonWebTokenNotFound);
            
            _logger.Log(LogLevel.Information, Localize.Log.Method(_fullName, nameof(SignOut), $"Received {jsonWebToken.GetType().Name} {jsonWebToken.Id}"));

            // Middleware already validated this case
            // if (jsonWebToken.ExpiresAt < DateTimeOffset.UtcNow)
            //     throw new CustomException(Localize.Error.JsonWebTokenExpired);

            _logger.Log(LogLevel.Information, Localize.Log.Method(_fullName, nameof(SignOut), $"Staging deletion of {jsonWebToken.GetType().Name} {jsonWebToken.Id}"));
            await _jsonWebTokenService.Delete(jsonWebToken, cancellationToken);

            var user = await _userService.GetFromHttpContext(cancellationToken);
            if (user == null)
                throw new CustomException();
            
            _logger.Log(LogLevel.Information, Localize.Log.Method(_fullName, nameof(SignOut), $"Received {user.GetType().Name} {user.Id}"));

            if (useCookies)
            {
                _httpContext.Response.Cookies.Delete(CookieKey.JsonWebToken);
                _httpContext.Response.Cookies.Delete(CookieKey.RefreshToken);
            }

            await _appDbContextAction.CommitTransactionAsync();

            _logger.Log(LogLevel.Information, Localize.Log.MethodEnd(_fullName, nameof(SignOut)));

            return new AuthSignOutResult();
        }
        catch (Exception e)
        {
            await _appDbContextAction.RollbackTransactionAsync();
            _logger.Log(LogLevel.Error, Localize.Log.MethodError(_fullName, nameof(SignOut), e.Message));

            var errorModelResult = new ErrorModelResult
            {
                Errors = new List<KeyValuePair<string, string>>
                {
                    new(Localize.ErrorType.Auth, Localize.Error.AuthSignOutFailed)
                }
            };

            if (e is CustomException)
                errorModelResult.Errors.Add(new KeyValuePair<string, string>(Localize.ErrorType.Auth, e.Message));

            return errorModelResult;
        }
    }

    #endregion
}