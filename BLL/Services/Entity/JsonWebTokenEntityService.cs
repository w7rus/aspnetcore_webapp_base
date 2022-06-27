using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
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
using Microsoft.IdentityModel.Tokens;

namespace BLL.Services.Entity;

public interface IJsonWebTokenEntityService : IEntityServiceBase<JsonWebToken>
{
    Task<JsonWebToken> GetByTokenAsync(string token);

    Task PurgeAsync(CancellationToken cancellationToken = default);

    string CreateWithClaims(
        string issuerSigningKey,
        string issuer,
        string audience,
        IEnumerable<Claim> claims,
        DateTime expires
    );
}

public class JsonWebTokenEntityService : IJsonWebTokenEntityService
{
    #region Ctor

    public JsonWebTokenEntityService(
        ILogger<JsonWebTokenEntityService> logger,
        IJsonWebTokenRepository jsonWebTokenRepository,
        IAppDbContextAction appDbContextAction
    )
    {
        _logger = logger;
        _jsonWebTokenRepository = jsonWebTokenRepository;
        _appDbContextAction = appDbContextAction;
    }

    #endregion

    #region Fields

    private readonly ILogger<JsonWebTokenEntityService> _logger;
    private readonly IJsonWebTokenRepository _jsonWebTokenRepository;
    private readonly IAppDbContextAction _appDbContextAction;

    #endregion

    #region Methods

    public async Task<JsonWebToken> Save(JsonWebToken entity, CancellationToken cancellationToken = default)
    {
        _logger.Log(LogLevel.Information,
            Localize.Log.Method(GetType(), nameof(Save), $"{entity?.GetType().Name} {entity?.Id}"));

        _jsonWebTokenRepository.Save(entity);
        await _appDbContextAction.CommitAsync(cancellationToken);

        return entity;
    }

    public async Task Delete(JsonWebToken entity, CancellationToken cancellationToken = default)
    {
        _logger.Log(LogLevel.Information,
            Localize.Log.Method(GetType(), nameof(Delete), $"{entity?.GetType().Name} {entity?.Id}"));

        _jsonWebTokenRepository.Delete(entity);
        await _appDbContextAction.CommitAsync(cancellationToken);
    }

    public async Task<JsonWebToken> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _jsonWebTokenRepository.SingleOrDefaultAsync(_ => _.Id == id);

        _logger.Log(LogLevel.Information,
            Localize.Log.Method(GetType(), nameof(GetByIdAsync), $"{entity?.GetType().Name} {entity?.Id}"));

        return entity;
    }

    public async Task<JsonWebToken> GetByTokenAsync(string token)
    {
        var entity = await _jsonWebTokenRepository.SingleOrDefaultAsync(_ => _.Token == token);

        _logger.Log(LogLevel.Information,
            Localize.Log.Method(GetType(), nameof(GetByIdAsync), $"{entity?.GetType().Name} {entity?.Id}"));

        return entity;
    }

    public async Task PurgeAsync(CancellationToken cancellationToken = default)
    {
        _logger.Log(LogLevel.Information,
            Localize.Log.Method(GetType(), nameof(PurgeAsync), null));

        var query = _jsonWebTokenRepository
            .QueryMany(_ => _.DeleteAfter < DateTimeOffset.UtcNow)
            .OrderBy(_ => _.CreatedAt);

        for (var page = 1;; page += 1)
        {
            var entities = await query.GetPage(new PageModel
            {
                Page = page,
                PageSize = 512
            }).ToArrayAsync(cancellationToken);

            _jsonWebTokenRepository.Delete(entities);
            await _appDbContextAction.CommitAsync(cancellationToken);

            if (entities.Length < 512)
                break;
        }
    }

    public string CreateWithClaims(
        string issuerSigningKey,
        string issuer,
        string audience,
        IEnumerable<Claim> claims,
        DateTime expires
    )
    {
        _logger.Log(LogLevel.Information,
            Localize.Log.Method(GetType(), nameof(CreateWithClaims), null));

        var symmetricSecurityKey =
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(issuerSigningKey));
        var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

        var jwtSecurityToken = new JwtSecurityToken(issuer, audience,
            claims ?? new List<Claim>(), expires: expires, signingCredentials: signingCredentials);

        return new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
    }

    #endregion
}