using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Entities.Base;

namespace Domain.Entities
{
    public class User : EntityBase<Guid>
    {
        /// <summary>
        /// Email of a user
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Is user email verified?
        /// </summary>
        public bool IsEmailValidated { get; set; }

        /// <summary>
        /// Phone number of a user
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Is user phone number verified?
        /// </summary>
        public bool IsPhoneNumberVerified { get; set; }

        /// <summary>
        /// Password of a user
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Amount of failed sign in attempts
        /// </summary>
        public int FailedSignInAttempts { get; set; }

        /// <summary>
        /// Time until sign in is disabled for a user
        /// </summary>
        public DateTimeOffset? DisableSignInUntil { get; set; }

        /// <summary>
        /// Time of last sign in of a user
        /// </summary>
        public DateTimeOffset LastSignIn { get; set; }

        /// <summary>
        /// Time of last activity of a user
        /// </summary>
        public DateTimeOffset LastActivity { get; set; }

        /// <summary>
        /// Last IP Address of a user
        /// </summary>
        public string LastIpAddress { get; set; }

        /// <summary>
        /// Group mappings of the entity
        /// Multiple-to-multiple table that links groups assigned to the entity
        /// </summary>
        public virtual ICollection<UserToGroupMapping> UserToGroupMappings { get; set; }

        /// <summary>
        /// Profile of a user (if exists)
        /// </summary>
        public virtual UserProfile UserProfile { get; set; }
    }
}