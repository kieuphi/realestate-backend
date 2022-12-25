using Microsoft.EntityFrameworkCore.Migrations;

namespace General.Infrastructure.Migrations
{
    public partial class updateProjectField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MapView",
                table: "Project");

            migrationBuilder.AddColumn<string>(
                name: "Developer",
                table: "Project",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Street",
                table: "Project",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Developer",
                table: "Project");

            migrationBuilder.DropColumn(
                name: "Street",
                table: "Project");

            migrationBuilder.AddColumn<string>(
                name: "MapView",
                table: "Project",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
