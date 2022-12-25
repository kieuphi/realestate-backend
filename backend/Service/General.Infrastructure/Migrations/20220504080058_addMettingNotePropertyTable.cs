using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace General.Infrastructure.Migrations
{
    public partial class addMettingNotePropertyTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MettingNotes",
                table: "Property");

            migrationBuilder.CreateTable(
                name: "PropertyMeetingNote",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreateTime = table.Column<DateTime>(nullable: false),
                    CreateBy = table.Column<string>(nullable: true),
                    UpdateTime = table.Column<DateTime>(nullable: true),
                    UpdateBy = table.Column<string>(nullable: true),
                    IsDeleted = table.Column<int>(nullable: false),
                    CurrentState = table.Column<int>(nullable: false),
                    MeetingNoteTitle = table.Column<string>(nullable: true),
                    MeetingNoteContent = table.Column<string>(nullable: true),
                    PropertyId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropertyMeetingNote", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PropertyMeetingNote");

            migrationBuilder.AddColumn<string>(
                name: "MettingNotes",
                table: "Property",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
