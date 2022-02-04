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
        Task<JsonWebToken> Add(
            string token,
            DateTimeOffset expiresAt,
            DateTimeOffset removeAfter,
            Guid userId,
            CancellationToken cancellationToken = new()
        );

        Task<JsonWebToken> GetByTokenAsync(string token);

        Task<IReadOnlyCollection<JsonWebToken>> GetExpiredByUserIdAsync(
            Guid userId,
            CancellationToken cancellationToken = new()
        );

        Task<IReadOnlyCollection<JsonWebToken>> GetDeleteAfterAsync(
            CancellationToken cancellationToken = new()
        );

        Task PurgeAsync(
            CancellationToken cancellationToken = new()
        );

        string CreateWithClaims(
            string issuerSigningKey,
            string issuer,
            string audience,
            IEnumerable<Claim> claims,
            DateTime expires
        );

        Task<JsonWebToken> GetFromHttpContext(CancellationToken cancellationToken = new());
    }

    public class JsonWebTokenService : IJsonWebTokenService
    {
        #region Fields

        private readonly ILogger<JsonWebTokenService> _logger;
        private readonly IJsonWebTokenRepository _jsonWebTokenRepository;
        private readonly IAppDbContextAction _appDbContextAction;
        private readonly HttpContext _httpContext;

        #endregion

        #region Ctor

        public JsonWebTokenService(
            ILogger<JsonWebTokenService> logger,
            IJsonWebTokenRepository jsonWebTokenRepository,
            IAppDbContextAction appDbContextAction,
            IHttpContextAccessor httpContextAccessor
        )
        {
            _logger = logger;
            _jsonWebTokenRepository = jsonWebTokenRepository;
            _appDbContextAction = appDbContextAction;
            _httpContext = httpContextAccessor.HttpContext;
        }

        #endregion

        #region Methods

        public async Task Save(JsonWebToken entity, CancellationToken cancellationToken = new())
        {
            _jsonWebTokenRepository.Save(entity);
            await _appDbContextAction.CommitAsync(cancellationToken);
        }

        public async Task<JsonWebToken> Add(
            string token,
            DateTimeOffset expiresAt,
            DateTimeOffset removeAfter,
            Guid userId,
            CancellationToken cancellationToken = new()
        )
        {
            var entity = new JsonWebToken
            {
                Token = token,
                ExpiresAt = expiresAt,
                DeleteAfter = removeAfter,
                UserId = userId,
            };

            await Save(entity, cancellationToken);
            return entity;
        }

        public async Task Delete(JsonWebToken entity, CancellationToken cancellationToken = new())
        {
            _jsonWebTokenRepository.Delete(entity);
            await _appDbContextAction.CommitAsync(cancellationToken);
        }

        public async Task<JsonWebToken> GetByIdAsync(Guid id, CancellationToken cancellationToken = new())
        {
            return await _jsonWebTokenRepository.SingleOrDefaultAsync(_ => _.Id == id);
        }

        public async Task<JsonWebToken> GetByTokenAsync(string token)
        {
            return await _jsonWebTokenRepository.SingleOrDefaultAsync(_ => _.Token == token);
        }

        public async Task<IReadOnlyCollection<JsonWebToken>> GetExpiredByUserIdAsync(
            Guid userId,
            CancellationToken cancellationToken = new()
        )
        {
            return await _jsonWebTokenRepository
                .QueryMany(_ => _.UserId == userId && _.ExpiresAt < DateTimeOffset.UtcNow)
                .ToArrayAsync(cancellationToken);
        }

        public async Task<IReadOnlyCollection<JsonWebToken>> GetDeleteAfterAsync(
            CancellationToken cancellationToken = new()
        )
        {
            return await _jsonWebTokenRepository
                .QueryMany(_ => _.DeleteAfter < DateTimeOffset.UtcNow)
                .ToArrayAsync(cancellationToken);
        }

        public async Task PurgeAsync(CancellationToken cancellationToken = new())
        {
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
            var symmetricSecurityKey =
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(issuerSigningKey));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(issuer: issuer, audience: audience,
                claims: claims ?? new List<Claim>(), expires: expires,
                signingCredentials: signingCredentials);

            return new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        }

        public async Task<JsonWebToken> GetFromHttpContext(CancellationToken cancellationToken = new())
        {
            if (!Guid.TryParse(_httpContext.User.Claims.SingleOrDefault(_ => _.Type == ClaimKey.JsonWebTokenId)?.Value,
                    out var jsonWebTokenId))
                throw new ApplicationException(Localize.Error.JsonWebTokenIdRetrievalFailed);

            return await GetByIdAsync(jsonWebTokenId, cancellationToken);
        }

        #endregion
    }
}