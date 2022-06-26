using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BLL.Services.Base;
using Common.Models;
using DAL.Data;
using DAL.Extensions;
using DAL.Repository;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BLL.Services.Entity;

/// <summary>
/// Service to work with RefreshToken entity
/// </summary>
public interface IRefreshTokenEntityService : IEntityServiceBase<RefreshToken>
{
    Task<RefreshToken> GetByTokenAsync(string token);
    Task PurgeAsync(CancellationToken cancellationToken = default);
}

public class RefreshTokenEntityService : IRefreshTokenEntityService
{
    #region Fields

    private readonly ILogger<RefreshTokenEntityService> _logger;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IAppDbContextAction _appDbContextAction;

    #endregion

    #region Ctor

    public RefreshTokenEntityService(
        ILogger<RefreshTokenEntityService> logger,
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
            Localize.Log.Method(GetType(), nameof(Save), $"{entity?.GetType().Name} {entity?.Id}"));

        _refreshTokenRepository.Save(entity);
        await _appDbContextAction.CommitAsync(cancellationToken);
        
        return entity;
    }

    public async Task Delete(RefreshToken entity, CancellationToken cancellationToken = default)
    {
        _logger.Log(LogLevel.Information,
            Localize.Log.Method(GetType(), nameof(Delete), $"{entity?.GetType().Name} {entity?.Id}"));

        _refreshTokenRepository.Delete(entity);
        await _appDbContextAction.CommitAsync(cancellationToken);
    }

    public async Task<RefreshToken> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _refreshTokenRepository.SingleOrDefaultAsync(_ => _.Id == id);

        _logger.Log(LogLevel.Information,
            Localize.Log.Method(GetType(), nameof(GetByIdAsync), $"{entity?.GetType().Name} {entity?.Id}"));

        return entity;
    }

    public async Task<RefreshToken> GetByTokenAsync(string token)
    {
        var entity = await _refreshTokenRepository.SingleOrDefaultAsync(_ => _.Token == token);

        _logger.Log(LogLevel.Information,
            Localize.Log.Method(GetType(), nameof(GetByTokenAsync), $"{entity?.GetType().Name} {entity?.Id}"));

        return entity;
    }

    public async Task PurgeAsync(CancellationToken cancellationToken = default)
    {
        _logger.Log(LogLevel.Information,
            Localize.Log.Method(GetType(), nameof(PurgeAsync), null));
            
        var query = _refreshTokenRepository
            .QueryMany(_ => _.ExpiresAt < DateTimeOffset.UtcNow);

        for (var page = 1;;page += 1)
        {
            var entities = await query.GetPage(new PageModel()
            {
                Page = page,
                PageSize = 512
            }).ToArrayAsync(cancellationToken);

            _refreshTokenRepository.Delete(entities);
            await _appDbContextAction.CommitAsync(cancellationToken);
            
            if (entities.Length < 512)
                break;
        }
    }

    #endregion
}