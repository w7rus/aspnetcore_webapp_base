using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    public partial class RemoveUnusedMembers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserGroups_Users_OwnerUserId",
                table: "UserGroups");

            migrationBuilder.DropColumn(
                name: "IsPhoneNumberVerified",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "UserProfiles");

            migrationBuilder.RenameColumn(
                name: "OwnerUserId",
                table: "UserGroups",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserGroups_OwnerUserId",
                table: "UserGroups",
                newName: "IX_UserGroups_UserId");

            migrationBuilder.CreateIndex(
                name: "IX_EntityToEntityMappingBase<User, UserGroup>_EntityLeftId",
                table: "EntityToEntityMappingBase<User, UserGroup>",
                column: "EntityLeftId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserGroups_Users_UserId",
                table: "UserGroups",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserGroups_Users_UserId",
                table: "UserGroups");

            migrationBuilder.DropIndex(
                name: "IX_EntityToEntityMappingBase<User, UserGroup>_EntityLeftId",
                table: "EntityToEntityMappingBase<User, UserGroup>");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "UserGroups",
                newName: "OwnerUserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserGroups_UserId",
                table: "UserGroups",
                newName: "IX_UserGroups_OwnerUserId");

            migrationBuilder.AddColumn<bool>(
                name: "IsPhoneNumberVerified",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "UserProfiles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "UserProfiles",
                type: "text",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_UserGroups_Users_OwnerUserId",
                table: "UserGroups",
                column: "OwnerUserId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
