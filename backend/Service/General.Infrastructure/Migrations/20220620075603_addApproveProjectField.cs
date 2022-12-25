using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace General.Infrastructure.Migrations
{
    public partial class addApproveProjectField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ApproveDate",
                table: "Project",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IsApprove",
                table: "Project",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApproveDate",
                table: "Project");

            migrationBuilder.DropColumn(
                name: "IsApprove",
                table: "Project");
        }
    }
}
