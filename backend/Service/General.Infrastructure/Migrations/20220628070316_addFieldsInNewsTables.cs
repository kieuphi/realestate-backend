using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace General.Infrastructure.Migrations
{
    public partial class addFieldsInNewsTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Views",
                table: "News");

            migrationBuilder.AddColumn<DateTime>(
                name: "ApproveDate",
                table: "NewsCategory",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IsApprove",
                table: "NewsCategory",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "ApproveDate",
                table: "News",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IsApprove",
                table: "News",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsHotNews",
                table: "News",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Position",
                table: "News",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApproveDate",
                table: "NewsCategory");

            migrationBuilder.DropColumn(
                name: "IsApprove",
                table: "NewsCategory");

            migrationBuilder.DropColumn(
                name: "ApproveDate",
                table: "News");

            migrationBuilder.DropColumn(
                name: "IsApprove",
                table: "News");

            migrationBuilder.DropColumn(
                name: "IsHotNews",
                table: "News");

            migrationBuilder.DropColumn(
                name: "Position",
                table: "News");

            migrationBuilder.AddColumn<int>(
                name: "Views",
                table: "News",
                type: "int",
                nullable: true);
        }
    }
}
