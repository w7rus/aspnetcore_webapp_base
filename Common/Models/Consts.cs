using System;
using Common.Enums;

namespace Common.Models;

public static class Consts
{
    public const long RootUserGroupValue = long.MaxValue;
    public const long RootUserGroupPriority = long.MaxValue;
    public const long MemberUserGroupValue = 50L;
    public const long MemberUserGroupPriority = 50L;
    public const long GuestUserGroupValue = 25L;
    public const long GuestUserGroupPriority = 25L;
    public const long BannedUserGroupValue = 0L;
    public const long BannedUserGroupPriority = long.MaxValue - 1;
    public const long TrueValue = 1;
    public const long FalseValue = 0;
    public const string AutoMapperModelAuthorizeDataKey = "AutoMapperModelAuthorizeData";
    public const string NpgSqlEntityFrameworkCorePostgreSQLProviderName = "Npgsql.EntityFrameworkCore.PostgreSQL";
    public const string DomainNamespace = "Domain.Entities";
    public static readonly Guid RootUserId = new("ce374862-f799-4519-9fa8-a8dcf1b9e8ab");
    public static readonly Guid PublicUserId = new("8a278c1c-f27d-46ae-b9cd-99ac2e6ab141");
    public static readonly Guid GroupMemberUserId = new("85f276b1-07b9-4a61-945e-4598241bd47f");

    public class PermissionAlias
    {
        //User,UserGroup PermissionValue
        public const string PermissionValueCreate = "PermissionValueCreate";
        public const string PermissionValueRead = "PermissionValueRead";
        public const string PermissionValueUpdate = "PermissionValueUpdate";
        //TODO: g_any_a_update_o_permissionvalue_o_mingrant_l_automapper
        //TODO: g_any_a_update_o_permissionvalue_o_maxgrant_l_automapper
        //TODO: g_any_a_update_o_permissionvalue_a_skipgrant
        public const string PermissionValueDelete = "PermissionValueDelete";

        //UserGroup
        public const string UserGroupCreate = "UserGroupCreate";
        public const string UserGroupCreate_Alias = "UserGroupCreate_Alias";
        public const string UserGroupCreate_Description = "UserGroupCreate_Description";
        public const string UserGroupCreate_Priority = "UserGroupCreate_Priority";
        public const string UserGroupRead = "UserGroupRead";
        public const string UserGroupUpdate = "UserGroupUpdate";
        public const string UserGroupUpdate_Alias = "UserGroupUpdate_Alias";
        public const string UserGroupUpdate_Description = "UserGroupUpdate_Description";
        public const string UserGroupUpdate_Priority = "UserGroupUpdate_Priority";
        public const string UserGroupDelete = "UserGroupDelete";
        public const string UserGroupJoin = "UserGroupJoin";
        public const string UserGroupLeave = "UserGroupLeave";
        public const string UserGroupTransferRequestCreate = "UserGroupTransferRequestCreate";
        public const string UserGroupTransferRequestRead = "UserGroupTransferRequestRead";
        public const string UserGroupTransferRequestUpdate = "UserGroupTransferRequestUpdate";
        public const string UserGroupInviteRequestCreate = "UserGroupInviteRequestCreate";
        public const string UserGroupInviteRequestRead = "UserGroupInviteRequestRead"; //TODO:
        public const string UserGroupInviteRequestUpdate = "UserGroupInviteRequestUpdate";
        public const string UserGroupKickUser = "UserGroupKickUser";
        public const string UserGroupAddUser = "UserGroupAddUser";
        public const string UserGroupMemberInviteRequestCreate = "UserGroupMemberInviteRequestCreate";
        public const string UserGroupMemberInviteRequestRead = "UserGroupMemberInviteRequestRead"; //TODO:
        public const string UserGroupMemberInviteRequestUpdate = "UserGroupMemberInviteRequestUpdate";
        public const string UserGroupMemberKickUser = "UserGroupMemberKickUser";
        public const string UserRead = "UserRead";
        public const string UserUpdate = "UserUpdate";
        public const string UserDelete = "UserDelete";
        public const string UserProfileRead = "UserProfileRead";
        public const string UserProfileUpdate = "UserProfileUpdate";
        // public const string g_userprofile_a_update_o_userprofile_o_avatar_l_maxfilesize = "g_userprofile_a_update_o_userprofile.o_avatar_l_maxfilesize";
        public const string FileCreate = "FileCreate";
        public const string FileCreate_Agerating = "FileCreate_Agerating";
        public const string FileRead = "FileRead";
        public const string FileUpdate = "FileUpdate";
        public const string FileUpdate_Agerating = "FileUpdate_Agerating";
        public const string FileDelete = "FileDelete";
        //TODO: g_file_a_transferownership_o_file
    }

