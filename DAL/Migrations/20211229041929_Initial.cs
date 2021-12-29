using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Permissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Alias = table.Column<string>(type: "text", nullable: true),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserGroups",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Alias = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: true),
                    IsEmailValidated = table.Column<bool>(type: "boolean", nullable: false),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    IsPhoneNumberVerified = table.Column<bool>(type: "boolean", nullable: false),
                    Password = table.Column<string>(type: "text", nullable: true),
                    FailedSignInAttempts = table.Column<int>(type: "integer", nullable: false),
                    DisableSignInUntil = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LastSignIn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastActivity = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastIpAddress = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EntityPermissionValueBase<UserGroup>",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Value = table.Column<byte[]>(type: "bytea", nullable: true),
                    Grant = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    PermissionId = table.Column<Guid>(type: "uuid", nullable: false),
                    EntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    Discriminator = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntityPermissionValueBase<UserGroup>", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EntityPermissionValueBase<UserGroup>_Permissions_Permission~",
                        column: x => x.PermissionId,
                        principalTable: "Permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EntityPermissionValueBase<UserGroup>_UserGroups_EntityId",
                        column: x => x.EntityId,
                        principalTable: "UserGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EntityToGroupMappingBase<User, UserGroup>",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    GroupId = table.Column<Guid>(type: "uuid", nullable: false),
                    Discriminator = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntityToGroupMappingBase<User, UserGroup>", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EntityToGroupMappingBase<User, UserGroup>_UserGroups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "UserGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EntityToGroupMappingBase<User, UserGroup>_Users_EntityId",
                        column: x => x.EntityId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JsonWebTokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Token = table.Column<string>(type: "text", nullable: true),
                    ExpiresAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JsonWebTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JsonWebTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Token = table.Column<string>(type: "text", nullable: true),
                    ExpiresAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserProfiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Username = table.Column<string>(type: "text", nullable: true),
                    FirstName = table.Column<string>(type: "text", nullable: true),
                    LastName = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserProfiles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "Alias", "CreatedAt", "Type", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("00000001-0000-0000-0000-000000000000"), "boolean_any_view_permission_overview", new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("00000002-0000-0000-0000-000000000000"), "boolean_any_view_permission_overview_own", new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("00000003-0000-0000-0000-000000000000"), "uint64_any_modify_permission_power", new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 9, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("00000004-0000-0000-0000-000000000000"), "boolean_group_create_groups", new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("00000005-0000-0000-0000-000000000000"), "boolean_group_view_groups", new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("00000006-0000-0000-0000-000000000000"), "boolean_group_view_groups_own", new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("00000007-0000-0000-0000-000000000000"), "uint64_group_delete_groups_power", new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 9, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("00000008-0000-0000-0000-000000000000"), "uint64_group_delete_groups_power_needed", new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 9, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("00000009-0000-0000-0000-000000000000"), "uint64_group_add_member_power", new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 9, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("0000000a-0000-0000-0000-000000000000"), "uint64_group_add_member_power_needed", new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 9, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("0000000b-0000-0000-0000-000000000000"), "uint64_group_remove_member_power", new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 9, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("0000000c-0000-0000-0000-000000000000"), "uint64_group_remove_member_power_needed", new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 9, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("0000000d-0000-0000-0000-000000000000"), "uint64_group_modify_alias_power", new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 9, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("0000000e-0000-0000-0000-000000000000"), "uint64_group_modify_alias_power_needed", new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 9, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("0000000f-0000-0000-0000-000000000000"), "uint64_group_modify_description_power", new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 9, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("00000010-0000-0000-0000-000000000000"), "uint64_group_modify_description_power_needed", new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 9, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("00000011-0000-0000-0000-000000000000"), "boolean_user_view_email", new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("00000012-0000-0000-0000-000000000000"), "boolean_user_view_email_own", new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("00000013-0000-0000-0000-000000000000"), "uint64_user_modify_email_power", new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 9, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("00000014-0000-0000-0000-000000000000"), "uint64_user_modify_email_power_needed", new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 9, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("00000015-0000-0000-0000-000000000000"), "uint64_user_modify_email_own_power_needed", new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 9, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("00000016-0000-0000-0000-000000000000"), "boolean_user_view_lastipaddress", new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("00000017-0000-0000-0000-000000000000"), "boolean_user_view_profile_username", new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("00000018-0000-0000-0000-000000000000"), "boolean_user_view_profile_username_own", new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("00000019-0000-0000-0000-000000000000"), "uint64_user_modify_profile_username_power", new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 9, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("0000001a-0000-0000-0000-000000000000"), "uint64_user_modify_profile_username_power_needed", new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 9, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("0000001b-0000-0000-0000-000000000000"), "uint64_user_modify_profile_username_own_power_needed", new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 9, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("0000001c-0000-0000-0000-000000000000"), "boolean_user_view_profile_basic", new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("0000001d-0000-0000-0000-000000000000"), "boolean_user_view_profile_basic_own", new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("0000001e-0000-0000-0000-000000000000"), "uint64_user_modify_profile_basic_power", new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 9, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("0000001f-0000-0000-0000-000000000000"), "uint64_user_modify_profile_basic_power_needed", new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 9, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("00000020-0000-0000-0000-000000000000"), "uint64_user_modify_profile_basic_own_power_needed", new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 9, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("00000021-0000-0000-0000-000000000000"), "boolean_user_view_profile_custom", new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("00000022-0000-0000-0000-000000000000"), "boolean_user_view_profile_custom_own", new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("00000023-0000-0000-0000-000000000000"), "uint64_user_modify_profile_custom_power", new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 9, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("00000024-0000-0000-0000-000000000000"), "uint64_user_modify_profile_custom_power_needed", new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 9, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("00000025-0000-0000-0000-000000000000"), "uint64_user_modify_profile_custom_own_power_needed", new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 9, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("00000026-0000-0000-0000-000000000000"), "uint64_user_delete_user_power", new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 9, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("00000027-0000-0000-0000-000000000000"), "uint64_user_delete_user_power_needed", new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 9, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("00000028-0000-0000-0000-000000000000"), "uint64_user_delete_user_own_power_needed", new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 9, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("00000029-0000-0000-0000-000000000000"), "uint64_user_communication_private_power", new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 9, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("0000002a-0000-0000-0000-000000000000"), "uint64_user_communication_private_power_needed", new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 9, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("0000002b-0000-0000-0000-000000000000"), "uint64_user_communication_private_own_power_needed", new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 9, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("0000002c-0000-0000-0000-000000000000"), "uint64_user_communication_global_power", new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 9, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("0000002d-0000-0000-0000-000000000000"), "uint64_user_communication_global_power_needed", new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 9, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("0000002e-0000-0000-0000-000000000000"), "uint64_user_poke_power", new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 9, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("0000002f-0000-0000-0000-000000000000"), "uint64_user_poke_power_needed", new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 9, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("00000030-0000-0000-0000-000000000000"), "uint64_user_max_apuint64_keys", new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 9, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("00000031-0000-0000-0000-000000000000"), "uint64_user_max_avatar_filesize", new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 9, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("00000032-0000-0000-0000-000000000000"), "uint64_user_create_complaints_power", new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 9, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("00000033-0000-0000-0000-000000000000"), "uint64_user_create_complaints_power_needed", new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 9, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("00000034-0000-0000-0000-000000000000"), "boolean_user_view_complaints", new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("00000035-0000-0000-0000-000000000000"), "boolean_user_view_complaints_own", new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("00000036-0000-0000-0000-000000000000"), "uint64_user_modify_complaints_power", new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 9, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("00000037-0000-0000-0000-000000000000"), "uint64_user_modify_complaints_power_needed", new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 9, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("00000038-0000-0000-0000-000000000000"), "uint64_user_modify_complaints_own_power_needed", new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 9, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("00000039-0000-0000-0000-000000000000"), "uint64_user_delete_complaints_power", new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 9, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("0000003a-0000-0000-0000-000000000000"), "uint64_user_delete_complaints_power_needed", new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 9, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("0000003b-0000-0000-0000-000000000000"), "uint64_user_delete_complaints_own_power_needed", new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 9, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("0000003c-0000-0000-0000-000000000000"), "uint64_user_create_bans_power", new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 9, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("0000003d-0000-0000-0000-000000000000"), "uint64_user_create_bans_power_needed", new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 9, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("0000003e-0000-0000-0000-000000000000"), "boolean_user_view_bans", new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("0000003f-0000-0000-0000-000000000000"), "boolean_user_view_bans_own", new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("00000040-0000-0000-0000-000000000000"), "uint64_user_modify_bans_power", new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 9, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("00000041-0000-0000-0000-000000000000"), "uint64_user_modify_bans_power_needed", new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 9, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("00000042-0000-0000-0000-000000000000"), "uint64_user_modify_bans_own_power_needed", new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 9, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("00000043-0000-0000-0000-000000000000"), "uint64_user_delete_bans_power", new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 9, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("00000044-0000-0000-0000-000000000000"), "uint64_user_delete_bans_power_needed", new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 9, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("00000045-0000-0000-0000-000000000000"), "uint64_user_delete_bans_own_power_needed", new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 9, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("00000046-0000-0000-0000-000000000000"), "uint64_user_max_ban_time", new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 9, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) }
                });

            migrationBuilder.InsertData(
                table: "UserGroups",
                columns: new[] { "Id", "Alias", "CreatedAt", "Description", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("00000049-0000-0000-0000-000000000000"), "Root", new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "System user group with root like permission set", new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("0000004a-0000-0000-0000-000000000000"), "Guest", new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "System user group with guest like permission set", new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "DisableSignInUntil", "Email", "FailedSignInAttempts", "IsEmailValidated", "IsPhoneNumberVerified", "LastActivity", "LastIpAddress", "LastSignIn", "Password", "PhoneNumber", "UpdatedAt" },
                values: new object[] { new Guid("00000047-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, 0, false, false, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.InsertData(
                table: "EntityPermissionValueBase<UserGroup>",
                columns: new[] { "Id", "CreatedAt", "Discriminator", "EntityId", "Grant", "PermissionId", "UpdatedAt", "Value" },
                values: new object[,]
                {
                    { new Guid("0000004b-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("00000049-0000-0000-0000-000000000000"), 18446744073709551615m, new Guid("00000001-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 1 } },
                    { new Guid("0000004c-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("00000049-0000-0000-0000-000000000000"), 18446744073709551615m, new Guid("00000002-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 1 } },
                    { new Guid("0000004d-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("00000049-0000-0000-0000-000000000000"), 18446744073709551615m, new Guid("00000003-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 255, 255, 255, 255, 255, 255, 255, 255 } },
                    { new Guid("0000004e-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("00000049-0000-0000-0000-000000000000"), 18446744073709551615m, new Guid("00000004-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 1 } },
                    { new Guid("0000004f-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("00000049-0000-0000-0000-000000000000"), 18446744073709551615m, new Guid("00000005-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 1 } },
                    { new Guid("00000050-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("00000049-0000-0000-0000-000000000000"), 18446744073709551615m, new Guid("00000006-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 1 } },
                    { new Guid("00000051-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("00000049-0000-0000-0000-000000000000"), 18446744073709551615m, new Guid("00000007-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 255, 255, 255, 255, 255, 255, 255, 255 } },
                    { new Guid("00000052-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("00000049-0000-0000-0000-000000000000"), 18446744073709551615m, new Guid("00000008-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 255, 255, 255, 255, 255, 255, 255, 255 } },
                    { new Guid("00000053-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("00000049-0000-0000-0000-000000000000"), 18446744073709551615m, new Guid("00000009-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 255, 255, 255, 255, 255, 255, 255, 255 } },
                    { new Guid("00000054-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("00000049-0000-0000-0000-000000000000"), 18446744073709551615m, new Guid("0000000a-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 255, 255, 255, 255, 255, 255, 255, 255 } },
                    { new Guid("00000055-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("00000049-0000-0000-0000-000000000000"), 18446744073709551615m, new Guid("0000000b-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 255, 255, 255, 255, 255, 255, 255, 255 } },
                    { new Guid("00000056-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("00000049-0000-0000-0000-000000000000"), 18446744073709551615m, new Guid("0000000c-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 255, 255, 255, 255, 255, 255, 255, 255 } },
                    { new Guid("00000057-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("00000049-0000-0000-0000-000000000000"), 18446744073709551615m, new Guid("0000000d-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 255, 255, 255, 255, 255, 255, 255, 255 } },
                    { new Guid("00000058-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("00000049-0000-0000-0000-000000000000"), 18446744073709551615m, new Guid("0000000e-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 255, 255, 255, 255, 255, 255, 255, 255 } },
                    { new Guid("00000059-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("00000049-0000-0000-0000-000000000000"), 18446744073709551615m, new Guid("0000000f-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 255, 255, 255, 255, 255, 255, 255, 255 } },
                    { new Guid("0000005a-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("00000049-0000-0000-0000-000000000000"), 18446744073709551615m, new Guid("00000010-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 255, 255, 255, 255, 255, 255, 255, 255 } },
                    { new Guid("0000005b-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("00000049-0000-0000-0000-000000000000"), 18446744073709551615m, new Guid("00000011-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 1 } },
                    { new Guid("0000005c-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("00000049-0000-0000-0000-000000000000"), 18446744073709551615m, new Guid("00000012-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 1 } },
                    { new Guid("0000005d-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("00000049-0000-0000-0000-000000000000"), 18446744073709551615m, new Guid("00000013-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 255, 255, 255, 255, 255, 255, 255, 255 } },
                    { new Guid("0000005e-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("00000049-0000-0000-0000-000000000000"), 18446744073709551615m, new Guid("00000014-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 255, 255, 255, 255, 255, 255, 255, 255 } },
                    { new Guid("0000005f-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("00000049-0000-0000-0000-000000000000"), 18446744073709551615m, new Guid("00000015-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 255, 255, 255, 255, 255, 255, 255, 255 } },
                    { new Guid("00000060-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("00000049-0000-0000-0000-000000000000"), 18446744073709551615m, new Guid("00000016-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 1 } },
                    { new Guid("00000061-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("00000049-0000-0000-0000-000000000000"), 18446744073709551615m, new Guid("00000017-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 1 } },
                    { new Guid("00000062-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("00000049-0000-0000-0000-000000000000"), 18446744073709551615m, new Guid("00000018-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 1 } },
                    { new Guid("00000063-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("00000049-0000-0000-0000-000000000000"), 18446744073709551615m, new Guid("00000019-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 255, 255, 255, 255, 255, 255, 255, 255 } },
                    { new Guid("00000064-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("00000049-0000-0000-0000-000000000000"), 18446744073709551615m, new Guid("0000001a-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 255, 255, 255, 255, 255, 255, 255, 255 } },
                    { new Guid("00000065-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("00000049-0000-0000-0000-000000000000"), 18446744073709551615m, new Guid("0000001b-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 255, 255, 255, 255, 255, 255, 255, 255 } },
                    { new Guid("00000066-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("00000049-0000-0000-0000-000000000000"), 18446744073709551615m, new Guid("0000001c-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 1 } },
                    { new Guid("00000067-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("00000049-0000-0000-0000-000000000000"), 18446744073709551615m, new Guid("0000001d-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 1 } },
                    { new Guid("00000068-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("00000049-0000-0000-0000-000000000000"), 18446744073709551615m, new Guid("0000001e-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 255, 255, 255, 255, 255, 255, 255, 255 } },
                    { new Guid("00000069-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("00000049-0000-0000-0000-000000000000"), 18446744073709551615m, new Guid("0000001f-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 255, 255, 255, 255, 255, 255, 255, 255 } },
                    { new Guid("0000006a-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("00000049-0000-0000-0000-000000000000"), 18446744073709551615m, new Guid("00000020-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 255, 255, 255, 255, 255, 255, 255, 255 } },
                    { new Guid("0000006b-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("00000049-0000-0000-0000-000000000000"), 18446744073709551615m, new Guid("00000021-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 1 } },
                    { new Guid("0000006c-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("00000049-0000-0000-0000-000000000000"), 18446744073709551615m, new Guid("00000022-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 1 } },
                    { new Guid("0000006d-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("00000049-0000-0000-0000-000000000000"), 18446744073709551615m, new Guid("00000023-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 255, 255, 255, 255, 255, 255, 255, 255 } },
                    { new Guid("0000006e-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("00000049-0000-0000-0000-000000000000"), 18446744073709551615m, new Guid("00000024-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 255, 255, 255, 255, 255, 255, 255, 255 } },
                    { new Guid("0000006f-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("00000049-0000-0000-0000-000000000000"), 18446744073709551615m, new Guid("00000025-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 255, 255, 255, 255, 255, 255, 255, 255 } },
                    { new Guid("00000070-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("00000049-0000-0000-0000-000000000000"), 18446744073709551615m, new Guid("00000026-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 255, 255, 255, 255, 255, 255, 255, 255 } },
                    { new Guid("00000071-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("00000049-0000-0000-0000-000000000000"), 18446744073709551615m, new Guid("00000027-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 255, 255, 255, 255, 255, 255, 255, 255 } },
                    { new Guid("00000072-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("00000049-0000-0000-0000-000000000000"), 18446744073709551615m, new Guid("00000028-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 255, 255, 255, 255, 255, 255, 255, 255 } },
                    { new Guid("00000073-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("00000049-0000-0000-0000-000000000000"), 18446744073709551615m, new Guid("00000029-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 255, 255, 255, 255, 255, 255, 255, 255 } },
                    { new Guid("00000074-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("00000049-0000-0000-0000-000000000000"), 18446744073709551615m, new Guid("0000002a-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 255, 255, 255, 255, 255, 255, 255, 255 } },
                    { new Guid("00000075-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("00000049-0000-0000-0000-000000000000"), 18446744073709551615m, new Guid("0000002b-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 255, 255, 255, 255, 255, 255, 255, 255 } },
                    { new Guid("00000076-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("00000049-0000-0000-0000-000000000000"), 18446744073709551615m, new Guid("0000002c-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 255, 255, 255, 255, 255, 255, 255, 255 } },
                    { new Guid("00000077-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("00000049-0000-0000-0000-000000000000"), 18446744073709551615m, new Guid("0000002e-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 255, 255, 255, 255, 255, 255, 255, 255 } },
                    { new Guid("00000078-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("00000049-0000-0000-0000-000000000000"), 18446744073709551615m, new Guid("0000002f-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 255, 255, 255, 255, 255, 255, 255, 255 } },
                    { new Guid("00000079-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("00000049-0000-0000-0000-000000000000"), 18446744073709551615m, new Guid("00000030-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 255, 255, 255, 255, 255, 255, 255, 255 } },
                    { new Guid("0000007a-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("00000049-0000-0000-0000-000000000000"), 18446744073709551615m, new Guid("00000031-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 255, 255, 255, 255, 255, 255, 255, 255 } },
                    { new Guid("0000007b-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("00000049-0000-0000-0000-000000000000"), 18446744073709551615m, new Guid("00000032-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 255, 255, 255, 255, 255, 255, 255, 255 } },
                    { new Guid("0000007c-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("00000049-0000-0000-0000-000000000000"), 18446744073709551615m, new Guid("00000033-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 255, 255, 255, 255, 255, 255, 255, 255 } },
                    { new Guid("0000007d-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("00000049-0000-0000-0000-000000000000"), 18446744073709551615m, new Guid("00000034-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 1 } },
                    { new Guid("0000007e-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("00000049-0000-0000-0000-000000000000"), 18446744073709551615m, new Guid("00000035-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 1 } },
                    { new Guid("0000007f-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("00000049-0000-0000-0000-000000000000"), 18446744073709551615m, new Guid("00000036-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 255, 255, 255, 255, 255, 255, 255, 255 } },
                    { new Guid("00000080-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("00000049-0000-0000-0000-000000000000"), 18446744073709551615m, new Guid("00000037-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 255, 255, 255, 255, 255, 255, 255, 255 } },
                    { new Guid("00000081-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("00000049-0000-0000-0000-000000000000"), 18446744073709551615m, new Guid("00000038-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 255, 255, 255, 255, 255, 255, 255, 255 } },
                    { new Guid("00000082-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("00000049-0000-0000-0000-000000000000"), 18446744073709551615m, new Guid("00000039-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 255, 255, 255, 255, 255, 255, 255, 255 } },
                    { new Guid("00000083-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("00000049-0000-0000-0000-000000000000"), 18446744073709551615m, new Guid("0000003a-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 255, 255, 255, 255, 255, 255, 255, 255 } },
                    { new Guid("00000084-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("00000049-0000-0000-0000-000000000000"), 18446744073709551615m, new Guid("0000003b-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 255, 255, 255, 255, 255, 255, 255, 255 } },
                    { new Guid("00000085-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("00000049-0000-0000-0000-000000000000"), 18446744073709551615m, new Guid("0000003c-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 255, 255, 255, 255, 255, 255, 255, 255 } },
                    { new Guid("00000086-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("00000049-0000-0000-0000-000000000000"), 18446744073709551615m, new Guid("0000003d-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 255, 255, 255, 255, 255, 255, 255, 255 } },
                    { new Guid("00000087-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("00000049-0000-0000-0000-000000000000"), 18446744073709551615m, new Guid("0000003e-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 1 } },
                    { new Guid("00000088-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("00000049-0000-0000-0000-000000000000"), 18446744073709551615m, new Guid("0000003f-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 1 } },
                    { new Guid("00000089-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("00000049-0000-0000-0000-000000000000"), 18446744073709551615m, new Guid("00000040-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 255, 255, 255, 255, 255, 255, 255, 255 } },
                    { new Guid("0000008a-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("00000049-0000-0000-0000-000000000000"), 18446744073709551615m, new Guid("00000041-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 255, 255, 255, 255, 255, 255, 255, 255 } },
                    { new Guid("0000008b-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("00000049-0000-0000-0000-000000000000"), 18446744073709551615m, new Guid("00000042-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 255, 255, 255, 255, 255, 255, 255, 255 } },
                    { new Guid("0000008c-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("00000049-0000-0000-0000-000000000000"), 18446744073709551615m, new Guid("00000043-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 255, 255, 255, 255, 255, 255, 255, 255 } },
                    { new Guid("0000008d-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("00000049-0000-0000-0000-000000000000"), 18446744073709551615m, new Guid("00000044-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 255, 255, 255, 255, 255, 255, 255, 255 } },
                    { new Guid("0000008e-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("00000049-0000-0000-0000-000000000000"), 18446744073709551615m, new Guid("00000045-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 255, 255, 255, 255, 255, 255, 255, 255 } },
                    { new Guid("0000008f-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("00000049-0000-0000-0000-000000000000"), 18446744073709551615m, new Guid("00000046-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 255, 255, 255, 255, 255, 255, 255, 255 } },
                    { new Guid("00000090-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("0000004a-0000-0000-0000-000000000000"), 18446744073709551615m, new Guid("00000002-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 1 } },
                    { new Guid("00000091-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("0000004a-0000-0000-0000-000000000000"), 18446744073709551615m, new Guid("00000005-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 1 } },
                    { new Guid("00000092-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("0000004a-0000-0000-0000-000000000000"), 18446744073709551615m, new Guid("00000006-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 1 } },
                    { new Guid("00000093-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("0000004a-0000-0000-0000-000000000000"), 18446744073709551615m, new Guid("00000008-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 255, 255, 255, 255, 255, 255, 255, 255 } },
                    { new Guid("00000094-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("0000004a-0000-0000-0000-000000000000"), 18446744073709551615m, new Guid("0000000a-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 255, 255, 255, 255, 255, 255, 255, 255 } },
                    { new Guid("00000095-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("0000004a-0000-0000-0000-000000000000"), 18446744073709551615m, new Guid("0000000c-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 255, 255, 255, 255, 255, 255, 255, 255 } },
                    { new Guid("00000096-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("0000004a-0000-0000-0000-000000000000"), 18446744073709551615m, new Guid("0000000e-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 255, 255, 255, 255, 255, 255, 255, 255 } },
                    { new Guid("00000097-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("0000004a-0000-0000-0000-000000000000"), 18446744073709551615m, new Guid("00000010-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 255, 255, 255, 255, 255, 255, 255, 255 } },
                    { new Guid("00000098-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("0000004a-0000-0000-0000-000000000000"), 18446744073709551615m, new Guid("00000011-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 0 } },
                    { new Guid("00000099-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("0000004a-0000-0000-0000-000000000000"), 18446744073709551615m, new Guid("00000012-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 1 } },
                    { new Guid("0000009a-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("0000004a-0000-0000-0000-000000000000"), 18446744073709551615m, new Guid("00000013-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 50, 0, 0, 0, 0, 0, 0, 0 } },
                    { new Guid("0000009b-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("0000004a-0000-0000-0000-000000000000"), 18446744073709551615m, new Guid("00000014-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 255, 255, 255, 255, 255, 255, 255, 255 } },
                    { new Guid("0000009c-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("0000004a-0000-0000-0000-000000000000"), 18446744073709551615m, new Guid("00000015-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 50, 0, 0, 0, 0, 0, 0, 0 } },
                    { new Guid("0000009d-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("0000004a-0000-0000-0000-000000000000"), 18446744073709551615m, new Guid("00000016-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 0 } },
                    { new Guid("0000009e-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("0000004a-0000-0000-0000-000000000000"), 18446744073709551615m, new Guid("00000017-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 1 } },
                    { new Guid("0000009f-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("0000004a-0000-0000-0000-000000000000"), 18446744073709551615m, new Guid("00000018-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 1 } },
                    { new Guid("000000a0-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("0000004a-0000-0000-0000-000000000000"), 18446744073709551615m, new Guid("00000019-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 50, 0, 0, 0, 0, 0, 0, 0 } },
                    { new Guid("000000a1-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("0000004a-0000-0000-0000-000000000000"), 18446744073709551615m, new Guid("0000001a-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 255, 255, 255, 255, 255, 255, 255, 255 } },
                    { new Guid("000000a2-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("0000004a-0000-0000-0000-000000000000"), 18446744073709551615m, new Guid("0000001b-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 50, 0, 0, 0, 0, 0, 0, 0 } },
                    { new Guid("000000a3-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("0000004a-0000-0000-0000-000000000000"), 18446744073709551615m, new Guid("0000001c-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 1 } },
                    { new Guid("000000a4-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("0000004a-0000-0000-0000-000000000000"), 18446744073709551615m, new Guid("0000001d-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 1 } },
                    { new Guid("000000a5-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("0000004a-0000-0000-0000-000000000000"), 18446744073709551615m, new Guid("0000001e-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 50, 0, 0, 0, 0, 0, 0, 0 } },
                    { new Guid("000000a6-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("0000004a-0000-0000-0000-000000000000"), 18446744073709551615m, new Guid("0000001f-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 255, 255, 255, 255, 255, 255, 255, 255 } },
                    { new Guid("000000a7-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("0000004a-0000-0000-0000-000000000000"), 18446744073709551615m, new Guid("00000020-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 50, 0, 0, 0, 0, 0, 0, 0 } },
                    { new Guid("000000a8-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("0000004a-0000-0000-0000-000000000000"), 18446744073709551615m, new Guid("00000021-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 1 } },
                    { new Guid("000000a9-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("0000004a-0000-0000-0000-000000000000"), 18446744073709551615m, new Guid("00000022-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 1 } },
                    { new Guid("000000aa-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("0000004a-0000-0000-0000-000000000000"), 18446744073709551615m, new Guid("00000023-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 50, 0, 0, 0, 0, 0, 0, 0 } },
                    { new Guid("000000ab-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("0000004a-0000-0000-0000-000000000000"), 18446744073709551615m, new Guid("00000024-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 255, 255, 255, 255, 255, 255, 255, 255 } },
                    { new Guid("000000ac-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("0000004a-0000-0000-0000-000000000000"), 18446744073709551615m, new Guid("00000025-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 50, 0, 0, 0, 0, 0, 0, 0 } },
                    { new Guid("000000ad-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("0000004a-0000-0000-0000-000000000000"), 18446744073709551615m, new Guid("00000026-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 50, 0, 0, 0, 0, 0, 0, 0 } },
                    { new Guid("000000ae-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("0000004a-0000-0000-0000-000000000000"), 18446744073709551615m, new Guid("00000027-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 255, 255, 255, 255, 255, 255, 255, 255 } },
                    { new Guid("000000af-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("0000004a-0000-0000-0000-000000000000"), 18446744073709551615m, new Guid("00000028-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 50, 0, 0, 0, 0, 0, 0, 0 } },
                    { new Guid("000000b0-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("0000004a-0000-0000-0000-000000000000"), 18446744073709551615m, new Guid("00000029-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 50, 0, 0, 0, 0, 0, 0, 0 } },
                    { new Guid("000000b1-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("0000004a-0000-0000-0000-000000000000"), 18446744073709551615m, new Guid("0000002a-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 50, 0, 0, 0, 0, 0, 0, 0 } },
                    { new Guid("000000b2-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("0000004a-0000-0000-0000-000000000000"), 18446744073709551615m, new Guid("0000002b-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 50, 0, 0, 0, 0, 0, 0, 0 } },
                    { new Guid("000000b3-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("0000004a-0000-0000-0000-000000000000"), 18446744073709551615m, new Guid("0000002c-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 50, 0, 0, 0, 0, 0, 0, 0 } },
                    { new Guid("000000b4-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("0000004a-0000-0000-0000-000000000000"), 18446744073709551615m, new Guid("0000002e-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 50, 0, 0, 0, 0, 0, 0, 0 } },
                    { new Guid("000000b5-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("0000004a-0000-0000-0000-000000000000"), 18446744073709551615m, new Guid("0000002f-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 50, 0, 0, 0, 0, 0, 0, 0 } },
                    { new Guid("000000b6-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("0000004a-0000-0000-0000-000000000000"), 18446744073709551615m, new Guid("00000030-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 0, 0, 0, 0 } },
                    { new Guid("000000b7-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("0000004a-0000-0000-0000-000000000000"), 18446744073709551615m, new Guid("00000031-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 0, 0, 0, 0 } },
                    { new Guid("000000b8-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("0000004a-0000-0000-0000-000000000000"), 18446744073709551615m, new Guid("00000032-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 50, 0, 0, 0, 0, 0, 0, 0 } },
                    { new Guid("000000b9-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("0000004a-0000-0000-0000-000000000000"), 18446744073709551615m, new Guid("00000033-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 50, 0, 0, 0, 0, 0, 0, 0 } },
                    { new Guid("000000ba-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("0000004a-0000-0000-0000-000000000000"), 18446744073709551615m, new Guid("00000034-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 0 } },
                    { new Guid("000000bb-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("0000004a-0000-0000-0000-000000000000"), 18446744073709551615m, new Guid("00000035-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 1 } },
                    { new Guid("000000bc-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("0000004a-0000-0000-0000-000000000000"), 18446744073709551615m, new Guid("00000036-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 50, 0, 0, 0, 0, 0, 0, 0 } },
                    { new Guid("000000bd-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("0000004a-0000-0000-0000-000000000000"), 18446744073709551615m, new Guid("00000037-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 255, 255, 255, 255, 255, 255, 255, 255 } },
                    { new Guid("000000be-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("0000004a-0000-0000-0000-000000000000"), 18446744073709551615m, new Guid("00000038-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 50, 0, 0, 0, 0, 0, 0, 0 } },
                    { new Guid("000000bf-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("0000004a-0000-0000-0000-000000000000"), 18446744073709551615m, new Guid("00000039-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 50, 0, 0, 0, 0, 0, 0, 0 } },
                    { new Guid("000000c0-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("0000004a-0000-0000-0000-000000000000"), 18446744073709551615m, new Guid("0000003a-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 255, 255, 255, 255, 255, 255, 255, 255 } },
                    { new Guid("000000c1-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("0000004a-0000-0000-0000-000000000000"), 18446744073709551615m, new Guid("0000003b-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 50, 0, 0, 0, 0, 0, 0, 0 } },
                    { new Guid("000000c2-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("0000004a-0000-0000-0000-000000000000"), 18446744073709551615m, new Guid("0000003d-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 255, 255, 255, 255, 255, 255, 255, 255 } },
                    { new Guid("000000c3-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("0000004a-0000-0000-0000-000000000000"), 18446744073709551615m, new Guid("00000041-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 255, 255, 255, 255, 255, 255, 255, 255 } },
                    { new Guid("000000c4-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("0000004a-0000-0000-0000-000000000000"), 18446744073709551615m, new Guid("00000044-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 255, 255, 255, 255, 255, 255, 255, 255 } }
                });

            migrationBuilder.InsertData(
                table: "EntityToGroupMappingBase<User, UserGroup>",
                columns: new[] { "Id", "CreatedAt", "Discriminator", "EntityId", "GroupId", "UpdatedAt" },
                values: new object[] { new Guid("0000004b-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserToGroupMapping", new Guid("00000047-0000-0000-0000-000000000000"), new Guid("0000004a-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.InsertData(
                table: "UserProfiles",
                columns: new[] { "Id", "CreatedAt", "Description", "FirstName", "LastName", "UpdatedAt", "UserId", "Username" },
                values: new object[] { new Guid("00000048-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Initial system user", null, null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new Guid("00000047-0000-0000-0000-000000000000"), "Guest" });

            migrationBuilder.CreateIndex(
                name: "IX_EntityPermissionValueBase<UserGroup>_EntityId",
                table: "EntityPermissionValueBase<UserGroup>",
                column: "EntityId");

            migrationBuilder.CreateIndex(
                name: "IX_EntityPermissionValueBase<UserGroup>_PermissionId",
                table: "EntityPermissionValueBase<UserGroup>",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_EntityPermissionValueBase<UserGroup>_PermissionId_EntityId",
                table: "EntityPermissionValueBase<UserGroup>",
                columns: new[] { "PermissionId", "EntityId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EntityToGroupMappingBase<User, UserGroup>_EntityId_GroupId",
                table: "EntityToGroupMappingBase<User, UserGroup>",
                columns: new[] { "EntityId", "GroupId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EntityToGroupMappingBase<User, UserGroup>_GroupId",
                table: "EntityToGroupMappingBase<User, UserGroup>",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_JsonWebTokens_Token",
                table: "JsonWebTokens",
                column: "Token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JsonWebTokens_UserId",
                table: "JsonWebTokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_Alias",
                table: "Permissions",
                column: "Alias",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_Token",
                table: "RefreshTokens",
                column: "Token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UserId",
                table: "RefreshTokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserGroups_Alias",
                table: "UserGroups",
                column: "Alias",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserProfiles_UserId",
                table: "UserProfiles",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserProfiles_Username",
                table: "UserProfiles",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EntityPermissionValueBase<UserGroup>");

            migrationBuilder.DropTable(
                name: "EntityToGroupMappingBase<User, UserGroup>");

            migrationBuilder.DropTable(
                name: "JsonWebTokens");

            migrationBuilder.DropTable(
                name: "RefreshTokens");

            migrationBuilder.DropTable(
                name: "UserProfiles");

            migrationBuilder.DropTable(
                name: "Permissions");

            migrationBuilder.DropTable(
                name: "UserGroups");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
