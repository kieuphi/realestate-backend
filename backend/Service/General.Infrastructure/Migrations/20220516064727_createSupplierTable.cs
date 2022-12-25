using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace General.Infrastructure.Migrations
{
    public partial class createSupplierTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "SupplierId",
                table: "SocialNetworkUser",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "Supplier",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreateTime = table.Column<DateTime>(nullable: false),
                    CreateBy = table.Column<string>(nullable: true),
                    UpdateTime = table.Column<DateTime>(nullable: true),
                    UpdateBy = table.Column<string>(nullable: true),
                    IsDeleted = table.Column<int>(nullable: false),
                    CurrentState = table.Column<int>(nullable: false),
                    Avatar = table.Column<string>(nullable: true),
                    FirstName = table.Column<string>(nullable: true),
                    LastName = table.Column<string>(nullable: true),
                    DateOfBirth = table.Column<DateTime>(nullable: true),
                    License = table.Column<string>(nullable: true),
                    Gender = table.Column<int>(nullable: true),
                    PhoneNumber1 = table.Column<string>(nullable: true),
                    PhoneNumber2 = table.Column<string>(nullable: true),
                    Descriptions = table.Column<string>(nullable: true),
                    AspNetUserId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Supplier", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Supplier");

            migrationBuilder.DropColumn(
                name: "SupplierId",
                table: "SocialNetworkUser");
        }
    }
}
