using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace General.Infrastructure.Migrations
{
    public partial class addSocialNetwork : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Avatar",
                table: "Seller",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SocialNetwork",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreateTime = table.Column<DateTime>(nullable: false),
                    CreateBy = table.Column<string>(nullable: true),
                    UpdateTime = table.Column<DateTime>(nullable: true),
                    UpdateBy = table.Column<string>(nullable: true),
                    IsDeleted = table.Column<int>(nullable: false),
                    CurrentState = table.Column<int>(nullable: false),
                    AppName = table.Column<string>(nullable: true),
                    ICon = table.Column<string>(nullable: true),
                    Descriptions = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SocialNetwork", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SocialNetworkUser",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreateTime = table.Column<DateTime>(nullable: false),
                    CreateBy = table.Column<string>(nullable: true),
                    UpdateTime = table.Column<DateTime>(nullable: true),
                    UpdateBy = table.Column<string>(nullable: true),
                    IsDeleted = table.Column<int>(nullable: false),
                    CurrentState = table.Column<int>(nullable: false),
                    SocialNetworkId = table.Column<Guid>(nullable: false),
                    SellerId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SocialNetworkUser", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SocialNetwork");

            migrationBuilder.DropTable(
                name: "SocialNetworkUser");

            migrationBuilder.DropColumn(
                name: "Avatar",
                table: "Seller");
        }
    }
}
