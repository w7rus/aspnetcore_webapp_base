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
        public DbSet<UserToUserGroupMapping> UserToUserGroupMappings { get; set; }
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

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
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
            public List<UserToUserGroupMapping> UserToUserGroupMappings { get; set; }
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
                UserToUserGroupMappings = new List<UserToUserGroupMapping>(),
                JsonWebTokens = new List<JsonWebToken>(),
                RefreshTokens = new List<RefreshToken>()
            };

            #region Permissions

            var permissions = new List<Permission>
            {
                /*
                 * Create/Read/Modify Permissions might have:
                 * 
                 * 1  (boolean_~)
                 * [Simple restriction]
                 * Example use cases:
                 * - System permits VIP Group users to set custom profile title
                 * 
                 * or
                 * 
                 * 2 UInt64 (uint64_~power + uint64_~power_needed)
                 * 
                 * [Power based restriction]
                 * Ones entity permission value is compared to the other entity permission value.
                 * Example use cases:
                 * - Store permits Owner Group & Manager Group to create new products listings, but not the Employee Group | Customer Group users
                 * 
                 * or
                 *
                 * 3 UInt64 (uint64_~power + uint64_~power_needed + uint64_~own_power_needed)
                 * Ones entity permission value is compared to the uint64_~power_needed entity permission value if entity tries to take an action NOT on owned object.
                 * Ones entity permission value is compared to the uint64_~own_power_needed entity permission value if entity tries to take an action on owned object.
                 * Example use cases:
                 * - You friend is away from PC and asked you to upload pictures you took as if you were logged into his account (Not to mistake with Sharing functionality). Same as `su <username>` in Linux-based distros, which is used to execute action under different user.
                 * - System allows Owner Group users to Modify any group, but VIP Group users to Modify only own groups
                 */
                
                //DO NOT FORGET TO ADD `migrationBuilder.Sql("create extension if not exists hstore");` inside Initial migration

                #region Any

                new()
                {
                    Id = RandGuid(),
                    Alias = "boolean_any_view_permission_overview",
                    Type = PermissionType.Boolean,
                    CompareMode = PermissionCompareMode.Equal
                },
                new()
                {
                    Id = RandGuid(),
                    Alias = "uint64_any_modify_permission_power",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },

                #endregion

                #region Group

                #region Create

                new()
                {
                    Id = RandGuid(),
                    Alias = "boolean_group_create_groups",
                    Type = PermissionType.Boolean,
                    CompareMode = PermissionCompareMode.Equal
                },

                #endregion

                #region Read

                new()
                {
                    Id = RandGuid(),
                    Alias = "uint64_group_view_groups_power",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },
                new()
                {
                    Id = RandGuid(),
                    Alias = "uint64_group_view_groups_power_needed",
                    Type = PermissionType.Boolean,
                    CompareMode = PermissionCompareMode.Equal
                },
                new()
                {
                    Id = RandGuid(),
                    Alias = "uint64_group_view_groups_own_power_needed",
                    Type = PermissionType.Boolean,
                    CompareMode = PermissionCompareMode.Equal
                },

                #endregion

                #region Modify

                new()
                {
                    Id = RandGuid(),
                    Alias = "uint64_group_modify_groups_power",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },
                new()
                {
                    Id = RandGuid(),
                    Alias = "uint64_group_modify_groups_power_needed",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },
                new()
                {
                    Id = RandGuid(),
                    Alias = "uint64_group_modify_groups_own_power_needed",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },

                #endregion

                #endregion

                #region User

                #region Read (Email)

                new()
                {
                    Id = RandGuid(),
                    Alias = "uint64_user_view_email_power",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },
                new()
                {
                    Id = RandGuid(),
                    Alias = "uint64_user_view_email_power_needed",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },
                new()
                {
                    Id = RandGuid(),
                    Alias = "uint64_user_view_email_own_power_needed",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },

                #endregion

                #region Modify (Email)

                new()
                {
                    Id = RandGuid(),
                    Alias = "uint64_user_modify_email_power",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },
                new()
                {
                    Id = RandGuid(),
                    Alias = "uint64_user_modify_email_power_needed",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },
                new()
                {
                    Id = RandGuid(),
                    Alias = "uint64_user_modify_email_own_power_needed",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },

                #endregion

                #region Read (LastIpAddress)

                new()
                {
                    Id = RandGuid(),
                    Alias = "uint64t_user_view_lastipaddress_power",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },
                new()
                {
                    Id = RandGuid(),
                    Alias = "uint64t_user_view_lastipaddress_power_needed",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },
                new()
                {
                    Id = RandGuid(),
                    Alias = "uint64t_user_view_lastipaddress_own_power_needed",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },

                #endregion

                #region Delete

                new()
                {
                    Id = RandGuid(),
                    Alias = "uint64_user_delete_user_power",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },
                new()
                {
                    Id = RandGuid(),
                    Alias = "uint64_user_delete_user_power_needed",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },
                new()
                {
                    Id = RandGuid(),
                    Alias = "uint64_user_delete_user_own_power_needed",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },

                #endregion

                #region Action (Communication Private)

                new()
                {
                    Id = RandGuid(),
                    Alias = "uint64_user_communication_private_power",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },
                new()
                {
                    Id = RandGuid(),
                    Alias = "uint64_user_communication_private_power_needed",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },
                new()
                {
                    Id = RandGuid(),
                    Alias = "uint64_user_communication_private_own_power_needed",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },

                #endregion

                #region Action (Communication Global)

                new()
                {
                    Id = RandGuid(),
                    Alias = "uint64_user_communication_global_power",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },

                #endregion

                #region Action (Communication Poke)

                new()
                {
                    Id = RandGuid(),
                    Alias = "uint64_user_poke_power",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },
                new()
                {
                    Id = RandGuid(),
                    Alias = "uint64_user_poke_power_needed",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },

                #endregion

                #region Complaints

                new()
                {
                    Id = RandGuid(),
                    Alias = "uint64_user_create_complaints_power",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },
                new()
                {
                    Id = RandGuid(),
                    Alias = "uint64_user_create_complaints_power_needed",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },
                new()
                {
                    Id = RandGuid(),
                    Alias = "boolean_user_view_complaints",
                    Type = PermissionType.Boolean,
                    CompareMode = PermissionCompareMode.Equal
                },
                new()
                {
                    Id = RandGuid(),
                    Alias = "boolean_user_view_complaints_own",
                    Type = PermissionType.Boolean,
                    CompareMode = PermissionCompareMode.Equal
                },
                new()
                {
                    Id = RandGuid(),
                    Alias = "uint64_user_modify_complaints_power",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },
                new()
                {
                    Id = RandGuid(),
                    Alias = "uint64_user_modify_complaints_power_needed",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },
                new()
                {
                    Id = RandGuid(),
                    Alias = "uint64_user_modify_complaints_own_power_needed",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },

                #endregion

                #region Bans

                new()
                {
                    Id = RandGuid(),
                    Alias = "uint64_user_create_bans_power",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },
                new()
                {
                    Id = RandGuid(),
                    Alias = "uint64_user_create_bans_power_needed",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },
                new()
                {
                    Id = RandGuid(),
                    Alias = "boolean_user_view_bans",
                    Type = PermissionType.Boolean,
                    CompareMode = PermissionCompareMode.Equal
                },
                new()
                {
                    Id = RandGuid(),
                    Alias = "boolean_user_view_bans_own",
                    Type = PermissionType.Boolean,
                    CompareMode = PermissionCompareMode.Equal
                },
                new()
                {
                    Id = RandGuid(),
                    Alias = "uint64_user_modify_bans_power",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },
                new()
                {
                    Id = RandGuid(),
                    Alias = "uint64_user_modify_bans_power_needed",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },
                new()
                {
                    Id = RandGuid(),
                    Alias = "uint64_user_modify_bans_own_power_needed",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },
                new()
                {
                    Id = RandGuid(),
                    Alias = "uint64_user_max_ban_time",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },

                #endregion

                #endregion
                
                #region User Profile

                #region Modify
                
                new()
                {
                    Id = RandGuid(),
                    Alias = "uint64_userprofile_modify_power",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },
                new()
                {
                    Id = RandGuid(),
                    Alias = "uint64_userprofile_modify_power_needed",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },
                new()
                {
                    Id = RandGuid(),
                    Alias = "uint64_userprofile_modify_own_power_needed",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },

                #endregion
                
                new()
                {
                    Id = RandGuid(),
                    Alias = "uint64_userprofile_avatar_maxfilesize",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },

                #endregion

                #region File

                #region Create

                new()
                {
                    Id = RandGuid(),
                    Alias = "uint64_file_create_power",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },

                #endregion

                #region Read

                new()
                {
                    Id = RandGuid(),
                    Alias = "uint64_file_read_power",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },
                new()
                {
                    Id = RandGuid(),
                    Alias = "uint64_file_read_power_needed",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },
                new()
                {
                    Id = RandGuid(),
                    Alias = "uint64_file_read_own_power_needed",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },

                #endregion

                #region Modify

                new()
                {
                    Id = RandGuid(),
                    Alias = "uint64_file_modify_power",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },
                new()
                {
                    Id = RandGuid(),
                    Alias = "uint64_file_modify_power_needed",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },
                new()
                {
                    Id = RandGuid(),
                    Alias = "uint64_file_modify_own_power_needed",
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

            appDbContextSeedLists.Users.AddRange(users);

            #endregion

            #region UserProfiles

            var userProfiles = new List<UserProfile>();

            appDbContextSeedLists.UserProfiles.AddRange(userProfiles);

            #endregion

            #region UserGroups

            var userGroups = new List<UserGroup>
            {
                new()
                {
                    Id = RandGuid(),
                    Alias = "Root",
                    Description = "System user group with root like permission set"
                },
                new()
                {
                    Id = RandGuid(),
                    Alias = "Guest",
                    Description = "System user group with guest like permission set"
                }
            };

            var rootUserGroup = userGroups.First(__ => __.Alias == "Root");
            var memberUserGroup = userGroups.First(__ => __.Alias == "Member");

            appDbContextSeedLists.UserGroups.AddRange(userGroups);

            #endregion

            #region UserGroupsPermissionValues

            var rootUserGroupPower = ulong.MaxValue;
            var memberUserGroupPower = 50ul;

            var rootUserGroupPermissionValues = new List<UserGroupPermissionValue>()
            {
                #region Any

                new()
                {
                    Id = RandGuid(),
                    Value = BitConverter.GetBytes(true),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "boolean_any_view_permission_overview").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = RandGuid(),
                    Value = BitConverter.GetBytes(rootUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_any_modify_permission_power").Id,
                    EntityId = rootUserGroup.Id,
                },

                #endregion

                #region Group

                #region Create

                new()
                {
                    Id = RandGuid(),
                    Value = BitConverter.GetBytes(true),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "boolean_group_create_groups").Id,
                    EntityId = rootUserGroup.Id,
                },

                #endregion

                #region Read

                new()
                {
                    Id = RandGuid(),
                    Value = BitConverter.GetBytes(rootUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_group_view_groups_power").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = RandGuid(),
                    Value = BitConverter.GetBytes(rootUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_group_view_groups_power_needed").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = RandGuid(),
                    Value = BitConverter.GetBytes(rootUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_group_view_groups_own_power_needed").Id,
                    EntityId = rootUserGroup.Id,
                },

                #endregion

                #region Modify

                new()
                {
                    Id = RandGuid(),
                    Value = BitConverter.GetBytes(rootUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_group_modify_groups_power").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = RandGuid(),
                    Value = BitConverter.GetBytes(rootUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_group_modify_groups_power_needed").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = RandGuid(),
                    Value = BitConverter.GetBytes(rootUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_group_modify_groups_own_power_needed").Id,
                    EntityId = rootUserGroup.Id,
                },

                #endregion

                #endregion

                #region User

                #region Read (Email)

                new()
                {
                    Id = RandGuid(),
                    Value = BitConverter.GetBytes(rootUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_view_email_power").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = RandGuid(),
                    Value = BitConverter.GetBytes(rootUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_view_email_power_needed").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = RandGuid(),
                    Value = BitConverter.GetBytes(rootUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_view_email_own_power_needed").Id,
                    EntityId = rootUserGroup.Id,
                },

                #endregion

                #region Modify (Email)

                new()
                {
                    Id = RandGuid(),
                    Value = BitConverter.GetBytes(rootUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_modify_email_power").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = RandGuid(),
                    Value = BitConverter.GetBytes(rootUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_modify_email_power_needed").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = RandGuid(),
                    Value = BitConverter.GetBytes(rootUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_modify_email_own_power_needed").Id,
                    EntityId = rootUserGroup.Id,
                },

                #endregion

                #region Read (LastIpAddress)

                new()
                {
                    Id = RandGuid(),
                    Value = BitConverter.GetBytes(rootUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64t_user_view_lastipaddress_power").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = RandGuid(),
                    Value = BitConverter.GetBytes(rootUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64t_user_view_lastipaddress_power_needed").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = RandGuid(),
                    Value = BitConverter.GetBytes(rootUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64t_user_view_lastipaddress_own_power_needed").Id,
                    EntityId = rootUserGroup.Id,
                },

                #endregion

                #region Delete

                new()
                {
                    Id = RandGuid(),
                    Value = BitConverter.GetBytes(rootUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_delete_user_power").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = RandGuid(),
                    Value = BitConverter.GetBytes(rootUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_delete_user_power_needed").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = RandGuid(),
                    Value = BitConverter.GetBytes(rootUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_delete_user_own_power_needed").Id,
                    EntityId = rootUserGroup.Id,
                },

                #endregion

                #region Action (Communication Private)

                new()
                {
                    Id = RandGuid(),
                    Value = BitConverter.GetBytes(rootUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_communication_private_power").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = RandGuid(),
                    Value = BitConverter.GetBytes(rootUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_communication_private_power_needed").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = RandGuid(),
                    Value = BitConverter.GetBytes(rootUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_communication_private_own_power_needed").Id,
                    EntityId = rootUserGroup.Id,
                },

                #endregion

                #region Action (Communication Global)

                new()
                {
                    Id = RandGuid(),
                    Value = BitConverter.GetBytes(rootUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_communication_global_power").Id,
                    EntityId = rootUserGroup.Id,
                },

                #endregion

                #region Action (Communication Poke)

                new()
                {
                    Id = RandGuid(),
                    Value = BitConverter.GetBytes(rootUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_poke_power").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = RandGuid(),
                    Value = BitConverter.GetBytes(rootUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_poke_power_needed").Id,
                    EntityId = rootUserGroup.Id,
                },

                #endregion

                #region Complaints

                new()
                {
                    Id = RandGuid(),
                    Value = BitConverter.GetBytes(rootUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_create_complaints_power").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = RandGuid(),
                    Value = BitConverter.GetBytes(rootUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_create_complaints_power_needed").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = RandGuid(),
                    Value = BitConverter.GetBytes(true),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "boolean_user_view_complaints").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = RandGuid(),
                    Value = BitConverter.GetBytes(true),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "boolean_user_view_complaints_own").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = RandGuid(),
                    Value = BitConverter.GetBytes(rootUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_modify_complaints_power").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = RandGuid(),
                    Value = BitConverter.GetBytes(rootUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_modify_complaints_power_needed").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = RandGuid(),
                    Value = BitConverter.GetBytes(rootUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_modify_complaints_own_power_needed").Id,
                    EntityId = rootUserGroup.Id,
                },

                #endregion

                #region Bans

                new()
                {
                    Id = RandGuid(),
                    Value = BitConverter.GetBytes(rootUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_create_bans_power").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = RandGuid(),
                    Value = BitConverter.GetBytes(rootUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_create_bans_power_needed").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = RandGuid(),
                    Value = BitConverter.GetBytes(true),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "boolean_user_view_bans").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = RandGuid(),
                    Value = BitConverter.GetBytes(true),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "boolean_user_view_bans_own").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = RandGuid(),
                    Value = BitConverter.GetBytes(rootUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_modify_bans_power").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = RandGuid(),
                    Value = BitConverter.GetBytes(rootUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_modify_bans_power_needed").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = RandGuid(),
                    Value = BitConverter.GetBytes(rootUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_modify_bans_own_power_needed").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = RandGuid(),
                    Value = BitConverter.GetBytes(ulong.MaxValue),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_max_ban_time").Id,
                    EntityId = rootUserGroup.Id,
                },

                #endregion

                #endregion

                #region User Profile

                #region Modify

                new()
                {
                    Id = RandGuid(),
                    Value = BitConverter.GetBytes(rootUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_userprofile_modify_power").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = RandGuid(),
                    Value = BitConverter.GetBytes(rootUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_userprofile_modify_power_needed").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = RandGuid(),
                    Value = BitConverter.GetBytes(rootUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_userprofile_modify_own_power_needed").Id,
                    EntityId = rootUserGroup.Id,
                },

                #endregion

                new()
                {
                    Id = RandGuid(),
                    Value = BitConverter.GetBytes(rootUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_userprofile_avatar_maxfilesize").Id,
                    EntityId = rootUserGroup.Id,
                },

                #endregion

                #region File

                #region Create

                new()
                {
                    Id = RandGuid(),
                    Value = BitConverter.GetBytes(rootUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_file_create_power").Id,
                    EntityId = rootUserGroup.Id,
                },

                #endregion

                #region Read

                new()
                {
                    Id = RandGuid(),
                    Value = BitConverter.GetBytes(rootUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_file_read_power").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = RandGuid(),
                    Value = BitConverter.GetBytes(rootUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_file_read_power_needed").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = RandGuid(),
                    Value = BitConverter.GetBytes(rootUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_file_read_own_power_needed").Id,
                    EntityId = rootUserGroup.Id,
                },

                #endregion

                #region Modify

                new()
                {
                    Id = RandGuid(),
                    Value = BitConverter.GetBytes(rootUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_file_modify_power").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = RandGuid(),
                    Value = BitConverter.GetBytes(rootUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_file_modify_power_needed").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = RandGuid(),
                    Value = BitConverter.GetBytes(rootUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_file_modify_own_power_needed").Id,
                    EntityId = rootUserGroup.Id,
                },

                #endregion

                #endregion
            };

            var memberUserGroupPermissionValues = new List<UserGroupPermissionValue>()
            {
                #region Any

                new()
                {
                    Id = RandGuid(),
                    Value = BitConverter.GetBytes(true),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "boolean_any_view_permission_overview").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = RandGuid(),
                    Value = BitConverter.GetBytes(0),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_any_modify_permission_power").Id,
                    EntityId = rootUserGroup.Id,
                },

                #endregion

                #region Group

                #region Create

                new()
                {
                    Id = RandGuid(),
                    Value = BitConverter.GetBytes(false),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "boolean_group_create_groups").Id,
                    EntityId = rootUserGroup.Id,
                },

                #endregion

                #region Read

                new()
                {
                    Id = RandGuid(),
                    Value = BitConverter.GetBytes(memberUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_group_view_groups_power").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = RandGuid(),
                    Value = BitConverter.GetBytes(memberUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_group_view_groups_power_needed").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = RandGuid(),
                    Value = BitConverter.GetBytes(memberUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_group_view_groups_own_power_needed").Id,
                    EntityId = rootUserGroup.Id,
                },

                #endregion

                #region Modify

                new()
                {
                    Id = RandGuid(),
                    Value = BitConverter.GetBytes(memberUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_group_modify_groups_power").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = RandGuid(),
                    Value = BitConverter.GetBytes(rootUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_group_modify_groups_power_needed").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = RandGuid(),
                    Value = BitConverter.GetBytes(rootUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_group_modify_groups_own_power_needed").Id,
                    EntityId = rootUserGroup.Id,
                },

                #endregion

                #endregion

                #region User

                #region Read (Email)

                new()
                {
                    Id = RandGuid(),
                    Value = BitConverter.GetBytes(memberUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_view_email_power").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = RandGuid(),
                    Value = BitConverter.GetBytes(rootUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_view_email_power_needed").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = RandGuid(),
                    Value = BitConverter.GetBytes(memberUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_view_email_own_power_needed").Id,
                    EntityId = rootUserGroup.Id,
                },

                #endregion

                #region Modify (Email)

                new()
                {
                    Id = RandGuid(),
                    Value = BitConverter.GetBytes(memberUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_modify_email_power").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = RandGuid(),
                    Value = BitConverter.GetBytes(rootUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_modify_email_power_needed").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = RandGuid(),
                    Value = BitConverter.GetBytes(memberUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_modify_email_own_power_needed").Id,
                    EntityId = rootUserGroup.Id,
                },

                #endregion

                #region Read (LastIpAddress)

                new()
                {
                    Id = RandGuid(),
                    Value = BitConverter.GetBytes(memberUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64t_user_view_lastipaddress_power").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = RandGuid(),
                    Value = BitConverter.GetBytes(rootUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64t_user_view_lastipaddress_power_needed").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = RandGuid(),
                    Value = BitConverter.GetBytes(memberUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64t_user_view_lastipaddress_own_power_needed").Id,
                    EntityId = rootUserGroup.Id,
                },

                #endregion

                #region Delete

                new()
                {
                    Id = RandGuid(),
                    Value = BitConverter.GetBytes(memberUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_delete_user_power").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = RandGuid(),
                    Value = BitConverter.GetBytes(rootUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_delete_user_power_needed").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = RandGuid(),
                    Value = BitConverter.GetBytes(memberUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_delete_user_own_power_needed").Id,
                    EntityId = rootUserGroup.Id,
                },

                #endregion

                #region Action (Communication Private)

                new()
                {
                    Id = RandGuid(),
                    Value = BitConverter.GetBytes(memberUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_communication_private_power").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = RandGuid(),
                    Value = BitConverter.GetBytes(memberUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_communication_private_power_needed").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = RandGuid(),
                    Value = BitConverter.GetBytes(memberUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_communication_private_own_power_needed").Id,
                    EntityId = rootUserGroup.Id,
                },

                #endregion

                #region Action (Communication Global)

                new()
                {
                    Id = RandGuid(),
                    Value = BitConverter.GetBytes(memberUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_communication_global_power").Id,
                    EntityId = rootUserGroup.Id,
                },

                #endregion

                #region Action (Communication Poke)

                new()
                {
                    Id = RandGuid(),
                    Value = BitConverter.GetBytes(memberUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_poke_power").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = RandGuid(),
                    Value = BitConverter.GetBytes(memberUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_poke_power_needed").Id,
                    EntityId = rootUserGroup.Id,
                },

                #endregion

                #region Complaints

                new()
                {
                    Id = RandGuid(),
                    Value = BitConverter.GetBytes(memberUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_create_complaints_power").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = RandGuid(),
                    Value = BitConverter.GetBytes(memberUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_create_complaints_power_needed").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = RandGuid(),
                    Value = BitConverter.GetBytes(false),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "boolean_user_view_complaints").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = RandGuid(),
                    Value = BitConverter.GetBytes(true),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "boolean_user_view_complaints_own").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = RandGuid(),
                    Value = BitConverter.GetBytes(memberUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_modify_complaints_power").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = RandGuid(),
                    Value = BitConverter.GetBytes(rootUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_modify_complaints_power_needed").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = RandGuid(),
                    Value = BitConverter.GetBytes(memberUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_modify_complaints_own_power_needed").Id,
                    EntityId = rootUserGroup.Id,
                },

                #endregion

                #region Bans

                new()
                {
                    Id = RandGuid(),
                    Value = BitConverter.GetBytes(0),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_create_bans_power").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = RandGuid(),
                    Value = BitConverter.GetBytes(rootUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_create_bans_power_needed").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = RandGuid(),
                    Value = BitConverter.GetBytes(false),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "boolean_user_view_bans").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = RandGuid(),
                    Value = BitConverter.GetBytes(false),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "boolean_user_view_bans_own").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = RandGuid(),
                    Value = BitConverter.GetBytes(0),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_modify_bans_power").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = RandGuid(),
                    Value = BitConverter.GetBytes(rootUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_modify_bans_power_needed").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = RandGuid(),
                    Value = BitConverter.GetBytes(rootUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_modify_bans_own_power_needed").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = RandGuid(),
                    Value = BitConverter.GetBytes(ulong.MaxValue),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_max_ban_time").Id,
                    EntityId = rootUserGroup.Id,
                },

                #endregion

                #endregion

                #region User Profile

                #region Modify

                new()
                {
                    Id = RandGuid(),
                    Value = BitConverter.GetBytes(memberUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_userprofile_modify_power").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = RandGuid(),
                    Value = BitConverter.GetBytes(rootUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_userprofile_modify_power_needed").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = RandGuid(),
                    Value = BitConverter.GetBytes(memberUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_userprofile_modify_own_power_needed").Id,
                    EntityId = rootUserGroup.Id,
                },

                #endregion

                new()
                {
                    Id = RandGuid(),
                    Value = BitConverter.GetBytes(0),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_userprofile_avatar_maxfilesize").Id,
                    EntityId = rootUserGroup.Id,
                },

                #endregion

                #region File

                #region Create

                new()
                {
                    Id = RandGuid(),
                    Value = BitConverter.GetBytes(memberUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_file_create_power").Id,
                    EntityId = rootUserGroup.Id,
                },

                #endregion

                #region Read

                new()
                {
                    Id = RandGuid(),
                    Value = BitConverter.GetBytes(memberUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_file_read_power").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = RandGuid(),
                    Value = BitConverter.GetBytes(rootUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_file_read_power_needed").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = RandGuid(),
                    Value = BitConverter.GetBytes(memberUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_file_read_own_power_needed").Id,
                    EntityId = rootUserGroup.Id,
                },

                #endregion

                #region Modify

                new()
                {
                    Id = RandGuid(),
                    Value = BitConverter.GetBytes(memberUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_file_modify_power").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = RandGuid(),
                    Value = BitConverter.GetBytes(rootUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_file_modify_power_needed").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = RandGuid(),
                    Value = BitConverter.GetBytes(memberUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_file_modify_own_power_needed").Id,
                    EntityId = rootUserGroup.Id,
                },

                #endregion

                #endregion
            };

            appDbContextSeedLists.UserGroupPermissionValues.AddRange(rootUserGroupPermissionValues);
            appDbContextSeedLists.UserGroupPermissionValues.AddRange(memberUserGroupPermissionValues);

            #endregion

            #region UserToUserGroupMappings

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

            #endregion

            #region Users

            builder.Entity<User>(_ =>
            {
                _.HasIndex(__ => __.Email).IsUnique();

                //Allow PhoneNumber to remain empty
                // _.HasIndex(__ => __.PhoneNumber).IsUnique().HasFilter(null);

                _.HasData(appDbContextSeedLists.Users);
            });

            #endregion

            #region UserProfiles

            builder.Entity<UserProfile>(_ =>
            {
                _.HasIndex(__ => __.Username).IsUnique();

                _.HasData(appDbContextSeedLists.UserProfiles);
            });

            #endregion

            #region UserGroups

            builder.Entity<UserGroup>(_ =>
            {
                _.HasIndex(__ => __.Alias).IsUnique();

                _.HasData(appDbContextSeedLists.UserGroups);
            });

            #endregion

            #region UserGroupPermissionValues

            builder.Entity<UserGroupPermissionValue>(_ =>
            {
                _.HasIndex(__ => new {__.PermissionId, __.EntityId}).IsUnique();

                _.HasData(appDbContextSeedLists.UserGroupPermissionValues);
            });

            #endregion

            #region UserToUserGroupMappings

            builder.Entity<UserToUserGroupMapping>(_ =>
            {
                _.HasIndex(__ => new {__.EntityId, __.GroupId}).IsUnique();

                _.HasData(appDbContextSeedLists.UserToUserGroupMappings);
            });

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

        private static Guid RandGuid()
        {
            return Guid.NewGuid();
        }

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