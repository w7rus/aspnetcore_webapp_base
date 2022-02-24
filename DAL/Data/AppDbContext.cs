using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common.Models;
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
            // Database.Migrate();
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
                 *
                 * 4 UInt64 (uint64_~power + uint64_~power_needed + uint64_~own_power_needed + uint64_~own_power_needed_system)
                 * Same as the previous one with addition to a needed amount of power that is checked system wise.
                 * More info on system wise permissions below.
                 */

                /*
                 * Permission info & naming structure
                 * <valuetype_>some_fancy_permission_alias<<_own?>_power?<_needed?<_system?>>>
                 *
                 * valuetype_ can be found in Domain.Enums.PermissionType.
                 *          This prefix is not required, but a good manner to prefix your permissions, so the type could be guessed just from the name
                 * 
                 * _own?    states that this permission value acts as for self action.
                 *          - Ex.: Modify your avatar, email, private message yourself to leave a note
                 * 
                 * _power?  states that permission value is power-like compared to the other permission value.
                 *          - Ex.: Amount of power to modify avatar, email, private message (which would be compared against ~_own_power_needed, ~_power_needed, ~_power_needed_system)
                 *          Do use this suffix for Domain.Enums.PermissionType.UInt64 as other types are not used to compare power, but to define usage limits or constants.
                 * 
                 * _needed? states that this permission value acts as the limit of an action.
                 *          - Ex.: Limit group users to be members for at least X seconds in order to chat.
                 *          Modify (your) avatar, email, private message (yourself) to leave a note
                 *
                 * _system? states that this permission value acts as the limit of an action system wise.
                 *          - Ex.: Limit max avatar file size system wise. A minimum power required to upload a file system wise.
                 *          Permission values of this permission are only to be assigned for Root Group (they will only be compared from there)
                 */

                //DO NOT FORGET TO ADD `migrationBuilder.Sql("create extension if not exists hstore");` inside Initial migration

                #region Any

                new()
                {
                    Id = new Guid("505502c4-4055-4267-b631-ff869f14885d"),
                    Alias = "boolean_any_view_permission_overview",
                    Type = PermissionType.Boolean,
                    CompareMode = PermissionCompareMode.Equal
                },
                new()
                {
                    Id = new Guid("d1344244-8ea2-42f1-bf5c-5803794333b4"),
                    Alias = "uint64_any_modify_permission_power",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },

                #endregion

                #region Group

                #region Create

                new()
                {
                    Id = new Guid("fa8071de-d010-43c4-ae7e-bae0f47cb6bd"),
                    Alias = "boolean_group_create_groups",
                    Type = PermissionType.Boolean,
                    CompareMode = PermissionCompareMode.Equal
                },

                #endregion

                #region Read

                new()
                {
                    Id = new Guid("f7462ca7-43e6-415e-817e-c942f5471e25"),
                    Alias = "uint64_group_view_groups_power",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },
                new()
                {
                    Id = new Guid("5deb5229-4488-4c2c-974a-a16279b29794"),
                    Alias = "uint64_group_view_groups_power_needed",
                    Type = PermissionType.Boolean,
                    CompareMode = PermissionCompareMode.Equal
                },
                new()
                {
                    Id = new Guid("b316b212-6b69-48ea-982b-f986bc478a7a"),
                    Alias = "uint64_group_view_groups_own_power_needed",
                    Type = PermissionType.Boolean,
                    CompareMode = PermissionCompareMode.Equal
                },

                #endregion

                #region Modify

                new()
                {
                    Id = new Guid("bf308070-53a1-4893-b348-e6267659573e"),
                    Alias = "uint64_group_modify_groups_power",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },
                new()
                {
                    Id = new Guid("634b0339-1e37-4510-b32e-4b549e37fb7e"),
                    Alias = "uint64_group_modify_groups_power_needed",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },
                new()
                {
                    Id = new Guid("082f33f0-b2c9-4930-9d5b-358299b75514"),
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
                    Id = new Guid("11d0bc47-d2c7-4eb5-9322-cccafe02cef7"),
                    Alias = "uint64_user_view_email_power",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },
                new()
                {
                    Id = new Guid("ba26d7c7-fd22-4455-b6fd-8f40b2c1b0cd"),
                    Alias = "uint64_user_view_email_power_needed",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },
                new()
                {
                    Id = new Guid("bdeabe5f-dafb-40e1-8209-2c63f8cbcc3c"),
                    Alias = "uint64_user_view_email_own_power_needed",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },

                #endregion

                #region Modify (Email)

                new()
                {
                    Id = new Guid("34ac147d-bbc5-4e21-a7b1-8978c9feec9c"),
                    Alias = "uint64_user_modify_email_power",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },
                new()
                {
                    Id = new Guid("e83d9d16-3be4-4f17-b26d-d0977e6f2b69"),
                    Alias = "uint64_user_modify_email_power_needed",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },
                new()
                {
                    Id = new Guid("709a247a-71fc-43c5-9401-98670cefd65f"),
                    Alias = "uint64_user_modify_email_own_power_needed",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },

                #endregion

                #region Read (LastIpAddress)

                new()
                {
                    Id = new Guid("f14ce44a-260d-4215-9d8a-5c211f353165"),
                    Alias = "uint64t_user_view_lastipaddress_power",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },
                new()
                {
                    Id = new Guid("d5c58e90-3065-4a51-b079-9738954c76e9"),
                    Alias = "uint64t_user_view_lastipaddress_power_needed",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },
                new()
                {
                    Id = new Guid("b45ab8a1-609b-4467-a42a-49a63a5b01ed"),
                    Alias = "uint64t_user_view_lastipaddress_own_power_needed",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },

                #endregion

                #region Delete

                new()
                {
                    Id = new Guid("0d5ed48d-91a5-49fe-b27c-b288de79a7c3"),
                    Alias = "uint64_user_delete_user_power",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },
                new()
                {
                    Id = new Guid("830e69e1-dfe4-445d-a60b-a9e6f8444463"),
                    Alias = "uint64_user_delete_user_power_needed",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },
                new()
                {
                    Id = new Guid("f5bd5b91-4ce6-4686-8521-0ebfbed21fff"),
                    Alias = "uint64_user_delete_user_own_power_needed",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },

                #endregion

                #region Action (Communication Private)

                new()
                {
                    Id = new Guid("6475655d-cb05-4301-94a5-591a23bc4c78"),
                    Alias = "uint64_user_communication_private_power",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },
                new()
                {
                    Id = new Guid("de10a7db-682f-4c0d-a3c0-482444bffae1"),
                    Alias = "uint64_user_communication_private_power_needed",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },
                new()
                {
                    Id = new Guid("4f03b155-7102-4d8a-8d24-fc5a865aee6d"),
                    Alias = "uint64_user_communication_private_own_power_needed",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },

                #endregion

                #region Action (Communication Global)

                new()
                {
                    Id = new Guid("553ae154-3635-462f-94d0-09dc9a758bcf"),
                    Alias = "uint64_user_communication_global_power",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },

                #endregion

                #region Action (Communication Poke)

                new()
                {
                    Id = new Guid("f099bbd3-cf54-49e8-a1d4-1ccd693253e9"),
                    Alias = "uint64_user_poke_power",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },
                new()
                {
                    Id = new Guid("a651fac7-21d1-44b4-9e1f-2f3e7cb5bcd4"),
                    Alias = "uint64_user_poke_power_needed",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },

                #endregion

                #region Complaints

                new()
                {
                    Id = new Guid("3b763e71-ed35-49d9-84bd-146eb5b56081"),
                    Alias = "uint64_user_create_complaints_power",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },
                new()
                {
                    Id = new Guid("4dea4e67-de87-4e1a-af10-1eeb6ac8d794"),
                    Alias = "uint64_user_create_complaints_power_needed",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },
                new()
                {
                    Id = new Guid("bf3f4a6e-00ec-4050-a073-6b89fe0de2c6"),
                    Alias = "boolean_user_view_complaints",
                    Type = PermissionType.Boolean,
                    CompareMode = PermissionCompareMode.Equal
                },
                new()
                {
                    Id = new Guid("7c062589-5c65-448a-a5e8-436993da90ae"),
                    Alias = "boolean_user_view_complaints_own",
                    Type = PermissionType.Boolean,
                    CompareMode = PermissionCompareMode.Equal
                },
                new()
                {
                    Id = new Guid("c2aafd3f-d204-4d3c-b341-8a8c2dfb5fad"),
                    Alias = "uint64_user_modify_complaints_power",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },
                new()
                {
                    Id = new Guid("99da0346-5865-4cdb-81f2-af4269768fd6"),
                    Alias = "uint64_user_modify_complaints_power_needed",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },
                new()
                {
                    Id = new Guid("50013ef0-e524-489a-a3b9-06591992d02a"),
                    Alias = "uint64_user_modify_complaints_own_power_needed",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },

                #endregion

                #region Bans

                new()
                {
                    Id = new Guid("39c6fb5d-7544-4512-a0b9-d77b784a05db"),
                    Alias = "uint64_user_create_bans_power",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },
                new()
                {
                    Id = new Guid("34a7edff-153f-418a-b5f9-c6c90d45d526"),
                    Alias = "uint64_user_create_bans_power_needed",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },
                new()
                {
                    Id = new Guid("0c8db7fa-2f38-404a-8193-78b350f638e3"),
                    Alias = "boolean_user_view_bans",
                    Type = PermissionType.Boolean,
                    CompareMode = PermissionCompareMode.Equal
                },
                new()
                {
                    Id = new Guid("902ae5d0-4930-43a2-9ea7-fa5999957786"),
                    Alias = "boolean_user_view_bans_own",
                    Type = PermissionType.Boolean,
                    CompareMode = PermissionCompareMode.Equal
                },
                new()
                {
                    Id = new Guid("877c5a27-c6fe-4354-b935-4416a786c5f4"),
                    Alias = "uint64_user_modify_bans_power",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },
                new()
                {
                    Id = new Guid("40eaaf64-d3df-44e2-b909-314e967931d9"),
                    Alias = "uint64_user_modify_bans_power_needed",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },
                new()
                {
                    Id = new Guid("f4dcd6d7-1c62-4c29-94ed-7cc55afdfb9c"),
                    Alias = "uint64_user_modify_bans_own_power_needed",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },
                new()
                {
                    Id = new Guid("a378650d-2281-4232-a1bc-798fa8a8914a"),
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
                    Id = new Guid("86ce307c-f9db-4a5a-9347-8a3ea7ef2442"),
                    Alias = "uint64_userprofile_modify_power",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },
                new()
                {
                    Id = new Guid("fd8bb6bc-5dc7-4cfd-be3c-7ff4dff6ae9b"),
                    Alias = "uint64_userprofile_modify_power_needed",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },
                new()
                {
                    Id = new Guid("0a23f40b-a3d5-4f40-a6df-42d9070c00a6"),
                    Alias = "uint64_userprofile_modify_own_power_needed",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },

                #endregion
                
                new()
                {
                    Id = new Guid("8c49fb84-818b-42bf-8a0d-05d827e97db2"),
                    Alias = "uint64_userprofile_avatar_maxfilesize",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },

                #endregion

                #region File

                #region Create

                new()
                {
                    Id = new Guid("22501ebc-5ebc-42a1-b07e-967b0fbed171"),
                    Alias = "uint64_file_create_power",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },
                new()
                {
                    Id = new Guid("e609217b-01be-4e28-85cc-001ee5a211ca"),
                    Alias = "uint64_file_create_power_needed_system",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },

                #region FileCreate->File AutoMapper Permissions

                new()
                {
                    Id = new Guid("6fc92a20-2405-45e3-95e5-234642d49221"),
                    Alias = "uint64_filecreate_automap_file.agerating_power",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },
                new()
                {
                    Id = new Guid("dc98ca5c-9e8b-49c7-bf95-6c7664115fc8"),
                    Alias = "uint64_filecreate_automap_file.agerating_power_needed",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },
                new()
                {
                    Id = new Guid("b7ca31fc-6062-43dd-bf25-2526daeca769"),
                    Alias = "uint64_filecreate_automap_file.agerating_power_needed_system",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },

                #endregion

                #endregion

                #region Read

                new()
                {
                    Id = new Guid("49ef7d3b-3d35-45e5-9995-6d4920413a8b"),
                    Alias = "uint64_file_read_power",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },
                new()
                {
                    Id = new Guid("b89b5856-18dd-49c7-9295-26927214276c"),
                    Alias = "uint64_file_read_power_needed",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },
                new()
                {
                    Id = new Guid("2f59c415-94bb-435b-837f-9b61f33a8723"),
                    Alias = "uint64_file_read_own_power_needed",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },
                new()
                {
                    Id = new Guid("d4e32d88-643c-4a34-842a-fb3b8ab502cd"),
                    Alias = "uint64_file_read_power_needed_system",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },

                #endregion

                #region Modify

                new()
                {
                    Id = new Guid("0ba51d69-ce7a-4969-b072-d5000229b8fb"),
                    Alias = "uint64_file_modify_power",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },
                new()
                {
                    Id = new Guid("cf05b493-8bb2-4a3f-a467-a073720c5d46"),
                    Alias = "uint64_file_modify_power_needed",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },
                new()
                {
                    Id = new Guid("5ee1fd85-4a95-4409-a0d8-96da8ccf855b"),
                    Alias = "uint64_file_modify_own_power_needed",
                    Type = PermissionType.UInt64,
                    CompareMode = PermissionCompareMode.GreaterOrEqual
                },
                new()
                {
                    Id = new Guid("780ebed2-c70e-43f0-95aa-1fd336d170b2"),
                    Alias = "uint64_file_modify_power_needed_system",
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
                    Id = new Guid("55119e40-f094-4560-877f-42d18ff197db"),
                    Alias = "Root",
                    Description = "System user group with root like permission set. Also used to store system wise permission values",
                    IsSystem = true
                },
                new()
                {
                    Id = new Guid("3a95ab80-ac54-4e23-a35b-aaa6ca726523"),
                    Alias = "Banned",
                    Description = "System user group with banned like permission set",
                    IsSystem = true
                },
                new()
                {
                    Id = new Guid("93998585-5a67-4a4e-ad2d-f29a4d080e98"),
                    Alias = "Member",
                    Description = "System user group with member like permission set",
                    IsSystem = true
                }
            };

            var rootUserGroup = userGroups.First(__ => __.Alias == "Root");
            var memberUserGroup = userGroups.First(__ => __.Alias == "Member");

            appDbContextSeedLists.UserGroups.AddRange(userGroups);

            #endregion

            #region UserGroupsPermissionValues

            var rootUserGroupPower = Consts.RootUserGroupPowerBase;
            var memberUserGroupPower = Consts.MemberUserGroupPowerBase;

            var rootUserGroupPermissionValues = new List<UserGroupPermissionValue>()
            {
                #region Any

                new()
                {
                    Id = new Guid("3bf0040b-760f-44f4-a156-2ebab1ab9388"),
                    Value = BitConverter.GetBytes(true),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "boolean_any_view_permission_overview").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = new Guid("a75fb562-4c8a-45d8-aedb-ece9ca118463"),
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
                    Id = new Guid("6a6a66fa-a645-45ce-80c9-974790d00910"),
                    Value = BitConverter.GetBytes(true),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "boolean_group_create_groups").Id,
                    EntityId = rootUserGroup.Id,
                },

                #endregion

                #region Read

                new()
                {
                    Id = new Guid("b225634d-bfeb-4524-8a09-b831cb045a4c"),
                    Value = BitConverter.GetBytes(rootUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_group_view_groups_power").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = new Guid("212b2344-3d5a-444d-9d73-fe5831bbe15c"),
                    Value = BitConverter.GetBytes(rootUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_group_view_groups_power_needed").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = new Guid("79d4fbba-1f23-4f06-b3b5-366253db224f"),
                    Value = BitConverter.GetBytes(rootUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_group_view_groups_own_power_needed").Id,
                    EntityId = rootUserGroup.Id,
                },

                #endregion

                #region Modify

                new()
                {
                    Id = new Guid("3c7aff82-af45-4400-a0c1-192281133a52"),
                    Value = BitConverter.GetBytes(rootUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_group_modify_groups_power").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = new Guid("c921ba36-09fa-4c4a-8e6e-b772567beadb"),
                    Value = BitConverter.GetBytes(rootUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_group_modify_groups_power_needed").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = new Guid("61c3fc90-5842-444d-a5c9-926e0535f658"),
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
                    Id = new Guid("9c4874bc-442d-4e7d-bbcc-db6df5db7e5b"),
                    Value = BitConverter.GetBytes(rootUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_view_email_power").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = new Guid("f178eb73-db24-4fdd-bb3c-9946c3ae05fa"),
                    Value = BitConverter.GetBytes(rootUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_view_email_power_needed").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = new Guid("4d8ee357-770d-4549-a3e7-7d2832c81536"),
                    Value = BitConverter.GetBytes(rootUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_view_email_own_power_needed").Id,
                    EntityId = rootUserGroup.Id,
                },

                #endregion

                #region Modify (Email)

                new()
                {
                    Id = new Guid("97774901-3608-4085-bd91-004dc75e564e"),
                    Value = BitConverter.GetBytes(rootUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_modify_email_power").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = new Guid("ce38cd9b-25df-4d74-a9b4-c7fab92fa042"),
                    Value = BitConverter.GetBytes(rootUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_modify_email_power_needed").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = new Guid("d3f47959-e427-453c-9343-78a3a55fb89c"),
                    Value = BitConverter.GetBytes(rootUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_modify_email_own_power_needed").Id,
                    EntityId = rootUserGroup.Id,
                },

                #endregion

                #region Read (LastIpAddress)

                new()
                {
                    Id = new Guid("66afc1f1-167d-4b05-8bc7-3572d2f6361f"),
                    Value = BitConverter.GetBytes(rootUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64t_user_view_lastipaddress_power").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = new Guid("257082df-0fb3-4960-8c58-78bcec07b384"),
                    Value = BitConverter.GetBytes(rootUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64t_user_view_lastipaddress_power_needed").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = new Guid("f38b3ba0-7263-4f97-95f6-ab77f1cbfc83"),
                    Value = BitConverter.GetBytes(rootUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64t_user_view_lastipaddress_own_power_needed").Id,
                    EntityId = rootUserGroup.Id,
                },

                #endregion

                #region Delete

                new()
                {
                    Id = new Guid("54109a14-922d-4b4f-9f3d-89a157beba00"),
                    Value = BitConverter.GetBytes(rootUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_delete_user_power").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = new Guid("c43de164-1008-4c59-ae77-efac0e4d4a75"),
                    Value = BitConverter.GetBytes(rootUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_delete_user_power_needed").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = new Guid("b08f108a-7cc0-4f79-9338-7bd29aae4ba9"),
                    Value = BitConverter.GetBytes(rootUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_delete_user_own_power_needed").Id,
                    EntityId = rootUserGroup.Id,
                },

                #endregion

                #region Action (Communication Private)

                new()
                {
                    Id = new Guid("1013e585-de2c-414d-966d-3a328473c71f"),
                    Value = BitConverter.GetBytes(rootUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_communication_private_power").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = new Guid("c75603c0-5cc0-40d6-8c05-4beb03fe8309"),
                    Value = BitConverter.GetBytes(rootUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_communication_private_power_needed").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = new Guid("1393cacb-1c3d-4886-92c2-f2a988fe25d0"),
                    Value = BitConverter.GetBytes(rootUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_communication_private_own_power_needed").Id,
                    EntityId = rootUserGroup.Id,
                },

                #endregion

                #region Action (Communication Global)

                new()
                {
                    Id = new Guid("643fb07b-28bf-42a2-8887-80154620c534"),
                    Value = BitConverter.GetBytes(rootUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_communication_global_power").Id,
                    EntityId = rootUserGroup.Id,
                },

                #endregion

                #region Action (Communication Poke)

                new()
                {
                    Id = new Guid("74be4f97-bcb8-4ce3-8620-32f0bfcaa9de"),
                    Value = BitConverter.GetBytes(rootUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_poke_power").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = new Guid("179700ef-c8c9-458f-a6ce-62fdf94a8dfa"),
                    Value = BitConverter.GetBytes(rootUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_poke_power_needed").Id,
                    EntityId = rootUserGroup.Id,
                },

                #endregion

                #region Complaints

                new()
                {
                    Id = new Guid("447abf01-3395-428d-bac8-af2b5cde20de"),
                    Value = BitConverter.GetBytes(rootUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_create_complaints_power").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = new Guid("54d527d9-c773-4835-b63f-9e9c295be285"),
                    Value = BitConverter.GetBytes(rootUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_create_complaints_power_needed").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = new Guid("2a6a1403-0982-4054-b584-0b998bff20a1"),
                    Value = BitConverter.GetBytes(true),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "boolean_user_view_complaints").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = new Guid("378d2e0f-9262-4ae2-96fa-6bd3b74703c5"),
                    Value = BitConverter.GetBytes(true),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "boolean_user_view_complaints_own").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = new Guid("3970a981-7978-41a9-8d58-42420177c9c0"),
                    Value = BitConverter.GetBytes(rootUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_modify_complaints_power").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = new Guid("7952f688-3ee9-4cf0-87a4-1819f0079dae"),
                    Value = BitConverter.GetBytes(rootUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_modify_complaints_power_needed").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = new Guid("9768dcbd-f0b6-4e92-a1fb-411d461d8d11"),
                    Value = BitConverter.GetBytes(rootUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_modify_complaints_own_power_needed").Id,
                    EntityId = rootUserGroup.Id,
                },

                #endregion

                #region Bans

                new()
                {
                    Id = new Guid("8dde5cbb-a050-4215-abd4-236580c7cc6a"),
                    Value = BitConverter.GetBytes(rootUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_create_bans_power").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = new Guid("3b8b366f-a27e-4eb4-a5c0-7f47fdb98393"),
                    Value = BitConverter.GetBytes(rootUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_create_bans_power_needed").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = new Guid("9316bbf5-2198-438c-8304-bc1f7b128e49"),
                    Value = BitConverter.GetBytes(true),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "boolean_user_view_bans").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = new Guid("cb1d1d36-083a-453c-9272-1f07dbd1b692"),
                    Value = BitConverter.GetBytes(true),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "boolean_user_view_bans_own").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = new Guid("8533f3cb-c97b-4ca1-8a88-0c9f1b78b657"),
                    Value = BitConverter.GetBytes(rootUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_modify_bans_power").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = new Guid("430384de-f18d-4cd8-9717-3b79fc1c2d98"),
                    Value = BitConverter.GetBytes(rootUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_modify_bans_power_needed").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = new Guid("3e41f74e-1849-4fe4-a847-54e286098056"),
                    Value = BitConverter.GetBytes(rootUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_modify_bans_own_power_needed").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = new Guid("59b0c74c-2366-4785-9132-a86a34ff6df6"),
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
                    Id = new Guid("166b3b84-3fdd-4f9c-a6a4-8cfa9dc77d83"),
                    Value = BitConverter.GetBytes(rootUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_userprofile_modify_power").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = new Guid("05d6487f-75bc-4a56-b758-65b40ff895c1"),
                    Value = BitConverter.GetBytes(rootUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_userprofile_modify_power_needed").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = new Guid("9ab16611-874d-4cc9-bca7-ccaf529b707b"),
                    Value = BitConverter.GetBytes(rootUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_userprofile_modify_own_power_needed").Id,
                    EntityId = rootUserGroup.Id,
                },

                #endregion

                new()
                {
                    Id = new Guid("401f2a60-e4af-49d4-b549-992b831ed1c6"),
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
                    Id = new Guid("ace1653d-2a97-44a7-b61b-9d8a9ca4ed9f"),
                    Value = BitConverter.GetBytes(rootUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_file_create_power").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = new Guid("107a4d24-6398-40c2-a23b-f7a9c25c07ce"),
                    Value = BitConverter.GetBytes(memberUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_file_create_power_needed_system").Id,
                    EntityId = rootUserGroup.Id,
                },
                
                #region FileCreate->File AutoMapper Permissions
                
                new()
                {
                    Id = new Guid("bde4867e-d661-4ac8-9c92-f2a89b77e153"),
                    Value = BitConverter.GetBytes(rootUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_filecreate_automap_file.agerating_power").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = new Guid("8790dd27-c5fc-401d-b385-87741e804e82"),
                    Value = BitConverter.GetBytes(memberUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_filecreate_automap_file.agerating_power_needed_system").Id,
                    EntityId = rootUserGroup.Id,
                },

                #endregion

                #endregion

                #region Read

                new()
                {
                    Id = new Guid("9d2e3031-3685-46cd-9df2-fcbfd4ca5d24"),
                    Value = BitConverter.GetBytes(rootUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_file_read_power").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = new Guid("f3620066-96ed-4dc2-9257-e490dc25f535"),
                    Value = BitConverter.GetBytes(rootUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_file_read_power_needed").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = new Guid("ee1c3b82-e8bc-46f0-932b-469c112c9ac3"),
                    Value = BitConverter.GetBytes(rootUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_file_read_own_power_needed").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = new Guid("1f993cfb-d8cf-4c6b-a524-bf3a5f75db38"),
                    Value = BitConverter.GetBytes(memberUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_file_read_power_needed_system").Id,
                    EntityId = rootUserGroup.Id,
                },

                #endregion

                #region Modify

                new()
                {
                    Id = new Guid("fbe17613-1648-42e2-9baf-3bb65af51c08"),
                    Value = BitConverter.GetBytes(rootUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_file_modify_power").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = new Guid("acfe9bec-d564-4132-ac16-c26f689b31fc"),
                    Value = BitConverter.GetBytes(rootUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_file_modify_power_needed").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = new Guid("a4a73121-cab4-4d45-92cd-45e56ade0281"),
                    Value = BitConverter.GetBytes(rootUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_file_modify_own_power_needed").Id,
                    EntityId = rootUserGroup.Id,
                },
                new()
                {
                    Id = new Guid("37561812-21e3-46ea-9396-70c9a27ff60c"),
                    Value = BitConverter.GetBytes(memberUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_file_modify_power_needed_system").Id,
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
                    Id = new Guid("b3504423-45d7-4661-9707-584c39f5b87f"),
                    Value = BitConverter.GetBytes(true),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "boolean_any_view_permission_overview").Id,
                    EntityId = memberUserGroup.Id,
                },
                new()
                {
                    Id = new Guid("7426839d-4b12-45c9-b2dc-a15c19550cf7"),
                    Value = BitConverter.GetBytes(0),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_any_modify_permission_power").Id,
                    EntityId = memberUserGroup.Id,
                },

                #endregion

                #region Group

                #region Create

                new()
                {
                    Id = new Guid("16491944-fff6-4289-8184-7420ea8c13d6"),
                    Value = BitConverter.GetBytes(false),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "boolean_group_create_groups").Id,
                    EntityId = memberUserGroup.Id,
                },

                #endregion

                #region Read

                new()
                {
                    Id = new Guid("d1c54a20-ced1-4c3a-8e33-fa583b999fcf"),
                    Value = BitConverter.GetBytes(memberUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_group_view_groups_power").Id,
                    EntityId = memberUserGroup.Id,
                },
                new()
                {
                    Id = new Guid("e140a3bf-9a80-4ae2-bf92-b5b5ecdcbac3"),
                    Value = BitConverter.GetBytes(memberUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_group_view_groups_power_needed").Id,
                    EntityId = memberUserGroup.Id,
                },
                new()
                {
                    Id = new Guid("9fccdf13-5c1c-4ed4-bb38-48c1af5e5c94"),
                    Value = BitConverter.GetBytes(memberUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_group_view_groups_own_power_needed").Id,
                    EntityId = memberUserGroup.Id,
                },

                #endregion

                #region Modify

                new()
                {
                    Id = new Guid("54a8aca3-ed24-448a-bb12-a7ae9f27cb90"),
                    Value = BitConverter.GetBytes(memberUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_group_modify_groups_power").Id,
                    EntityId = memberUserGroup.Id,
                },
                new()
                {
                    Id = new Guid("a6d5e4a5-65a1-4061-b862-bccd19658d2e"),
                    Value = BitConverter.GetBytes(rootUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_group_modify_groups_power_needed").Id,
                    EntityId = memberUserGroup.Id,
                },
                new()
                {
                    Id = new Guid("58d7abfe-6b7e-4195-907e-2918a9bba11b"),
                    Value = BitConverter.GetBytes(rootUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_group_modify_groups_own_power_needed").Id,
                    EntityId = memberUserGroup.Id,
                },

                #endregion

                #endregion

                #region User

                #region Read (Email)

                new()
                {
                    Id = new Guid("4db37ad7-80f1-4c8c-ae4e-af58e7d7a8c2"),
                    Value = BitConverter.GetBytes(memberUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_view_email_power").Id,
                    EntityId = memberUserGroup.Id,
                },
                new()
                {
                    Id = new Guid("dcd6f699-bd1f-4e32-af3f-1b7b8b4e7f3a"),
                    Value = BitConverter.GetBytes(rootUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_view_email_power_needed").Id,
                    EntityId = memberUserGroup.Id,
                },
                new()
                {
                    Id = new Guid("18cc3f9a-a616-48ef-a32a-fa7a4a09fa73"),
                    Value = BitConverter.GetBytes(memberUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_view_email_own_power_needed").Id,
                    EntityId = memberUserGroup.Id,
                },

                #endregion

                #region Modify (Email)

                new()
                {
                    Id = new Guid("bc128d54-75c3-4569-a114-8bfc9cf1d621"),
                    Value = BitConverter.GetBytes(memberUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_modify_email_power").Id,
                    EntityId = memberUserGroup.Id,
                },
                new()
                {
                    Id = new Guid("eb66b927-01ff-4c8e-9b36-9800cb489147"),
                    Value = BitConverter.GetBytes(rootUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_modify_email_power_needed").Id,
                    EntityId = memberUserGroup.Id,
                },
                new()
                {
                    Id = new Guid("0a974f90-e0b2-45e1-bc19-da396f905fbc"),
                    Value = BitConverter.GetBytes(memberUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_modify_email_own_power_needed").Id,
                    EntityId = memberUserGroup.Id,
                },

                #endregion

                #region Read (LastIpAddress)

                new()
                {
                    Id = new Guid("cc95e57a-b4e8-4372-adce-e0537991d27a"),
                    Value = BitConverter.GetBytes(memberUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64t_user_view_lastipaddress_power").Id,
                    EntityId = memberUserGroup.Id,
                },
                new()
                {
                    Id = new Guid("6f00ca28-175d-444e-aaa8-06c6409f407b"),
                    Value = BitConverter.GetBytes(rootUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64t_user_view_lastipaddress_power_needed").Id,
                    EntityId = memberUserGroup.Id,
                },
                new()
                {
                    Id = new Guid("c3e65d57-9ee5-45d3-bfc2-f0c7f47a5572"),
                    Value = BitConverter.GetBytes(memberUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64t_user_view_lastipaddress_own_power_needed").Id,
                    EntityId = memberUserGroup.Id,
                },

                #endregion

                #region Delete

                new()
                {
                    Id = new Guid("1a7d0103-35de-4707-bb68-59a6c77ea7f1"),
                    Value = BitConverter.GetBytes(memberUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_delete_user_power").Id,
                    EntityId = memberUserGroup.Id,
                },
                new()
                {
                    Id = new Guid("13441904-635b-4919-bb28-cee6a2d8e845"),
                    Value = BitConverter.GetBytes(rootUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_delete_user_power_needed").Id,
                    EntityId = memberUserGroup.Id,
                },
                new()
                {
                    Id = new Guid("e21b99b9-69cc-4a05-96fd-578ff4f2e6c4"),
                    Value = BitConverter.GetBytes(memberUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_delete_user_own_power_needed").Id,
                    EntityId = memberUserGroup.Id,
                },

                #endregion

                #region Action (Communication Private)

                new()
                {
                    Id = new Guid("ba477dc5-f0b5-4405-81c4-74f0000139d9"),
                    Value = BitConverter.GetBytes(memberUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_communication_private_power").Id,
                    EntityId = memberUserGroup.Id,
                },
                new()
                {
                    Id = new Guid("b46d829f-65d1-4111-a444-50535b23687c"),
                    Value = BitConverter.GetBytes(memberUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_communication_private_power_needed").Id,
                    EntityId = memberUserGroup.Id,
                },
                new()
                {
                    Id = new Guid("cad02bb3-c32f-4774-b68a-83dcd57a96d9"),
                    Value = BitConverter.GetBytes(memberUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_communication_private_own_power_needed").Id,
                    EntityId = memberUserGroup.Id,
                },

                #endregion

                #region Action (Communication Global)

                new()
                {
                    Id = new Guid("9b28929f-23f9-48b8-b777-d4c0a2f51ccf"),
                    Value = BitConverter.GetBytes(memberUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_communication_global_power").Id,
                    EntityId = memberUserGroup.Id,
                },

                #endregion

                #region Action (Communication Poke)

                new()
                {
                    Id = new Guid("9711f54c-9cbd-4dc3-b178-105c391645e9"),
                    Value = BitConverter.GetBytes(memberUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_poke_power").Id,
                    EntityId = memberUserGroup.Id,
                },
                new()
                {
                    Id = new Guid("1e075684-44e2-4bfa-b396-7717aa249d09"),
                    Value = BitConverter.GetBytes(memberUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_poke_power_needed").Id,
                    EntityId = memberUserGroup.Id,
                },

                #endregion

                #region Complaints

                new()
                {
                    Id = new Guid("c7a1908a-e9e0-40da-a60f-9863ee3e13e1"),
                    Value = BitConverter.GetBytes(memberUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_create_complaints_power").Id,
                    EntityId = memberUserGroup.Id,
                },
                new()
                {
                    Id = new Guid("4b4002bc-fa13-4434-b603-4a87529ea758"),
                    Value = BitConverter.GetBytes(memberUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_create_complaints_power_needed").Id,
                    EntityId = memberUserGroup.Id,
                },
                new()
                {
                    Id = new Guid("f975aff2-4507-4778-851b-e0d111228787"),
                    Value = BitConverter.GetBytes(false),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "boolean_user_view_complaints").Id,
                    EntityId = memberUserGroup.Id,
                },
                new()
                {
                    Id = new Guid("ca2aea3a-a161-4493-b77d-c0b15188b912"),
                    Value = BitConverter.GetBytes(true),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "boolean_user_view_complaints_own").Id,
                    EntityId = memberUserGroup.Id,
                },
                new()
                {
                    Id = new Guid("449ca1bc-07a3-4470-a84b-f38aadab5af6"),
                    Value = BitConverter.GetBytes(memberUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_modify_complaints_power").Id,
                    EntityId = memberUserGroup.Id,
                },
                new()
                {
                    Id = new Guid("cac0cb1d-6aa5-45b8-b512-be78d341b475"),
                    Value = BitConverter.GetBytes(rootUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_modify_complaints_power_needed").Id,
                    EntityId = memberUserGroup.Id,
                },
                new()
                {
                    Id = new Guid("a2eb095a-d327-4a0f-bda9-8ededac9e944"),
                    Value = BitConverter.GetBytes(memberUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_modify_complaints_own_power_needed").Id,
                    EntityId = memberUserGroup.Id,
                },

                #endregion

                #region Bans

                new()
                {
                    Id = new Guid("bec9bc10-b315-473f-8026-17a417acc641"),
                    Value = BitConverter.GetBytes(0),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_create_bans_power").Id,
                    EntityId = memberUserGroup.Id,
                },
                new()
                {
                    Id = new Guid("0c0ff1b2-3040-410c-8854-cede0d46d3e6"),
                    Value = BitConverter.GetBytes(rootUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_create_bans_power_needed").Id,
                    EntityId = memberUserGroup.Id,
                },
                new()
                {
                    Id = new Guid("ee8ff8e7-b228-461d-8ab0-7d7db1f25958"),
                    Value = BitConverter.GetBytes(false),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "boolean_user_view_bans").Id,
                    EntityId = memberUserGroup.Id,
                },
                new()
                {
                    Id = new Guid("f1b3fd5b-8dde-4f11-a3c4-cf6ccf0e8b57"),
                    Value = BitConverter.GetBytes(false),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "boolean_user_view_bans_own").Id,
                    EntityId = memberUserGroup.Id,
                },
                new()
                {
                    Id = new Guid("6ea85aa4-3007-4b99-8655-f00e25f78834"),
                    Value = BitConverter.GetBytes(0),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_modify_bans_power").Id,
                    EntityId = memberUserGroup.Id,
                },
                new()
                {
                    Id = new Guid("39112117-413b-4a68-b54a-880eb5e04223"),
                    Value = BitConverter.GetBytes(rootUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_modify_bans_power_needed").Id,
                    EntityId = memberUserGroup.Id,
                },
                new()
                {
                    Id = new Guid("aee4a75b-6fbb-49d6-82c2-2eb450401118"),
                    Value = BitConverter.GetBytes(rootUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_modify_bans_own_power_needed").Id,
                    EntityId = memberUserGroup.Id,
                },
                new()
                {
                    Id = new Guid("83c284b0-4724-4855-99f1-c3254f473eaf"),
                    Value = BitConverter.GetBytes(ulong.MaxValue),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_user_max_ban_time").Id,
                    EntityId = memberUserGroup.Id,
                },

                #endregion

                #endregion

                #region User Profile

                #region Modify

                new()
                {
                    Id = new Guid("1746ae11-d778-4f24-ad36-367f10c88cd8"),
                    Value = BitConverter.GetBytes(memberUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_userprofile_modify_power").Id,
                    EntityId = memberUserGroup.Id,
                },
                new()
                {
                    Id = new Guid("e3023bb9-a9c7-4744-80bc-2e4b06caef7c"),
                    Value = BitConverter.GetBytes(rootUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_userprofile_modify_power_needed").Id,
                    EntityId = memberUserGroup.Id,
                },
                new()
                {
                    Id = new Guid("12e52256-2b33-4791-91cc-443f493469d5"),
                    Value = BitConverter.GetBytes(memberUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_userprofile_modify_own_power_needed").Id,
                    EntityId = memberUserGroup.Id,
                },

                #endregion

                new()
                {
                    Id = new Guid("cd0488cb-370e-41e0-aad8-120f4d86c8db"),
                    Value = BitConverter.GetBytes(0),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_userprofile_avatar_maxfilesize").Id,
                    EntityId = memberUserGroup.Id,
                },

                #endregion

                #region File

                #region Create

                new()
                {
                    Id = new Guid("47656724-47d2-43e0-bd58-b957d2aa2f74"),
                    Value = BitConverter.GetBytes(memberUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_file_create_power").Id,
                    EntityId = memberUserGroup.Id,
                },
                
                #region FileCreate->File AutoMapper Permissions
                
                new()
                {
                    Id = new Guid("2cb0dc86-7027-4739-a5c2-25f7fa7755bf"),
                    Value = BitConverter.GetBytes(memberUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_filecreate_automap_file.agerating_power").Id,
                    EntityId = memberUserGroup.Id,
                },

                #endregion

                #endregion

                #region Read

                new()
                {
                    Id = new Guid("c0edebd2-fa11-4882-b411-682588267b22"),
                    Value = BitConverter.GetBytes(memberUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_file_read_power").Id,
                    EntityId = memberUserGroup.Id,
                },
                new()
                {
                    Id = new Guid("61f80fbf-80d7-441b-b5fc-c54feda34f73"),
                    Value = BitConverter.GetBytes(rootUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_file_read_power_needed").Id,
                    EntityId = memberUserGroup.Id,
                },
                new()
                {
                    Id = new Guid("e3c2969f-55b3-471e-a928-0e3880b93d70"),
                    Value = BitConverter.GetBytes(memberUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_file_read_own_power_needed").Id,
                    EntityId = memberUserGroup.Id,
                },

                #endregion

                #region Modify

                new()
                {
                    Id = new Guid("5cc0e746-6308-4780-8022-602623f01a16"),
                    Value = BitConverter.GetBytes(memberUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_file_modify_power").Id,
                    EntityId = memberUserGroup.Id,
                },
                new()
                {
                    Id = new Guid("189eb982-312c-4740-9c08-f990e1eb97f9"),
                    Value = BitConverter.GetBytes(rootUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_file_modify_power_needed").Id,
                    EntityId = memberUserGroup.Id,
                },
                new()
                {
                    Id = new Guid("b1b5cda7-bed3-4eed-84ec-ca36787e9b77"),
                    Value = BitConverter.GetBytes(memberUserGroupPower),
                    Grant = rootUserGroupPower,
                    PermissionId = permissions.First(_ => _.Alias == "uint64_file_modify_own_power_needed").Id,
                    EntityId = memberUserGroup.Id,
                },

                #endregion

                #endregion
            };

            appDbContextSeedLists.UserGroupPermissionValues.AddRange(rootUserGroupPermissionValues);
            appDbContextSeedLists.UserGroupPermissionValues.AddRange(memberUserGroupPermissionValues);

            #endregion

            #region UserToUserGroupMappings
            
            var userToUserGroupMappings = new List<UserToUserGroupMapping>();

            appDbContextSeedLists.UserToUserGroupMappings.AddRange(userToUserGroupMappings);

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
                _.HasIndex(__ => new {
                    EntityId = __.EntityLeftId,
                    GroupId = __.EntityRightId}).IsUnique();

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