    public class MigrationBuilderRawSql
    {
        public const string CreateExtensionHStore = "CREATE extension IF NOT EXISTS hstore";
        public const string CreateExtensionUUIDOSSP = "CREATE extension IF NOT EXISTS \"uuid-ossp\"";

        public const string CreateOrReplaceFunctionAuthorizeEntityPermissionValueToEntityPermissionValue = @"
CREATE OR REPLACE FUNCTION public.""AuthorizeEntityPermissionValueToEntityPermissionValue""(
    IN entityleftpermission record DEFAULT NULL::record,
    IN entityleftpermissionvalue record DEFAULT  NULL::record,
    IN entityrightpermission record DEFAULT  NULL::record,
    IN entityrightpermissionvalue record DEFAULT  NULL::record
)
    RETURNS boolean
    LANGUAGE 'plpgsql'
    VOLATILE
    PARALLEL UNSAFE
    COST 100
    
AS $BODY$
    DECLARE
        FResult boolean := false;
        ValueLeft bigint := 0;
        ValueRight bigint := 0;
    BEGIN
        RAISE DEBUG '   └┐AuthorizeEntityPermissionValueToEntityPermissionValue()';
        
        IF EntityLeftPermissionValue IS NULL OR EntityRightPermissionValue IS NULL THEN
            RAISE DEBUG '    ├EntityLeftPermissionValue IS NULL OR EntityRightPermissionValue IS NULL';
            
            RETURN FResult;
        END IF;

        IF EntityLeftPermission.""CompareMode"" != EntityRightPermission.""CompareMode"" THEN
            RAISE EXCEPTION '    ├[AuthorizeEntityPermissionValueToEntityPermissionValue]: EntityLeftPermission.CompareMode != EntityRightPermission.CompareMode';
        END IF;

        ValueLeft := get_byte(EntityLeftPermissionValue.""Value"", 7)::bigint << 8
            | get_byte(EntityLeftPermissionValue.""Value"", 6) << 8
            | get_byte(EntityLeftPermissionValue.""Value"", 5) << 8
            | get_byte(EntityLeftPermissionValue.""Value"", 4) << 8
            | get_byte(EntityLeftPermissionValue.""Value"", 3) << 8
            | get_byte(EntityLeftPermissionValue.""Value"", 2) << 8
            | get_byte(EntityLeftPermissionValue.""Value"", 1) << 8
            | get_byte(EntityLeftPermissionValue.""Value"", 0);
        ValueRight := get_byte(EntityRightPermissionValue.""Value"", 7)::bigint << 8
            | get_byte(EntityRightPermissionValue.""Value"", 6) << 8
            | get_byte(EntityRightPermissionValue.""Value"", 5) << 8
            | get_byte(EntityRightPermissionValue.""Value"", 4) << 8
            | get_byte(EntityRightPermissionValue.""Value"", 3) << 8
            | get_byte(EntityRightPermissionValue.""Value"", 2) << 8
            | get_byte(EntityRightPermissionValue.""Value"", 1) << 8
            | get_byte(EntityRightPermissionValue.""Value"", 0);
        
        RAISE DEBUG '    ├<TryUseCompareMode>';

