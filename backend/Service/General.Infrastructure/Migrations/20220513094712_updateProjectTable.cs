using Microsoft.EntityFrameworkCore.Migrations;

namespace General.Infrastructure.Migrations
{
    public partial class updateProjectTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProjectImage",
                table: "Project");

            migrationBuilder.AddColumn<string>(
                name: "CoverImage",
                table: "Project",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CoverImage",
                table: "Project");

            migrationBuilder.AddColumn<string>(
                name: "ProjectImage",
                table: "Project",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
