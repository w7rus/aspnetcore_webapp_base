using System;
using Domain.Entities.Base;
using Microsoft.EntityFrameworkCore;

namespace Domain.Entities
{
    public class JsonWebToken : EntityBase<Guid>
    {
        /// <summary>
        /// String representation of a JsonWebToken
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Date past which JsonWebToken expires
        /// </summary>
        public DateTimeOffset ExpiresAt { get; set; }

        /// <summary>
        /// Date past which JsonWebToken is pruneable
        /// </summary>
        public DateTimeOffset DeleteAfter { get; set; }

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