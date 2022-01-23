using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Common.Options;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.SqlServer.Infrastructure.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DAL.Data
{
    public sealed class AppDbContext : DbContext
    {
        #region DbSets

        public DbSet<Permission> Permissions { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserGroup> UserGroups { get; set; }
        public DbSet<UserGroupPermissionValue> UserGroupPermissionValues { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<UserToGroupMapping> UserToGroupMappings { get; set; }
        public DbSet<JsonWebToken> JsonWebTokens { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        #endregion

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            Database.Migrate();
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        // {
        //     base.OnConfiguring(optionsBuilder);
        // }

        public class AppDbContextSeedLists
        {
            public virtual List<Permission> Permissions { get; set; }
            public virtual List<User> Users { get; set; }
            public virtual List<UserGroup> UserGroups { get; set; }
            public virtual List<UserGroupPermissionValue> UserGroupPermissionValues { get; set; }
            public virtual List<UserProfile> UserProfiles { get; set; }
            public virtual List<UserToGroupMapping> UserToGroupMappings { get; set; }
            public virtual List<JsonWebToken> JsonWebTokens { get; set; }
            public virtual List<RefreshToken> RefreshTokens { get; set; }
        }

        public static AppDbContextSeedLists Seed()
        {
            var appDbContextSeedLists = new AppDbContextSeedLists
            {
                Permissions = new List<Permission>(),
                Users = new List<User>(),
                UserGroups = new List<UserGroup>(),
                UserGroupPermissionValues = new List<UserGroupPermissionValue>(),
                UserProfiles = new List<UserProfile>(),
                UserToGroupMappings = new List<UserToGroupMapping>(),
                JsonWebTokens = new List<JsonWebToken>(),
                RefreshTokens = new List<RefreshToken>()
            };

            var globalGuidOffset = 0;

            #region Permissions

            var permissionsGuidOffset = globalGuidOffset;

            var permissions = new List<Permission>
            {
                #region Any

                new()
                {
                    Id = ToGuid(++permissionsGuidOffset),
                    Alias = "boolean_any_view_permission_overview",
                    Type = PermissionType.Boolean,
                    CompareMode = PermissionCompareMode.Equal
                },
                new()
                {
                    Id = ToGuid(++permissionsGuidOffset),
                    Alias = "boolean_any_view_permission_overview_own",
                    Type = PermissionType.Boolean,
                    CompareMode = PermissionCompareMode.Equal
                },
                new()
                {
                    Id = ToGuid(++permissionsGuidOffset),
                    Alias = "uint64_any_modify_permission_power",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.LessOrEqual
                },

                #endregion

                #region Group

                new()
                {
                    Id = ToGuid(++permissionsGuidOffset),
                    Alias = "boolean_group_create_groups",
                    Type = PermissionType.Boolean,
                    CompareMode = PermissionCompareMode.Equal
                },
                new()
                {
                    Id = ToGuid(++permissionsGuidOffset),
                    Alias = "boolean_group_view_groups",
                    Type = PermissionType.Boolean,
                    CompareMode = PermissionCompareMode.Equal
                },
                new()
                {
                    Id = ToGuid(++permissionsGuidOffset),
                    Alias = "boolean_group_view_groups_own",
                    Type = PermissionType.Boolean,
                    CompareMode = PermissionCompareMode.Equal
                },
                new()
                {
                    Id = ToGuid(++permissionsGuidOffset),
                    Alias = "uint64_group_delete_groups_power",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.LessOrEqual
                },
                new()
                {
                    Id = ToGuid(++permissionsGuidOffset),
                    Alias = "uint64_group_delete_groups_power_needed",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.LessOrEqual
                },
                new()
                {
                    Id = ToGuid(++permissionsGuidOffset),
                    Alias = "uint64_group_add_member_power",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.LessOrEqual
                },
                new()
                {
                    Id = ToGuid(++permissionsGuidOffset),
                    Alias = "uint64_group_add_member_power_needed",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.LessOrEqual
                },
                new()
                {
                    Id = ToGuid(++permissionsGuidOffset),
                    Alias = "uint64_group_remove_member_power",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.LessOrEqual
                },
                new()
                {
                    Id = ToGuid(++permissionsGuidOffset),
                    Alias = "uint64_group_remove_member_power_needed",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.LessOrEqual
                },
                new()
                {
                    Id = ToGuid(++permissionsGuidOffset),
                    Alias = "uint64_group_modify_alias_power",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.LessOrEqual
                },
                new()
                {
                    Id = ToGuid(++permissionsGuidOffset),
                    Alias = "uint64_group_modify_alias_power_needed",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.LessOrEqual
                },
                new()
                {
                    Id = ToGuid(++permissionsGuidOffset),
                    Alias = "uint64_group_modify_description_power",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.LessOrEqual
                },
                new()
                {
                    Id = ToGuid(++permissionsGuidOffset),
                    Alias = "uint64_group_modify_description_power_needed",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.LessOrEqual
                },

                #endregion

                #region User

                #region Email

                new()
                {
                    Id = ToGuid(++permissionsGuidOffset),
                    Alias = "boolean_user_view_email",
                    Type = PermissionType.Boolean
                },
                new()
                {
                    Id = ToGuid(++permissionsGuidOffset),
                    Alias = "boolean_user_view_email_own",
                    Type = PermissionType.Boolean
                },
                new()
                {
                    Id = ToGuid(++permissionsGuidOffset),
                    Alias = "uint64_user_modify_email_power",
                    Type = PermissionType.UInt64
                },
                new()
                {
                    Id = ToGuid(++permissionsGuidOffset),
                    Alias = "uint64_user_modify_email_power_needed",
                    Type = PermissionType.UInt64
                },
                new()
                {
                    Id = ToGuid(++permissionsGuidOffset),
                    Alias = "uint64_user_modify_email_own_power_needed",
                    Type = PermissionType.UInt64
                },

                #endregion

                new()
                {
                    Id = ToGuid(++permissionsGuidOffset),
                    Alias = "boolean_user_view_lastipaddress",
                    Type = PermissionType.Boolean,
                    CompareMode = PermissionCompareMode.Equal
                },

                #region Profile Username

                new()
                {
                    Id = ToGuid(++permissionsGuidOffset),
                    Alias = "boolean_user_view_profile_username",
                    Type = PermissionType.Boolean,
                    CompareMode = PermissionCompareMode.Equal
                },
                new()
                {
                    Id = ToGuid(++permissionsGuidOffset),
                    Alias = "boolean_user_view_profile_username_own",
                    Type = PermissionType.Boolean,
                    CompareMode = PermissionCompareMode.Equal
                },
                new()
                {
                    Id = ToGuid(++permissionsGuidOffset),
                    Alias = "uint64_user_modify_profile_username_power",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.LessOrEqual
                },
                new()
                {
                    Id = ToGuid(++permissionsGuidOffset),
                    Alias = "uint64_user_modify_profile_username_power_needed",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.LessOrEqual
                },
                new()
                {
                    Id = ToGuid(++permissionsGuidOffset),
                    Alias = "uint64_user_modify_profile_username_own_power_needed",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.LessOrEqual
                },

                #endregion

                #region Profile Basic

                new()
                {
                    Id = ToGuid(++permissionsGuidOffset),
                    Alias = "boolean_user_view_profile_basic",
                    Type = PermissionType.Boolean,
                    CompareMode = PermissionCompareMode.Equal
                },
                new()
                {
                    Id = ToGuid(++permissionsGuidOffset),
                    Alias = "boolean_user_view_profile_basic_own",
                    Type = PermissionType.Boolean,
                    CompareMode = PermissionCompareMode.Equal
                },
                new()
                {
                    Id = ToGuid(++permissionsGuidOffset),
                    Alias = "uint64_user_modify_profile_basic_power",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.LessOrEqual
                },
                new()
                {
                    Id = ToGuid(++permissionsGuidOffset),
                    Alias = "uint64_user_modify_profile_basic_power_needed",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.LessOrEqual
                },
                new()
                {
                    Id = ToGuid(++permissionsGuidOffset),
                    Alias = "uint64_user_modify_profile_basic_own_power_needed",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.LessOrEqual
                },

                #endregion

                #region Profile Custom

                new()
                {
                    Id = ToGuid(++permissionsGuidOffset),
                    Alias = "boolean_user_view_profile_custom",
                    Type = PermissionType.Boolean,
                    CompareMode = PermissionCompareMode.Equal
                },
                new()
                {
                    Id = ToGuid(++permissionsGuidOffset),
                    Alias = "boolean_user_view_profile_custom_own",
                    Type = PermissionType.Boolean,
                    CompareMode = PermissionCompareMode.Equal
                },
                new()
                {
                    Id = ToGuid(++permissionsGuidOffset),
                    Alias = "uint64_user_modify_profile_custom_power",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.LessOrEqual
                },
                new()
                {
                    Id = ToGuid(++permissionsGuidOffset),
                    Alias = "uint64_user_modify_profile_custom_power_needed",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.LessOrEqual
                },
                new()
                {
                    Id = ToGuid(++permissionsGuidOffset),
                    Alias = "uint64_user_modify_profile_custom_own_power_needed",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.LessOrEqual
                },

                #endregion

                #region Delete

                new()
                {
                    Id = ToGuid(++permissionsGuidOffset),
                    Alias = "uint64_user_delete_user_power",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.LessOrEqual
                },
                new()
                {
                    Id = ToGuid(++permissionsGuidOffset),
                    Alias = "uint64_user_delete_user_power_needed",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.LessOrEqual
                },
                new()
                {
                    Id = ToGuid(++permissionsGuidOffset),
                    Alias = "uint64_user_delete_user_own_power_needed",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.LessOrEqual
                },

                #endregion

                #region Communication Private

                new()
                {
                    Id = ToGuid(++permissionsGuidOffset),
                    Alias = "uint64_user_communication_private_power",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.LessOrEqual
                },
                new()
                {
                    Id = ToGuid(++permissionsGuidOffset),
                    Alias = "uint64_user_communication_private_power_needed",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.LessOrEqual
                },
                new()
                {
                    Id = ToGuid(++permissionsGuidOffset),
                    Alias = "uint64_user_communication_private_own_power_needed",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.LessOrEqual
                },

                #endregion

                #region Communication Global

                new()
                {
                    Id = ToGuid(++permissionsGuidOffset),
                    Alias = "uint64_user_communication_global_power",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.LessOrEqual
                },
                new()
                {
                    Id = ToGuid(++permissionsGuidOffset),
                    Alias = "uint64_user_communication_global_power_needed",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.LessOrEqual
                },

                #endregion

                #region Communication Poke

                new()
                {
                    Id = ToGuid(++permissionsGuidOffset),
                    Alias = "uint64_user_poke_power",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.LessOrEqual
                },
                new()
                {
                    Id = ToGuid(++permissionsGuidOffset),
                    Alias = "uint64_user_poke_power_needed",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.LessOrEqual
                },

                #endregion

                new()
                {
                    Id = ToGuid(++permissionsGuidOffset),
                    Alias = "uint64_user_max_apuint64_keys",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.LessOrEqual
                },
                new()
                {
                    Id = ToGuid(++permissionsGuidOffset),
                    Alias = "uint64_user_max_avatar_filesize",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.LessOrEqual
                },

                #region Complaints

                new()
                {
                    Id = ToGuid(++permissionsGuidOffset),
                    Alias = "uint64_user_create_complaints_power",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.LessOrEqual
                },
                new()
                {
                    Id = ToGuid(++permissionsGuidOffset),
                    Alias = "uint64_user_create_complaints_power_needed",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.LessOrEqual
                },
                new()
                {
                    Id = ToGuid(++permissionsGuidOffset),
                    Alias = "boolean_user_view_complaints",
                    Type = PermissionType.Boolean,
                    CompareMode = PermissionCompareMode.Equal
                },
                new()
                {
                    Id = ToGuid(++permissionsGuidOffset),
                    Alias = "boolean_user_view_complaints_own",
                    Type = PermissionType.Boolean,
                    CompareMode = PermissionCompareMode.Equal
                },
                new()
                {
                    Id = ToGuid(++permissionsGuidOffset),
                    Alias = "uint64_user_modify_complaints_power",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.LessOrEqual
                },
                new()
                {
                    Id = ToGuid(++permissionsGuidOffset),
                    Alias = "uint64_user_modify_complaints_power_needed",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.LessOrEqual
                },
                new()
                {
                    Id = ToGuid(++permissionsGuidOffset),
                    Alias = "uint64_user_modify_complaints_own_power_needed",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.LessOrEqual
                },
                new()
                {
                    Id = ToGuid(++permissionsGuidOffset),
                    Alias = "uint64_user_delete_complaints_power",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.LessOrEqual
                },
                new()
                {
                    Id = ToGuid(++permissionsGuidOffset),
                    Alias = "uint64_user_delete_complaints_power_needed",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.LessOrEqual
                },
                new()
                {
                    Id = ToGuid(++permissionsGuidOffset),
                    Alias = "uint64_user_delete_complaints_own_power_needed",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.LessOrEqual
                },

                #endregion

                #region Bans

                new()
                {
                    Id = ToGuid(++permissionsGuidOffset),
                    Alias = "uint64_user_create_bans_power",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.LessOrEqual
                },
                new()
                {
                    Id = ToGuid(++permissionsGuidOffset),
                    Alias = "uint64_user_create_bans_power_needed",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.LessOrEqual
                },
                new()
                {
                    Id = ToGuid(++permissionsGuidOffset),
                    Alias = "boolean_user_view_bans",
                    Type = PermissionType.Boolean,
                    CompareMode = PermissionCompareMode.Equal
                },
                new()
                {
                    Id = ToGuid(++permissionsGuidOffset),
                    Alias = "boolean_user_view_bans_own",
                    Type = PermissionType.Boolean,
                    CompareMode = PermissionCompareMode.Equal
                },
                new()
                {
                    Id = ToGuid(++permissionsGuidOffset),
                    Alias = "uint64_user_modify_bans_power",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.LessOrEqual
                },
                new()
                {
                    Id = ToGuid(++permissionsGuidOffset),
                    Alias = "uint64_user_modify_bans_power_needed",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.LessOrEqual
                },
                new()
                {
                    Id = ToGuid(++permissionsGuidOffset),
                    Alias = "uint64_user_modify_bans_own_power_needed",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.LessOrEqual
                },
                new()
                {
                    Id = ToGuid(++permissionsGuidOffset),
                    Alias = "uint64_user_delete_bans_power",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.LessOrEqual
                },
                new()
                {
                    Id = ToGuid(++permissionsGuidOffset),
                    Alias = "uint64_user_delete_bans_power_needed",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.LessOrEqual
                },
                new()
                {
                    Id = ToGuid(++permissionsGuidOffset),
                    Alias = "uint64_user_delete_bans_own_power_needed",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.LessOrEqual
                },
                new()
                {
                    Id = ToGuid(++permissionsGuidOffset),
                    Alias = "uint64_user_max_ban_time",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.LessOrEqual
                },

                #endregion

                #endregion
            };

            globalGuidOffset = permissionsGuidOffset;

            appDbContextSeedLists.Permissions.AddRange(permissions);

            #endregion

            #region Users

            var usersGuidOffset = globalGuidOffset;

            var users = new List<User>();

            var userGuest = new User
            {
                Id = ToGuid(++usersGuidOffset),
            };
            users.Add(userGuest);

            globalGuidOffset = usersGuidOffset;

            appDbContextSeedLists.Users.AddRange(users);

            #endregion

            #region UserProfiles

            var userProfilesGuidOffset = globalGuidOffset;

            var userProfiles = new List<UserProfile>();

            var userProfileGuest = new UserProfile
            {
                Id = ToGuid(++userProfilesGuidOffset),
                Username = "Guest",
                Description = "Initial system user",
                UserId = userGuest.Id,
            };
            userProfiles.Add(userProfileGuest);

            globalGuidOffset = userProfilesGuidOffset;

            appDbContextSeedLists.UserProfiles.AddRange(userProfiles);

            #endregion

            #region UserGroups

            var userGroupsGuidOffset = globalGuidOffset;

            var userGroups = new List<UserGroup>
            {
                new()
                {
                    Id = ToGuid(++userGroupsGuidOffset),
                    Alias = "Root",
                    Description = "System user group with root like permission set"
                },
                new()
                {
                    Id = ToGuid(++userGroupsGuidOffset),
                    Alias = "Guest",
                    Description = "System user group with guest like permission set"
                }
            };

            var rootUserGroup = userGroups.First(__ => __.Alias == "Root");
            var guestUserGroup = userGroups.First(__ => __.Alias == "Guest");

            globalGuidOffset = userGroupsGuidOffset;

            appDbContextSeedLists.UserGroups.AddRange(userGroups);

            #endregion

            #region UserGroupsPermissionValues

            var userGroupPermissionValuesGuidOffset = globalGuidOffset;

            var rootPower = ulong.MaxValue;

            var rootUserGroupPermissionValues = new List<UserGroupPermissionValue>()
            {
                #region Any

                new()
                {
                    Id = ToGuid(++userGroupPermissionValuesGuidOffset),
                    Value = BitConverter.GetBytes(true),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "boolean_any_view_permission_overview").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = ToGuid(++userGroupPermissionValuesGuidOffset),
                    Value = BitConverter.GetBytes(true),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "boolean_any_view_permission_overview_own").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = ToGuid(++userGroupPermissionValuesGuidOffset),
                    Value = BitConverter.GetBytes(rootPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_any_modify_permission_power").Id,
                    EntityId = rootUserGroup.Id,
                },

                #endregion

                #region Group

                new()
                {
                    Id = ToGuid(++userGroupPermissionValuesGuidOffset),
                    Value = BitConverter.GetBytes(true),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "boolean_group_create_groups").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = ToGuid(++userGroupPermissionValuesGuidOffset),
                    Value = BitConverter.GetBytes(true),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "boolean_group_view_groups").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = ToGuid(++userGroupPermissionValuesGuidOffset),
                    Value = BitConverter.GetBytes(true),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "boolean_group_view_groups_own").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = ToGuid(++userGroupPermissionValuesGuidOffset),
                    Value = BitConverter.GetBytes(rootPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_group_delete_groups_power").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = ToGuid(++userGroupPermissionValuesGuidOffset),
                    Value = BitConverter.GetBytes(rootPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_group_delete_groups_power_needed").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = ToGuid(++userGroupPermissionValuesGuidOffset),
                    Value = BitConverter.GetBytes(rootPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_group_add_member_power").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = ToGuid(++userGroupPermissionValuesGuidOffset),
                    Value = BitConverter.GetBytes(rootPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_group_add_member_power_needed").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = ToGuid(++userGroupPermissionValuesGuidOffset),
                    Value = BitConverter.GetBytes(rootPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_group_remove_member_power").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = ToGuid(++userGroupPermissionValuesGuidOffset),
                    Value = BitConverter.GetBytes(rootPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_group_remove_member_power_needed").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = ToGuid(++userGroupPermissionValuesGuidOffset),
                    Value = BitConverter.GetBytes(rootPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_group_modify_alias_power").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = ToGuid(++userGroupPermissionValuesGuidOffset),
                    Value = BitConverter.GetBytes(rootPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_group_modify_alias_power_needed").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = ToGuid(++userGroupPermissionValuesGuidOffset),
                    Value = BitConverter.GetBytes(rootPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_group_modify_description_power").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = ToGuid(++userGroupPermissionValuesGuidOffset),
                    Value = BitConverter.GetBytes(rootPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_group_modify_description_power_needed").Id,
                    EntityId = rootUserGroup.Id,
                },

                #endregion

                #region User

                #region Email

                new()
                {
                    Id = ToGuid(++userGroupPermissionValuesGuidOffset),
                    Value = BitConverter.GetBytes(true),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "boolean_user_view_email").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = ToGuid(++userGroupPermissionValuesGuidOffset),
                    Value = BitConverter.GetBytes(true),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "boolean_user_view_email_own").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = ToGuid(++userGroupPermissionValuesGuidOffset),
                    Value = BitConverter.GetBytes(rootPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_modify_email_power").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = ToGuid(++userGroupPermissionValuesGuidOffset),
                    Value = BitConverter.GetBytes(rootPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_modify_email_power_needed").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = ToGuid(++userGroupPermissionValuesGuidOffset),
                    Value = BitConverter.GetBytes(rootPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_modify_email_own_power_needed").Id,
                    EntityId = rootUserGroup.Id,
                },

                #endregion

                new()
                {
                    Id = ToGuid(++userGroupPermissionValuesGuidOffset),
                    Value = BitConverter.GetBytes(true),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "boolean_user_view_lastipaddress").Id,
                    EntityId = rootUserGroup.Id,
                },

                #region Profile Username

                new()
                {
                    Id = ToGuid(++userGroupPermissionValuesGuidOffset),
                    Value = BitConverter.GetBytes(true),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "boolean_user_view_profile_username").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = ToGuid(++userGroupPermissionValuesGuidOffset),
                    Value = BitConverter.GetBytes(true),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "boolean_user_view_profile_username_own").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = ToGuid(++userGroupPermissionValuesGuidOffset),
                    Value = BitConverter.GetBytes(rootPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_modify_profile_username_power").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = ToGuid(++userGroupPermissionValuesGuidOffset),
                    Value = BitConverter.GetBytes(rootPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_modify_profile_username_power_needed")
                        .Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = ToGuid(++userGroupPermissionValuesGuidOffset),
                    Value = BitConverter.GetBytes(rootPower),
                    Grant = rootPower,
                    PermissionId = permissions
                        .First(_ => _.Alias == "uint64_user_modify_profile_username_own_power_needed").Id,
                    EntityId = rootUserGroup.Id,
                },

                #endregion

                #region Profile Basic

                new()
                {
                    Id = ToGuid(++userGroupPermissionValuesGuidOffset),
                    Value = BitConverter.GetBytes(true),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "boolean_user_view_profile_basic").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = ToGuid(++userGroupPermissionValuesGuidOffset),
                    Value = BitConverter.GetBytes(true),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "boolean_user_view_profile_basic_own").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = ToGuid(++userGroupPermissionValuesGuidOffset),
                    Value = BitConverter.GetBytes(rootPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_modify_profile_basic_power").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = ToGuid(++userGroupPermissionValuesGuidOffset),
                    Value = BitConverter.GetBytes(rootPower),
                    Grant = rootPower,
                    PermissionId =
                        permissions.First(_ => _.Alias == "uint64_user_modify_profile_basic_power_needed").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = ToGuid(++userGroupPermissionValuesGuidOffset),
                    Value = BitConverter.GetBytes(rootPower),
                    Grant = rootPower,
                    PermissionId = permissions
                        .First(_ => _.Alias == "uint64_user_modify_profile_basic_own_power_needed").Id,
                    EntityId = rootUserGroup.Id,
                },

                #endregion

                #region Profile Custom

                new()
                {
                    Id = ToGuid(++userGroupPermissionValuesGuidOffset),
                    Value = BitConverter.GetBytes(true),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "boolean_user_view_profile_custom").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = ToGuid(++userGroupPermissionValuesGuidOffset),
                    Value = BitConverter.GetBytes(true),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "boolean_user_view_profile_custom_own").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = ToGuid(++userGroupPermissionValuesGuidOffset),
                    Value = BitConverter.GetBytes(rootPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_modify_profile_custom_power").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = ToGuid(++userGroupPermissionValuesGuidOffset),
                    Value = BitConverter.GetBytes(rootPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_modify_profile_custom_power_needed")
                        .Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = ToGuid(++userGroupPermissionValuesGuidOffset),
                    Value = BitConverter.GetBytes(rootPower),
                    Grant = rootPower,
                    PermissionId = permissions
                        .First(_ => _.Alias == "uint64_user_modify_profile_custom_own_power_needed").Id,
                    EntityId = rootUserGroup.Id,
                },

                #endregion

                #region Delete

                new()
                {
                    Id = ToGuid(++userGroupPermissionValuesGuidOffset),
                    Value = BitConverter.GetBytes(rootPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_delete_user_power").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = ToGuid(++userGroupPermissionValuesGuidOffset),
                    Value = BitConverter.GetBytes(rootPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_delete_user_power_needed").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = ToGuid(++userGroupPermissionValuesGuidOffset),
                    Value = BitConverter.GetBytes(rootPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_delete_user_own_power_needed").Id,
                    EntityId = rootUserGroup.Id,
                },

                #endregion

                #region Communication Private

                new()
                {
                    Id = ToGuid(++userGroupPermissionValuesGuidOffset),
                    Value = BitConverter.GetBytes(rootPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_communication_private_power").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = ToGuid(++userGroupPermissionValuesGuidOffset),
                    Value = BitConverter.GetBytes(rootPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_communication_private_power_needed")
                        .Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = ToGuid(++userGroupPermissionValuesGuidOffset),
                    Value = BitConverter.GetBytes(rootPower),
                    Grant = rootPower,
                    PermissionId = permissions
                        .First(_ => _.Alias == "uint64_user_communication_private_own_power_needed").Id,
                    EntityId = rootUserGroup.Id,
                },

                #endregion

                #region Communication Global

                new()
                {
                    Id = ToGuid(++userGroupPermissionValuesGuidOffset),
                    Value = BitConverter.GetBytes(rootPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_communication_global_power").Id,
                    EntityId = rootUserGroup.Id,
                },

                #endregion

                #region Communication Poke

                new()
                {
                    Id = ToGuid(++userGroupPermissionValuesGuidOffset),
                    Value = BitConverter.GetBytes(rootPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_poke_power").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = ToGuid(++userGroupPermissionValuesGuidOffset),
                    Value = BitConverter.GetBytes(rootPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_poke_power_needed").Id,
                    EntityId = rootUserGroup.Id,
                },

                #endregion

                new()
                {
                    Id = ToGuid(++userGroupPermissionValuesGuidOffset),
                    Value = BitConverter.GetBytes(rootPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_max_apuint64_keys").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = ToGuid(++userGroupPermissionValuesGuidOffset),
                    Value = BitConverter.GetBytes(rootPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_max_avatar_filesize").Id,
                    EntityId = rootUserGroup.Id,
                },

                #region Complaints

                new()
                {
                    Id = ToGuid(++userGroupPermissionValuesGuidOffset),
                    Value = BitConverter.GetBytes(rootPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_create_complaints_power").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = ToGuid(++userGroupPermissionValuesGuidOffset),
                    Value = BitConverter.GetBytes(rootPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_create_complaints_power_needed").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = ToGuid(++userGroupPermissionValuesGuidOffset),
                    Value = BitConverter.GetBytes(true),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "boolean_user_view_complaints").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = ToGuid(++userGroupPermissionValuesGuidOffset),
                    Value = BitConverter.GetBytes(true),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "boolean_user_view_complaints_own").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = ToGuid(++userGroupPermissionValuesGuidOffset),
                    Value = BitConverter.GetBytes(rootPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_modify_complaints_power").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = ToGuid(++userGroupPermissionValuesGuidOffset),
                    Value = BitConverter.GetBytes(rootPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_modify_complaints_power_needed").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = ToGuid(++userGroupPermissionValuesGuidOffset),
                    Value = BitConverter.GetBytes(rootPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_modify_complaints_own_power_needed")
                        .Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = ToGuid(++userGroupPermissionValuesGuidOffset),
                    Value = BitConverter.GetBytes(rootPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_delete_complaints_power").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = ToGuid(++userGroupPermissionValuesGuidOffset),
                    Value = BitConverter.GetBytes(rootPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_delete_complaints_power_needed").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = ToGuid(++userGroupPermissionValuesGuidOffset),
                    Value = BitConverter.GetBytes(rootPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_delete_complaints_own_power_needed")
                        .Id,
                    EntityId = rootUserGroup.Id,
                },

                #endregion

                #region Bans

                new()
                {
                    Id = ToGuid(++userGroupPermissionValuesGuidOffset),
                    Value = BitConverter.GetBytes(rootPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_create_bans_power").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = ToGuid(++userGroupPermissionValuesGuidOffset),
                    Value = BitConverter.GetBytes(rootPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_create_bans_power_needed").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = ToGuid(++userGroupPermissionValuesGuidOffset),
                    Value = BitConverter.GetBytes(true),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "boolean_user_view_bans").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = ToGuid(++userGroupPermissionValuesGuidOffset),
                    Value = BitConverter.GetBytes(true),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "boolean_user_view_bans_own").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = ToGuid(++userGroupPermissionValuesGuidOffset),
                    Value = BitConverter.GetBytes(rootPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_modify_bans_power").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = ToGuid(++userGroupPermissionValuesGuidOffset),
                    Value = BitConverter.GetBytes(rootPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_modify_bans_power_needed").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = ToGuid(++userGroupPermissionValuesGuidOffset),
                    Value = BitConverter.GetBytes(rootPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_modify_bans_own_power_needed").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = ToGuid(++userGroupPermissionValuesGuidOffset),
                    Value = BitConverter.GetBytes(rootPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_delete_bans_power").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = ToGuid(++userGroupPermissionValuesGuidOffset),
                    Value = BitConverter.GetBytes(rootPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_delete_bans_power_needed").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = ToGuid(++userGroupPermissionValuesGuidOffset),
                    Value = BitConverter.GetBytes(rootPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_delete_bans_own_power_needed").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = ToGuid(++userGroupPermissionValuesGuidOffset),
                    Value = BitConverter.GetBytes(rootPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_max_ban_time").Id,
                    EntityId = rootUserGroup.Id,
                },

                #endregion

                #endregion
            };

            var guestPower = 50ul;

            var guestUserGroupPermissionValues = new List<UserGroupPermissionValue>()
            {
                #region Any

                new()
                {
                    Id = ToGuid(++userGroupPermissionValuesGuidOffset),
                    Value = BitConverter.GetBytes(true),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "boolean_any_view_permission_overview_own").Id,
                    EntityId = guestUserGroup.Id,
                },

                #endregion

                #region Group

                new()
                {
                    Id = ToGuid(++userGroupPermissionValuesGuidOffset),
                    Value = BitConverter.GetBytes(true),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "boolean_group_view_groups").Id,
                    EntityId = guestUserGroup.Id,
                },
                new()
                {
                    Id = ToGuid(++userGroupPermissionValuesGuidOffset),
                    Value = BitConverter.GetBytes(true),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "boolean_group_view_groups_own").Id,
                    EntityId = guestUserGroup.Id,
                },
                new()
                {
                    Id = ToGuid(++userGroupPermissionValuesGuidOffset),
                    Value = BitConverter.GetBytes(rootPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_group_delete_groups_power_needed").Id,
                    EntityId = guestUserGroup.Id,
                },
                new()
                {
                    Id = ToGuid(++userGroupPermissionValuesGuidOffset),
                    Value = BitConverter.GetBytes(rootPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_group_add_member_power_needed").Id,
                    EntityId = guestUserGroup.Id,
                },
                new()
                {
                    Id = ToGuid(++userGroupPermissionValuesGuidOffset),
                    Value = BitConverter.GetBytes(rootPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_group_remove_member_power_needed").Id,
                    EntityId = guestUserGroup.Id,
                },
                new()
                {
                    Id = ToGuid(++userGroupPermissionValuesGuidOffset),
                    Value = BitConverter.GetBytes(rootPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_group_modify_alias_power_needed").Id,
                    EntityId = guestUserGroup.Id,
                },
                new()
                {
                    Id = ToGuid(++userGroupPermissionValuesGuidOffset),
                    Value = BitConverter.GetBytes(rootPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_group_modify_description_power_needed").Id,
                    EntityId = guestUserGroup.Id,
                },

                #endregion

                #region User

                #region Email

                new()
                {
                    Id = ToGuid(++userGroupPermissionValuesGuidOffset),
                    Value = BitConverter.GetBytes(false),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "boolean_user_view_email").Id,
                    EntityId = guestUserGroup.Id,
                },
                new()
                {
                    Id = ToGuid(++userGroupPermissionValuesGuidOffset),
                    Value = BitConverter.GetBytes(true),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "boolean_user_view_email_own").Id,
                    EntityId = guestUserGroup.Id,
                },
                new()
                {
                    Id = ToGuid(++userGroupPermissionValuesGuidOffset),
                    Value = BitConverter.GetBytes(guestPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_modify_email_power").Id,
                    EntityId = guestUserGroup.Id,
                },
                new()
                {
                    Id = ToGuid(++userGroupPermissionValuesGuidOffset),
                    Value = BitConverter.GetBytes(rootPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_modify_email_power_needed").Id,
                    EntityId = guestUserGroup.Id,
                },
                new()
                {
                    Id = ToGuid(++userGroupPermissionValuesGuidOffset),
                    Value = BitConverter.GetBytes(guestPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_modify_email_own_power_needed").Id,
                    EntityId = guestUserGroup.Id,
                },

                #endregion

                new()
                {
                    Id = ToGuid(++userGroupPermissionValuesGuidOffset),
                    Value = BitConverter.GetBytes(false),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "boolean_user_view_lastipaddress").Id,
                    EntityId = guestUserGroup.Id,
                },

                #region Profile Username

                new()
                {
                    Id = ToGuid(++userGroupPermissionValuesGuidOffset),
                    Value = BitConverter.GetBytes(true),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "boolean_user_view_profile_username").Id,
                    EntityId = guestUserGroup.Id,
                },
                new()
                {
                    Id = ToGuid(++userGroupPermissionValuesGuidOffset),
                    Value = BitConverter.GetBytes(true),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "boolean_user_view_profile_username_own").Id,
                    EntityId = guestUserGroup.Id,
                },
                new()
                {
                    Id = ToGuid(++userGroupPermissionValuesGuidOffset),
                    Value = BitConverter.GetBytes(guestPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_modify_profile_username_power").Id,
                    EntityId = guestUserGroup.Id,
                },
                new()
                {
                    Id = ToGuid(++userGroupPermissionValuesGuidOffset),
                    Value = BitConverter.GetBytes(rootPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_modify_profile_username_power_needed")
                        .Id,
                    EntityId = guestUserGroup.Id,
                },
                new()
                {
                    Id = ToGuid(++userGroupPermissionValuesGuidOffset),
                    Value = BitConverter.GetBytes(guestPower),
                    Grant = rootPower,
                    PermissionId = permissions
                        .First(_ => _.Alias == "uint64_user_modify_profile_username_own_power_needed").Id,
                    EntityId = guestUserGroup.Id,
                },

                #endregion

                #region Profile Basic

                new()
                {
                    Id = ToGuid(++userGroupPermissionValuesGuidOffset),
                    Value = BitConverter.GetBytes(true),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "boolean_user_view_profile_basic").Id,
                    EntityId = guestUserGroup.Id,
                },
                new()
                {
                    Id = ToGuid(++userGroupPermissionValuesGuidOffset),
                    Value = BitConverter.GetBytes(true),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "boolean_user_view_profile_basic_own").Id,
                    EntityId = guestUserGroup.Id,
                },
                new()
                {
                    Id = ToGuid(++userGroupPermissionValuesGuidOffset),
                    Value = BitConverter.GetBytes(guestPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_modify_profile_basic_power").Id,
                    EntityId = guestUserGroup.Id,
                },
                new()
                {
                    Id = ToGuid(++userGroupPermissionValuesGuidOffset),
                    Value = BitConverter.GetBytes(rootPower),
                    Grant = rootPower,
                    PermissionId =
                        permissions.First(_ => _.Alias == "uint64_user_modify_profile_basic_power_needed").Id,
                    EntityId = guestUserGroup.Id,
                },
                new()
                {
                    Id = ToGuid(++userGroupPermissionValuesGuidOffset),
                    Value = BitConverter.GetBytes(guestPower),
                    Grant = rootPower,
                    PermissionId = permissions
                        .First(_ => _.Alias == "uint64_user_modify_profile_basic_own_power_needed").Id,
                    EntityId = guestUserGroup.Id,
                },

                #endregion

                #region Profile Custom

                new()
                {
                    Id = ToGuid(++userGroupPermissionValuesGuidOffset),
                    Value = BitConverter.GetBytes(true),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "boolean_user_view_profile_custom").Id,
                    EntityId = guestUserGroup.Id,
                },
                new()
                {
                    Id = ToGuid(++userGroupPermissionValuesGuidOffset),
                    Value = BitConverter.GetBytes(true),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "boolean_user_view_profile_custom_own").Id,
                    EntityId = guestUserGroup.Id,
                },
                new()
                {
                    Id = ToGuid(++userGroupPermissionValuesGuidOffset),
                    Value = BitConverter.GetBytes(guestPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_modify_profile_custom_power").Id,
                    EntityId = guestUserGroup.Id,
                },
                new()
                {
                    Id = ToGuid(++userGroupPermissionValuesGuidOffset),
                    Value = BitConverter.GetBytes(rootPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_modify_profile_custom_power_needed")
                        .Id,
                    EntityId = guestUserGroup.Id,
                },
                new()
                {
                    Id = ToGuid(++userGroupPermissionValuesGuidOffset),
                    Value = BitConverter.GetBytes(guestPower),
                    Grant = rootPower,
                    PermissionId = permissions
                        .First(_ => _.Alias == "uint64_user_modify_profile_custom_own_power_needed").Id,
                    EntityId = guestUserGroup.Id,
                },

                #endregion

                #region Delete

                new()
                {
                    Id = ToGuid(++userGroupPermissionValuesGuidOffset),
                    Value = BitConverter.GetBytes(guestPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_delete_user_power").Id,
                    EntityId = guestUserGroup.Id,
                },
                new()
                {
                    Id = ToGuid(++userGroupPermissionValuesGuidOffset),
                    Value = BitConverter.GetBytes(rootPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_delete_user_power_needed").Id,
                    EntityId = guestUserGroup.Id,
                },
                new()
                {
                    Id = ToGuid(++userGroupPermissionValuesGuidOffset),
                    Value = BitConverter.GetBytes(guestPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_delete_user_own_power_needed").Id,
                    EntityId = guestUserGroup.Id,
                },

                #endregion

                #region Communication Private

                new()
                {
                    Id = ToGuid(++userGroupPermissionValuesGuidOffset),
                    Value = BitConverter.GetBytes(guestPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_communication_private_power").Id,
                    EntityId = guestUserGroup.Id,
                },
                new()
                {
                    Id = ToGuid(++userGroupPermissionValuesGuidOffset),
                    Value = BitConverter.GetBytes(guestPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_communication_private_power_needed")
                        .Id,
                    EntityId = guestUserGroup.Id,
                },
                new()
                {
                    Id = ToGuid(++userGroupPermissionValuesGuidOffset),
                    Value = BitConverter.GetBytes(guestPower),
                    Grant = rootPower,
                    PermissionId = permissions
                        .First(_ => _.Alias == "uint64_user_communication_private_own_power_needed").Id,
                    EntityId = guestUserGroup.Id,
                },

                #endregion

                #region Communication Global

                new()
                {
                    Id = ToGuid(++userGroupPermissionValuesGuidOffset),
                    Value = BitConverter.GetBytes(guestPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_communication_global_power").Id,
                    EntityId = guestUserGroup.Id,
                },

                #endregion

                #region Communication Poke

                new()
                {
                    Id = ToGuid(++userGroupPermissionValuesGuidOffset),
                    Value = BitConverter.GetBytes(guestPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_poke_power").Id,
                    EntityId = guestUserGroup.Id,
                },
                new()
                {
                    Id = ToGuid(++userGroupPermissionValuesGuidOffset),
                    Value = BitConverter.GetBytes(guestPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_poke_power_needed").Id,
                    EntityId = guestUserGroup.Id,
                },

                #endregion

                new()
                {
                    Id = ToGuid(++userGroupPermissionValuesGuidOffset),
                    Value = BitConverter.GetBytes(0),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_max_apuint64_keys").Id,
                    EntityId = guestUserGroup.Id,
                },
                new()
                {
                    Id = ToGuid(++userGroupPermissionValuesGuidOffset),
                    Value = BitConverter.GetBytes(0),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_max_avatar_filesize").Id,
                    EntityId = guestUserGroup.Id,
                },

                #region Complaints

                new()
                {
                    Id = ToGuid(++userGroupPermissionValuesGuidOffset),
                    Value = BitConverter.GetBytes(guestPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_create_complaints_power").Id,
                    EntityId = guestUserGroup.Id,
                },
                new()
                {
                    Id = ToGuid(++userGroupPermissionValuesGuidOffset),
                    Value = BitConverter.GetBytes(guestPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_create_complaints_power_needed").Id,
                    EntityId = guestUserGroup.Id,
                },
                new()
                {
                    Id = ToGuid(++userGroupPermissionValuesGuidOffset),
                    Value = BitConverter.GetBytes(false),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "boolean_user_view_complaints").Id,
                    EntityId = guestUserGroup.Id,
                },
                new()
                {
                    Id = ToGuid(++userGroupPermissionValuesGuidOffset),
                    Value = BitConverter.GetBytes(true),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "boolean_user_view_complaints_own").Id,
                    EntityId = guestUserGroup.Id,
                },
                new()
                {
                    Id = ToGuid(++userGroupPermissionValuesGuidOffset),
                    Value = BitConverter.GetBytes(guestPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_modify_complaints_power").Id,
                    EntityId = guestUserGroup.Id,
                },
                new()
                {
                    Id = ToGuid(++userGroupPermissionValuesGuidOffset),
                    Value = BitConverter.GetBytes(rootPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_modify_complaints_power_needed").Id,
                    EntityId = guestUserGroup.Id,
                },
                new()
                {
                    Id = ToGuid(++userGroupPermissionValuesGuidOffset),
                    Value = BitConverter.GetBytes(guestPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_modify_complaints_own_power_needed")
                        .Id,
                    EntityId = guestUserGroup.Id,
                },
                new()
                {
                    Id = ToGuid(++userGroupPermissionValuesGuidOffset),
                    Value = BitConverter.GetBytes(guestPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_delete_complaints_power").Id,
                    EntityId = guestUserGroup.Id,
                },
                new()
                {
                    Id = ToGuid(++userGroupPermissionValuesGuidOffset),
                    Value = BitConverter.GetBytes(rootPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_delete_complaints_power_needed").Id,
                    EntityId = guestUserGroup.Id,
                },
                new()
                {
                    Id = ToGuid(++userGroupPermissionValuesGuidOffset),
                    Value = BitConverter.GetBytes(guestPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_delete_complaints_own_power_needed")
                        .Id,
                    EntityId = guestUserGroup.Id,
                },

                #endregion

                #region Bans

                new()
                {
                    Id = ToGuid(++userGroupPermissionValuesGuidOffset),
                    Value = BitConverter.GetBytes(rootPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_create_bans_power_needed").Id,
                    EntityId = guestUserGroup.Id,
                },
                new()
                {
                    Id = ToGuid(++userGroupPermissionValuesGuidOffset),
                    Value = BitConverter.GetBytes(rootPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_modify_bans_power_needed").Id,
                    EntityId = guestUserGroup.Id,
                },
                new()
                {
                    Id = ToGuid(++userGroupPermissionValuesGuidOffset),
                    Value = BitConverter.GetBytes(rootPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_delete_bans_power_needed").Id,
                    EntityId = guestUserGroup.Id,
                },

                #endregion

                #endregion
            };

            globalGuidOffset = userGroupPermissionValuesGuidOffset;

            appDbContextSeedLists.UserGroupPermissionValues.AddRange(rootUserGroupPermissionValues);
            appDbContextSeedLists.UserGroupPermissionValues.AddRange(guestUserGroupPermissionValues);

            #endregion

            #region UserToGroupMappings

            var userToGroupMappingsGuidOffset = globalGuidOffset;

            var userGuestToGuestUserGroupMapping = new UserToGroupMapping
            {
                Id = ToGuid(++userGroupsGuidOffset),
                EntityId = userGuest.Id,
                GroupId = guestUserGroup.Id,
            };

            globalGuidOffset = userToGroupMappingsGuidOffset;

            appDbContextSeedLists.UserToGroupMappings.Add(userGuestToGuestUserGroupMapping);

            #endregion

            return appDbContextSeedLists;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            var appDbContextSeedLists = Seed();

            #region Permissions

            builder.Entity<Permission>(_ =>
            {
                _.HasIndex(__ => __.Alias).IsUnique();

                _.HasData(appDbContextSeedLists.Permissions);
            });

            // builder.Entity<Permission>().ToTable(nameof(Permission));

            #endregion

            #region Users

            builder.Entity<User>(_ =>
            {
                _.HasIndex(__ => __.Email).IsUnique();
                
                //Allow PhoneNumber to remain empty
                // _.HasIndex(__ => __.PhoneNumber).IsUnique().HasFilter(null);

                _.HasData(appDbContextSeedLists.Users);
            });

            // builder.Entity<User>().ToTable(nameof(User));

            #endregion

            #region UserProfiles

            builder.Entity<UserProfile>(_ =>
            {
                _.HasIndex(__ => __.Username).IsUnique();

                _.HasData(appDbContextSeedLists.UserProfiles);
            });

            // builder.Entity<UserProfile>().ToTable(nameof(UserProfile));

            #endregion

            #region UserGroups

            builder.Entity<UserGroup>(_ =>
            {
                _.HasIndex(__ => __.Alias).IsUnique();

                _.HasData(appDbContextSeedLists.UserGroups);
            });

            // builder.Entity<UserGroup>().ToTable(nameof(UserGroup));

            #endregion

            #region UserGroupPermissionValues

            builder.Entity<UserGroupPermissionValue>(_ =>
            {
                _.HasIndex(__ => new {__.PermissionId, __.EntityId}).IsUnique();

                _.HasData(appDbContextSeedLists.UserGroupPermissionValues);
            });

            // builder.Entity<UserGroupPermissionValue>().ToTable(nameof(UserGroupPermissionValue));

            #endregion

            #region UserToGroupMappings

            builder.Entity<UserToGroupMapping>(_ =>
            {
                _.HasIndex(__ => new {__.EntityId, __.GroupId}).IsUnique();

                _.HasData(appDbContextSeedLists.UserToGroupMappings);
            });

            // builder.Entity<UserToGroupMapping>().ToTable(nameof(UserToGroupMapping));

            #endregion

            #region JsonWebTokens

            builder.Entity<JsonWebToken>(_ => { _.HasIndex(__ => __.Token).IsUnique(); });

            #endregion

            #region RefreshTokens

            builder.Entity<RefreshToken>(_ => { _.HasIndex(__ => __.Token).IsUnique(); });

            #endregion
        }

        #region Helpers

        private static Guid ToGuid(int value)
        {
            var bytes = new byte[16];
            BitConverter.GetBytes(value).CopyTo(bytes, 0);
            return new Guid(bytes);
        }

        private static byte[] GenerateRandomBytes(int byteCount)
        {
            var csp = RandomNumberGenerator.Create();

            var buffer = new byte[byteCount];
            csp.GetBytes(buffer);
            return buffer;
        }

        #endregion
    }
}