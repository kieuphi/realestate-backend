using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace General.Infrastructure.Migrations
{
    public partial class createPropertyFavoriteTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClientTestimonial");

            migrationBuilder.CreateTable(
                name: "PropertyFavorite",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreateTime = table.Column<DateTime>(nullable: false),
                    CreateBy = table.Column<string>(nullable: true),
                    UpdateTime = table.Column<DateTime>(nullable: true),
                    UpdateBy = table.Column<string>(nullable: true),
                    IsDeleted = table.Column<int>(nullable: false),
                    CurrentState = table.Column<int>(nullable: false),
                    UserId = table.Column<Guid>(nullable: false),
                    PropertyId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropertyFavorite", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PropertyFavorite");

            migrationBuilder.CreateTable(
                name: "ClientTestimonial",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClientAccount = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClientName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContentEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContentVi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CurrentState = table.Column<int>(type: "int", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<int>(type: "int", nullable: false),
                    RatingNumber = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    UpdateBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientTestimonial", x => x.Id);
                });
        }
    }
}
