using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace General.Infrastructure.Migrations
{
    public partial class addColumnContactEntityContactType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ContactType",
                table: "Contact",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "PropertyViewCount",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreateTime = table.Column<DateTime>(nullable: false),
                    CreateBy = table.Column<string>(nullable: true),
                    UpdateTime = table.Column<DateTime>(nullable: true),
                    UpdateBy = table.Column<string>(nullable: true),
                    IsDeleted = table.Column<int>(nullable: false),
                    CurrentState = table.Column<int>(nullable: false),
                    PropertyId = table.Column<Guid>(nullable: false),
                    ViewCount = table.Column<int>(nullable: false),
                    UserLoginViewCount = table.Column<int>(nullable: false),
                    UnLoginViewCount = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropertyViewCount", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PropertyViewCount");

            migrationBuilder.DropColumn(
                name: "ContactType",
                table: "Contact");
        }
    }
}
