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
        /// Date after JsonWebToken is expired
        /// </summary>
        public DateTimeOffset ExpiresAt { get; set; }

        /// <summary>
        /// Date after JsonWebToken is pruneable
        /// </summary>
        public DateTimeOffset DeleteAfter { get; set; }

        /// <summary>
        /// Id of a User this JsonWebToken references
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// [Proxy]
        /// User this JsonWebToken references
        /// </summary>
        public virtual User User { get; set; }
    }
}