        CASE EntityLeftPermission.""CompareMode""
            WHEN 0 THEN
                FResult := true;
            WHEN 1 THEN
                FResult := ValueLeft = ValueRight;
            WHEN 2 THEN
                FResult := ValueLeft != ValueRight;
            WHEN 3 THEN
                FResult := ValueLeft < ValueRight;
            WHEN 4 THEN
                FResult := ValueLeft <= ValueRight;
            WHEN 5 THEN
                FResult := ValueLeft > ValueRight;
            WHEN 6 THEN
                FResult := ValueLeft >= ValueRight;
            ELSE
                RAISE EXCEPTION '    ├[AuthorizeEntityPermissionValueToEntityPermissionValue]: ArgumentOutOfRangeException(EntityLeftPermission.CompareMode)';
        END CASE;
        
        RAISE DEBUG '   ┌┘';

        RETURN FResult;
    END;
$BODY$;
";

        public const string CreateOrReplaceFunctionAuthorizeEntityPermissionToEntityPermissionValue = @"
CREATE OR REPLACE FUNCTION public.""AuthorizeEntityPermissionToEntityPermissionValue""(
    IN entityleft record DEFAULT NULL::record,
    IN entityleftgroupstablename text DEFAULT  NULL::text,
    IN entityleftgroupmappingstablename text DEFAULT  NULL::text,
    IN entityleftpermission record DEFAULT  NULL::record,
    IN entityrightpermission record DEFAULT  NULL::record,
    IN entityrightpermissionvalue record DEFAULT  NULL::record
)
    RETURNS boolean
    LANGUAGE 'plpgsql'
    VOLATILE
    PARALLEL UNSAFE
    COST 100
    
AS $BODY$
    DECLARE
        FResult boolean := false;
        EntityLeftGroup record := null;
        EntityLeftPermissionValue record := null;
    BEGIN
        RAISE DEBUG ' └┐AuthorizeEntityPermissionToEntityPermissionValue()';
        
        IF EntityLeftGroupsTableName IS NOT NULL AND EntityLeftGroupMappingsTableName IS NOT NULL THEN
            RAISE DEBUG '  ├<TryUseEntityLeftGroupPermissionValue>';
            RAISE DEBUG '  └┐';
            
            EXECUTE format('SELECT * FROM public.""%s"" AS T1 WHERE EXISTS (SELECT * FROM public.""%s"" AS T2 WHERE T1.""Id"" = T2.""EntityRightId"" AND T2.""EntityLeftId"" = cast($1 as uuid)) AND EXISTS (SELECT * FROM public.""PermissionValues"" AS T3 WHERE T3.""PermissionId"" = $2 AND T3.""EntityId"" = T1.""Id"") ORDER BY T1.""Priority"" DESC LIMIT 1', EntityLeftGroupsTableName, EntityLeftGroupMappingsTableName)
                INTO EntityLeftGroup
                USING EntityLeft.""Id"",
                    EntityLeftPermission.""Id"";
            
            IF NOT(EntityLeftGroup IS NULL) THEN
                RAISE DEBUG '   ├EntityLeftGroup = % %', EntityLeftGroup.""Id"", EntityLeftGroup.""Alias"";
                
                EXECUTE 'SELECT * FROM public.""PermissionValues"" WHERE ""PermissionId"" = $1 AND ""EntityId"" = $2'
                    INTO EntityLeftPermissionValue
                    USING EntityLeftPermission.""Id"",
                        EntityLeftGroup.""Id"";
                
                RAISE DEBUG '   ├EntityLeftPermissionValue = % %', EntityLeftPermissionValue.""Id"", EntityLeftPermissionValue.""Value"";
                
                FResult := public.""AuthorizeEntityPermissionValueToEntityPermissionValue""(EntityLeftPermission, EntityLeftPermissionValue, EntityRightPermission, EntityRightPermissionValue);
                
                RAISE DEBUG '   ├FResult = %', FResult;
                
                EntityLeftPermissionValue := row(null);
            ELSE
                RAISE DEBUG '   ├EntityLeftGroup IS NULL';
            END IF;
            
            RAISE DEBUG '  ┌┘';
        END IF;
        
        RAISE DEBUG '  ├<TryUseEntityLeftPermissionValue>';
        RAISE DEBUG '  └┐';

