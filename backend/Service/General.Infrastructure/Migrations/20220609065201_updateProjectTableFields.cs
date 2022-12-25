using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace General.Infrastructure.Migrations
{
    public partial class updateProjectTableFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DistrictCode",
                table: "Project",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FloorPlans",
                table: "Project",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MapView",
                table: "Project",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MapViewImage",
                table: "Project",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "OpenForSaleDate",
                table: "Project",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProvinceCode",
                table: "Project",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Project",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Video",
                table: "Project",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VirtualTour",
                table: "Project",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WardCode",
                table: "Project",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ProjectViewCount",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreateTime = table.Column<DateTime>(nullable: false),
                    CreateBy = table.Column<string>(nullable: true),
                    UpdateTime = table.Column<DateTime>(nullable: true),
                    UpdateBy = table.Column<string>(nullable: true),
                    IsDeleted = table.Column<int>(nullable: false),
                    CurrentState = table.Column<int>(nullable: false),
                    IPAddress = table.Column<string>(nullable: true),
                    ProjectId = table.Column<Guid>(nullable: false),
                    ViewCount = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectViewCount", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProjectViewCount");

            migrationBuilder.DropColumn(
                name: "DistrictCode",
                table: "Project");

            migrationBuilder.DropColumn(
                name: "FloorPlans",
                table: "Project");

            migrationBuilder.DropColumn(
                name: "MapView",
                table: "Project");

            migrationBuilder.DropColumn(
                name: "MapViewImage",
                table: "Project");

            migrationBuilder.DropColumn(
                name: "OpenForSaleDate",
                table: "Project");

            migrationBuilder.DropColumn(
                name: "ProvinceCode",
                table: "Project");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Project");

            migrationBuilder.DropColumn(
                name: "Video",
                table: "Project");

            migrationBuilder.DropColumn(
                name: "VirtualTour",
                table: "Project");

            migrationBuilder.DropColumn(
                name: "WardCode",
                table: "Project");
        }
    }
}
