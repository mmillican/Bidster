using Microsoft.EntityFrameworkCore.Migrations;

namespace Bidster.Migrations
{
    public partial class Product_CurrentBidAmount_NotNull : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "CurrentBidAmount",
                table: "Products",
                nullable: false,
                oldClrType: typeof(decimal),
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "CurrentBidAmount",
                table: "Products",
                nullable: true,
                oldClrType: typeof(decimal));
        }
    }
}
