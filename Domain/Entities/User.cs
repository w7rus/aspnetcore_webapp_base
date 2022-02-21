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
        /// Email of a User
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Is Users email validated?
        /// </summary>
        public bool IsEmailValidated { get; set; }

        /// <summary>
        /// Phone number of a User
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Is Users phone number validated?
        /// </summary>
        public bool IsPhoneNumberVerified { get; set; }

        /// <summary>
        /// Password of a User
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Failed sign in attempts count of a User
        /// </summary>
        public int FailedSignInAttempts { get; set; }

        /// <summary>
        /// Date until which sign in is disabled for a User
        /// </summary>
        public DateTimeOffset? DisableSignInUntil { get; set; }

        //TODO: Make use of this field (Also auth refresh)
        /// <summary>
        /// Date of last sign in of a User
        /// </summary>
        public DateTimeOffset LastSignIn { get; set; }

        //TODO: Make use of this field (Add middleware)
        /// <summary>
        /// Date of last activity of a User
        /// </summary>
        public DateTimeOffset LastActivity { get; set; }

        /// <summary>
        /// Last IP Address of a User
        /// </summary>
        public string LastIpAddress { get; set; }

        /// <summary>
        /// Group mappings of the entity
        /// Multiple-to-multiple table that links groups assigned to the entity
        /// </summary>
        public virtual ICollection<UserToUserGroupMapping> UserToUserGroupMappings { get; set; }

        /// <summary>
        /// Profile of a user (if exists)
        /// </summary>
        public virtual UserProfile UserProfile { get; set; }
    }
}