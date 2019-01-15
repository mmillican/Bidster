using Microsoft.EntityFrameworkCore.Migrations;

namespace Bidster.Migrations
{
    public partial class ProductThumbnailImage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ThumbnailFilename",
                table: "Products",
                maxLength: 100,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ThumbnailFilename",
                table: "Products");
        }
    }
}
