using Microsoft.EntityFrameworkCore.Migrations;

namespace Flix.Data.Migrations
{
    public partial class usersUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UsersWatched",
                table: "UsersWatched");

            migrationBuilder.AlterColumn<string>(
                name: "User",
                table: "UsersWatched",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<int>(
                name: "ID",
                table: "UsersWatched",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UsersWatched",
                table: "UsersWatched",
                column: "ID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UsersWatched",
                table: "UsersWatched");

            migrationBuilder.DropColumn(
                name: "ID",
                table: "UsersWatched");

            migrationBuilder.AlterColumn<string>(
                name: "User",
                table: "UsersWatched",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UsersWatched",
                table: "UsersWatched",
                column: "User");
        }
    }
}
