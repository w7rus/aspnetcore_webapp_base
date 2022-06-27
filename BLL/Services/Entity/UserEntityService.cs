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

/// <summary>
/// Service to work with User entity
/// </summary>
public interface IUserEntityService : IEntityServiceBase<User>
{
    Task<User> GetByEmailAsync(string email);

    Task<IReadOnlyCollection<User>> GetByPhoneNumberAsync(
        string phoneNumber,
        PageModel pageModel,
        CancellationToken cancellationToken = default
    );

    Task PurgeAsync(
        CancellationToken cancellationToken = default
    );
}

public class UserEntityService : IUserEntityService
{
    #region Fields

    private readonly ILogger<UserEntityService> _logger;
    private readonly IUserRepository _userRepository;
    private readonly IAppDbContextAction _appDbContextAction;

    #endregion

    #region Ctor

    public UserEntityService(
        ILogger<UserEntityService> logger,
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
            Localize.Log.Method(GetType(), nameof(Save), $"{entity?.GetType().Name} {entity?.Id}"));

        _userRepository.Save(entity);
        await _appDbContextAction.CommitAsync(cancellationToken);

        return entity;
    }

    public async Task Delete(User entity, CancellationToken cancellationToken = default)
    {
        _logger.Log(LogLevel.Information,
            Localize.Log.Method(GetType(), nameof(Delete), $"{entity?.GetType().Name} {entity?.Id}"));

        _userRepository.Delete(entity);
        await _appDbContextAction.CommitAsync(cancellationToken);
    }

    public async Task<User> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _userRepository.SingleOrDefaultAsync(_ => _.Id == id);

        _logger.Log(LogLevel.Information,
            Localize.Log.Method(GetType(), nameof(GetByIdAsync), $"{entity?.GetType().Name} {entity?.Id}"));

        return entity;
    }

    public async Task<User> GetByEmailAsync(string email)
    {
        var entity = await _userRepository.SingleOrDefaultAsync(_ => _.Email == email);

        _logger.Log(LogLevel.Information,
            Localize.Log.Method(GetType(), nameof(GetByEmailAsync), $"{entity?.GetType().Name} {entity?.Id}"));

        return entity;
    }
    
    public async Task<IReadOnlyCollection<User>> GetByPhoneNumberAsync(
        string phoneNumber,
        PageModel pageModel,
        CancellationToken cancellationToken = default
    )
    {
        var result = await _userRepository
            .QueryMany(_ => _.PhoneNumber == phoneNumber)
            .GetPage(pageModel)
            .ToArrayAsync(cancellationToken);

        _logger.Log(LogLevel.Information,
            Localize.Log.Method(GetType(), nameof(GetByPhoneNumberAsync),
                $"{result.GetType().Name} {result.Length}"));

        return result;
    }
    
    public async Task PurgeAsync(CancellationToken cancellationToken = default)
    {
        _logger.Log(LogLevel.Information, Localize.Log.Method(GetType(), nameof(PurgeAsync), null));

        var query = _userRepository
            .QueryMany(_ => _.LastActivity <= DateTimeOffset.UtcNow.AddDays(-1) && _.IsTemporary)
            .OrderBy(_ => _.CreatedAt);
        
        for (var page = 1;;page += 1)
        {
            var entities = await query.GetPage(new PageModel()
            {
                Page = page,
                PageSize = 512
            }).ToArrayAsync(cancellationToken);

            _userRepository.Delete(entities);
            await _appDbContextAction.CommitAsync(cancellationToken);
            
            if (entities.Length < 512)
                break;
        }
    }

    #endregion
}