        EntityLeftPermissionValue := row(null);
        EXECUTE 'SELECT * FROM public.""PermissionValues"" WHERE ""PermissionId"" = $1 AND ""EntityId"" = $2'
            INTO EntityLeftPermissionValue
            USING EntityLeftPermission.""Id"",
                EntityLeft.""Id"";
        
        IF NOT(EntityLeftPermissionValue IS NULL) THEN
            RAISE DEBUG '   ├EntityLeftPermissionValue = % %', EntityLeftPermissionValue.""Id"", EntityLeftPermissionValue.""Value"";
            FResult := public.""AuthorizeEntityPermissionValueToEntityPermissionValue""(EntityLeftPermission, EntityLeftPermissionValue, EntityRightPermission, EntityRightPermissionValue);
            
            RAISE DEBUG '   ├FResult = %', FResult;
        ELSE
            RAISE DEBUG '   ├EntityLeftPermissionValue IS NULL';
        END IF;
        
        RAISE DEBUG ' ┌─┘';

        RETURN FResult;
    END;
$BODY$;
";

        //TODO: Authorize RootUser without any checks
        public const string CreateOrReplaceFunctionAuthorizeEntityPermissionToEntityPermission = @"
CREATE OR REPLACE FUNCTION public.""AuthorizeEntityPermissionToEntityPermission""(
    IN entitylefttablename text DEFAULT NULL::text,
    IN entityleftgroupstablename text DEFAULT  NULL::text,
    IN entityleftgroupmappingstablename text DEFAULT  NULL::text,
    IN entityleftuuid uuid DEFAULT  NULL::uuid,
    IN entityleftpermissionalias text DEFAULT  NULL::text,
    IN entityrighttablename text DEFAULT  NULL::text,
    IN entityrightgroupstablename text DEFAULT  NULL::text,
    IN entityrightgroupmappingstablename text DEFAULT  NULL::text,
    IN entityrightuuid uuid DEFAULT  NULL::uuid,
    IN entityrightpermissionalias text DEFAULT  NULL::text,
    IN sqlexpressionpermissiontypevalueneededowner text DEFAULT  NULL::text,
    IN usecache boolean DEFAULT  false
)
    RETURNS boolean
    LANGUAGE 'plpgsql'
    VOLATILE
    PARALLEL UNSAFE
    COST 100
    
AS $BODY$
    DECLARE
        FResult boolean := false;
        PermissionTypeValueNeededOwner boolean := false;
        EntityLeft record := null;
        EntityLeftPermission record := null;
        EntityRight record := null;
        EntityRightGroup record := null;
        EntityRightPermission record := null;
        EntityRightPermissionValue record := null;
        Authorize record := null;
    BEGIN
        RAISE DEBUG '┌AuthorizeEntityPermissionToEntityPermission()';
      
        EXECUTE format('SELECT * FROM public.""%s"" WHERE ""Id"" = cast($1 as uuid)', EntityLeftTableName) INTO EntityLeft USING EntityLeftUuid;
        EXECUTE format('SELECT * FROM public.""%s"" WHERE ""Id"" = cast($1 as uuid)', EntityRightTableName) INTO EntityRight USING EntityRightUuid;

        IF EntityLeft IS NULL  THEN
            RAISE EXCEPTION '├[AuthorizeEntityPermissionToEntityPermission]: EntityLeft IS NULL!';
        END IF;
        IF EntityRight IS NULL THEN
            RAISE EXCEPTION '├[AuthorizeEntityPermissionToEntityPermission]: EntityRight IS NULL!';
        END IF;
        
        IF UseCache THEN
            RAISE DEBUG '├<TryUseCache>';
            RAISE DEBUG '└┐';
        
