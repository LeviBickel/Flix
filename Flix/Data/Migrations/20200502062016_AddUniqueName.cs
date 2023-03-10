using Microsoft.EntityFrameworkCore.Migrations;

namespace Flix.Data.Migrations
{
    public partial class AddUniqueName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UniqueName",
                table: "Movies",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UniqueName",
                table: "Movies");
        }
    }
}
