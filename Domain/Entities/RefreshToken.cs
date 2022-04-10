using System;
using Domain.Entities.Base;

namespace Domain.Entities
{
    public class RefreshToken : EntityBase<Guid>
    {
        /// <summary>
        /// String representation of a RefreshToken
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Date after RefreshToken is expired
        /// </summary>
        public DateTimeOffset ExpiresAt { get; set; }

        /// <summary>
        /// Id of a User this RefreshToken references
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// [Proxy]
        /// User this RefreshToken references
        /// </summary>
        public virtual User User { get; set; }
    }
}