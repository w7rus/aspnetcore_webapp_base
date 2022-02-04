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
    Task<User> Add(string email, string password, CancellationToken cancellationToken = new());
    Task<User> GetByEmailAsync(string email);
    Task<User> GetByPhoneNumberAsync(string phoneNumber);
    Task<IReadOnlyCollection<User>> GetExpiredByDisableSignInUntil(CancellationToken cancellationToken = new());

    Task<IReadOnlyCollection<User>> GetInRangeByLastSignIn(
        DateTimeOffset from,
        DateTimeOffset to,
        CancellationToken cancellationToken = new()
    );

    Task<IReadOnlyCollection<User>> GetInRangeByLastActivity(
        DateTimeOffset from,
        DateTimeOffset to,
        CancellationToken cancellationToken = new()
    );

    Task<IReadOnlyCollection<User>> GetByLastIpAddress(string ipAddress, CancellationToken cancellationToken = new());
    Task<User> GetFromHttpContext(CancellationToken cancellationToken = new());
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

    public async Task Save(User entity, CancellationToken cancellationToken = new())
    {
        _userRepository.Save(entity);
        await _appDbContextAction.CommitAsync(cancellationToken);
    }

    public async Task<User> Add(string email, string password, CancellationToken cancellationToken = new())
    {
        var entity = new User
        {
            Email = email,
            Password = password,
        };

        await Save(entity, cancellationToken);
        return entity;
    }

    public async Task Delete(User entity, CancellationToken cancellationToken = new())
    {
        _userRepository.Delete(entity);
        await _appDbContextAction.CommitAsync(cancellationToken);
    }

    public async Task<User> GetByIdAsync(Guid id, CancellationToken cancellationToken = new())
    {
        return await _userRepository.SingleOrDefaultAsync(_ => _.Id == id);
    }

    public async Task<User> GetByEmailAsync(string email)
    {
        return await _userRepository.SingleOrDefaultAsync(_ => _.Email == email);
    }

    public async Task<User> GetByPhoneNumberAsync(string phoneNumber)
    {
        return await _userRepository.SingleOrDefaultAsync(_ => _.PhoneNumber == phoneNumber);
    }

    public async Task<IReadOnlyCollection<User>> GetExpiredByDisableSignInUntil(
        CancellationToken cancellationToken = new()
    )
    {
        return await _userRepository.QueryMany(_ => _.DisableSignInUntil < DateTimeOffset.UtcNow)
            .ToArrayAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<User>> GetInRangeByLastSignIn(
        DateTimeOffset @from,
        DateTimeOffset to,
        CancellationToken cancellationToken = new()
    )
    {
        return await _userRepository.QueryMany(_ => _.LastSignIn >= @from && _.LastSignIn <= to)
            .ToArrayAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<User>> GetInRangeByLastActivity(
        DateTimeOffset @from,
        DateTimeOffset to,
        CancellationToken cancellationToken = new()
    )
    {
        return await _userRepository.QueryMany(_ => _.LastActivity >= @from && _.LastActivity <= to)
            .ToArrayAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<User>> GetByLastIpAddress(
        string ipAddress,
        CancellationToken cancellationToken = new()
    )
    {
        return await _userRepository.QueryMany(_ => _.LastIpAddress == ipAddress).ToArrayAsync(cancellationToken);
    }

    public async Task<User> GetFromHttpContext(CancellationToken cancellationToken = new())
    {
        if (!Guid.TryParse(_httpContext.User.Claims.SingleOrDefault(_ => _.Type == ClaimKey.UserId)?.Value,
                out var userId))
            throw new CustomException(Localize.Error.UserIdRetrievalFailed);

        return await GetByIdAsync(userId, cancellationToken);
    }

    #endregion
}