            EXECUTE 'SELECT * FROM public.""Authorizes"" WHERE ""EntityLeftTableName"" = $1 AND ""EntityLeftGroupsTableName"" = $2 AND ""EntityLeftEntityToEntityMappingsTableName"" = $3 AND ""EntityLeftId"" = $4 AND ""EntityLeftPermissionAlias"" = $5 AND ""EntityRightTableName"" = $6 AND ""EntityRightGroupsTableName"" = $7 AND ""EntityRightEntityToEntityMappingsTableName"" = $8 AND ""EntityRightId"" = $9 AND ""EntityRightPermissionAlias"" = $10 AND ""SQLExpressionPermissionTypeValueNeededOwner"" = $11'
                INTO Authorize 
                USING EntityLeftTableName,
                    EntityLeftGroupsTableName,
                    EntityLeftGroupMappingsTableName,
                    cast(EntityLeft.""Id"" as uuid),
                    EntityLeftPermissionAlias,
                    EntityRightTableName,
                    EntityRightGroupsTableName,
                    EntityRightGroupMappingsTableName,
                    cast(EntityRight.""Id"" as uuid),
                    EntityRightPermissionAlias,
                    SQLExpressionPermissionTypeValueNeededOwner;
            
            IF NOT(Authorize IS NULL) THEN
                RAISE DEBUG ' └Authorize = % %', Authorize.""Id"", Authorize.""Value"";
            
                RETURN Authorize.Result;
            END IF;
            
            RAISE DEBUG '┌┘';
        END IF;

        EXECUTE format('SELECT count(*) > 0 FROM public.""%s"" AS T1 WHERE T1.""Id"" = cast($1 as uuid) AND EXISTS (SELECT * FROM public.""%s"" AS T2 WHERE T2.""Id"" = cast($2 as uuid) AND %s)', EntityLeftTableName, EntityRightTableName, SQLExpressionPermissionTypeValueNeededOwner) INTO PermissionTypeValueNeededOwner USING EntityLeftUuid, EntityRightUuid;

        RAISE DEBUG '├PermissionTypeValueNeededOwner = %', PermissionTypeValueNeededOwner;

        EXECUTE 'SELECT * FROM public.""Permissions"" WHERE ""Alias"" = $1 AND ""Type"" = $2' INTO EntityLeftPermission USING EntityLeftPermissionAlias, 2;
        IF EntityLeftPermission IS NULL THEN
            RAISE EXCEPTION '├[AuthorizeEntityPermissionToEntityPermission]: EntityLeftPermission IS NULL!';
        END IF;
        
        RAISE DEBUG '├EntityLeftPermission = % %', EntityLeftPermission.""Id"", EntityLeftPermission.""Alias"";

        IF PermissionTypeValueNeededOwner = true THEN
            EXECUTE 'SELECT * FROM public.""Permissions"" WHERE ""Alias"" = $1 AND ""Type"" = $2' INTO EntityRightPermission USING EntityRightPermissionAlias, 3;
        ELSE
            EXECUTE 'SELECT * FROM public.""Permissions"" WHERE ""Alias"" = $1 AND ""Type"" = $2' INTO EntityRightPermission USING EntityRightPermissionAlias, 4;
        END IF;
        IF EntityRightPermission IS NULL THEN
            RAISE EXCEPTION '├[AuthorizeEntityPermissionToEntityPermission]: EntityRightPermission IS NULL!';
        END IF;
        
        RAISE DEBUG '├EntityRightPermission = % %', EntityRightPermission.""Id"", EntityRightPermission.""Alias"";
        
        IF EntityRightGroupsTableName IS NOT NULL AND EntityRightGroupMappingsTableName IS NOT NULL THEN
            RAISE DEBUG '├<TryUseEntityRightGroupPermissionValue>';
            RAISE DEBUG '└┐';
            
            EXECUTE format('SELECT * FROM public.""%s"" AS T1 WHERE EXISTS (SELECT * FROM public.""%s"" AS T2 WHERE T1.""Id"" = T2.""EntityRightId"" AND T2.""EntityLeftId"" = cast($1 as uuid)) AND EXISTS (SELECT * FROM public.""PermissionValues"" AS T3 WHERE T3.""PermissionId"" = $2 AND T3.""EntityId"" = T1.""Id"") ORDER BY T1.""Priority"" DESC LIMIT 1', EntityRightGroupsTableName, EntityRightGroupMappingsTableName)
                INTO EntityRightGroup
                USING EntityRight.""Id"",
                    EntityRightPermission.""Id"";
            
