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
    /// Deletes entities with IsTemporary & LastActivity that is less than current date + 1 d
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task PurgeAsync(
        CancellationToken cancellationToken = default
    );
}

public class UserService : IUserService
{
    #region Fields

    private readonly ILogger<UserService> _logger;
    private readonly IUserRepository _userRepository;
    private readonly IAppDbContextAction _appDbContextAction;

    #endregion

    #region Ctor

    public UserService(
        ILogger<UserService> logger,
        IUserRepository userRepository,
        IAppDbContextAction appDbContextAction
    )
    {
        _logger = logger;
        _userRepository = userRepository;
        _appDbContextAction = appDbContextAction;
    }

    #endregion

    #region Methods

    public async Task<User> Save(User entity, CancellationToken cancellationToken = default)
    {
        _logger.Log(LogLevel.Information,
            Localize.Log.Method(GetType(), nameof(Save), $"{entity.GetType().Name} {entity.Id}"));

        _userRepository.Save(entity);
        await _appDbContextAction.CommitAsync(cancellationToken);
        
        return entity;
    }

    public async Task Delete(User entity, CancellationToken cancellationToken = default)
    {
        _logger.Log(LogLevel.Information,
            Localize.Log.Method(GetType(), nameof(Delete), $"{entity.GetType().Name} {entity.Id}"));

        _userRepository.Delete(entity);
        await _appDbContextAction.CommitAsync(cancellationToken);
    }

    public async Task<User> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _userRepository.SingleOrDefaultAsync(_ => _.Id == id);

        _logger.Log(LogLevel.Information,
            Localize.Log.Method(GetType(), nameof(GetByIdAsync), $"{entity.GetType().Name} {entity.Id}"));

        return entity;
    }

    public async Task<User> GetByEmailAsync(string email)
    {
        var entity = await _userRepository.SingleOrDefaultAsync(_ => _.Email == email);

        _logger.Log(LogLevel.Information,
            Localize.Log.Method(GetType(), nameof(GetByEmailAsync), $"{entity.GetType().Name} {entity.Id}"));

        return entity;
    }

    public async Task<IReadOnlyCollection<User>> GetByPhoneNumberAsync(
        string phoneNumber,
        CancellationToken cancellationToken = default
    )
    {
        var result = await _userRepository.QueryMany(_ => _.PhoneNumber == phoneNumber).ToArrayAsync(cancellationToken);

        _logger.Log(LogLevel.Information,
            Localize.Log.Method(GetType(), nameof(GetByPhoneNumberAsync), $"{result.GetType().Name} {result.Length}"));

        return result;
    }

    public async Task<IReadOnlyCollection<User>> GetExpiredByDisableSignInUntil(
        CancellationToken cancellationToken = default
    )
    {
        var result = await _userRepository.QueryMany(_ => _.DisableSignInUntil < DateTimeOffset.UtcNow)
            .ToArrayAsync(cancellationToken);

        _logger.Log(LogLevel.Information,
            Localize.Log.Method(GetType(), nameof(GetExpiredByDisableSignInUntil),
                $"{result.GetType().Name} {result.Length}"));

        return result;
    }

    public async Task<IReadOnlyCollection<User>> GetInRangeByLastSignIn(
        DateTimeOffset @from,
        DateTimeOffset to,
        CancellationToken cancellationToken = default
    )
    {
        var result = await _userRepository.QueryMany(_ => _.LastSignIn >= @from && _.LastSignIn <= to)
            .ToArrayAsync(cancellationToken);

        _logger.Log(LogLevel.Information,
            Localize.Log.Method(GetType(), nameof(GetInRangeByLastSignIn), $"{result.GetType().Name} {result.Length}"));

        return result;
    }

    public async Task<IReadOnlyCollection<User>> GetInRangeByLastActivity(
        DateTimeOffset @from,
        DateTimeOffset to,
        CancellationToken cancellationToken = default
    )
    {
        var result = await _userRepository.QueryMany(_ => _.LastActivity >= @from && _.LastActivity <= to)
            .ToArrayAsync(cancellationToken);

        _logger.Log(LogLevel.Information,
            Localize.Log.Method(GetType(), nameof(GetInRangeByLastActivity),
                $"{result.GetType().Name} {result.Length}"));

        return result;
    }

    public async Task<IReadOnlyCollection<User>> GetByLastIpAddress(
        string ipAddress,
        CancellationToken cancellationToken = default
    )
    {
        var result = await _userRepository.QueryMany(_ => _.LastIpAddress == ipAddress).ToArrayAsync(cancellationToken);

        _logger.Log(LogLevel.Information,
            Localize.Log.Method(GetType(), nameof(GetByLastIpAddress), $"{result.GetType().Name} {result.Length}"));

        return result;
    }

    public async Task PurgeAsync(CancellationToken cancellationToken = default)
    {
        _logger.Log(LogLevel.Information, Localize.Log.Method(GetType(), nameof(PurgeAsync), null));
        
        var result = await _userRepository.QueryMany(_ => _.LastActivity <= DateTimeOffset.UtcNow.AddDays(-1) && _.IsTemporary)
            .ToArrayAsync(cancellationToken);
        
        _userRepository.Delete(result);
        await _appDbContextAction.CommitAsync(cancellationToken);
    }

    #endregion
}