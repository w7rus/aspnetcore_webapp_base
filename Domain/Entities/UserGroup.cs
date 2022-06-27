using System;
using Domain.Entities.Base;

namespace Domain.Entities;

public class UserGroup : EntityGroupBase<User, UserGroup>
{
    public bool IsSystem { get; set; }
    public long Priority { get; set; }
    public Guid? OwnerUserId { get; set; }
    public virtual User OwnerUser { get; set; }
}