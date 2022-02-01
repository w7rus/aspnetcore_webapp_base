using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BLL.Services.Base;
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
    new Task Save(RefreshToken refreshToken, CancellationToken cancellationToken);
    Task<RefreshToken> Add(string token, DateTimeOffset expiresAt, Guid userId, CancellationToken cancellationToken);
    new Task Delete(RefreshToken refreshToken, CancellationToken cancellationToken);
    new Task<RefreshToken> GetByIdAsync(Guid id);
    Task<RefreshToken> GetByTokenAsync(string token);
    Task<IReadOnlyCollection<RefreshToken>> GetExpiredByUserIdAsync(Guid userId, CancellationToken cancellationToken);
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

    public async Task Save(RefreshToken refreshToken, CancellationToken cancellationToken)
    {
        _refreshTokenRepository.Save(refreshToken);
        await _appDbContextAction.CommitAsync(cancellationToken);
    }

    public async Task<RefreshToken> Add(
        string token,
        DateTimeOffset expiresAt,
        Guid userId,
        CancellationToken cancellationToken
    )
    {
        var entity = new RefreshToken
        {
            Token = token,
            ExpiresAt = expiresAt,
            UserId = userId,
        };

        await Save(entity, cancellationToken);
        return entity;
    }

    public async Task Delete(RefreshToken refreshToken, CancellationToken cancellationToken)
    {
        _refreshTokenRepository.Delete(refreshToken);
        await _appDbContextAction.CommitAsync(cancellationToken);
    }

    public async Task<RefreshToken> GetByIdAsync(Guid id)
    {
        return await _refreshTokenRepository.SingleOrDefaultAsync(_ => _.Id == id);
    }

    public async Task<RefreshToken> GetByTokenAsync(string token)
    {
        return await _refreshTokenRepository.SingleOrDefaultAsync(_ => _.Token == token);
    }

    public async Task<IReadOnlyCollection<RefreshToken>> GetExpiredByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken
    )
    {
        return await _refreshTokenRepository.QueryMany(_ => _.UserId == userId && _.ExpiresAt < DateTimeOffset.UtcNow)
            .ToArrayAsync(cancellationToken);
    }

    #endregion
}