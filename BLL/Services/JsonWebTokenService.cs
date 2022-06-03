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
using DAL.Repository;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace BLL.Services
{
    /// <summary>
    /// Service to work with JsonWebToken entity
    /// </summary>
    public interface IJsonWebTokenService : IEntityServiceBase<JsonWebToken>
    {
        /// <summary>
        /// Gets entity with equal Token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<JsonWebToken> GetByTokenAsync(string token);

        /// <summary>
        /// Gets entities with equal UserId & DateTime.UtcNow() less than ExpiresAt
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<IReadOnlyCollection<JsonWebToken>> GetExpiredByUserIdAsync(
            Guid userId,
            CancellationToken cancellationToken = default
        );

        /// <summary>
        /// Gets entities with DeleteAfter that is less than current date
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<IReadOnlyCollection<JsonWebToken>> GetDeleteAfterAsync(
            CancellationToken cancellationToken = default
        );

        /// <summary>
        /// Deletes entities with DeleteAfter that is less than current date
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task PurgeAsync(
            CancellationToken cancellationToken = default
        );

        /// <summary>
        /// Creates new JWT object
        /// </summary>
        /// <param name="issuerSigningKey"></param>
        /// <param name="issuer"></param>
        /// <param name="audience"></param>
        /// <param name="claims"></param>
        /// <param name="expires"></param>
        /// <returns></returns>
        string CreateWithClaims(
            string issuerSigningKey,
            string issuer,
            string audience,
            IEnumerable<Claim> claims,
            DateTime expires
        );
    }

    public class JsonWebTokenService : IJsonWebTokenService
    {
        #region Fields

        private readonly ILogger<JsonWebTokenService> _logger;
        private readonly IJsonWebTokenRepository _jsonWebTokenRepository;
        private readonly IAppDbContextAction _appDbContextAction;

        #endregion

        #region Ctor

        public JsonWebTokenService(
            ILogger<JsonWebTokenService> logger,
            IJsonWebTokenRepository jsonWebTokenRepository,
            IAppDbContextAction appDbContextAction
        )
        {
            _logger = logger;
            _jsonWebTokenRepository = jsonWebTokenRepository;
            _appDbContextAction = appDbContextAction;
        }

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

        public async Task<IReadOnlyCollection<JsonWebToken>> GetExpiredByUserIdAsync(
            Guid userId,
            CancellationToken cancellationToken = default
        )
        {
            var result = await _jsonWebTokenRepository
                .QueryMany(_ => _.UserId == userId && _.ExpiresAt < DateTimeOffset.UtcNow)
                .ToArrayAsync(cancellationToken);

            _logger.Log(LogLevel.Information,
                Localize.Log.Method(GetType(), nameof(GetExpiredByUserIdAsync),
                    $"{result?.GetType().Name} {result?.Length}"));

            return result;
        }

        public async Task<IReadOnlyCollection<JsonWebToken>> GetDeleteAfterAsync(
            CancellationToken cancellationToken = default
        )
        {
            var result = await _jsonWebTokenRepository
                .QueryMany(_ => _.DeleteAfter < DateTimeOffset.UtcNow)
                .ToArrayAsync(cancellationToken);

            _logger.Log(LogLevel.Information,
                Localize.Log.Method(GetType(), nameof(GetExpiredByUserIdAsync),
                    $"{result?.GetType().Name} {result?.Length}"));

            return result;
        }

        public async Task PurgeAsync(CancellationToken cancellationToken = default)
        {
            _logger.Log(LogLevel.Information,
                Localize.Log.Method(GetType(), nameof(PurgeAsync), null));

            var jsonWebTokens = await GetDeleteAfterAsync(cancellationToken);

            _jsonWebTokenRepository.Delete(jsonWebTokens);
            await _appDbContextAction.CommitAsync(cancellationToken);
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
}