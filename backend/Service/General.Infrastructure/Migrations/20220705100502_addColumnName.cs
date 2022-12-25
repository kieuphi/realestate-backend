using Microsoft.EntityFrameworkCore.Migrations;

namespace General.Infrastructure.Migrations
{
    public partial class addColumnName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SearchText",
                table: "UserSavedSearch");

            migrationBuilder.AddColumn<string>(
                name: "Keyword",
                table: "UserSavedSearch",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "UserSavedSearch",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Keyword",
                table: "UserSavedSearch");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "UserSavedSearch");

            migrationBuilder.AddColumn<string>(
                name: "SearchText",
                table: "UserSavedSearch",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
