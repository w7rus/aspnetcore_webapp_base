using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using BLL.Handlers.Base;
using BLL.Services.Advanced;
using BLL.Services.Entity;
using Common.Enums;
using Common.Exceptions;
using Common.Helpers;
using Common.Models;
using Common.Models.Base;
using Common.Options;
using DAL.Data;
using Domain.Entities;
using DTO.Models.Auth;
using DTO.Models.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BLL.Handlers.Auth;

public interface IAuthHandler
{
    Task<IDtoResultBase> SignInAsGuest(AuthSignUpInAsGuestDto data, CancellationToken cancellationToken = default);
    Task<IDtoResultBase> SignUp(AuthSignUpDto data, CancellationToken cancellationToken = default);
    Task<IDtoResultBase> SignIn(AuthSignInDto data, CancellationToken cancellationToken = default);
    Task<IDtoResultBase> Refresh(AuthRefreshDto data, CancellationToken cancellationToken = default);
    Task<IDtoResultBase> SignOut(AuthSignOutDto data, CancellationToken cancellationToken = default);
}

public class AuthHandler : HandlerBase, IAuthHandler
{
    #region Ctor

    public AuthHandler(
        ILogger<AuthHandler> logger,
        IAppDbContextAction appDbContextAction,
        IJsonWebTokenEntityService jsonWebTokenEntityService,
        IRefreshTokenEntityService refreshTokenEntityService,
        IUserEntityService userEntityService,
        IOptions<RefreshTokenOptions> refreshTokenOptions,
        IOptions<JsonWebTokenOptions> jsonWebTokenOptions,
        IHttpContextAccessor httpContextAccessor,
        IOptions<MiscOptions> miscOptions,
        IUserToUserGroupMappingEntityService userToUserGroupMappingEntityService,
        IUserGroupEntityService userGroupEntityService,
        IUserAdvancedService userAdvancedService,
        IJsonWebTokenAdvancedService jsonWebTokenAdvancedService,
        IWarningAdvancedService warningAdvancedService
    )
    {
        _logger = logger;
        _appDbContextAction = appDbContextAction;
        _jsonWebTokenEntityService = jsonWebTokenEntityService;
        _refreshTokenEntityService = refreshTokenEntityService;
        _userEntityService = userEntityService;
        _userToUserGroupMappingEntityService = userToUserGroupMappingEntityService;
        _userGroupEntityService = userGroupEntityService;
        _userAdvancedService = userAdvancedService;
        _jsonWebTokenAdvancedService = jsonWebTokenAdvancedService;
        _warningAdvancedService = warningAdvancedService;
        _refreshTokenOptions = refreshTokenOptions.Value;
        _jsonWebTokenOptions = jsonWebTokenOptions.Value;
        _httpContext = httpContextAccessor.HttpContext;
        _miscOptions = miscOptions.Value;
    }

    #endregion

    #region Fields

    private readonly ILogger<AuthHandler> _logger;
    private readonly IAppDbContextAction _appDbContextAction;
    private readonly IJsonWebTokenEntityService _jsonWebTokenEntityService;
    private readonly IRefreshTokenEntityService _refreshTokenEntityService;
    private readonly IUserEntityService _userEntityService;
    private readonly RefreshTokenOptions _refreshTokenOptions;
    private readonly JsonWebTokenOptions _jsonWebTokenOptions;
    private readonly HttpContext _httpContext;
    private readonly MiscOptions _miscOptions;
    private readonly IUserToUserGroupMappingEntityService _userToUserGroupMappingEntityService;
    private readonly IUserGroupEntityService _userGroupEntityService;
    private readonly IUserAdvancedService _userAdvancedService;
    private readonly IJsonWebTokenAdvancedService _jsonWebTokenAdvancedService;
    private readonly IWarningAdvancedService _warningAdvancedService;

    #endregion

    #region Methods

