using System;
using Common.Attributes;
using Domain.Entities.Base;

namespace Domain.Entities;

public class PermissionValue : EntityBase<Guid>
{
    public byte[] Value { get; set; }

    [AllowFilterExpression]
    [AllowFilterSort]
    public Guid PermissionId { get; set; }

    public virtual Permission Permission { get; set; }

    [AllowFilterExpression]
    [AllowFilterSort]
    public Guid EntityId { get; set; }
    
    public string EntityDiscriminator { get; set; }
}