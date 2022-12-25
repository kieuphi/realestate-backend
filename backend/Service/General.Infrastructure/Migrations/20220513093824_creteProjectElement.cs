using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace General.Infrastructure.Migrations
{
    public partial class creteProjectElement : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "SupplierId",
                table: "Property",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "Project",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Latitude",
                table: "Project",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Longtitude",
                table: "Project",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProjectImage",
                table: "Project",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "Project",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ProjectFeature",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreateTime = table.Column<DateTime>(nullable: false),
                    CreateBy = table.Column<string>(nullable: true),
                    UpdateTime = table.Column<DateTime>(nullable: true),
                    UpdateBy = table.Column<string>(nullable: true),
                    IsDeleted = table.Column<int>(nullable: false),
                    CurrentState = table.Column<int>(nullable: false),
                    ProjectId = table.Column<Guid>(nullable: false),
                    ProjectFeatureId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectFeature", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProjectImage",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreateTime = table.Column<DateTime>(nullable: false),
                    CreateBy = table.Column<string>(nullable: true),
                    UpdateTime = table.Column<DateTime>(nullable: true),
                    UpdateBy = table.Column<string>(nullable: true),
                    IsDeleted = table.Column<int>(nullable: false),
                    CurrentState = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    ImagesPath = table.Column<string>(nullable: true),
                    ProjectId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectImage", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProjectSeller",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreateTime = table.Column<DateTime>(nullable: false),
                    CreateBy = table.Column<string>(nullable: true),
                    UpdateTime = table.Column<DateTime>(nullable: true),
                    UpdateBy = table.Column<string>(nullable: true),
                    IsDeleted = table.Column<int>(nullable: false),
                    CurrentState = table.Column<int>(nullable: false),
                    ProjectId = table.Column<Guid>(nullable: false),
                    SellerId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectSeller", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProjectFeature");

            migrationBuilder.DropTable(
                name: "ProjectImage");

            migrationBuilder.DropTable(
                name: "ProjectSeller");

            migrationBuilder.DropColumn(
                name: "SupplierId",
                table: "Property");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "Project");

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Project");

            migrationBuilder.DropColumn(
                name: "Longtitude",
                table: "Project");

            migrationBuilder.DropColumn(
                name: "ProjectImage",
                table: "Project");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "Project");
        }
    }
}
