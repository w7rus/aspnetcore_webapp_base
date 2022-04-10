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
using Common.Enums;
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
    Task<DTOResultBase> SignUpInAsGuest(AuthSignUpInAsGuest data, CancellationToken cancellationToken = default);
    Task<DTOResultBase> SignUp(AuthSignUp data, CancellationToken cancellationToken = default);
    Task<DTOResultBase> SignIn(AuthSignIn data, CancellationToken cancellationToken = default);
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
    private readonly IUserAdvancedService _userAdvancedService;
    private readonly IJsonWebTokenAdvancedService _jsonWebTokenAdvancedService;

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
        IUserGroupService userGroupService,
        IUserAdvancedService userAdvancedService,
        IJsonWebTokenAdvancedService jsonWebTokenAdvancedService
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
        _userAdvancedService = userAdvancedService;
        _jsonWebTokenAdvancedService = jsonWebTokenAdvancedService;
        _refreshTokenOptions = refreshTokenOptions.Value;
        _jsonWebTokenOptions = jsonWebTokenOptions.Value;
        _httpContext = httpContextAccessor.HttpContext;
        _miscOptions = miscOptions.Value;
    }

    #endregion

    #region Methods
    
    public async Task<DTOResultBase> SignUpInAsGuest(AuthSignUpInAsGuest data, CancellationToken cancellationToken = default)
    {
        _logger.Log(LogLevel.Information, Localize.Log.MethodStart(GetType(), nameof(SignUpInAsGuest)));

        if (ValidateModel(data) is { } validationResult)
            return validationResult;
        
        var warnings = new List<WarningModelResultEntry>();
        
        try
        {
            await _appDbContextAction.BeginTransactionAsync();

            var user = await _userService.Create(new User
            {
                Email = null,
                Password = null,
                IsTemporary = true
            }, cancellationToken);

            //TODO: Create Guest UserGroup
            var guestUserGroup = await _userGroupService.GetByAliasAsync("Guest");

            var userToUserGroupMapping = await _userToUserGroupMappingService.Create(new UserToUserGroupMapping
            {
                EntityLeftId = user.Id,
                EntityRightId = guestUserGroup.Id,
            }, cancellationToken);
            
            var refreshTokenString = Utilities.GenerateRandomBase64String(256);
            var refreshTokenExpiresAt =
                data.RefreshTokenExpireAt ??
                DateTimeOffset.UtcNow.AddSeconds(_refreshTokenOptions.DefaultExpirySeconds);
            await _refreshTokenService.Create(new RefreshToken
            {
                Token = refreshTokenString,
                ExpiresAt = refreshTokenExpiresAt,
                UserId = user.Id
            }, cancellationToken);

            var jsonWebTokenExpiresAt = DateTimeOffset.UtcNow.AddSeconds(_jsonWebTokenOptions.ExpirySeconds);
            var jsonWebTokenString = _jsonWebTokenService.CreateWithClaims(_jsonWebTokenOptions.IssuerSigningKey,
                _jsonWebTokenOptions.Issuer, _jsonWebTokenOptions.Audience, new List<Claim>
                {
                    new(ClaimKey.UserId, user.Id.ToString(), ClaimValueTypes.String),
                }, jsonWebTokenExpiresAt.UtcDateTime);
            await _jsonWebTokenService.Create(new JsonWebToken
                {
                    Token = jsonWebTokenString,
                    ExpiresAt = jsonWebTokenExpiresAt,
                    DeleteAfter = refreshTokenExpiresAt,
                    UserId = user.Id
                },
                cancellationToken);
            
            if (data.UseCookies)
            {
                _logger.Log(LogLevel.Information,
                    Localize.Log.Method(GetType(), nameof(SignIn), $"Client requested to use cookies"));

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
                warnings.Add(new WarningModelResultEntry(WarningType.Security, Localize.Warning.XssVulnerable));
            }

            user.LastSignIn = DateTimeOffset.UtcNow;
            
            await _userService.Save(user, cancellationToken);

            await _appDbContextAction.CommitTransactionAsync();

            _logger.Log(LogLevel.Information, Localize.Log.MethodEnd(GetType(), nameof(SignUpInAsGuest)));

            return new AuthSignUpResult
            {
                UserId = user.Id
            };
        }
        catch (Exception)
        {
            await _appDbContextAction.RollbackTransactionAsync();

            throw;
        }
    }

    public async Task<DTOResultBase> SignUp(AuthSignUp data, CancellationToken cancellationToken = default)
    {
        _logger.Log(LogLevel.Information, Localize.Log.MethodStart(GetType(), nameof(SignUp)));

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

            var memberUserGroup = await _userGroupService.GetByAliasAsync("Member");

            var userToUserGroupMapping = await _userToUserGroupMappingService.Create(new UserToUserGroupMapping
            {
                EntityLeftId = user.Id,
                EntityRightId = memberUserGroup.Id,
            }, cancellationToken);

            await _appDbContextAction.CommitTransactionAsync();

            _logger.Log(LogLevel.Information, Localize.Log.MethodEnd(GetType(), nameof(SignUp)));

            return new AuthSignUpResult
            {
                UserId = user.Id
            };
        }
        catch (Exception)
        {
            await _appDbContextAction.RollbackTransactionAsync();

            throw;
        }
    }

    public async Task<DTOResultBase> SignIn(AuthSignIn data, CancellationToken cancellationToken = default)
    {
        _logger.Log(LogLevel.Information, Localize.Log.MethodStart(GetType(), nameof(SignIn)));

        if (ValidateModel(data) is { } validationResult)
            return validationResult;

        var warnings = new List<WarningModelResultEntry>();

        try
        {
            await _appDbContextAction.BeginTransactionAsync();

            var customPasswordHasher = new CustomPasswordHasher();

            var user = await _userService.GetByEmailAsync(data.Email);
            if (user == null || user.IsTemporary || !customPasswordHasher.VerifyPassword(user.Password, data.Password))
                throw new HttpResponseException(StatusCodes.Status400BadRequest, ErrorType.Model,
                    Localize.Error.UserDoesNotExistOrWrongCredentials);

            var refreshTokenString = Utilities.GenerateRandomBase64String(256);
            var refreshTokenExpiresAt =
                data.RefreshTokenExpireAt ??
                DateTimeOffset.UtcNow.AddSeconds(_refreshTokenOptions.DefaultExpirySeconds);
            await _refreshTokenService.Create(new RefreshToken
            {
                Token = refreshTokenString,
                ExpiresAt = refreshTokenExpiresAt,
                UserId = user.Id
            }, cancellationToken);

            var jsonWebTokenExpiresAt = DateTimeOffset.UtcNow.AddSeconds(_jsonWebTokenOptions.ExpirySeconds);
            var jsonWebTokenString = _jsonWebTokenService.CreateWithClaims(_jsonWebTokenOptions.IssuerSigningKey,
                _jsonWebTokenOptions.Issuer, _jsonWebTokenOptions.Audience, new List<Claim>
                {
                    new(ClaimKey.UserId, user.Id.ToString(), ClaimValueTypes.String),
                }, jsonWebTokenExpiresAt.UtcDateTime);
            await _jsonWebTokenService.Create(new JsonWebToken
                {
                    Token = jsonWebTokenString,
                    ExpiresAt = jsonWebTokenExpiresAt,
                    DeleteAfter = refreshTokenExpiresAt,
                    UserId = user.Id
                },
                cancellationToken);

            if (data.UseCookies)
            {
                _logger.Log(LogLevel.Information,
                    Localize.Log.Method(GetType(), nameof(SignIn), $"Client requested to use cookies"));

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
                warnings.Add(new WarningModelResultEntry(WarningType.Security, Localize.Warning.XssVulnerable));
            }

            user.LastSignIn = DateTimeOffset.UtcNow;

            await _userService.Save(user, cancellationToken);

            await _appDbContextAction.CommitTransactionAsync();

            _logger.Log(LogLevel.Information, Localize.Log.MethodEnd(GetType(), nameof(SignIn)));

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
        catch (Exception)
        {
            await _appDbContextAction.RollbackTransactionAsync();

            throw;
        }
    }

    public async Task<DTOResultBase> Refresh(AuthRefresh data, CancellationToken cancellationToken = default)
    {
        _logger.Log(LogLevel.Information, Localize.Log.MethodStart(GetType(), nameof(Refresh)));

        if (ValidateModel(data) is { } validationResult)
            return validationResult;

        var warnings = new List<WarningModelResultEntry>();

        try
        {
            await _appDbContextAction.BeginTransactionAsync();

            var useCookies = data.RefreshToken == null;

            if (useCookies)
                _logger.Log(LogLevel.Information,
                    Localize.Log.Method(GetType(), nameof(Refresh), $"Client requested to use cookies"));

            data.RefreshToken ??=
                _httpContext.Request.Cookies.SingleOrDefault(_ => _.Key == CookieKey.RefreshToken).Value;
            if (data.RefreshToken == null)
                throw new HttpResponseException(StatusCodes.Status400BadRequest, ErrorType.Auth,
                    Localize.Error.RefreshTokenNotProvided);

            _logger.Log(LogLevel.Information,
                Localize.Log.Method(GetType(), nameof(Refresh),
                    $"{nameof(data.RefreshToken)} presented {data.RefreshToken}"));

            var refreshToken = await _refreshTokenService.GetByTokenAsync(data.RefreshToken);
            if (refreshToken == null)
                throw new HttpResponseException(StatusCodes.Status400BadRequest, ErrorType.Auth,
                    Localize.Error.RefreshTokenNotFound);

            if (refreshToken.ExpiresAt < DateTimeOffset.UtcNow)
                throw new HttpResponseException(StatusCodes.Status400BadRequest, ErrorType.Auth,
                    Localize.Error.RefreshTokenExpired);

            _logger.Log(LogLevel.Information,
                Localize.Log.Method(GetType(), nameof(Refresh),
                    $"{refreshToken.GetType().Name} is valid until {refreshToken.ExpiresAt}"));

            await _refreshTokenService.Delete(refreshToken, cancellationToken);

            var jsonWebToken = await _jsonWebTokenAdvancedService.GetFromHttpContext(cancellationToken);
            if (jsonWebToken == null)
                throw new HttpResponseException(StatusCodes.Status400BadRequest, ErrorType.Auth,
                    Localize.Error.JsonWebTokenNotFound);

            await _jsonWebTokenService.Delete(jsonWebToken, cancellationToken);

            var user = await _userAdvancedService.GetFromHttpContext(cancellationToken);
            if (user == null)
                throw new HttpResponseException(StatusCodes.Status500InternalServerError, ErrorType.HttpContext,
                    Localize.Error.UserDoesNotExistOrHttpContextMissingClaims);

            var refreshTokenString = Utilities.GenerateRandomBase64String(256);
            var refreshTokenExpiresAt =
                data.RefreshTokenExpireAt ??
                DateTimeOffset.UtcNow.AddSeconds(_refreshTokenOptions.DefaultExpirySeconds);
            await _refreshTokenService.Create(new RefreshToken
            {
                Token = refreshTokenString,
                ExpiresAt = refreshTokenExpiresAt,
                UserId = user.Id
            }, cancellationToken);

            var jsonWebTokenExpiresAt = DateTimeOffset.UtcNow.AddSeconds(_jsonWebTokenOptions.ExpirySeconds);
            var jsonWebTokenString = _jsonWebTokenService.CreateWithClaims(_jsonWebTokenOptions.IssuerSigningKey,
                _jsonWebTokenOptions.Issuer, _jsonWebTokenOptions.Audience, new List<Claim>
                {
                    new(ClaimKey.UserId, user.Id.ToString(), ClaimValueTypes.String),
                }, jsonWebTokenExpiresAt.UtcDateTime);
            await _jsonWebTokenService.Create(new JsonWebToken
                {
                    Token = jsonWebTokenString,
                    ExpiresAt = jsonWebTokenExpiresAt,
                    DeleteAfter = refreshTokenExpiresAt,
                    UserId = user.Id
                },
                cancellationToken);

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
                warnings.Add(new WarningModelResultEntry(WarningType.Security, Localize.Warning.XssVulnerable));
            }

            user.LastSignIn = DateTimeOffset.UtcNow;

            await _userService.Save(user, cancellationToken);

            await _appDbContextAction.CommitTransactionAsync();

            _logger.Log(LogLevel.Information, Localize.Log.MethodEnd(GetType(), nameof(Refresh)));

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

            throw;
        }
    }

    public async Task<DTOResultBase> SignOut(AuthSignOut data, CancellationToken cancellationToken = default)
    {
        _logger.Log(LogLevel.Information, Localize.Log.MethodStart(GetType(), nameof(SignOut)));

        if (ValidateModel(data) is { } validationResult)
            return validationResult;

        try
        {
            await _appDbContextAction.BeginTransactionAsync();

            var useCookies = data.RefreshToken == null;

            if (useCookies)
                _logger.Log(LogLevel.Information,
                    Localize.Log.Method(GetType(), nameof(SignOut), $"Client requested to use cookies"));

            data.RefreshToken ??=
                _httpContext.Request.Cookies.SingleOrDefault(_ => _.Key == CookieKey.RefreshToken).Value;
            if (data.RefreshToken == null)
                throw new HttpResponseException(StatusCodes.Status400BadRequest, ErrorType.Auth,
                    Localize.Error.RefreshTokenNotProvided);

            var refreshToken = await _refreshTokenService.GetByTokenAsync(data.RefreshToken);
            if (refreshToken == null)
                throw new HttpResponseException(StatusCodes.Status400BadRequest, ErrorType.Auth,
                    Localize.Error.RefreshTokenNotFound);

            if (refreshToken.ExpiresAt < DateTimeOffset.UtcNow)
                throw new HttpResponseException(StatusCodes.Status400BadRequest, ErrorType.Auth,
                    Localize.Error.RefreshTokenExpired);

            _logger.Log(LogLevel.Information,
                Localize.Log.Method(GetType(), nameof(SignOut),
                    $"RefreshToken is valid until {refreshToken.ExpiresAt}"));

            await _refreshTokenService.Delete(refreshToken, cancellationToken);

            var jsonWebToken = await _jsonWebTokenAdvancedService.GetFromHttpContext(cancellationToken);
            if (jsonWebToken == null)
                throw new HttpResponseException(StatusCodes.Status400BadRequest, ErrorType.Auth,
                    Localize.Error.JsonWebTokenNotFound);

            // Middleware already validated this case
            // if (jsonWebToken.ExpiresAt < DateTimeOffset.UtcNow)
            //     throw new CustomException(Localize.Error.JsonWebTokenExpired);

            await _jsonWebTokenService.Delete(jsonWebToken, cancellationToken);

            var user = await _userAdvancedService.GetFromHttpContext(cancellationToken);
            if (user == null)
                throw new HttpResponseException(StatusCodes.Status500InternalServerError, ErrorType.HttpContext,
                    Localize.Error.UserDoesNotExistOrHttpContextMissingClaims);

            if (useCookies)
            {
                _httpContext.Response.Cookies.Delete(CookieKey.JsonWebToken);
                _httpContext.Response.Cookies.Delete(CookieKey.RefreshToken);
            }

            await _appDbContextAction.CommitTransactionAsync();

            _logger.Log(LogLevel.Information, Localize.Log.MethodEnd(GetType(), nameof(SignOut)));

            return new AuthSignOutResult();
        }
        catch (Exception e)
        {
            await _appDbContextAction.RollbackTransactionAsync();

            throw;
        }
    }

    #endregion
}