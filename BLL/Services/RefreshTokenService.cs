using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BLL.Services.Base;
using Common.Models;
using DAL.Data;
using DAL.Repository;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BLL.Services;

/// <summary>
/// Service to work with RefreshToken entity
/// </summary>
public interface IRefreshTokenService : IEntityServiceBase<RefreshToken>
{
    /// <summary>
    /// Gets entity with Token that equals given one
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    Task<RefreshToken> GetByTokenAsync(string token);

    /// <summary>
    /// Gets entities with UserId that equals given one & ExpiresAt that is less than current date
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IReadOnlyCollection<RefreshToken>> GetExpiredByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default
    );
}

public class RefreshTokenService : IRefreshTokenService
{
    #region Fields

    private readonly ILogger<RefreshTokenService> _logger;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IAppDbContextAction _appDbContextAction;

    #endregion

    #region Ctor

    public RefreshTokenService(
        ILogger<RefreshTokenService> logger,
        IRefreshTokenRepository refreshTokenRepository,
        IAppDbContextAction appDbContextAction
    )
    {
        _logger = logger;
        _refreshTokenRepository = refreshTokenRepository;
        _appDbContextAction = appDbContextAction;
    }

    #endregion

    #region Methods

    public async Task<RefreshToken> Save(RefreshToken entity, CancellationToken cancellationToken = default)
    {
        _logger.Log(LogLevel.Information,
            Localize.Log.Method(GetType(), nameof(Save), $"{entity.GetType().Name} {entity.Id}"));

        _refreshTokenRepository.Save(entity);
        await _appDbContextAction.CommitAsync(cancellationToken);
        
        return entity;
    }

    public async Task Delete(RefreshToken entity, CancellationToken cancellationToken = default)
    {
        _logger.Log(LogLevel.Information,
            Localize.Log.Method(GetType(), nameof(Delete), $"{entity.GetType().Name} {entity.Id}"));

        _refreshTokenRepository.Delete(entity);
        await _appDbContextAction.CommitAsync(cancellationToken);
    }

    public async Task<RefreshToken> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _refreshTokenRepository.SingleOrDefaultAsync(_ => _.Id == id);

        _logger.Log(LogLevel.Information,
            Localize.Log.Method(GetType(), nameof(GetByIdAsync), $"{entity.GetType().Name} {entity.Id}"));

        return entity;
    }

    public async Task<RefreshToken> GetByTokenAsync(string token)
    {
        var entity = await _refreshTokenRepository.SingleOrDefaultAsync(_ => _.Token == token);

        _logger.Log(LogLevel.Information,
            Localize.Log.Method(GetType(), nameof(GetByTokenAsync), $"{entity.GetType().Name} {entity.Id}"));

        return entity;
    }

    public async Task<IReadOnlyCollection<RefreshToken>> GetExpiredByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default
    )
    {
        var result = await _refreshTokenRepository
            .QueryMany(_ => _.UserId == userId && _.ExpiresAt < DateTimeOffset.UtcNow)
            .ToArrayAsync(cancellationToken);

        _logger.Log(LogLevel.Information,
            Localize.Log.Method(GetType(), nameof(GetExpiredByUserIdAsync),
                $"{result.GetType().Name} {result.Length}"));

        return result;
    }

    #endregion
}