using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Flix.Data.Migrations
{
    /// <inheritdoc />
    public partial class existingdirectory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "directoryScannerInterval",
                table: "AdminSettings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "useExistingDirectory",
                table: "AdminSettings",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "directoryScannerInterval",
                table: "AdminSettings");

            migrationBuilder.DropColumn(
                name: "useExistingDirectory",
                table: "AdminSettings");
        }
    }
}