    public async Task<IDtoResultBase> SignInAsGuest(
        AuthSignUpInAsGuestDto data,
        CancellationToken cancellationToken = default
    )
    {
        _logger.Log(LogLevel.Information, Localize.Log.MethodStart(GetType(), nameof(SignInAsGuest)));

        if (ValidateModel(data) is { } validationResult)
            return validationResult;

        try
        {
            await _appDbContextAction.BeginTransactionAsync();

            var user = await _userEntityService.Save(new Domain.Entities.User
            {
                Email = null,
                Password = null,
                IsTemporary = true
            }, cancellationToken);

            var guestUserGroup = await _userGroupEntityService.GetByAliasAsync("Guest");

            await _userToUserGroupMappingEntityService.Save(new UserToUserGroupMapping
            {
                EntityLeftId = user.Id,
                EntityRightId = guestUserGroup.Id
            }, cancellationToken);

            var refreshTokenString = Utilities.GenerateRandomBase64String(256);
            var refreshTokenExpiresAt =
                data.RefreshTokenExpireAt ??
                DateTimeOffset.UtcNow.AddSeconds(_refreshTokenOptions.DefaultExpirySeconds);
            await _refreshTokenEntityService.Save(new RefreshToken
            {
                Token = refreshTokenString,
                ExpiresAt = refreshTokenExpiresAt,
                UserId = user.Id
            }, cancellationToken);

            var jsonWebTokenExpiresAt = DateTimeOffset.UtcNow.AddSeconds(_jsonWebTokenOptions.ExpirySeconds);
            var jsonWebTokenString = _jsonWebTokenEntityService.CreateWithClaims(_jsonWebTokenOptions.IssuerSigningKey,
                _jsonWebTokenOptions.Issuer, _jsonWebTokenOptions.Audience, new List<Claim>
                {
                    new(ClaimKey.UserId, user.Id.ToString(), ClaimValueTypes.String)
                }, jsonWebTokenExpiresAt.UtcDateTime);
            await _jsonWebTokenEntityService.Save(new JsonWebToken
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
                    Localize.Log.Method(GetType(), nameof(SignIn), "Client requested to use cookies"));

                var refreshTokenCookieOptions = new CookieOptions
                {
                    Expires = refreshTokenExpiresAt,
                    Secure = _miscOptions.SecureCookies,
                    SameSite = SameSiteMode.Strict,
                    HttpOnly = true
                };
                var jsonWebTokenCookieOptions = new CookieOptions
                {
                    Expires = refreshTokenExpiresAt,
                    Secure = _miscOptions.SecureCookies,
                    SameSite = SameSiteMode.Strict,
                    HttpOnly = true
                };

                _httpContext.Response.Cookies.Append(CookieKey.RefreshToken, refreshTokenString,
                    refreshTokenCookieOptions);
                _httpContext.Response.Cookies.Append(CookieKey.JsonWebToken, jsonWebTokenString,
                    jsonWebTokenCookieOptions);
            }
            else
            {
                _warningAdvancedService.Add(new WarningModelResultEntry(WarningType.Security,
                    Localize.Warning.XssVulnerable));
            }

            user.LastSignIn = DateTimeOffset.UtcNow;

            await _userEntityService.Save(user, cancellationToken);

            await _appDbContextAction.CommitTransactionAsync();

            _logger.Log(LogLevel.Information, Localize.Log.MethodEnd(GetType(), nameof(SignInAsGuest)));

            return new AuthSignInResultDto
            {
                UserId = user.Id,
                JsonWebToken = !data.UseCookies ? jsonWebTokenString : null,
                JsonWebTokenExpiresAt = jsonWebTokenExpiresAt,
                RefreshToken = !data.UseCookies ? refreshTokenString : null,
                RefreshTokenExpiresAt = refreshTokenExpiresAt
            };
        }
        catch (Exception)
        {
            await _appDbContextAction.RollbackTransactionAsync();

            throw;
        }
    }

    public async Task<IDtoResultBase> SignUp(AuthSignUpDto data, CancellationToken cancellationToken = default)
    {
        _logger.Log(LogLevel.Information, Localize.Log.MethodStart(GetType(), nameof(SignUp)));

        if (ValidateModel(data) is { } validationResult)
            return validationResult;

        try
        {
            await _appDbContextAction.BeginTransactionAsync();

            var user = await _userAdvancedService.GetFromHttpContext(cancellationToken: cancellationToken);
            if (user == null)
                throw new HttpResponseException(StatusCodes.Status400BadRequest, ErrorType.HttpContext,
                    Localize.Error.UserNotFoundOrHttpContextMissingClaims);

            var customPasswordHasher = new CustomPasswordHasher();

            var passwordHashed = customPasswordHasher.HashPassword(data.Password);

            user.Email = data.Email;
            user.Password = passwordHashed;
            user.IsTemporary = false;

            await _userEntityService.Save(user, cancellationToken);

            var memberUserGroup = await _userGroupEntityService.GetByAliasAsync("Member");

            await _userToUserGroupMappingEntityService.Save(new UserToUserGroupMapping
            {
                EntityLeftId = user.Id,
                EntityRightId = memberUserGroup.Id
            }, cancellationToken);

            await _appDbContextAction.CommitTransactionAsync();

            _logger.Log(LogLevel.Information, Localize.Log.MethodEnd(GetType(), nameof(SignUp)));

            return new AuthSignUpResultDto
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

    public async Task<IDtoResultBase> SignIn(AuthSignInDto data, CancellationToken cancellationToken = default)
    {
        _logger.Log(LogLevel.Information, Localize.Log.MethodStart(GetType(), nameof(SignIn)));

        if (ValidateModel(data) is { } validationResult)
            return validationResult;

        try
        {
            await _appDbContextAction.BeginTransactionAsync();

            var customPasswordHasher = new CustomPasswordHasher();

            var user = await _userEntityService.GetByEmailAsync(data.Email);
            if (user == null || user.IsTemporary || !customPasswordHasher.VerifyPassword(user.Password, data.Password))
                throw new HttpResponseException(StatusCodes.Status400BadRequest, ErrorType.Generic,
                    Localize.Error.UserNotFoundOrWrongCredentials);

            var refreshTokenString = Utilities.GenerateRandomBase64String(256);
            var refreshTokenExpiresAt =
                data.RefreshTokenExpireAt ??
                DateTimeOffset.UtcNow.AddSeconds(_refreshTokenOptions.DefaultExpirySeconds);
            await _refreshTokenEntityService.Save(new RefreshToken
            {
                Token = refreshTokenString,
                ExpiresAt = refreshTokenExpiresAt,
                UserId = user.Id
            }, cancellationToken);

            var jsonWebTokenExpiresAt = DateTimeOffset.UtcNow.AddSeconds(_jsonWebTokenOptions.ExpirySeconds);
            var jsonWebTokenString = _jsonWebTokenEntityService.CreateWithClaims(_jsonWebTokenOptions.IssuerSigningKey,
                _jsonWebTokenOptions.Issuer, _jsonWebTokenOptions.Audience, new List<Claim>
                {
                    new(ClaimKey.UserId, user.Id.ToString(), ClaimValueTypes.String)
                }, jsonWebTokenExpiresAt.UtcDateTime);
            await _jsonWebTokenEntityService.Save(new JsonWebToken
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
                    Localize.Log.Method(GetType(), nameof(SignIn), "Client requested to use cookies"));

                var refreshTokenCookieOptions = new CookieOptions
                {
                    Expires = refreshTokenExpiresAt,
                    Secure = _miscOptions.SecureCookies,
                    SameSite = SameSiteMode.Strict,
                    HttpOnly = true
                };
                var jsonWebTokenCookieOptions = new CookieOptions
                {
                    Expires = refreshTokenExpiresAt,
                    Secure = _miscOptions.SecureCookies,
                    SameSite = SameSiteMode.Strict,
                    HttpOnly = true
                };

                _httpContext.Response.Cookies.Append(CookieKey.RefreshToken, refreshTokenString,
                    refreshTokenCookieOptions);
                _httpContext.Response.Cookies.Append(CookieKey.JsonWebToken, jsonWebTokenString,
                    jsonWebTokenCookieOptions);
            }
            else
            {
                _warningAdvancedService.Add(new WarningModelResultEntry(WarningType.Security,
                    Localize.Warning.XssVulnerable));
            }

            user.LastSignIn = DateTimeOffset.UtcNow;

            await _userEntityService.Save(user, cancellationToken);

            await _appDbContextAction.CommitTransactionAsync();

            _logger.Log(LogLevel.Information, Localize.Log.MethodEnd(GetType(), nameof(SignIn)));

            return new AuthSignInResultDto
            {
                UserId = user.Id,
                JsonWebToken = !data.UseCookies ? jsonWebTokenString : null,
                JsonWebTokenExpiresAt = jsonWebTokenExpiresAt,
                RefreshToken = !data.UseCookies ? refreshTokenString : null,
                RefreshTokenExpiresAt = refreshTokenExpiresAt
            };
        }
        catch (Exception)
        {
            await _appDbContextAction.RollbackTransactionAsync();

            throw;
        }
    }

    public async Task<IDtoResultBase> Refresh(AuthRefreshDto data, CancellationToken cancellationToken = default)
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
                    Localize.Log.Method(GetType(), nameof(Refresh), "Client requested to use cookies"));

            data.RefreshToken ??=
                _httpContext.Request.Cookies.SingleOrDefault(_ => _.Key == CookieKey.RefreshToken).Value;
            if (data.RefreshToken == null)
                throw new HttpResponseException(StatusCodes.Status400BadRequest, ErrorType.Auth,
                    Localize.Error.RefreshTokenNotProvided);

            _logger.Log(LogLevel.Information,
                Localize.Log.Method(GetType(), nameof(Refresh),
                    $"{nameof(data.RefreshToken)} presented {data.RefreshToken}"));

            var refreshToken = await _refreshTokenEntityService.GetByTokenAsync(data.RefreshToken);
            if (refreshToken == null)
                throw new HttpResponseException(StatusCodes.Status404NotFound, ErrorType.Auth,
                    Localize.Error.RefreshTokenNotFound);

            if (refreshToken.ExpiresAt < DateTimeOffset.UtcNow)
                throw new HttpResponseException(StatusCodes.Status400BadRequest, ErrorType.Auth,
                    Localize.Error.RefreshTokenExpired);

            _logger.Log(LogLevel.Information,
                Localize.Log.Method(GetType(), nameof(Refresh),
                    $"{refreshToken.GetType().Name} is valid until {refreshToken.ExpiresAt}"));

            await _refreshTokenEntityService.Delete(refreshToken, cancellationToken);

            var jsonWebToken =
                await _jsonWebTokenAdvancedService.GetFromHttpContext(cancellationToken: cancellationToken);
            if (jsonWebToken == null)
                throw new HttpResponseException(StatusCodes.Status404NotFound, ErrorType.Auth,
                    Localize.Error.JsonWebTokenNotFound);

            await _jsonWebTokenEntityService.Delete(jsonWebToken, cancellationToken);

            var user = await _userAdvancedService.GetFromHttpContext(cancellationToken: cancellationToken);
            if (user == null)
                throw new HttpResponseException(StatusCodes.Status400BadRequest, ErrorType.HttpContext,
                    Localize.Error.UserNotFoundOrHttpContextMissingClaims);

            var refreshTokenString = Utilities.GenerateRandomBase64String(256);
            var refreshTokenExpiresAt =
                data.RefreshTokenExpireAt ??
                DateTimeOffset.UtcNow.AddSeconds(_refreshTokenOptions.DefaultExpirySeconds);
            await _refreshTokenEntityService.Save(new RefreshToken
            {
                Token = refreshTokenString,
                ExpiresAt = refreshTokenExpiresAt,
                UserId = user.Id
            }, cancellationToken);

            var jsonWebTokenExpiresAt = DateTimeOffset.UtcNow.AddSeconds(_jsonWebTokenOptions.ExpirySeconds);
            var jsonWebTokenString = _jsonWebTokenEntityService.CreateWithClaims(_jsonWebTokenOptions.IssuerSigningKey,
                _jsonWebTokenOptions.Issuer, _jsonWebTokenOptions.Audience, new List<Claim>
                {
                    new(ClaimKey.UserId, user.Id.ToString(), ClaimValueTypes.String)
                }, jsonWebTokenExpiresAt.UtcDateTime);
            await _jsonWebTokenEntityService.Save(new JsonWebToken
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
                    HttpOnly = true
                };
                var jsonWebTokenCookieOptions = new CookieOptions
                {
                    Expires = refreshTokenExpiresAt,
                    Secure = _miscOptions.SecureCookies,
                    SameSite = SameSiteMode.Strict,
                    HttpOnly = true
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

            await _userEntityService.Save(user, cancellationToken);

            await _appDbContextAction.CommitTransactionAsync();

            _logger.Log(LogLevel.Information, Localize.Log.MethodEnd(GetType(), nameof(Refresh)));

            return new AuthRefreshResultDto
            {
                UserId = user.Id,
                JsonWebToken = !useCookies ? jsonWebTokenString : null,
                JsonWebTokenExpiresAt = jsonWebTokenExpiresAt,
                RefreshToken = !useCookies ? refreshTokenString : null,
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

    public async Task<IDtoResultBase> SignOut(AuthSignOutDto data, CancellationToken cancellationToken = default)
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
                    Localize.Log.Method(GetType(), nameof(SignOut), "Client requested to use cookies"));

            data.RefreshToken ??=
                _httpContext.Request.Cookies.SingleOrDefault(_ => _.Key == CookieKey.RefreshToken).Value;
            if (data.RefreshToken == null)
                throw new HttpResponseException(StatusCodes.Status400BadRequest, ErrorType.Auth,
                    Localize.Error.RefreshTokenNotProvided);

            var refreshToken = await _refreshTokenEntityService.GetByTokenAsync(data.RefreshToken);
            if (refreshToken == null)
                throw new HttpResponseException(StatusCodes.Status404NotFound, ErrorType.Auth,
                    Localize.Error.RefreshTokenNotFound);

            if (refreshToken.ExpiresAt < DateTimeOffset.UtcNow)
                throw new HttpResponseException(StatusCodes.Status400BadRequest, ErrorType.Auth,
                    Localize.Error.RefreshTokenExpired);

            _logger.Log(LogLevel.Information,
                Localize.Log.Method(GetType(), nameof(SignOut),
                    $"RefreshToken is valid until {refreshToken.ExpiresAt}"));

            await _refreshTokenEntityService.Delete(refreshToken, cancellationToken);

            var jsonWebToken =
                await _jsonWebTokenAdvancedService.GetFromHttpContext(cancellationToken: cancellationToken);
            if (jsonWebToken == null)
                throw new HttpResponseException(StatusCodes.Status404NotFound, ErrorType.Auth,
                    Localize.Error.JsonWebTokenNotFound);

            // Middleware already validated this case
            // if (jsonWebToken.ExpiresAt < DateTimeOffset.UtcNow)
            //     throw new CustomException(Localize.Error.JsonWebTokenExpired);

            await _jsonWebTokenEntityService.Delete(jsonWebToken, cancellationToken);

            var user = await _userAdvancedService.GetFromHttpContext(cancellationToken: cancellationToken);
            if (user == null)
                throw new HttpResponseException(StatusCodes.Status400BadRequest, ErrorType.HttpContext,
                    Localize.Error.UserNotFoundOrHttpContextMissingClaims);

            if (useCookies)
            {
                _httpContext.Response.Cookies.Delete(CookieKey.JsonWebToken);
                _httpContext.Response.Cookies.Delete(CookieKey.RefreshToken);
            }

            await _appDbContextAction.CommitTransactionAsync();

            _logger.Log(LogLevel.Information, Localize.Log.MethodEnd(GetType(), nameof(SignOut)));

            return new OkResultDto();
        }
        catch (Exception)
        {
            await _appDbContextAction.RollbackTransactionAsync();

            throw;
        }
    }

    #endregion
}