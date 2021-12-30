using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    public partial class RenameRemoveAfterOfJsonWebToken : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RemoveAfter",
                table: "JsonWebTokens",
                newName: "DeleteAfter");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DeleteAfter",
                table: "JsonWebTokens",
                newName: "RemoveAfter");
        }
    }
}
