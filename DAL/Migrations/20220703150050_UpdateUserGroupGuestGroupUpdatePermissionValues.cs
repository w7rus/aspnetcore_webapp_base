using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    public partial class UpdateUserGroupGuestGroupUpdatePermissionValues : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "PermissionValues",
                columns: new[] { "Id", "CreatedAt", "EntityId", "PermissionId", "UpdatedAt", "UserGroupId", "Value" },
                values: new object[,]
                {
                    { new Guid("0831b98e-6a6b-45ee-bf0a-903cd76dcbf4"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new Guid("b26a9112-211b-462f-bd41-8f38a3568106"), new Guid("03fa77db-bb4a-4b3f-9ae0-6883b046ef5e"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, new byte[] { 255, 255, 255, 255, 255, 255, 255, 127 } },
                    { new Guid("2533fbbb-6818-4f34-9dfc-8137457d7c34"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new Guid("b26a9112-211b-462f-bd41-8f38a3568106"), new Guid("5a797b63-266a-4e68-89a3-9a476b84042e"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, new byte[] { 255, 255, 255, 255, 255, 255, 255, 127 } },
                    { new Guid("4901bd1b-5a65-4c7d-a139-3022ed27c47e"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new Guid("b26a9112-211b-462f-bd41-8f38a3568106"), new Guid("ba27392f-ae6a-4c16-8041-e6f1af43f498"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, new byte[] { 255, 255, 255, 255, 255, 255, 255, 127 } },
                    { new Guid("77044d65-81e2-41af-9804-7deee132cb45"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new Guid("b26a9112-211b-462f-bd41-8f38a3568106"), new Guid("f581ad76-dbcf-49ab-ab56-eed2f05e8dff"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, new byte[] { 255, 255, 255, 255, 255, 255, 255, 127 } },
                    { new Guid("cf2c8263-fba4-4e51-b766-7cf2346de2a3"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new Guid("b26a9112-211b-462f-bd41-8f38a3568106"), new Guid("08b4b605-8297-44a9-b47a-1b285d98219c"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, new byte[] { 255, 255, 255, 255, 255, 255, 255, 127 } },
                    { new Guid("dd1b04b8-c02b-4b94-8097-b06ede4bba56"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new Guid("b26a9112-211b-462f-bd41-8f38a3568106"), new Guid("73e04e4d-4d96-4a1b-9e26-ba8ceb0562cc"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, new byte[] { 255, 255, 255, 255, 255, 255, 255, 127 } }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "PermissionValues",
                keyColumn: "Id",
                keyValue: new Guid("0831b98e-6a6b-45ee-bf0a-903cd76dcbf4"));

            migrationBuilder.DeleteData(
                table: "PermissionValues",
                keyColumn: "Id",
                keyValue: new Guid("2533fbbb-6818-4f34-9dfc-8137457d7c34"));

            migrationBuilder.DeleteData(
                table: "PermissionValues",
                keyColumn: "Id",
                keyValue: new Guid("4901bd1b-5a65-4c7d-a139-3022ed27c47e"));

            migrationBuilder.DeleteData(
                table: "PermissionValues",
                keyColumn: "Id",
                keyValue: new Guid("77044d65-81e2-41af-9804-7deee132cb45"));

            migrationBuilder.DeleteData(
                table: "PermissionValues",
                keyColumn: "Id",
                keyValue: new Guid("cf2c8263-fba4-4e51-b766-7cf2346de2a3"));

            migrationBuilder.DeleteData(
                table: "PermissionValues",
                keyColumn: "Id",
                keyValue: new Guid("dd1b04b8-c02b-4b94-8097-b06ede4bba56"));
        }
    }
}
