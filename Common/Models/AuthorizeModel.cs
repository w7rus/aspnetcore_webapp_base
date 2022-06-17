using System;

namespace Common.Models;

public class AuthorizeModel
{
    public string EntityLeftTableName { get; set; }
    public string EntityLeftGroupsTableName { get; set; }
    public string EntityLeftEntityToEntityMappingsTableName { get; set; }
    public string EntityLeftId { get; set; }
    public string EntityLeftPermissionAlias { get; set; }
    
    public string EntityRightTableName { get; set; }
    public string EntityRightGroupsTableName { get; set; }
    public string EntityRightEntityToEntityMappingsTableName { get; set; }
    public string EntityRightId { get; set; }
    public string EntityRightPermissionAlias { get; set; }
    
    /// <summary>
    /// T1 is EntityLeft, T2 is EntityRight ex. T1."Id" = T2."OwnerUserId"
    /// </summary>
    public string SQLExpressionPermissionTypeValueNeededOwner { get; set; }
}