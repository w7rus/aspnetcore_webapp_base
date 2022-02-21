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
        /// Date past which RefreshToken expires
        /// </summary>
        public DateTimeOffset ExpiresAt { get; set; }

        /// <summary>
        /// Id of a User this JsonWebToken belongs to
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// User this JsonWebToken belongs to
        /// </summary>
        public virtual User User { get; set; }
    }
}