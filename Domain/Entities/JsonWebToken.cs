using System;
using Domain.Entities.Base;
using Microsoft.EntityFrameworkCore;

namespace Domain.Entities
{
    public class JsonWebToken : EntityBase<Guid>
    {
        /// <summary>
        /// String representation of a token
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Date past which JWT token counts as expired
        /// </summary>
        public DateTimeOffset ExpiresAt { get; set; }
        
        /// <summary>
        /// Date past which JWT token must be removed
        /// </summary>
        public DateTimeOffset RemoveAfter { get; set; }

        public Guid UserId { get; set; }
        public virtual User User { get; set; }
    }
}