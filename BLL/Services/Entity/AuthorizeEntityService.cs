using System;
using System.Collections.Generic;
using System.Linq;
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

public interface IAuthorizeEntityService : IEntityServiceBase<Authorize>
{
    Task PurgeAsync(CancellationToken cancellationToken = default);
}

public class AuthorizeEntityService : IAuthorizeEntityService
{
    #region Fields

    private readonly ILogger<AuthorizeEntityService> _logger;
    private readonly IAuthorizeRepository _authorizeRepository;
    private readonly IAppDbContextAction _appDbContextAction;

    #endregion

    #region Ctor

    public AuthorizeEntityService(
        ILogger<AuthorizeEntityService> logger,
        IAuthorizeRepository authorizeRepository,
        IAppDbContextAction appDbContextAction
    )
    {
        _logger = logger;
        _authorizeRepository = authorizeRepository;
        _appDbContextAction = appDbContextAction;
    }

    #endregion

    #region Methods

    public async Task<Authorize> Save(Authorize entity, CancellationToken cancellationToken = default)
    {
        _logger.Log(LogLevel.Information,
            Localize.Log.Method(GetType(), nameof(Save), $"{entity?.GetType().Name} {entity?.Id}"));

        _authorizeRepository.Save(entity);
        await _appDbContextAction.CommitAsync(cancellationToken);

        return entity;
    }

    public async Task Delete(Authorize entity, CancellationToken cancellationToken = default)
    {
        _logger.Log(LogLevel.Information,
            Localize.Log.Method(GetType(), nameof(Delete), $"{entity?.GetType().Name} {entity?.Id}"));

        _authorizeRepository.Delete(entity);
        await _appDbContextAction.CommitAsync(cancellationToken);
    }

    public async Task<Authorize> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _authorizeRepository.SingleOrDefaultAsync(_ => _.Id == id);

        _logger.Log(LogLevel.Information,
            Localize.Log.Method(GetType(), nameof(GetByIdAsync), $"{entity?.GetType().Name} {entity?.Id}"));

        return entity;
    }

    public async Task PurgeAsync(CancellationToken cancellationToken = default)
    {
        
        _logger.Log(LogLevel.Information,
            Localize.Log.Method(GetType(), nameof(PurgeAsync), null));
        
        var query = _authorizeRepository
            .QueryMany(_ => _.CreatedAt < DateTimeOffset.UtcNow.AddDays(1));

        for (var page = 1;;page += 1)
        {
            var entities = await query.GetPage(new PageModel()
            {
                Page = page,
                PageSize = 512
            }).ToArrayAsync(cancellationToken);

            _authorizeRepository.Delete(entities);
            await _appDbContextAction.CommitAsync(cancellationToken);
            
            if (entities.Length < 512)
                break;
        }
    }

    #endregion
}