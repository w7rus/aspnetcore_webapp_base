using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    public partial class Add_FileCreate_File_Agerating_Mapping_Permissions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "Alias", "CompareMode", "CreatedAt", "Type", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("6fc92a20-2405-45e3-95e5-234642d49221"), "uint64_filecreate_automap_file.agerating_power", 6, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 9, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("b7ca31fc-6062-43dd-bf25-2526daeca769"), "uint64_filecreate_automap_file.agerating_power_needed_system", 6, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 9, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("dc98ca5c-9e8b-49c7-bf95-6c7664115fc8"), "uint64_filecreate_automap_file.agerating_power_needed", 6, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 9, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) }
                });

            migrationBuilder.InsertData(
                table: "EntityPermissionValueBase<UserGroup>",
                columns: new[] { "Id", "CreatedAt", "Discriminator", "EntityId", "Grant", "PermissionId", "UpdatedAt", "Value" },
                values: new object[,]
                {
                    { new Guid("2cb0dc86-7027-4739-a5c2-25f7fa7755bf"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("93998585-5a67-4a4e-ad2d-f29a4d080e98"), 18446744073709551615m, new Guid("6fc92a20-2405-45e3-95e5-234642d49221"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 50, 0, 0, 0, 0, 0, 0, 0 } },
                    { new Guid("8790dd27-c5fc-401d-b385-87741e804e82"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("55119e40-f094-4560-877f-42d18ff197db"), 18446744073709551615m, new Guid("b7ca31fc-6062-43dd-bf25-2526daeca769"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 50, 0, 0, 0, 0, 0, 0, 0 } },
                    { new Guid("bde4867e-d661-4ac8-9c92-f2a89b77e153"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("55119e40-f094-4560-877f-42d18ff197db"), 18446744073709551615m, new Guid("6fc92a20-2405-45e3-95e5-234642d49221"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 255, 255, 255, 255, 255, 255, 255, 255 } }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("2cb0dc86-7027-4739-a5c2-25f7fa7755bf"));

            migrationBuilder.DeleteData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("8790dd27-c5fc-401d-b385-87741e804e82"));

            migrationBuilder.DeleteData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("bde4867e-d661-4ac8-9c92-f2a89b77e153"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("dc98ca5c-9e8b-49c7-bf95-6c7664115fc8"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("6fc92a20-2405-45e3-95e5-234642d49221"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("b7ca31fc-6062-43dd-bf25-2526daeca769"));
        }
    }
}
