using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace General.Infrastructure.Migrations
{
    public partial class creteTimeForPostTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TimeForPost",
                table: "Property");

            migrationBuilder.AddColumn<Guid>(
                name: "TimeForPostId",
                table: "Property",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TimeForPost",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreateTime = table.Column<DateTime>(nullable: false),
                    CreateBy = table.Column<string>(nullable: true),
                    UpdateTime = table.Column<DateTime>(nullable: true),
                    UpdateBy = table.Column<string>(nullable: true),
                    IsDeleted = table.Column<int>(nullable: false),
                    CurrentState = table.Column<int>(nullable: false),
                    Value = table.Column<double>(nullable: true),
                    DisplayName = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimeForPost", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TimeForPost");

            migrationBuilder.DropColumn(
                name: "TimeForPostId",
                table: "Property");

            migrationBuilder.AddColumn<int>(
                name: "TimeForPost",
                table: "Property",
                type: "int",
                nullable: true);
        }
    }
}
