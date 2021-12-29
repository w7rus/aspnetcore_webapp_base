using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Entities.Base;

namespace Domain.Entities
{
    public class UserProfile : EntityBase<Guid>
    {
        /// <summary>
        /// Username of a user
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// First name of a user
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Last name of a user
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Description of a user
        /// </summary>
        public string Description { get; set; }

        public Guid UserId { get; set; }
        public virtual User User { get; set; }
    }
}