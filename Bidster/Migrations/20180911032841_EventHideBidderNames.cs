using Microsoft.EntityFrameworkCore.Migrations;

namespace Bidster.Migrations
{
    public partial class EventHideBidderNames : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HideBidderNames",
                table: "Events",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HideBidderNames",
                table: "Events");
        }
    }
}
