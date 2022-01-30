using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    public partial class AddPermissionCompareType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CompareMode",
                table: "Permissions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("00000001-0000-0000-0000-000000000000"),
                column: "CompareMode",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("00000002-0000-0000-0000-000000000000"),
                column: "CompareMode",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("00000003-0000-0000-0000-000000000000"),
                column: "CompareMode",
                value: 4);

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("00000004-0000-0000-0000-000000000000"),
                column: "CompareMode",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("00000005-0000-0000-0000-000000000000"),
                column: "CompareMode",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("00000006-0000-0000-0000-000000000000"),
                column: "CompareMode",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("00000007-0000-0000-0000-000000000000"),
                column: "CompareMode",
                value: 4);

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("00000008-0000-0000-0000-000000000000"),
                column: "CompareMode",
                value: 4);

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("00000009-0000-0000-0000-000000000000"),
                column: "CompareMode",
                value: 4);

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("0000000a-0000-0000-0000-000000000000"),
                column: "CompareMode",
                value: 4);

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("0000000b-0000-0000-0000-000000000000"),
                column: "CompareMode",
                value: 4);

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("0000000c-0000-0000-0000-000000000000"),
                column: "CompareMode",
                value: 4);

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("0000000d-0000-0000-0000-000000000000"),
                column: "CompareMode",
                value: 4);

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("0000000e-0000-0000-0000-000000000000"),
                column: "CompareMode",
                value: 4);

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("0000000f-0000-0000-0000-000000000000"),
                column: "CompareMode",
                value: 4);

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("00000010-0000-0000-0000-000000000000"),
                column: "CompareMode",
                value: 4);

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("00000016-0000-0000-0000-000000000000"),
                column: "CompareMode",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("00000017-0000-0000-0000-000000000000"),
                column: "CompareMode",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("00000018-0000-0000-0000-000000000000"),
                column: "CompareMode",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("00000019-0000-0000-0000-000000000000"),
                column: "CompareMode",
                value: 4);

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("0000001a-0000-0000-0000-000000000000"),
                column: "CompareMode",
                value: 4);

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("0000001b-0000-0000-0000-000000000000"),
                column: "CompareMode",
                value: 4);

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("0000001c-0000-0000-0000-000000000000"),
                column: "CompareMode",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("0000001d-0000-0000-0000-000000000000"),
                column: "CompareMode",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("0000001e-0000-0000-0000-000000000000"),
                column: "CompareMode",
                value: 4);

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("0000001f-0000-0000-0000-000000000000"),
                column: "CompareMode",
                value: 4);

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("00000020-0000-0000-0000-000000000000"),
                column: "CompareMode",
                value: 4);

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("00000021-0000-0000-0000-000000000000"),
                column: "CompareMode",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("00000022-0000-0000-0000-000000000000"),
                column: "CompareMode",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("00000023-0000-0000-0000-000000000000"),
                column: "CompareMode",
                value: 4);

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("00000024-0000-0000-0000-000000000000"),
                column: "CompareMode",
                value: 4);

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("00000025-0000-0000-0000-000000000000"),
                column: "CompareMode",
                value: 4);

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("00000026-0000-0000-0000-000000000000"),
                column: "CompareMode",
                value: 4);

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("00000027-0000-0000-0000-000000000000"),
                column: "CompareMode",
                value: 4);

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("00000028-0000-0000-0000-000000000000"),
                column: "CompareMode",
                value: 4);

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("00000029-0000-0000-0000-000000000000"),
                column: "CompareMode",
                value: 4);

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("0000002a-0000-0000-0000-000000000000"),
                column: "CompareMode",
                value: 4);

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("0000002b-0000-0000-0000-000000000000"),
                column: "CompareMode",
                value: 4);

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("0000002c-0000-0000-0000-000000000000"),
                column: "CompareMode",
                value: 4);

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("0000002d-0000-0000-0000-000000000000"),
                column: "CompareMode",
                value: 4);

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("0000002e-0000-0000-0000-000000000000"),
                column: "CompareMode",
                value: 4);

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("0000002f-0000-0000-0000-000000000000"),
                column: "CompareMode",
                value: 4);

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("00000030-0000-0000-0000-000000000000"),
                column: "CompareMode",
                value: 4);

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("00000031-0000-0000-0000-000000000000"),
                column: "CompareMode",
                value: 4);

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("00000032-0000-0000-0000-000000000000"),
                column: "CompareMode",
                value: 4);

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("00000033-0000-0000-0000-000000000000"),
                column: "CompareMode",
                value: 4);

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("00000034-0000-0000-0000-000000000000"),
                column: "CompareMode",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("00000035-0000-0000-0000-000000000000"),
                column: "CompareMode",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("00000036-0000-0000-0000-000000000000"),
                column: "CompareMode",
                value: 4);

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("00000037-0000-0000-0000-000000000000"),
                column: "CompareMode",
                value: 4);

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("00000038-0000-0000-0000-000000000000"),
                column: "CompareMode",
                value: 4);

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("00000039-0000-0000-0000-000000000000"),
                column: "CompareMode",
                value: 4);

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("0000003a-0000-0000-0000-000000000000"),
                column: "CompareMode",
                value: 4);

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("0000003b-0000-0000-0000-000000000000"),
                column: "CompareMode",
                value: 4);

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("0000003c-0000-0000-0000-000000000000"),
                column: "CompareMode",
                value: 4);

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("0000003d-0000-0000-0000-000000000000"),
                column: "CompareMode",
                value: 4);

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("0000003e-0000-0000-0000-000000000000"),
                column: "CompareMode",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("0000003f-0000-0000-0000-000000000000"),
                column: "CompareMode",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("00000040-0000-0000-0000-000000000000"),
                column: "CompareMode",
                value: 4);

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("00000041-0000-0000-0000-000000000000"),
                column: "CompareMode",
                value: 4);

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("00000042-0000-0000-0000-000000000000"),
                column: "CompareMode",
                value: 4);

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("00000043-0000-0000-0000-000000000000"),
                column: "CompareMode",
                value: 4);

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("00000044-0000-0000-0000-000000000000"),
                column: "CompareMode",
                value: 4);

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("00000045-0000-0000-0000-000000000000"),
                column: "CompareMode",
                value: 4);

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("00000046-0000-0000-0000-000000000000"),
                column: "CompareMode",
                value: 4);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompareMode",
                table: "Permissions");
        }
    }
}
