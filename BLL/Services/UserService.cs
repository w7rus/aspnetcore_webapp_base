using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BLL.Services.Base;
using Common.Exceptions;
using Common.Models;
using DAL.Data;
using DAL.Repository;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BLL.Services;

/// <summary>
/// Service to work with User entity
/// </summary>
public interface IUserService : IEntityServiceBase<User>
{
    /// <summary>
    /// Gets entity with Email that equals given one
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    Task<User> GetByEmailAsync(string email);

    /// <summary>
    /// Gets entities with PhoneNumber that equals given one
    /// </summary>
    /// <param name="phoneNumber"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IReadOnlyCollection<User>> GetByPhoneNumberAsync(
        string phoneNumber,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Gets entities with DisableSignInUntil that is less than current date
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IReadOnlyCollection<User>> GetExpiredByDisableSignInUntil(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets entities with LastSignIn that are in given inclusive range
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IReadOnlyCollection<User>> GetInRangeByLastSignIn(
        DateTimeOffset from,
        DateTimeOffset to,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Gets entities with LastActivity that are in given inclusive range
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IReadOnlyCollection<User>> GetInRangeByLastActivity(
        DateTimeOffset from,
        DateTimeOffset to,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Gets entities with IpAddress that equals given one
    /// </summary>
    /// <param name="ipAddress"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IReadOnlyCollection<User>> GetByLastIpAddress(string ipAddress, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets entity by HttpContext authorization data
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<User> GetFromHttpContext(CancellationToken cancellationToken = default);
}

public class UserService : IUserService
{
    #region Fields

    private readonly ILogger<UserService> _logger;
    private readonly IUserRepository _userRepository;
    private readonly IAppDbContextAction _appDbContextAction;
    private readonly HttpContext _httpContext;

    #endregion

    #region Ctor

    public UserService(
        ILogger<UserService> logger,
        IUserRepository userRepository,
        IAppDbContextAction appDbContextAction,
        IHttpContextAccessor httpContextAccessor
    )
    {
        _logger = logger;
        _userRepository = userRepository;
        _appDbContextAction = appDbContextAction;
        _httpContext = httpContextAccessor.HttpContext;
    }

    #endregion

    #region Methods

    public async Task Save(User entity, CancellationToken cancellationToken = default)
    {
        _userRepository.Save(entity);
        await _appDbContextAction.CommitAsync(cancellationToken);
    }

    public async Task Delete(User entity, CancellationToken cancellationToken = default)
    {
        _userRepository.Delete(entity);
        await _appDbContextAction.CommitAsync(cancellationToken);
    }

    public async Task<User> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _userRepository.SingleOrDefaultAsync(_ => _.Id == id);
    }

    public async Task<User> Create(User entity, CancellationToken cancellationToken = default)
    {
        await Save(entity, cancellationToken);
        return entity;
    }

    public async Task<User> GetByEmailAsync(string email)
    {
        return await _userRepository.SingleOrDefaultAsync(_ => _.Email == email);
    }

    public async Task<IReadOnlyCollection<User>> GetByPhoneNumberAsync(
        string phoneNumber,
        CancellationToken cancellationToken = default
    )
    {
        return await _userRepository.QueryMany(_ => _.PhoneNumber == phoneNumber).ToArrayAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<User>> GetExpiredByDisableSignInUntil(
        CancellationToken cancellationToken = default
    )
    {
        return await _userRepository.QueryMany(_ => _.DisableSignInUntil < DateTimeOffset.UtcNow)
            .ToArrayAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<User>> GetInRangeByLastSignIn(
        DateTimeOffset @from,
        DateTimeOffset to,
        CancellationToken cancellationToken = default
    )
    {
        return await _userRepository.QueryMany(_ => _.LastSignIn >= @from && _.LastSignIn <= to)
            .ToArrayAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<User>> GetInRangeByLastActivity(
        DateTimeOffset @from,
        DateTimeOffset to,
        CancellationToken cancellationToken = default
    )
    {
        return await _userRepository.QueryMany(_ => _.LastActivity >= @from && _.LastActivity <= to)
            .ToArrayAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<User>> GetByLastIpAddress(
        string ipAddress,
        CancellationToken cancellationToken = default
    )
    {
        return await _userRepository.QueryMany(_ => _.LastIpAddress == ipAddress).ToArrayAsync(cancellationToken);
    }

    public async Task<User> GetFromHttpContext(CancellationToken cancellationToken = default)
    {
        if (!Guid.TryParse(_httpContext.User.Claims.SingleOrDefault(_ => _.Type == ClaimKey.UserId)?.Value,
                out var userId))
            throw new CustomException(Localize.Error.UserIdRetrievalFailed);

        return await GetByIdAsync(userId, cancellationToken);
    }

    #endregion
}