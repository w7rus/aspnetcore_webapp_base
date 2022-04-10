using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Entities.Base;

namespace Domain.Entities
{
    public class UserProfile : EntityBase<Guid>
    {
        /// <summary>
        /// Username of a UserProfile
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// First name of a UserProfile
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Last name of a UserProfile
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Description of a UserProfile
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Id of a User this UserProfile references
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// [Proxy]
        /// User this UserProfile references
        /// </summary>
        public virtual User User { get; set; }
    }
}