using Microsoft.EntityFrameworkCore.Migrations;

namespace Flix.Data.Migrations
{
    public partial class AddingMovies : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContentType",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "Data",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "Path",
                table: "Movies");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Movies",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Movies",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "VideoPath",
                table: "Movies",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "VideoPath",
                table: "Movies");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Movies",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AddColumn<string>(
                name: "ContentType",
                table: "Movies",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Data",
                table: "Movies",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Path",
                table: "Movies",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
