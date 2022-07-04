using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    public partial class AddPermissionValueUserGroupTransfer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "Alias", "CompareMode", "CreatedAt", "Type", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("2ab008f0-3df9-4a55-b60e-b8c893e3d91e"), "g_group_a_transfer_o_usergroup", 6, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("800a3f14-5f9f-4f4c-8e72-60030699464e"), "g_group_a_transfer_o_usergroup", 6, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 3, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("ee866899-80b0-4e1a-bf2b-78170a4d8aba"), "g_group_a_transfer_o_usergroup", 6, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 4, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) }
                });

            migrationBuilder.InsertData(
                table: "PermissionValues",
                columns: new[] { "Id", "CreatedAt", "EntityDiscriminator", "EntityId", "PermissionId", "UpdatedAt", "UserGroupId", "Value" },
                values: new object[,]
                {
                    { new Guid("09c3147c-b704-426e-814b-151c96930b0a"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "bb9d93888168454904aee93bab250267a8a3410dc8928602e757a4a411544654", new Guid("93998585-5a67-4a4e-ad2d-f29a4d080e98"), new Guid("2ab008f0-3df9-4a55-b60e-b8c893e3d91e"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, new byte[] { 50, 0, 0, 0, 0, 0, 0, 0 } },
                    { new Guid("3bb38d1b-9f82-47db-9814-82a80d5ef6c5"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "bb9d93888168454904aee93bab250267a8a3410dc8928602e757a4a411544654", new Guid("55119e40-f094-4560-877f-42d18ff197db"), new Guid("800a3f14-5f9f-4f4c-8e72-60030699464e"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, new byte[] { 255, 255, 255, 255, 255, 255, 255, 127 } },
                    { new Guid("3d7e89a6-fa22-4490-9dce-766e25ef19ad"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "bb9d93888168454904aee93bab250267a8a3410dc8928602e757a4a411544654", new Guid("3a95ab80-ac54-4e23-a35b-aaa6ca726523"), new Guid("ee866899-80b0-4e1a-bf2b-78170a4d8aba"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, new byte[] { 255, 255, 255, 255, 255, 255, 255, 127 } },
                    { new Guid("7d06386f-6111-4fb9-bcf7-60d9927a01ec"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "bb9d93888168454904aee93bab250267a8a3410dc8928602e757a4a411544654", new Guid("55119e40-f094-4560-877f-42d18ff197db"), new Guid("ee866899-80b0-4e1a-bf2b-78170a4d8aba"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, new byte[] { 255, 255, 255, 255, 255, 255, 255, 127 } },
                    { new Guid("82f04979-b39f-4b66-9dae-51e966521cf0"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "bb9d93888168454904aee93bab250267a8a3410dc8928602e757a4a411544654", new Guid("b26a9112-211b-462f-bd41-8f38a3568106"), new Guid("800a3f14-5f9f-4f4c-8e72-60030699464e"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, new byte[] { 255, 255, 255, 255, 255, 255, 255, 127 } },
                    { new Guid("a043b2a5-591a-451a-8734-8653ce509d9b"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "bb9d93888168454904aee93bab250267a8a3410dc8928602e757a4a411544654", new Guid("93998585-5a67-4a4e-ad2d-f29a4d080e98"), new Guid("ee866899-80b0-4e1a-bf2b-78170a4d8aba"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, new byte[] { 255, 255, 255, 255, 255, 255, 255, 127 } },
                    { new Guid("a5910ec7-f347-45bc-b3fe-d35e0275b160"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "bb9d93888168454904aee93bab250267a8a3410dc8928602e757a4a411544654", new Guid("55119e40-f094-4560-877f-42d18ff197db"), new Guid("2ab008f0-3df9-4a55-b60e-b8c893e3d91e"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, new byte[] { 255, 255, 255, 255, 255, 255, 255, 127 } },
                    { new Guid("c8760a60-c49a-4c7e-8e2f-240850e8cc5b"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "bb9d93888168454904aee93bab250267a8a3410dc8928602e757a4a411544654", new Guid("3a95ab80-ac54-4e23-a35b-aaa6ca726523"), new Guid("2ab008f0-3df9-4a55-b60e-b8c893e3d91e"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 } },
                    { new Guid("d01570f7-1a7b-4df2-94b0-689bccf41a91"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "bb9d93888168454904aee93bab250267a8a3410dc8928602e757a4a411544654", new Guid("93998585-5a67-4a4e-ad2d-f29a4d080e98"), new Guid("800a3f14-5f9f-4f4c-8e72-60030699464e"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, new byte[] { 50, 0, 0, 0, 0, 0, 0, 0 } },
                    { new Guid("e7e3a61d-1d7d-425d-9f70-e8773df18a94"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "bb9d93888168454904aee93bab250267a8a3410dc8928602e757a4a411544654", new Guid("b26a9112-211b-462f-bd41-8f38a3568106"), new Guid("ee866899-80b0-4e1a-bf2b-78170a4d8aba"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, new byte[] { 255, 255, 255, 255, 255, 255, 255, 127 } },
                    { new Guid("ea84bfd2-5c9f-4de1-8652-b9fabee09aeb"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "bb9d93888168454904aee93bab250267a8a3410dc8928602e757a4a411544654", new Guid("3a95ab80-ac54-4e23-a35b-aaa6ca726523"), new Guid("800a3f14-5f9f-4f4c-8e72-60030699464e"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, new byte[] { 255, 255, 255, 255, 255, 255, 255, 127 } }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "PermissionValues",
                keyColumn: "Id",
                keyValue: new Guid("09c3147c-b704-426e-814b-151c96930b0a"));

            migrationBuilder.DeleteData(
                table: "PermissionValues",
                keyColumn: "Id",
                keyValue: new Guid("3bb38d1b-9f82-47db-9814-82a80d5ef6c5"));

            migrationBuilder.DeleteData(
                table: "PermissionValues",
                keyColumn: "Id",
                keyValue: new Guid("3d7e89a6-fa22-4490-9dce-766e25ef19ad"));

            migrationBuilder.DeleteData(
                table: "PermissionValues",
                keyColumn: "Id",
                keyValue: new Guid("7d06386f-6111-4fb9-bcf7-60d9927a01ec"));

            migrationBuilder.DeleteData(
                table: "PermissionValues",
                keyColumn: "Id",
                keyValue: new Guid("82f04979-b39f-4b66-9dae-51e966521cf0"));

            migrationBuilder.DeleteData(
                table: "PermissionValues",
                keyColumn: "Id",
                keyValue: new Guid("a043b2a5-591a-451a-8734-8653ce509d9b"));

            migrationBuilder.DeleteData(
                table: "PermissionValues",
                keyColumn: "Id",
                keyValue: new Guid("a5910ec7-f347-45bc-b3fe-d35e0275b160"));

            migrationBuilder.DeleteData(
                table: "PermissionValues",
                keyColumn: "Id",
                keyValue: new Guid("c8760a60-c49a-4c7e-8e2f-240850e8cc5b"));

            migrationBuilder.DeleteData(
                table: "PermissionValues",
                keyColumn: "Id",
                keyValue: new Guid("d01570f7-1a7b-4df2-94b0-689bccf41a91"));

            migrationBuilder.DeleteData(
                table: "PermissionValues",
                keyColumn: "Id",
                keyValue: new Guid("e7e3a61d-1d7d-425d-9f70-e8773df18a94"));

            migrationBuilder.DeleteData(
                table: "PermissionValues",
                keyColumn: "Id",
                keyValue: new Guid("ea84bfd2-5c9f-4de1-8652-b9fabee09aeb"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("2ab008f0-3df9-4a55-b60e-b8c893e3d91e"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("800a3f14-5f9f-4f4c-8e72-60030699464e"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("ee866899-80b0-4e1a-bf2b-78170a4d8aba"));
        }
    }
}
