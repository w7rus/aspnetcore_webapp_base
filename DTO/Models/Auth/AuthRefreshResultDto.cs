using System;
using Common.Models;
using Common.Models.Base;

namespace DTO.Models.Auth
{
    public class AuthRefreshResultDto : DTOResultBase
    {
        /// <summary>
        /// Id of a user signed in
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Only available when AuthSignIn.UseCookies is false
        /// </summary>
        public string JsonWebToken { get; set; }

        /// <summary>
        /// DateTime before JsonWebToken considered as valid
        /// </summary>
        public DateTimeOffset JsonWebTokenExpiresAt { get; set; }

        /// <summary>
        /// Only available when AuthSignIn.UseCookies is false
        /// </summary>
        public string RefreshToken { get; set; }

        /// <summary>
        /// DateTime before RefreshToken considered as valid
        /// </summary>
        public DateTimeOffset RefreshTokenExpiresAt { get; set; }
    }
}