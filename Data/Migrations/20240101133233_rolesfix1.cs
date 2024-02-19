using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace proiectasp.Data.Migrations
{
    public partial class rolesfix1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FollowLists_AspNetUsers_UserId",
                table: "FollowLists");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "FollowLists",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_FollowLists_AspNetUsers_UserId",
                table: "FollowLists",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FollowLists_AspNetUsers_UserId",
                table: "FollowLists");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "FollowLists",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddForeignKey(
                name: "FK_FollowLists_AspNetUsers_UserId",
                table: "FollowLists",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
