using Microsoft.EntityFrameworkCore.Migrations;

namespace Flix.Data.Migrations
{
    public partial class identity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserRolesView",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    UserName = table.Column<string>(nullable: true),
                    IsSelected = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRolesView", x => x.UserId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserRolesView");
        }
    }
}
