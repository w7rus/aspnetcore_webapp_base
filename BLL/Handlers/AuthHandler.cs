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
    Task<DTOResultBase> SignUp(AuthSignUp data, CancellationToken cancellationToken);
    Task<DTOResultBase> SignInViaEmail(AuthSignInViaEmail data, CancellationToken cancellationToken);
    Task<DTOResultBase> Refresh(AuthRefresh data, CancellationToken cancellationToken);
    Task<DTOResultBase> SignOut(AuthSignOut data, CancellationToken cancellationToken);
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
        IOptions<MiscOptions> miscOptions
    )
    {
        _fullName = GetType().FullName;
        _logger = logger;
        _appDbContextAction = appDbContextAction;
        _jsonWebTokenService = jsonWebTokenService;
        _refreshTokenService = refreshTokenService;
        _userService = userService;
        _refreshTokenOptions = refreshTokenOptions.Value;
        _jsonWebTokenOptions = jsonWebTokenOptions.Value;
        _httpContext = httpContextAccessor.HttpContext;
        _miscOptions = miscOptions.Value;
    }

    #endregion

    #region Methods

    public async Task<DTOResultBase> SignUp(AuthSignUp data, CancellationToken cancellationToken)
    {
        _logger.Log(LogLevel.Information, Localize.Log.MethodStart(_fullName, nameof(SignUp)));

        if (ValidateModel(data) is { } validationResult)
            return validationResult;

        try
        {
            await _appDbContextAction.BeginTransactionAsync();

            var customPasswordHasher = new CustomPasswordHasher();

            var passwordHashed = customPasswordHasher.HashPassword(data.Password);

            var user = await _userService.Add(data.Email, passwordHashed, cancellationToken);

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
                    new(Localize.ErrorType.Auth, Localize.Error.SignUpFailed)
                }
            };

            if (e is CustomException)
                errorModelResult.Errors.Add(new KeyValuePair<string, string>(Localize.ErrorType.Auth, e.Message));

            return errorModelResult;
        }
    }

    public async Task<DTOResultBase> SignInViaEmail(AuthSignInViaEmail data, CancellationToken cancellationToken)
    {
        _logger.Log(LogLevel.Information, Localize.Log.MethodStart(_fullName, nameof(SignInViaEmail)));

        if (ValidateModel(data) is { } validationResult)
            return validationResult;

        try
        {
            await _appDbContextAction.BeginTransactionAsync();

            var customPasswordHasher = new CustomPasswordHasher();

            var user = await _userService.GetByEmailAsync(data.Email);
            if (user == null || !customPasswordHasher.VerifyPassword(user.Password, data.Password))
                throw new CustomException();

            var refreshTokenString = Utilities.GenerateRandomBase64String(256);
            var refreshTokenExpiresAt =
                data.RefreshTokenExpireAt ??
                DateTimeOffset.UtcNow.AddSeconds(_refreshTokenOptions.DefaultExpirySeconds);
            await _refreshTokenService.Add(refreshTokenString, refreshTokenExpiresAt,
                user.Id, cancellationToken);

            var jsonWebTokenExpiresAt = DateTimeOffset.UtcNow.AddSeconds(_jsonWebTokenOptions.ExpirySeconds);
            var jsonWebTokenString = _jsonWebTokenService.CreateWithClaims(_jsonWebTokenOptions.IssuerSigningKey,
                _jsonWebTokenOptions.Issuer, _jsonWebTokenOptions.Audience, new List<Claim>
                {
                    new(ClaimKey.UserId, user.Id.ToString(), ClaimValueTypes.String),
                }, jsonWebTokenExpiresAt.UtcDateTime);
            await _jsonWebTokenService.Add(jsonWebTokenString, jsonWebTokenExpiresAt, refreshTokenExpiresAt, user.Id,
                cancellationToken);

            if (data.UseCookies)
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

            await _appDbContextAction.CommitTransactionAsync();

            _logger.Log(LogLevel.Information, Localize.Log.MethodEnd(_fullName, nameof(SignInViaEmail)));

            return new AuthSignInResult
            {
                UserId = user.Id,
                JsonWebToken = !data.UseCookies ? jsonWebTokenString : null,
                JsonWebTokenExpiresAt = jsonWebTokenExpiresAt,
                RefreshToken = !data.UseCookies ? refreshTokenString : null,
                RefreshTokenExpiresAt = refreshTokenExpiresAt
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
                    new(Localize.ErrorType.Auth, Localize.Error.SignInFailed)
                }
            };

            if (e is CustomException)
                errorModelResult.Errors.Add(new KeyValuePair<string, string>(Localize.ErrorType.Auth, e.Message));

            return errorModelResult;
        }
    }

    public async Task<DTOResultBase> Refresh(AuthRefresh data, CancellationToken cancellationToken)
    {
        _logger.Log(LogLevel.Information, Localize.Log.MethodStart(_fullName, nameof(Refresh)));

        if (ValidateModel(data) is { } validationResult)
            return validationResult;

        try
        {
            await _appDbContextAction.BeginTransactionAsync();

            var useCookies = data.RefreshToken == null;

            data.RefreshToken ??=
                _httpContext.Request.Cookies.SingleOrDefault(_ => _.Key == CookieKey.RefreshToken).Value;
            if (data.RefreshToken == null)
                throw new CustomException(Localize.Error.RefreshTokenNotFound);

            var refreshTokenOld = await _refreshTokenService.GetByTokenAsync(data.RefreshToken);
            if (refreshTokenOld == null)
                throw new CustomException(Localize.Error.RefreshTokenNotFound);

            if (refreshTokenOld.ExpiresAt < DateTimeOffset.UtcNow)
                throw new CustomException(Localize.Error.RefreshTokenExpired);

            await _refreshTokenService.Delete(refreshTokenOld, cancellationToken);

            var jsonWebToken = await _jsonWebTokenService.GetFromHttpContext();
            if (jsonWebToken == null)
                throw new CustomException(Localize.Error.JsonWebTokenNotFound);

            // Middleware already validated this case
            // if (jsonWebToken.ExpiresAt < DateTimeOffset.UtcNow)
            //     throw new CustomException(Localize.Error.JsonWebTokenExpired);

            await _jsonWebTokenService.Delete(jsonWebToken, cancellationToken);

            var user = await _userService.GetFromHttpContext();
            if (user == null)
                throw new CustomException();

            var refreshTokenString = Utilities.GenerateRandomBase64String(256);
            var refreshTokenExpiresAt =
                data.RefreshTokenExpireAt ??
                DateTimeOffset.UtcNow.AddSeconds(_refreshTokenOptions.DefaultExpirySeconds);
            await _refreshTokenService.Add(refreshTokenString, refreshTokenExpiresAt,
                user.Id, cancellationToken);

            var jsonWebTokenExpiresAt = DateTimeOffset.UtcNow.AddSeconds(_jsonWebTokenOptions.ExpirySeconds);
            var jsonWebTokenString = _jsonWebTokenService.CreateWithClaims(_jsonWebTokenOptions.IssuerSigningKey,
                _jsonWebTokenOptions.Issuer, _jsonWebTokenOptions.Audience, new List<Claim>
                {
                    new(ClaimKey.UserId, user.Id.ToString(), ClaimValueTypes.String),
                }, jsonWebTokenExpiresAt.UtcDateTime);
            await _jsonWebTokenService.Add(jsonWebTokenString, jsonWebTokenExpiresAt, refreshTokenExpiresAt, user.Id,
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

            await _appDbContextAction.CommitTransactionAsync();

            _logger.Log(LogLevel.Information, Localize.Log.MethodEnd(_fullName, nameof(Refresh)));

            return new AuthRefreshResult()
            {
                UserId = user.Id,
                JsonWebToken = !useCookies ? jsonWebTokenString : null,
                JsonWebTokenExpiresAt = jsonWebTokenExpiresAt,
                RefreshToken = !useCookies ? refreshTokenString : null,
                RefreshTokenExpiresAt = refreshTokenExpiresAt
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
                    new(Localize.ErrorType.Auth, Localize.Error.RefreshFailed)
                }
            };

            if (e is CustomException)
                errorModelResult.Errors.Add(new KeyValuePair<string, string>(Localize.ErrorType.Auth, e.Message));

            return errorModelResult;
        }
    }

    public async Task<DTOResultBase> SignOut(AuthSignOut data, CancellationToken cancellationToken)
    {
        _logger.Log(LogLevel.Information, Localize.Log.MethodStart(_fullName, nameof(SignOut)));

        if (ValidateModel(data) is { } validationResult)
            return validationResult;

        try
        {
            await _appDbContextAction.BeginTransactionAsync();

            var useCookies = data.RefreshToken == null;

            data.RefreshToken ??=
                _httpContext.Request.Cookies.SingleOrDefault(_ => _.Key == CookieKey.RefreshToken).Value;
            if (data.RefreshToken == null)
                throw new CustomException(Localize.Error.RefreshTokenNotFound);

            var refreshTokenOld = await _refreshTokenService.GetByTokenAsync(data.RefreshToken);
            if (refreshTokenOld == null)
                throw new CustomException(Localize.Error.RefreshTokenNotFound);

            if (refreshTokenOld.ExpiresAt < DateTimeOffset.UtcNow)
                throw new CustomException(Localize.Error.RefreshTokenExpired);

            await _refreshTokenService.Delete(refreshTokenOld, cancellationToken);

            var jsonWebToken = await _jsonWebTokenService.GetFromHttpContext();
            if (jsonWebToken == null)
                throw new CustomException(Localize.Error.JsonWebTokenNotFound);

            if (jsonWebToken.ExpiresAt < DateTimeOffset.UtcNow)
                throw new CustomException(Localize.Error.JsonWebTokenExpired);

            await _jsonWebTokenService.Delete(jsonWebToken, cancellationToken);

            var user = await _userService.GetFromHttpContext();
            if (user == null)
                throw new CustomException();

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
                    new(Localize.ErrorType.Auth, Localize.Error.SignOutFailed)
                }
            };

            if (e is CustomException)
                errorModelResult.Errors.Add(new KeyValuePair<string, string>(Localize.ErrorType.Auth, e.Message));

            return errorModelResult;
        }
    }

    #endregion
}