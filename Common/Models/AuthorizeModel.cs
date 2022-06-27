using System;

namespace Common.Models;

public class AuthorizeModel
{
    public string EntityLeftTableName { get; set; }
    public string EntityLeftGroupsTableName { get; set; }
    public string EntityLeftEntityToEntityMappingsTableName { get; set; }
    public Guid EntityLeftId { get; set; }
    public string EntityLeftPermissionAlias { get; set; }
    
    public string EntityRightTableName { get; set; }
    public string EntityRightGroupsTableName { get; set; }
    public string EntityRightEntityToEntityMappingsTableName { get; set; }
    public Guid? EntityRightId { get; set; }
    
    /// <summary>
    /// Define field of PermissionValues to be used instead of EntityRightId ex. $"\"EntityId\""
    /// </summary>
    public string EntityRightIdRawSql { get; set; }
    public string EntityRightPermissionAlias { get; set; }
    
    /// <summary>
    /// T1 is EntityLeft, T2 is EntityRight ex. $"T1.\"Id\" = T2.\"OwnerUserId\""
    /// </summary>
    public string SqlExpressionPermissionTypeValueNeededOwner { get; set; }
    
    public bool UseCache { get; set; }

    public string GetRawSqlAuthorizeResult()
    {
        return $"SELECT {GetRawSqlExpression()} as Result";
    }
    
    public string GetRawSqlExpression()
    {
        return $@"public.""AuthorizeEntityPermissionToEntityPermission""(
            {(string.IsNullOrEmpty(EntityLeftTableName) ? throw new ArgumentNullException(EntityLeftTableName) : $"'{EntityLeftTableName}'")},
            {(string.IsNullOrEmpty(EntityLeftGroupsTableName) ? "null" : $"'{EntityLeftGroupsTableName}'")},
            {(string.IsNullOrEmpty(EntityLeftEntityToEntityMappingsTableName) ? "null" : $"'{EntityLeftEntityToEntityMappingsTableName}'")},
            {$"'{EntityLeftId.ToString()}'"},
            {(string.IsNullOrEmpty(EntityLeftPermissionAlias) ? throw new ArgumentNullException(EntityLeftPermissionAlias) : $"'{EntityLeftPermissionAlias}'")},
            {(string.IsNullOrEmpty(EntityRightTableName) ? throw new ArgumentNullException(EntityRightTableName) : $"'{EntityRightTableName}'")},
            {(string.IsNullOrEmpty(EntityRightGroupsTableName) ? "null" : $"'{EntityRightGroupsTableName}'")},
            {(string.IsNullOrEmpty(EntityRightEntityToEntityMappingsTableName) ? "null" : $"'{EntityRightEntityToEntityMappingsTableName}'")},
            {(EntityRightId.HasValue ? $"'{EntityRightId.Value.ToString()}'" : (string.IsNullOrEmpty(EntityRightIdRawSql) ? throw new ArgumentNullException(EntityRightIdRawSql) : EntityRightIdRawSql))},
            {(string.IsNullOrEmpty(EntityRightPermissionAlias) ? throw new ArgumentNullException(EntityRightPermissionAlias) : $"'{EntityRightPermissionAlias}'")},
            {(string.IsNullOrEmpty(SqlExpressionPermissionTypeValueNeededOwner) ? throw new ArgumentNullException(SqlExpressionPermissionTypeValueNeededOwner) : $"'{SqlExpressionPermissionTypeValueNeededOwner}'")},
            {$"{UseCache}"}
        )";
    }
}