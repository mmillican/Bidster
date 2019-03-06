using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Bidster.Migrations
{
    public partial class ProductBuyItNow : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "BuyItNowPrice",
                table: "Products",
                type: "decimal(10,2)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PurchasedDate",
                table: "Products",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PurchasedUserId",
                table: "Products",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BuyItNowPrice",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "PurchasedDate",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "PurchasedUserId",
                table: "Products");
        }
    }
}
