using Microsoft.EntityFrameworkCore.Migrations;

namespace Bidster.Migrations
{
    public partial class EventDefaultMinBidAmt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "DefaultMinimumBidAmount",
                table: "Events",
                type: "decimal(6,2)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DefaultMinimumBidAmount",
                table: "Events");
        }
    }
}
