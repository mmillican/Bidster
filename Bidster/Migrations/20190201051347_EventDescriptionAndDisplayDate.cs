using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Bidster.Migrations
{
    public partial class EventDescriptionAndDisplayDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Events",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DisplayOn",
                table: "Events",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "DisplayOn",
                table: "Events");
        }
    }
}
