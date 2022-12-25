using Microsoft.EntityFrameworkCore.Migrations;

namespace General.Infrastructure.Migrations
{
    public partial class addUSDPriceToPropertyTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "USDPrice",
                table: "Property",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "USDPrice",
                table: "Property");
        }
    }
}
