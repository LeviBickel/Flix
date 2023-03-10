using Microsoft.EntityFrameworkCore.Migrations;

namespace Flix.Data.Migrations
{
    public partial class category : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "videoCategory",
                table: "Movies",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "videoCategory",
                table: "Movies");
        }
    }
}
