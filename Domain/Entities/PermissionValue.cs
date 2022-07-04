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
    
    //TODO: MinGrant-MaxGrant (special value that limits what is the Min-Max values can be set in Value field by those who are allowed to edit permission values. To alter MinGrant-MaxGrant values there needs to be a special boolean permission given (should only be available to root users only)
}