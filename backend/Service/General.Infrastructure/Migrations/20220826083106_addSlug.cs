using Microsoft.EntityFrameworkCore.Migrations;

namespace General.Infrastructure.Migrations
{
    public partial class addSlug : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "Property",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "Project",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "News",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Slug",
                table: "Property");

            migrationBuilder.DropColumn(
                name: "Slug",
                table: "Project");

            migrationBuilder.DropColumn(
                name: "Slug",
                table: "News");
        }
    }
}
