using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    public partial class UpdateUserGroupPermissionValues : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "PermissionValues",
                keyColumn: "Id",
                keyValue: new Guid("392db217-e312-44e3-ba5f-48bd46a0f913"),
                column: "Value",
                value: new byte[] { 255, 255, 255, 255, 255, 255, 255, 127 });

            migrationBuilder.UpdateData(
                table: "PermissionValues",
                keyColumn: "Id",
                keyValue: new Guid("ec98b81c-50f4-465f-8f1a-6768c175849b"),
                column: "Value",
                value: new byte[] { 255, 255, 255, 255, 255, 255, 255, 127 });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "PermissionValues",
                keyColumn: "Id",
                keyValue: new Guid("392db217-e312-44e3-ba5f-48bd46a0f913"),
                column: "Value",
                value: new byte[] { 50, 0, 0, 0, 0, 0, 0, 0 });

            migrationBuilder.UpdateData(
                table: "PermissionValues",
                keyColumn: "Id",
                keyValue: new Guid("ec98b81c-50f4-465f-8f1a-6768c175849b"),
                column: "Value",
                value: new byte[] { 50, 0, 0, 0, 0, 0, 0, 0 });
        }
    }
}
