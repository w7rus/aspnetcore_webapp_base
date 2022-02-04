using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Entities.Base;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace DAL.Data
{
    public sealed class AppDbContext : DbContext
    {
        #region Fields

        private static int _guidSeedOffset;

        #endregion

        #region DbSets

        public DbSet<Permission> Permissions { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserGroup> UserGroups { get; set; }
        public DbSet<UserGroupPermissionValue> UserGroupPermissionValues { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<UserToGroupMapping> UserToGroupMappings { get; set; }
        public DbSet<JsonWebToken> JsonWebTokens { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<File> Files { get; set; }

        #endregion

        #region Ctor

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            Database.Migrate();
        }

        #endregion

        private void UpdateTimestamps<TKey>() where TKey : IEquatable<TKey>
        {
            var entityEntries = ChangeTracker.Entries();

            foreach (var entityEntry in entityEntries.Where(_ => _.State is EntityState.Modified or EntityState.Added))
            {
                var dateTimeOffsetUtcNow = DateTimeOffset.UtcNow;

                if (entityEntry.State == EntityState.Added)
                    ((EntityBase<TKey>) entityEntry.Entity).CreatedAt = dateTimeOffsetUtcNow;
                ((EntityBase<TKey>) entityEntry.Entity).UpdatedAt = dateTimeOffsetUtcNow;
            }
        }

        #region Overrides

        public override void Dispose()
        {
            base.Dispose();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        public override int SaveChanges()
        {
            UpdateTimestamps<Guid>();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
        {
            UpdateTimestamps<Guid>();
            return base.SaveChangesAsync(cancellationToken);
        }

        #endregion

        private sealed class AppDbContextSeedLists
        {
            public List<Permission> Permissions { get; set; }
            public List<User> Users { get; set; }
            public List<UserGroup> UserGroups { get; set; }
            public List<UserGroupPermissionValue> UserGroupPermissionValues { get; set; }
            public List<UserProfile> UserProfiles { get; set; }
            public List<UserToGroupMapping> UserToGroupMappings { get; set; }
            public List<JsonWebToken> JsonWebTokens { get; set; }
            public List<RefreshToken> RefreshTokens { get; set; }
        }

        private static AppDbContextSeedLists Seed()
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

            #region Permissions

            var permissions = new List<Permission>
            {
                #region Any

                new()
                {
                    Id = NextGuid(),
                    Alias = "boolean_any_view_permission_overview",
                    Type = PermissionType.Boolean,
                    CompareMode = PermissionCompareMode.Equal
                },
                new()
                {
                    Id = NextGuid(),
                    Alias = "boolean_any_view_permission_overview_own",
                    Type = PermissionType.Boolean,
                    CompareMode = PermissionCompareMode.Equal
                },
                new()
                {
                    Id = NextGuid(),
                    Alias = "uint64_any_modify_permission_power",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },

                #endregion

                #region Group

                new()
                {
                    Id = NextGuid(),
                    Alias = "boolean_group_create_groups",
                    Type = PermissionType.Boolean,
                    CompareMode = PermissionCompareMode.Equal
                },
                new()
                {
                    Id = NextGuid(),
                    Alias = "boolean_group_view_groups",
                    Type = PermissionType.Boolean,
                    CompareMode = PermissionCompareMode.Equal
                },
                new()
                {
                    Id = NextGuid(),
                    Alias = "boolean_group_view_groups_own",
                    Type = PermissionType.Boolean,
                    CompareMode = PermissionCompareMode.Equal
                },
                new()
                {
                    Id = NextGuid(),
                    Alias = "uint64_group_delete_groups_power",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },
                new()
                {
                    Id = NextGuid(),
                    Alias = "uint64_group_delete_groups_power_needed",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },
                new()
                {
                    Id = NextGuid(),
                    Alias = "uint64_group_add_member_power",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },
                new()
                {
                    Id = NextGuid(),
                    Alias = "uint64_group_add_member_power_needed",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },
                new()
                {
                    Id = NextGuid(),
                    Alias = "uint64_group_remove_member_power",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },
                new()
                {
                    Id = NextGuid(),
                    Alias = "uint64_group_remove_member_power_needed",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },
                new()
                {
                    Id = NextGuid(),
                    Alias = "uint64_group_modify_alias_power",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },
                new()
                {
                    Id = NextGuid(),
                    Alias = "uint64_group_modify_alias_power_needed",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },
                new()
                {
                    Id = NextGuid(),
                    Alias = "uint64_group_modify_description_power",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },
                new()
                {
                    Id = NextGuid(),
                    Alias = "uint64_group_modify_description_power_needed",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },

                #endregion

                #region User

                #region Email

                new()
                {
                    Id = NextGuid(),
                    Alias = "boolean_user_view_email",
                    Type = PermissionType.Boolean
                },
                new()
                {
                    Id = NextGuid(),
                    Alias = "boolean_user_view_email_own",
                    Type = PermissionType.Boolean
                },
                new()
                {
                    Id = NextGuid(),
                    Alias = "uint64_user_modify_email_power",
                    Type = PermissionType.UInt64
                },
                new()
                {
                    Id = NextGuid(),
                    Alias = "uint64_user_modify_email_power_needed",
                    Type = PermissionType.UInt64
                },
                new()
                {
                    Id = NextGuid(),
                    Alias = "uint64_user_modify_email_own_power_needed",
                    Type = PermissionType.UInt64
                },

                #endregion

                new()
                {
                    Id = NextGuid(),
                    Alias = "boolean_user_view_lastipaddress",
                    Type = PermissionType.Boolean,
                    CompareMode = PermissionCompareMode.Equal
                },

                #region Profile Username

                new()
                {
                    Id = NextGuid(),
                    Alias = "boolean_user_view_profile_username",
                    Type = PermissionType.Boolean,
                    CompareMode = PermissionCompareMode.Equal
                },
                new()
                {
                    Id = NextGuid(),
                    Alias = "boolean_user_view_profile_username_own",
                    Type = PermissionType.Boolean,
                    CompareMode = PermissionCompareMode.Equal
                },
                new()
                {
                    Id = NextGuid(),
                    Alias = "uint64_user_modify_profile_username_power",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },
                new()
                {
                    Id = NextGuid(),
                    Alias = "uint64_user_modify_profile_username_power_needed",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },
                new()
                {
                    Id = NextGuid(),
                    Alias = "uint64_user_modify_profile_username_own_power_needed",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },

                #endregion

                #region Profile Basic

                new()
                {
                    Id = NextGuid(),
                    Alias = "boolean_user_view_profile_basic",
                    Type = PermissionType.Boolean,
                    CompareMode = PermissionCompareMode.Equal
                },
                new()
                {
                    Id = NextGuid(),
                    Alias = "boolean_user_view_profile_basic_own",
                    Type = PermissionType.Boolean,
                    CompareMode = PermissionCompareMode.Equal
                },
                new()
                {
                    Id = NextGuid(),
                    Alias = "uint64_user_modify_profile_basic_power",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },
                new()
                {
                    Id = NextGuid(),
                    Alias = "uint64_user_modify_profile_basic_power_needed",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },
                new()
                {
                    Id = NextGuid(),
                    Alias = "uint64_user_modify_profile_basic_own_power_needed",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },

                #endregion

                #region Profile Custom

                new()
                {
                    Id = NextGuid(),
                    Alias = "boolean_user_view_profile_custom",
                    Type = PermissionType.Boolean,
                    CompareMode = PermissionCompareMode.Equal
                },
                new()
                {
                    Id = NextGuid(),
                    Alias = "boolean_user_view_profile_custom_own",
                    Type = PermissionType.Boolean,
                    CompareMode = PermissionCompareMode.Equal
                },
                new()
                {
                    Id = NextGuid(),
                    Alias = "uint64_user_modify_profile_custom_power",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },
                new()
                {
                    Id = NextGuid(),
                    Alias = "uint64_user_modify_profile_custom_power_needed",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },
                new()
                {
                    Id = NextGuid(),
                    Alias = "uint64_user_modify_profile_custom_own_power_needed",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },

                #endregion

                #region Delete

                new()
                {
                    Id = NextGuid(),
                    Alias = "uint64_user_delete_user_power",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },
                new()
                {
                    Id = NextGuid(),
                    Alias = "uint64_user_delete_user_power_needed",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },
                new()
                {
                    Id = NextGuid(),
                    Alias = "uint64_user_delete_user_own_power_needed",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },

                #endregion

                #region Communication Private

                new()
                {
                    Id = NextGuid(),
                    Alias = "uint64_user_communication_private_power",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },
                new()
                {
                    Id = NextGuid(),
                    Alias = "uint64_user_communication_private_power_needed",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },
                new()
                {
                    Id = NextGuid(),
                    Alias = "uint64_user_communication_private_own_power_needed",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },

                #endregion

                #region Communication Global

                new()
                {
                    Id = NextGuid(),
                    Alias = "uint64_user_communication_global_power",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },
                new()
                {
                    Id = NextGuid(),
                    Alias = "uint64_user_communication_global_power_needed",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },

                #endregion

                #region Communication Poke

                new()
                {
                    Id = NextGuid(),
                    Alias = "uint64_user_poke_power",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },
                new()
                {
                    Id = NextGuid(),
                    Alias = "uint64_user_poke_power_needed",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },

                #endregion

                new()
                {
                    Id = NextGuid(),
                    Alias = "uint64_user_max_apuint64_keys",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },
                new()
                {
                    Id = NextGuid(),
                    Alias = "uint64_user_max_avatar_filesize",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },

                #region Complaints

                new()
                {
                    Id = NextGuid(),
                    Alias = "uint64_user_create_complaints_power",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },
                new()
                {
                    Id = NextGuid(),
                    Alias = "uint64_user_create_complaints_power_needed",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },
                new()
                {
                    Id = NextGuid(),
                    Alias = "boolean_user_view_complaints",
                    Type = PermissionType.Boolean,
                    CompareMode = PermissionCompareMode.Equal
                },
                new()
                {
                    Id = NextGuid(),
                    Alias = "boolean_user_view_complaints_own",
                    Type = PermissionType.Boolean,
                    CompareMode = PermissionCompareMode.Equal
                },
                new()
                {
                    Id = NextGuid(),
                    Alias = "uint64_user_modify_complaints_power",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },
                new()
                {
                    Id = NextGuid(),
                    Alias = "uint64_user_modify_complaints_power_needed",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },
                new()
                {
                    Id = NextGuid(),
                    Alias = "uint64_user_modify_complaints_own_power_needed",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },
                new()
                {
                    Id = NextGuid(),
                    Alias = "uint64_user_delete_complaints_power",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },
                new()
                {
                    Id = NextGuid(),
                    Alias = "uint64_user_delete_complaints_power_needed",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },
                new()
                {
                    Id = NextGuid(),
                    Alias = "uint64_user_delete_complaints_own_power_needed",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },

                #endregion

                #region Bans

                new()
                {
                    Id = NextGuid(),
                    Alias = "uint64_user_create_bans_power",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },
                new()
                {
                    Id = NextGuid(),
                    Alias = "uint64_user_create_bans_power_needed",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },
                new()
                {
                    Id = NextGuid(),
                    Alias = "boolean_user_view_bans",
                    Type = PermissionType.Boolean,
                    CompareMode = PermissionCompareMode.Equal
                },
                new()
                {
                    Id = NextGuid(),
                    Alias = "boolean_user_view_bans_own",
                    Type = PermissionType.Boolean,
                    CompareMode = PermissionCompareMode.Equal
                },
                new()
                {
                    Id = NextGuid(),
                    Alias = "uint64_user_modify_bans_power",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },
                new()
                {
                    Id = NextGuid(),
                    Alias = "uint64_user_modify_bans_power_needed",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },
                new()
                {
                    Id = NextGuid(),
                    Alias = "uint64_user_modify_bans_own_power_needed",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },
                new()
                {
                    Id = NextGuid(),
                    Alias = "uint64_user_delete_bans_power",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },
                new()
                {
                    Id = NextGuid(),
                    Alias = "uint64_user_delete_bans_power_needed",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },
                new()
                {
                    Id = NextGuid(),
                    Alias = "uint64_user_delete_bans_own_power_needed",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },
                new()
                {
                    Id = NextGuid(),
                    Alias = "uint64_user_max_ban_time",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },

                #endregion

                #endregion
            };

            appDbContextSeedLists.Permissions.AddRange(permissions);

            #endregion

            #region Users

            var users = new List<User>();

            var userGuest = new User
            {
                Id = NextGuid(),
            };
            users.Add(userGuest);

            appDbContextSeedLists.Users.AddRange(users);

            #endregion

            #region UserProfiles

            var userProfiles = new List<UserProfile>();

            var userProfileGuest = new UserProfile
            {
                Id = NextGuid(),
                Username = "Guest",
                Description = "Initial system user",
                UserId = userGuest.Id,
            };
            userProfiles.Add(userProfileGuest);

            appDbContextSeedLists.UserProfiles.AddRange(userProfiles);

            #endregion

            #region UserGroups

            var userGroups = new List<UserGroup>
            {
                new()
                {
                    Id = NextGuid(),
                    Alias = "Root",
                    Description = "System user group with root like permission set"
                },
                new()
                {
                    Id = NextGuid(),
                    Alias = "Guest",
                    Description = "System user group with guest like permission set"
                }
            };

            var rootUserGroup = userGroups.First(__ => __.Alias == "Root");
            var guestUserGroup = userGroups.First(__ => __.Alias == "Guest");

            appDbContextSeedLists.UserGroups.AddRange(userGroups);

            #endregion

            #region UserGroupsPermissionValues

            var rootPower = ulong.MaxValue;

            var rootUserGroupPermissionValues = new List<UserGroupPermissionValue>()
            {
                #region Any

                new()
                {
                    Id = NextGuid(),
                    Value = BitConverter.GetBytes(true),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "boolean_any_view_permission_overview").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = NextGuid(),
                    Value = BitConverter.GetBytes(true),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "boolean_any_view_permission_overview_own").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = NextGuid(),
                    Value = BitConverter.GetBytes(rootPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_any_modify_permission_power").Id,
                    EntityId = rootUserGroup.Id,
                },

                #endregion

                #region Group

                new()
                {
                    Id = NextGuid(),
                    Value = BitConverter.GetBytes(true),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "boolean_group_create_groups").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = NextGuid(),
                    Value = BitConverter.GetBytes(true),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "boolean_group_view_groups").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = NextGuid(),
                    Value = BitConverter.GetBytes(true),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "boolean_group_view_groups_own").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = NextGuid(),
                    Value = BitConverter.GetBytes(rootPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_group_delete_groups_power").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = NextGuid(),
                    Value = BitConverter.GetBytes(rootPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_group_delete_groups_power_needed").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = NextGuid(),
                    Value = BitConverter.GetBytes(rootPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_group_add_member_power").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = NextGuid(),
                    Value = BitConverter.GetBytes(rootPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_group_add_member_power_needed").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = NextGuid(),
                    Value = BitConverter.GetBytes(rootPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_group_remove_member_power").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = NextGuid(),
                    Value = BitConverter.GetBytes(rootPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_group_remove_member_power_needed").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = NextGuid(),
                    Value = BitConverter.GetBytes(rootPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_group_modify_alias_power").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = NextGuid(),
                    Value = BitConverter.GetBytes(rootPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_group_modify_alias_power_needed").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = NextGuid(),
                    Value = BitConverter.GetBytes(rootPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_group_modify_description_power").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = NextGuid(),
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
                    Id = NextGuid(),
                    Value = BitConverter.GetBytes(true),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "boolean_user_view_email").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = NextGuid(),
                    Value = BitConverter.GetBytes(true),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "boolean_user_view_email_own").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = NextGuid(),
                    Value = BitConverter.GetBytes(rootPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_modify_email_power").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = NextGuid(),
                    Value = BitConverter.GetBytes(rootPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_modify_email_power_needed").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = NextGuid(),
                    Value = BitConverter.GetBytes(rootPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_modify_email_own_power_needed").Id,
                    EntityId = rootUserGroup.Id,
                },

                #endregion

                new()
                {
                    Id = NextGuid(),
                    Value = BitConverter.GetBytes(true),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "boolean_user_view_lastipaddress").Id,
                    EntityId = rootUserGroup.Id,
                },

                #region Profile Username

                new()
                {
                    Id = NextGuid(),
                    Value = BitConverter.GetBytes(true),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "boolean_user_view_profile_username").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = NextGuid(),
                    Value = BitConverter.GetBytes(true),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "boolean_user_view_profile_username_own").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = NextGuid(),
                    Value = BitConverter.GetBytes(rootPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_modify_profile_username_power").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = NextGuid(),
                    Value = BitConverter.GetBytes(rootPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_modify_profile_username_power_needed")
                        .Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = NextGuid(),
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
                    Id = NextGuid(),
                    Value = BitConverter.GetBytes(true),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "boolean_user_view_profile_basic").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = NextGuid(),
                    Value = BitConverter.GetBytes(true),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "boolean_user_view_profile_basic_own").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = NextGuid(),
                    Value = BitConverter.GetBytes(rootPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_modify_profile_basic_power").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = NextGuid(),
                    Value = BitConverter.GetBytes(rootPower),
                    Grant = rootPower,
                    PermissionId =
                        permissions.First(_ => _.Alias == "uint64_user_modify_profile_basic_power_needed").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = NextGuid(),
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
                    Id = NextGuid(),
                    Value = BitConverter.GetBytes(true),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "boolean_user_view_profile_custom").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = NextGuid(),
                    Value = BitConverter.GetBytes(true),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "boolean_user_view_profile_custom_own").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = NextGuid(),
                    Value = BitConverter.GetBytes(rootPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_modify_profile_custom_power").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = NextGuid(),
                    Value = BitConverter.GetBytes(rootPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_modify_profile_custom_power_needed")
                        .Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = NextGuid(),
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
                    Id = NextGuid(),
                    Value = BitConverter.GetBytes(rootPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_delete_user_power").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = NextGuid(),
                    Value = BitConverter.GetBytes(rootPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_delete_user_power_needed").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = NextGuid(),
                    Value = BitConverter.GetBytes(rootPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_delete_user_own_power_needed").Id,
                    EntityId = rootUserGroup.Id,
                },

                #endregion

                #region Communication Private

                new()
                {
                    Id = NextGuid(),
                    Value = BitConverter.GetBytes(rootPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_communication_private_power").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = NextGuid(),
                    Value = BitConverter.GetBytes(rootPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_communication_private_power_needed")
                        .Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = NextGuid(),
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
                    Id = NextGuid(),
                    Value = BitConverter.GetBytes(rootPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_communication_global_power").Id,
                    EntityId = rootUserGroup.Id,
                },

                #endregion

                #region Communication Poke

                new()
                {
                    Id = NextGuid(),
                    Value = BitConverter.GetBytes(rootPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_poke_power").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = NextGuid(),
                    Value = BitConverter.GetBytes(rootPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_poke_power_needed").Id,
                    EntityId = rootUserGroup.Id,
                },

                #endregion

                new()
                {
                    Id = NextGuid(),
                    Value = BitConverter.GetBytes(rootPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_max_apuint64_keys").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = NextGuid(),
                    Value = BitConverter.GetBytes(rootPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_max_avatar_filesize").Id,
                    EntityId = rootUserGroup.Id,
                },

                #region Complaints

                new()
                {
                    Id = NextGuid(),
                    Value = BitConverter.GetBytes(rootPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_create_complaints_power").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = NextGuid(),
                    Value = BitConverter.GetBytes(rootPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_create_complaints_power_needed").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = NextGuid(),
                    Value = BitConverter.GetBytes(true),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "boolean_user_view_complaints").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = NextGuid(),
                    Value = BitConverter.GetBytes(true),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "boolean_user_view_complaints_own").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = NextGuid(),
                    Value = BitConverter.GetBytes(rootPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_modify_complaints_power").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = NextGuid(),
                    Value = BitConverter.GetBytes(rootPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_modify_complaints_power_needed").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = NextGuid(),
                    Value = BitConverter.GetBytes(rootPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_modify_complaints_own_power_needed")
                        .Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = NextGuid(),
                    Value = BitConverter.GetBytes(rootPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_delete_complaints_power").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = NextGuid(),
                    Value = BitConverter.GetBytes(rootPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_delete_complaints_power_needed").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = NextGuid(),
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
                    Id = NextGuid(),
                    Value = BitConverter.GetBytes(rootPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_create_bans_power").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = NextGuid(),
                    Value = BitConverter.GetBytes(rootPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_create_bans_power_needed").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = NextGuid(),
                    Value = BitConverter.GetBytes(true),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "boolean_user_view_bans").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = NextGuid(),
                    Value = BitConverter.GetBytes(true),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "boolean_user_view_bans_own").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = NextGuid(),
                    Value = BitConverter.GetBytes(rootPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_modify_bans_power").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = NextGuid(),
                    Value = BitConverter.GetBytes(rootPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_modify_bans_power_needed").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = NextGuid(),
                    Value = BitConverter.GetBytes(rootPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_modify_bans_own_power_needed").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = NextGuid(),
                    Value = BitConverter.GetBytes(rootPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_delete_bans_power").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = NextGuid(),
                    Value = BitConverter.GetBytes(rootPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_delete_bans_power_needed").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = NextGuid(),
                    Value = BitConverter.GetBytes(rootPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_delete_bans_own_power_needed").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = NextGuid(),
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
                    Id = NextGuid(),
                    Value = BitConverter.GetBytes(true),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "boolean_any_view_permission_overview_own").Id,
                    EntityId = guestUserGroup.Id,
                },

                #endregion

                #region Group

                new()
                {
                    Id = NextGuid(),
                    Value = BitConverter.GetBytes(true),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "boolean_group_view_groups").Id,
                    EntityId = guestUserGroup.Id,
                },
                new()
                {
                    Id = NextGuid(),
                    Value = BitConverter.GetBytes(true),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "boolean_group_view_groups_own").Id,
                    EntityId = guestUserGroup.Id,
                },
                new()
                {
                    Id = NextGuid(),
                    Value = BitConverter.GetBytes(rootPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_group_delete_groups_power_needed").Id,
                    EntityId = guestUserGroup.Id,
                },
                new()
                {
                    Id = NextGuid(),
                    Value = BitConverter.GetBytes(rootPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_group_add_member_power_needed").Id,
                    EntityId = guestUserGroup.Id,
                },
                new()
                {
                    Id = NextGuid(),
                    Value = BitConverter.GetBytes(rootPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_group_remove_member_power_needed").Id,
                    EntityId = guestUserGroup.Id,
                },
                new()
                {
                    Id = NextGuid(),
                    Value = BitConverter.GetBytes(rootPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_group_modify_alias_power_needed").Id,
                    EntityId = guestUserGroup.Id,
                },
                new()
                {
                    Id = NextGuid(),
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
                    Id = NextGuid(),
                    Value = BitConverter.GetBytes(false),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "boolean_user_view_email").Id,
                    EntityId = guestUserGroup.Id,
                },
                new()
                {
                    Id = NextGuid(),
                    Value = BitConverter.GetBytes(true),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "boolean_user_view_email_own").Id,
                    EntityId = guestUserGroup.Id,
                },
                new()
                {
                    Id = NextGuid(),
                    Value = BitConverter.GetBytes(guestPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_modify_email_power").Id,
                    EntityId = guestUserGroup.Id,
                },
                new()
                {
                    Id = NextGuid(),
                    Value = BitConverter.GetBytes(rootPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_modify_email_power_needed").Id,
                    EntityId = guestUserGroup.Id,
                },
                new()
                {
                    Id = NextGuid(),
                    Value = BitConverter.GetBytes(guestPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_modify_email_own_power_needed").Id,
                    EntityId = guestUserGroup.Id,
                },

                #endregion

                new()
                {
                    Id = NextGuid(),
                    Value = BitConverter.GetBytes(false),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "boolean_user_view_lastipaddress").Id,
                    EntityId = guestUserGroup.Id,
                },

                #region Profile Username

                new()
                {
                    Id = NextGuid(),
                    Value = BitConverter.GetBytes(true),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "boolean_user_view_profile_username").Id,
                    EntityId = guestUserGroup.Id,
                },
                new()
                {
                    Id = NextGuid(),
                    Value = BitConverter.GetBytes(true),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "boolean_user_view_profile_username_own").Id,
                    EntityId = guestUserGroup.Id,
                },
                new()
                {
                    Id = NextGuid(),
                    Value = BitConverter.GetBytes(guestPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_modify_profile_username_power").Id,
                    EntityId = guestUserGroup.Id,
                },
                new()
                {
                    Id = NextGuid(),
                    Value = BitConverter.GetBytes(rootPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_modify_profile_username_power_needed")
                        .Id,
                    EntityId = guestUserGroup.Id,
                },
                new()
                {
                    Id = NextGuid(),
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
                    Id = NextGuid(),
                    Value = BitConverter.GetBytes(true),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "boolean_user_view_profile_basic").Id,
                    EntityId = guestUserGroup.Id,
                },
                new()
                {
                    Id = NextGuid(),
                    Value = BitConverter.GetBytes(true),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "boolean_user_view_profile_basic_own").Id,
                    EntityId = guestUserGroup.Id,
                },
                new()
                {
                    Id = NextGuid(),
                    Value = BitConverter.GetBytes(guestPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_modify_profile_basic_power").Id,
                    EntityId = guestUserGroup.Id,
                },
                new()
                {
                    Id = NextGuid(),
                    Value = BitConverter.GetBytes(rootPower),
                    Grant = rootPower,
                    PermissionId =
                        permissions.First(_ => _.Alias == "uint64_user_modify_profile_basic_power_needed").Id,
                    EntityId = guestUserGroup.Id,
                },
                new()
                {
                    Id = NextGuid(),
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
                    Id = NextGuid(),
                    Value = BitConverter.GetBytes(true),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "boolean_user_view_profile_custom").Id,
                    EntityId = guestUserGroup.Id,
                },
                new()
                {
                    Id = NextGuid(),
                    Value = BitConverter.GetBytes(true),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "boolean_user_view_profile_custom_own").Id,
                    EntityId = guestUserGroup.Id,
                },
                new()
                {
                    Id = NextGuid(),
                    Value = BitConverter.GetBytes(guestPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_modify_profile_custom_power").Id,
                    EntityId = guestUserGroup.Id,
                },
                new()
                {
                    Id = NextGuid(),
                    Value = BitConverter.GetBytes(rootPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_modify_profile_custom_power_needed")
                        .Id,
                    EntityId = guestUserGroup.Id,
                },
                new()
                {
                    Id = NextGuid(),
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
                    Id = NextGuid(),
                    Value = BitConverter.GetBytes(guestPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_delete_user_power").Id,
                    EntityId = guestUserGroup.Id,
                },
                new()
                {
                    Id = NextGuid(),
                    Value = BitConverter.GetBytes(rootPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_delete_user_power_needed").Id,
                    EntityId = guestUserGroup.Id,
                },
                new()
                {
                    Id = NextGuid(),
                    Value = BitConverter.GetBytes(guestPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_delete_user_own_power_needed").Id,
                    EntityId = guestUserGroup.Id,
                },

                #endregion

                #region Communication Private

                new()
                {
                    Id = NextGuid(),
                    Value = BitConverter.GetBytes(guestPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_communication_private_power").Id,
                    EntityId = guestUserGroup.Id,
                },
                new()
                {
                    Id = NextGuid(),
                    Value = BitConverter.GetBytes(guestPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_communication_private_power_needed")
                        .Id,
                    EntityId = guestUserGroup.Id,
                },
                new()
                {
                    Id = NextGuid(),
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
                    Id = NextGuid(),
                    Value = BitConverter.GetBytes(guestPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_communication_global_power").Id,
                    EntityId = guestUserGroup.Id,
                },

                #endregion

                #region Communication Poke

                new()
                {
                    Id = NextGuid(),
                    Value = BitConverter.GetBytes(guestPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_poke_power").Id,
                    EntityId = guestUserGroup.Id,
                },
                new()
                {
                    Id = NextGuid(),
                    Value = BitConverter.GetBytes(guestPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_poke_power_needed").Id,
                    EntityId = guestUserGroup.Id,
                },

                #endregion

                new()
                {
                    Id = NextGuid(),
                    Value = BitConverter.GetBytes(0),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_max_apuint64_keys").Id,
                    EntityId = guestUserGroup.Id,
                },
                new()
                {
                    Id = NextGuid(),
                    Value = BitConverter.GetBytes(0),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_max_avatar_filesize").Id,
                    EntityId = guestUserGroup.Id,
                },

                #region Complaints

                new()
                {
                    Id = NextGuid(),
                    Value = BitConverter.GetBytes(guestPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_create_complaints_power").Id,
                    EntityId = guestUserGroup.Id,
                },
                new()
                {
                    Id = NextGuid(),
                    Value = BitConverter.GetBytes(guestPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_create_complaints_power_needed").Id,
                    EntityId = guestUserGroup.Id,
                },
                new()
                {
                    Id = NextGuid(),
                    Value = BitConverter.GetBytes(false),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "boolean_user_view_complaints").Id,
                    EntityId = guestUserGroup.Id,
                },
                new()
                {
                    Id = NextGuid(),
                    Value = BitConverter.GetBytes(true),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "boolean_user_view_complaints_own").Id,
                    EntityId = guestUserGroup.Id,
                },
                new()
                {
                    Id = NextGuid(),
                    Value = BitConverter.GetBytes(guestPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_modify_complaints_power").Id,
                    EntityId = guestUserGroup.Id,
                },
                new()
                {
                    Id = NextGuid(),
                    Value = BitConverter.GetBytes(rootPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_modify_complaints_power_needed").Id,
                    EntityId = guestUserGroup.Id,
                },
                new()
                {
                    Id = NextGuid(),
                    Value = BitConverter.GetBytes(guestPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_modify_complaints_own_power_needed")
                        .Id,
                    EntityId = guestUserGroup.Id,
                },
                new()
                {
                    Id = NextGuid(),
                    Value = BitConverter.GetBytes(guestPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_delete_complaints_power").Id,
                    EntityId = guestUserGroup.Id,
                },
                new()
                {
                    Id = NextGuid(),
                    Value = BitConverter.GetBytes(rootPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_delete_complaints_power_needed").Id,
                    EntityId = guestUserGroup.Id,
                },
                new()
                {
                    Id = NextGuid(),
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
                    Id = NextGuid(),
                    Value = BitConverter.GetBytes(rootPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_create_bans_power_needed").Id,
                    EntityId = guestUserGroup.Id,
                },
                new()
                {
                    Id = NextGuid(),
                    Value = BitConverter.GetBytes(rootPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_modify_bans_power_needed").Id,
                    EntityId = guestUserGroup.Id,
                },
                new()
                {
                    Id = NextGuid(),
                    Value = BitConverter.GetBytes(rootPower),
                    Grant = rootPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_delete_bans_power_needed").Id,
                    EntityId = guestUserGroup.Id,
                },

                #endregion

                #endregion
            };

            appDbContextSeedLists.UserGroupPermissionValues.AddRange(rootUserGroupPermissionValues);
            appDbContextSeedLists.UserGroupPermissionValues.AddRange(guestUserGroupPermissionValues);

            #endregion

            #region UserToGroupMappings

            var userGuestToGuestUserGroupMapping = new UserToGroupMapping
            {
                Id = NextGuid(),
                EntityId = userGuest.Id,
                GroupId = guestUserGroup.Id,
            };

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

            #region Files

            builder.Entity<File>(_ => { _.HasIndex(__ => __.Name).IsUnique(); });

            #endregion
        }

        #region Helpers

        private static Guid NextGuid()
        {
            return ToGuid(++_guidSeedOffset);
        }

        private static Guid ToGuid(int value)
        {
            var bytes = new byte[16];
            BitConverter.GetBytes(value).CopyTo(bytes, 0);
            return new Guid(bytes);
        }

        #endregion
    }
}