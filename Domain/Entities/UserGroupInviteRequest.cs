using System;
using Domain.Entities.Base;

namespace Domain.Entities;

public class UserGroupInviteRequest : EntityBase<Guid>
{
    public Guid UserGroupId { get; set; }
    public virtual UserGroup UserGroup { get; set; }
    public Guid SrcUserId { get; set; }
    public virtual User SrcUser { get; set; }
    public Guid DestUserId { get; set; }
    public virtual User DestUser { get; set; }
    public DateTimeOffset ExpiresAt { get; set; }
}