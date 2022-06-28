using System;
using Domain.Entities.Base;

namespace Domain.Entities;

public class Authorize : EntityBase<Guid>
{
    public string EntityLeftTableName { get; set; }
    public string EntityLeftGroupsTableName { get; set; }
    public string EntityLeftEntityToEntityMappingsTableName { get; set; }
    public Guid EntityLeftId { get; set; }
    public string EntityLeftPermissionAlias { get; set; }
    public string EntityRightTableName { get; set; }
    public string EntityRightGroupsTableName { get; set; }
    public string EntityRightEntityToEntityMappingsTableName { get; set; }
    public Guid EntityRightId { get; set; }
    public string EntityRightPermissionAlias { get; set; }

    /// <summary>
    ///     T1 is EntityLeft, T2 is EntityRight ex. 'T1."Id" = T2."UserId"'
    /// </summary>
    public string SQLExpressionPermissionTypeValueNeededOwner { get; set; }

    public bool Result { get; set; }
}