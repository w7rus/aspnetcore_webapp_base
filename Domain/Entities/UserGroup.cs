using System;
using Common.Attributes;
using Domain.Entities.Base;

namespace Domain.Entities;

public class UserGroup : EntityGroupBase<User, UserGroup>
{
    [AllowFilterExpression]
    [AllowFilterSort]
    public bool IsSystem { get; set; }

    [AllowFilterExpression]
    [AllowFilterSort]
    public long Priority { get; set; }

    [AllowFilterExpression]
    [AllowFilterSort]
    public Guid UserId { get; set; }

    public virtual User User { get; set; }
}