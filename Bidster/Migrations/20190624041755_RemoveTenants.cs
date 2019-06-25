using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Bidster.Migrations
{
    public partial class RemoveTenants : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_Tenants_TenantId",
                table: "Events");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_Tenants_TenantId",
                table: "Products");

            migrationBuilder.DropTable(
                name: "TenantUsers");

            migrationBuilder.DropTable(
                name: "Tenants");

            migrationBuilder.DropIndex(
                name: "IX_Products_TenantId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Events_TenantId",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Events");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "Products",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "Events",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Tenants",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    HostNames = table.Column<string>(maxLength: 255, nullable: false),
                    IsDisabled = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(maxLength: 50, nullable: false),
                    Settings = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tenants", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TenantUsers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AddedOn = table.Column<DateTime>(nullable: false),
                    IsAdmin = table.Column<bool>(nullable: false),
                    TenantId = table.Column<int>(nullable: false),
                    UserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TenantUsers_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TenantUsers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Products_TenantId",
                table: "Products",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Events_TenantId",
                table: "Events",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_TenantUsers_TenantId",
                table: "TenantUsers",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_TenantUsers_UserId",
                table: "TenantUsers",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Tenants_TenantId",
                table: "Events",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Tenants_TenantId",
                table: "Products",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
