using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace proiectasp.Data.Migrations
{
    public partial class aaa2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationUserGroup_AspNetUsers_UsersId",
                table: "ApplicationUserGroup");

            migrationBuilder.RenameColumn(
                name: "UsersId",
                table: "ApplicationUserGroup",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_ApplicationUserGroup_UsersId",
                table: "ApplicationUserGroup",
                newName: "IX_ApplicationUserGroup_UserId");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Groups",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<bool>(
                name: "GroupJoinMethod",
                table: "Groups",
                type: "bit",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "GroupDescription",
                table: "Groups",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationUserGroup_AspNetUsers_UserId",
                table: "ApplicationUserGroup",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationUserGroup_AspNetUsers_UserId",
                table: "ApplicationUserGroup");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "ApplicationUserGroup",
                newName: "UsersId");

            migrationBuilder.RenameIndex(
                name: "IX_ApplicationUserGroup_UserId",
                table: "ApplicationUserGroup",
                newName: "IX_ApplicationUserGroup_UsersId");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Groups",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "GroupJoinMethod",
                table: "Groups",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "GroupDescription",
                table: "Groups",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationUserGroup_AspNetUsers_UsersId",
                table: "ApplicationUserGroup",
                column: "UsersId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
