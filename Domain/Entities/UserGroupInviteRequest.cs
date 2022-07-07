using System;
using Common.Attributes;
using Domain.Entities.Base;

namespace Domain.Entities;

public class UserGroupInviteRequest : EntityBase<Guid>
{
    [AllowFilterExpression]
    [AllowFilterSort]
    public Guid UserGroupId { get; set; }

    public virtual UserGroup UserGroup { get; set; }

    [AllowFilterExpression]
    [AllowFilterSort]
    public Guid SrcUserId { get; set; }

    public virtual User SrcUser { get; set; }

    [AllowFilterExpression]
    [AllowFilterSort]
    public Guid DestUserId { get; set; }

    public virtual User DestUser { get; set; }

    [AllowFilterExpression]
    [AllowFilterSort]
    public DateTimeOffset ExpiresAt { get; set; }
}