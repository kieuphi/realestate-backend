using Microsoft.EntityFrameworkCore.Migrations;

namespace General.Infrastructure.Migrations
{
    public partial class createTitleUserProfile : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TitleDescriptionEn",
                table: "ProfileInformation",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TitleDescriptionVi",
                table: "ProfileInformation",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TitleEn",
                table: "ProfileInformation",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TitleVi",
                table: "ProfileInformation",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TitleDescriptionEn",
                table: "ProfileInformation");

            migrationBuilder.DropColumn(
                name: "TitleDescriptionVi",
                table: "ProfileInformation");

            migrationBuilder.DropColumn(
                name: "TitleEn",
                table: "ProfileInformation");

            migrationBuilder.DropColumn(
                name: "TitleVi",
                table: "ProfileInformation");
        }
    }
}
