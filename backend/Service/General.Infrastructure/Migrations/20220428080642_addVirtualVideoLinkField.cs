using Microsoft.EntityFrameworkCore.Migrations;

namespace General.Infrastructure.Migrations
{
    public partial class addVirtualVideoLinkField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AmentitiesNearbyId",
                table: "Property");

            migrationBuilder.AddColumn<string>(
                name: "VirtualVideoLink",
                table: "Property",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VirtualVideoLink",
                table: "Property");

            migrationBuilder.AddColumn<string>(
                name: "AmentitiesNearbyId",
                table: "Property",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
