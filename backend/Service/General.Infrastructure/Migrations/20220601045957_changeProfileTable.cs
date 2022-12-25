using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace General.Infrastructure.Migrations
{
    public partial class changeProfileTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Seller");

            migrationBuilder.DropTable(
                name: "Supplier");

            migrationBuilder.DropColumn(
                name: "SellerId",
                table: "SocialNetworkUser");

            migrationBuilder.DropColumn(
                name: "SupplierId",
                table: "SocialNetworkUser");

            migrationBuilder.DropColumn(
                name: "SellerId",
                table: "PropertySeller");

            migrationBuilder.DropColumn(
                name: "SellerId",
                table: "ProjectSeller");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "ProfileInformation");

            migrationBuilder.DropColumn(
                name: "SirName",
                table: "ProfileInformation");

            migrationBuilder.AddColumn<Guid>(
                name: "ProfileId",
                table: "SocialNetworkUser",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "PropertySeller",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "ProjectSeller",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "Agency",
                table: "ProfileInformation",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Descriptions",
                table: "ProfileInformation",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "ProfileInformation",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "License",
                table: "ProfileInformation",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber1",
                table: "ProfileInformation",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber2",
                table: "ProfileInformation",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber3",
                table: "ProfileInformation",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProfileId",
                table: "SocialNetworkUser");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "PropertySeller");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "ProjectSeller");

            migrationBuilder.DropColumn(
                name: "Agency",
                table: "ProfileInformation");

            migrationBuilder.DropColumn(
                name: "Descriptions",
                table: "ProfileInformation");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "ProfileInformation");

            migrationBuilder.DropColumn(
                name: "License",
                table: "ProfileInformation");

            migrationBuilder.DropColumn(
                name: "PhoneNumber1",
                table: "ProfileInformation");

            migrationBuilder.DropColumn(
                name: "PhoneNumber2",
                table: "ProfileInformation");

            migrationBuilder.DropColumn(
                name: "PhoneNumber3",
                table: "ProfileInformation");

            migrationBuilder.AddColumn<Guid>(
                name: "SellerId",
                table: "SocialNetworkUser",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "SupplierId",
                table: "SocialNetworkUser",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "SellerId",
                table: "PropertySeller",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "SellerId",
                table: "ProjectSeller",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "ProfileInformation",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SirName",
                table: "ProfileInformation",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Seller",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Agency = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AspNetUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Avatar = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CurrentState = table.Column<int>(type: "int", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<int>(type: "int", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PropertyCount = table.Column<int>(type: "int", nullable: true),
                    UpdateBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Seller", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Supplier",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AspNetUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Avatar = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CurrentState = table.Column<int>(type: "int", nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Descriptions = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Gender = table.Column<int>(type: "int", nullable: true),
                    IsDeleted = table.Column<int>(type: "int", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    License = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdateBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Supplier", x => x.Id);
                });
        }
    }
}