            IF NOT(EntityRightGroup IS NULL) THEN
                RAISE DEBUG ' ├EntityRightGroup = % %', EntityRightGroup.""Id"", EntityRightGroup.""Alias"";
            
                EXECUTE 'SELECT * FROM public.""PermissionValues"" WHERE ""PermissionId"" = $1 AND ""EntityId"" = $2'
                INTO EntityRightPermissionValue USING
                    EntityRightPermission.""Id"",
                    EntityRightGroup.""Id"";

                RAISE DEBUG ' ├EntityRightPermissionValue = % %', EntityRightPermissionValue.""Id"", EntityRightPermissionValue.""Value"";
                
                FResult := public.""AuthorizeEntityPermissionToEntityPermissionValue""(EntityLeft, EntityLeftGroupsTableName, EntityLeftGroupMappingsTableName, EntityLeftPermission, EntityRightPermission, EntityRightPermissionValue);
                
                RAISE DEBUG ' ├FResult = %', FResult;
                
                EntityRightPermissionValue := row(null);
            ELSE
                RAISE DEBUG ' ├EntityRightGroup IS NULL';
            END IF;
            
            RAISE DEBUG '┌┘';
        END IF;
        
        RAISE DEBUG '├<TryUseEntityRightPermissionValue>';
        RAISE DEBUG '└┐';

        EntityRightPermissionValue := row(null);
        EXECUTE 'SELECT * FROM public.""PermissionValues"" WHERE ""PermissionId"" = $1 AND ""EntityId"" = $2'
            INTO EntityRightPermissionValue USING
                EntityRightPermission.""Id"",
                EntityRight.""Id"";
        
        IF NOT(EntityRightPermissionValue IS NULL) THEN
            RAISE DEBUG ' ├EntityRightPermissionValue = % %', EntityRightPermissionValue.""Id"", EntityRightPermissionValue.""Value"";
        
            FResult := public.""AuthorizeEntityPermissionToEntityPermissionValue""(EntityLeft, EntityLeftGroupsTableName, EntityLeftGroupMappingsTableName, EntityLeftPermission, EntityRightPermission, EntityRightPermissionValue);
            
            RAISE DEBUG ' ├FResult = %', FResult;
        ELSE
            RAISE DEBUG ' ├EntityRightPermissionValue IS NULL';
        END IF;
        
        RAISE DEBUG '┌┘';
        
        IF UseCache THEN
          RAISE DEBUG '├<SaveCache>';
        
          EXECUTE 'INSERT INTO public.""Authorizes"" (""Id"", ""CreatedAt"", ""UpdatedAt"", ""EntityLeftTableName"", ""EntityLeftGroupsTableName"", ""EntityLeftEntityToEntityMappingsTableName"", ""EntityLeftId"", ""EntityLeftPermissionAlias"", ""EntityRightTableName"", ""EntityRightGroupsTableName"", ""EntityRightEntityToEntityMappingsTableName"", ""EntityRightId"", ""EntityRightPermissionAlias"", ""SQLExpressionPermissionTypeValueNeededOwner"", ""Result"") VALUES ((SELECT uuid_generate_v4()), (SELECT CURRENT_TIMESTAMP), (SELECT CURRENT_TIMESTAMP), $1, $2, $3, $4, $5, $6, $7, $8, $9, $10, $11, $12)'
              USING EntityLeftTableName,
                  EntityLeftGroupsTableName,
                  EntityLeftGroupMappingsTableName,
                  cast(EntityLeft.""Id"" as uuid),
                  EntityLeftPermissionAlias,
                  EntityRightTableName,
                  EntityRightGroupsTableName,
                  EntityRightGroupMappingsTableName,
                  cast(EntityRight.""Id"" as uuid),
                  EntityRightPermissionAlias,
                  SQLExpressionPermissionTypeValueNeededOwner,
                  FResult;
        END IF;
        
        RAISE DEBUG '└FResult = %', FResult;
            
        RETURN FResult;
    END;
$BODY$;
";
    }
}