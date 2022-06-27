using System;
using System.Collections.Generic;
using Domain.Entities.Base;

namespace Domain.Entities;

public class User : EntityBase<Guid>
{
    public string Email { get; set; }
    public bool IsEmailVerified { get; set; }
    public string PhoneNumber { get; set; }
    public bool IsPhoneNumberVerified { get; set; }
    public string Password { get; set; }
    public int FailedSignInAttempts { get; set; }
    public DateTimeOffset? DisableSignInUntil { get; set; }
    public DateTimeOffset LastSignIn { get; set; }
    public DateTimeOffset LastActivity { get; set; }
    public string LastIpAddress { get; set; }
    public bool IsTemporary { get; set; }
    public virtual ICollection<UserToUserGroupMapping> UserToUserGroupMappings { get; set; }
    public virtual UserProfile UserProfile { get; set; }
    public virtual ICollection<File> Files { get; set; }
}