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
    public const string AutoMapperModelAuthorizeDataKey = "AutoMapperModelAuthorizeData";
    public const string NpgSqlEntityFrameworkCorePostgreSQLProviderName = "Npgsql.EntityFrameworkCore.PostgreSQL";
    public const string DomainNamespace = "Domain.Entities";
    
    public class MigrationBulderAdditions
    {
        public const string CreateExtensionHStore = "create extension if not exists hstore";

        public const string CreateOrReplaceFunctionAuthorizeEntityPermissionValueToEntityPermissionValue = @"
CREATE OR REPLACE FUNCTION public.""AuthorizeEntityPermissionValueToEntityPermissionValue""(
    EntityPermissionLeft record default null,
    EntityPermissionValueLeft record  default null,
    EntityPermissionRight record default null,
    EntityPermissionValueRight record default null
  )
    RETURNS boolean
    LANGUAGE 'plpgsql'
    AS $$
      DECLARE
        FResult boolean := false;

        CompareResult int := 0;
        ValueLeft bigint := 0;
        ValueRight bigint := 0;
      BEGIN
        IF EntityPermissionValueLeft IS NULL OR EntityPermissionValueRight IS NULL THEN
          RAISE NOTICE ' - - AuthorizeEntityPermissionValueToEntityPermissionValue: EntityPermissionValueLeft OR EntityPermissionValueRight are NULL! %d', FResult;
          RETURN FResult;
        END IF;

        IF EntityPermissionLeft.""ValueType"" != EntityPermissionRight.""ValueType"" THEN
          RAISE EXCEPTION ' - - AuthorizeEntityPermissionValueToEntityPermissionValue: ValueType are not the same! %d', FResult;
        END IF;

        IF EntityPermissionLeft.""CompareMode"" != EntityPermissionRight.""CompareMode"" THEN
          RAISE EXCEPTION ' - - AuthorizeEntityPermissionValueToEntityPermissionValue: ValueType are not the same! %d', FResult;
        END IF;

        CASE EntityPermissionLeft.""ValueType""
          WHEN 0 THEN
            RAISE NOTICE ' - - AuthorizeEntityPermissionValueToEntityPermissionValue: ValueType is NONE! %d', FResult;
            RETURN FResult;
          WHEN 1 THEN
            RAISE NOTICE ' - - AuthorizeEntityPermissionValueToEntityPermissionValue: ValueType is UNKNOWN! %d', FResult;
            RETURN FResult;
          WHEN 2 THEN --Boolean
            RAISE NOTICE ' - - AuthorizeEntityPermissionValueToEntityPermissionValue: EntityPermissionLeft.ValueType Boolean';
            ValueLeft := get_byte(EntityPermissionValueLeft.""Value"", 0)::bigint << 8;
            ValueRight := get_byte(EntityPermissionValueRight.""Value"", 0)::bigint << 8;
          WHEN 3 THEN --Int64
            RAISE NOTICE ' - - AuthorizeEntityPermissionValueToEntityPermissionValue: EntityPermissionLeft.ValueType Int64';
            ValueLeft := get_byte(EntityPermissionValueLeft.""Value"", 7)::bigint << 8
            | get_byte(EntityPermissionValueLeft.""Value"", 6) << 8
            | get_byte(EntityPermissionValueLeft.""Value"", 5) << 8
            | get_byte(EntityPermissionValueLeft.""Value"", 4) << 8
            | get_byte(EntityPermissionValueLeft.""Value"", 3) << 8
            | get_byte(EntityPermissionValueLeft.""Value"", 2) << 8
            | get_byte(EntityPermissionValueLeft.""Value"", 1) << 8
            | get_byte(EntityPermissionValueLeft.""Value"", 0);
            ValueRight := get_byte(EntityPermissionValueRight.""Value"", 7)::bigint << 8
            | get_byte(EntityPermissionValueRight.""Value"", 6) << 8
            | get_byte(EntityPermissionValueRight.""Value"", 5) << 8
            | get_byte(EntityPermissionValueRight.""Value"", 4) << 8
            | get_byte(EntityPermissionValueRight.""Value"", 3) << 8
            | get_byte(EntityPermissionValueRight.""Value"", 2) << 8
            | get_byte(EntityPermissionValueRight.""Value"", 1) << 8
            | get_byte(EntityPermissionValueRight.""Value"", 0);
          ELSE
            RAISE EXCEPTION ' - - AuthorizeEntityPermissionValueToEntityPermissionValue: EntityPermissionLeft.ValueType: ArgumentOutOfRangeException';
        END CASE;

        CASE EntityPermissionLeft.""CompareMode""
          WHEN 0 THEN
            RAISE NOTICE ' - - AuthorizeEntityPermissionValueToEntityPermissionValue: EntityPermissionLeft.CompareMode None';
            FResult := true;
          WHEN 1 THEN --Equal
            RAISE NOTICE ' - - AuthorizeEntityPermissionValueToEntityPermissionValue: EntityPermissionLeft.CompareMode Equal';
            FResult := ValueLeft = ValueRight;
          WHEN 2 THEN --NotEqual
            RAISE NOTICE ' - - AuthorizeEntityPermissionValueToEntityPermissionValue: EntityPermissionLeft.CompareMode NotEqual';
            FResult := ValueLeft != ValueRight;
          WHEN 3 THEN --Less
            RAISE NOTICE ' - - AuthorizeEntityPermissionValueToEntityPermissionValue: EntityPermissionLeft.CompareMode Less';
            FResult := ValueLeft < ValueRight;
          WHEN 4 THEN --LessOrEqual
            RAISE NOTICE ' - - AuthorizeEntityPermissionValueToEntityPermissionValue: EntityPermissionLeft.CompareMode LessOrEqual';
            FResult := ValueLeft <= ValueRight;
          WHEN 5 THEN --Greater
            RAISE NOTICE ' - - AuthorizeEntityPermissionValueToEntityPermissionValue: EntityPermissionLeft.CompareMode Greater';
            FResult := ValueLeft > ValueRight;
          WHEN 6 THEN --GreaterOrEqual
            RAISE NOTICE ' - - AuthorizeEntityPermissionValueToEntityPermissionValue: EntityPermissionLeft.CompareMode GreaterOrEqual';
            FResult := ValueLeft >= ValueRight;
          ELSE
            RAISE EXCEPTION ' - - AuthorizeEntityPermissionValueToEntityPermissionValue: EntityPermissionLeft.CompareMode: ArgumentOutOfRangeException';
        END CASE;

        RAISE NOTICE ' - - AuthorizeEntityPermissionValueToEntityPermissionValue: RET: %s', FResult::text;

        RETURN FResult;
      END;
    $$;
";
        
        public const string CreateOrReplaceFunctionAuthorizeEntityPermissionToEntityPermissionValue = @"
CREATE OR REPLACE FUNCTION public.""AuthorizeEntityPermissionToEntityPermissionValue""(
    EntityLeft record,
    EntityGroupLeftTableName text default null,
    EntityToEntityGroupMappingLeftTableName text default null,
    EntityPermissionLeft record default null,
    EntityPermissionRight record default null,
    EntityPermissionValueRight record default null
  )
    RETURNS boolean
    LANGUAGE 'plpgsql'
    AS $$
      DECLARE
        FResult boolean := false;

        EntityGroupLeft record := null;

        EntityPermissionValueLeft public.""PermissionValues""%ROWTYPE := null;
      BEGIN
        --This case would iterate over all groups this entity belongs to and retrieve PermissionValues
        IF EntityGroupLeftTableName IS NOT NULL AND EntityToEntityGroupMappingLeftTableName IS NOT NULL THEN

          --This Entity has Groups that might contain PermissionValues
          FOR EntityGroupLeft IN EXECUTE format('SELECT * FROM public.""%s"" AS T1 WHERE EXISTS (SELECT * FROM public.""%s"" AS T2 WHERE T1.""Id"" = T2.""EntityRightId"" AND T2.""EntityLeftId"" = cast($1 as uuid)) AND EXISTS (SELECT * FROM public.""PermissionValues"" AS T3 WHERE T3.""PermissionId"" = $2 AND T3.""EntityId"" = T1.""Id"") ORDER BY T1.""Priority"" ASC', EntityGroupLeftTableName, EntityToEntityGroupMappingLeftTableName) USING EntityLeft.""Id"", EntityPermissionLeft.""Id""
          LOOP
            RAISE NOTICE ' - AuthorizeEntityPermissionToEntityPermissionValue: EntityGroupLeft: %s %s', EntityGroupLeft.""Id""::text, EntityGroupLeft.""Alias""::text;

            EXECUTE 'SELECT * FROM public.""PermissionValues"" WHERE ""PermissionId"" = $1 AND ""EntityId"" = $2' INTO EntityPermissionValueLeft USING EntityPermissionLeft.""Id"", EntityGroupLeft.""Id"";

            CONTINUE WHEN EntityPermissionValueLeft IS NULL;

            RAISE NOTICE ' - AuthorizeEntityPermissionToEntityPermissionValue: EntityPermissionValueLeft: FOUND!';

            FResult := public.""AuthorizeEntityPermissionValueToEntityPermissionValue""(EntityPermissionLeft, EntityPermissionValueLeft, EntityPermissionRight, EntityPermissionValueRight);
          END LOOP;
        END IF;

        EntityPermissionValueLeft := row(null);

        --This case would retrieve PermissionValue set exactly for this entity (overrides groups authorize result)
        EXECUTE 'SELECT * FROM public.""PermissionValues"" WHERE ""PermissionId"" = $1 AND ""EntityId"" = $2' INTO EntityPermissionValueLeft USING EntityPermissionLeft.""Id"", EntityLeft.""Id"";
        IF NOT(EntityPermissionValueLeft IS NULL) THEN

          --This Entity might contain PermissionValues
          RAISE NOTICE 'AuthorizeEntityPermissionToEntityPermission: EntityLeft: %s', EntityLeft.""Id""::text;
          RAISE NOTICE 'AuthorizeEntityPermissionToEntityPermission: EntityPermissionValueLeft: FOUND!';

          FResult := public.""AuthorizeEntityPermissionValueToEntityPermissionValue""(EntityPermissionLeft, EntityPermissionValueLeft, EntityPermissionRight, EntityPermissionValueRight);
        END IF;

        RAISE NOTICE ' - AuthorizeEntityPermissionToEntityPermissionValue: RET: %s', FResult::text;

        RETURN FResult;
      END;
    $$;
";
        
        public const string CreateOrReplaceFunctionAuthorizeEntityPermissionToEntityPermission = @"
CREATE OR REPLACE FUNCTION public.""AuthorizeEntityPermissionToEntityPermission""(
    --EntityLeft
    EntityLeftTableName text default null,
    EntityGroupLeftTableName text default null,
    EntityToEntityGroupMappingLeftTableName text default null,
    EntityLeftUuid uuid default null,
    EntityLeftPermissionAlias text default null,

    --EntityRight
    EntityRightTableName text default null,
    EntityGroupRightTableName text default null,
    EntityToEntityGroupMappingRightTableName text default null,
    EntityRightUuid uuid default null,
    EntityRightPermissionAlias text default null,

    --Defines whether PermissionType.ValueNeededOwner or PermissionType.ValueNeededOthers is used
    --It may be injection unsafe, but those statements are only defined by programmer
    SQLExpressionPermissionTypeValueNeededOwner text default null
  )
    RETURNS boolean
    LANGUAGE 'plpgsql'
    AS $$
      DECLARE
        FResult boolean := false;
        PermissionTypeValueNeededOwner boolean := false;

        EntityLeft record := null;
        EntityRight record := null;

        EntityGroupRight record := null;

        EntityPermissionRight public.""Permissions""%ROWTYPE := null;
        EntityPermissionLeft public.""Permissions""%ROWTYPE := null;

        EntityPermissionValueRight public.""PermissionValues""%ROWTYPE := null;
      BEGIN
        --EntityLeft
        EXECUTE format('SELECT * FROM public.""%s"" WHERE ""Id"" = cast($1 as uuid)', EntityLeftTableName) INTO EntityLeft USING EntityLeftUuid;

        --EntityRight
        EXECUTE format('SELECT * FROM public.""%s"" WHERE ""Id"" = cast($1 as uuid)', EntityRightTableName) INTO EntityRight USING EntityRightUuid;

        IF EntityLeft IS NULL OR EntityRight IS NULL THEN
          RAISE EXCEPTION 'AuthorizeEntityPermissionToEntityPermission: EntityLeft OR EntityRight are NULL! %d', FResult;
        END IF;
        RAISE NOTICE 'AuthorizeEntityPermissionToEntityPermission: EntityLeft: %s', EntityLeft.""Id""::text;
        RAISE NOTICE 'AuthorizeEntityPermissionToEntityPermission: EntityRight: %s', EntityRight.""Id""::text;

        --PermissionTypeValueNeededOwner
        EXECUTE format('SELECT count(*) > 0 FROM public.""%s"" AS T1 WHERE T1.""Id"" = cast($1 as uuid) AND EXISTS (SELECT * FROM public.""%s"" AS T2 WHERE T2.""Id"" = cast($2 as uuid) AND %s)', EntityLeftTableName, EntityRightTableName, SQLExpressionPermissionTypeValueNeededOwner) INTO PermissionTypeValueNeededOwner USING EntityLeftUuid, EntityRightUuid;
        RAISE NOTICE 'AuthorizeEntityPermissionToEntityPermission: PermissionTypeValueNeededOwner: %s', PermissionTypeValueNeededOwner::text;

        --EntityPermissionLeft
        EXECUTE 'SELECT * FROM public.""Permissions"" WHERE ""Alias"" = $1 AND ""Type"" = $2' INTO EntityPermissionLeft USING EntityLeftPermissionAlias, 2;
        IF EntityPermissionLeft IS NULL THEN
          RAISE EXCEPTION 'AuthorizeEntityPermissionToEntityPermission: EntityPermissionLeft IS NULL! %d', FResult;
        END IF;
        RAISE NOTICE 'AuthorizeEntityPermissionToEntityPermission: EntityPermissionLeft: %s', EntityPermissionLeft.""Id""::text;

        --EntityPermissionRight
        IF PermissionTypeValueNeededOwner = true THEN
            EXECUTE 'SELECT * FROM public.""Permissions"" WHERE ""Alias"" = $1 AND ""Type"" = $2' INTO EntityPermissionRight USING EntityRightPermissionAlias, 3;
        ELSE
            EXECUTE 'SELECT * FROM public.""Permissions"" WHERE ""Alias"" = $1 AND ""Type"" = $2' INTO EntityPermissionRight USING EntityRightPermissionAlias, 4;
        END IF;
        IF EntityPermissionRight IS NULL THEN
          RAISE EXCEPTION 'AuthorizeEntityPermissionToEntityPermission: EntityPermissionRight IS NULL! %d', FResult;
        END IF;
        RAISE NOTICE 'AuthorizeEntityPermissionToEntityPermission: EntityPermissionRight: %s', EntityPermissionRight.""Id""::text;

        --This case would iterate over all groups this entity belongs to and retrieve PermissionValues
        IF EntityGroupRightTableName IS NOT NULL AND EntityToEntityGroupMappingRightTableName IS NOT NULL THEN

          --This Entity has Groups that might contain PermissionValues
          FOR EntityGroupRight IN EXECUTE format('SELECT * FROM public.""%s"" AS T1 WHERE EXISTS (SELECT * FROM public.""%s"" AS T2 WHERE T1.""Id"" = T2.""EntityRightId"" AND T2.""EntityLeftId"" = cast($1 as uuid)) AND EXISTS (SELECT * FROM public.""PermissionValues"" AS T3 WHERE T3.""PermissionId"" = $2 AND T3.""EntityId"" = T1.""Id"") ORDER BY T1.""Priority"" ASC', EntityGroupRightTableName, EntityToEntityGroupMappingRightTableName) USING EntityRight.""Id"", EntityPermissionRight.""Id""
          LOOP
            RAISE NOTICE 'AuthorizeEntityPermissionToEntityPermission: EntityGroupRight: %s %s', EntityGroupRight.""Id""::text, EntityGroupRight.""Alias""::text;

            EXECUTE 'SELECT * FROM public.""PermissionValues"" WHERE ""PermissionId"" = $1 AND ""EntityId"" = $2' INTO EntityPermissionValueRight USING EntityPermissionRight.""Id"", EntityGroupRight.""Id"";

            CONTINUE WHEN EntityPermissionValueRight IS NULL;

            RAISE NOTICE 'AuthorizeEntityPermissionToEntityPermission: EntityPermissionValueRight: FOUND!';

            FResult := public.""AuthorizeEntityPermissionToEntityPermissionValue""(EntityLeft, EntityGroupLeftTableName, EntityToEntityGroupMappingLeftTableName, EntityPermissionLeft, EntityPermissionRight, EntityPermissionValueRight);
          END LOOP;
        END IF;

        EntityPermissionValueRight := row(null);

        --This case would retrieve PermissionValue set exactly for this entity (overrides groups authorize result)
        EXECUTE 'SELECT * FROM public.""PermissionValues"" WHERE ""PermissionId"" = $1 AND ""EntityId"" = $2' INTO EntityPermissionValueRight USING EntityPermissionRight.""Id"", EntityRight.""Id"";
        IF NOT(EntityPermissionValueRight IS NULL) THEN
          --This Entity might contain PermissionValues
          RAISE NOTICE 'AuthorizeEntityPermissionToEntityPermission: EntityRight: %s', EntityRight.""Id""::text;
          RAISE NOTICE 'AuthorizeEntityPermissionToEntityPermission: EntityPermissionValueRight: FOUND!';

          FResult := public.""AuthorizeEntityPermissionToEntityPermissionValue""(EntityLeft, EntityGroupLeftTableName, EntityToEntityGroupMappingLeftTableName, EntityPermissionLeft, EntityPermissionRight, EntityPermissionValueRight);
        END IF;

        RAISE NOTICE 'AuthorizeEntityPermissionToEntityPermission: RET: %s', FResult::text;

        RETURN FResult;
      END;
    $$;
";
    }
}