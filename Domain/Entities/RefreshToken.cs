using System;
using Domain.Entities.Base;

namespace Domain.Entities
{
    public class RefreshToken : EntityBase<Guid>
    {
        /// <summary>
        /// String representation of a token
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Date past which refresh token counts as expired
        /// </summary>
        public DateTimeOffset ExpiresAt { get; set; }

        public Guid UserId { get; set; }
        public virtual User User { get; set; }
    }
}