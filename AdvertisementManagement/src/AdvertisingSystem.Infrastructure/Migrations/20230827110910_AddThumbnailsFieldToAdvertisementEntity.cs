using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdvertisingSystem.Infrastructure.Migrations
{
    public partial class AddThumbnailsFieldToAdvertisementEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string[]>(
                name: "Thumbnails",
                table: "Advertisements",
                type: "text[]",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Thumbnails",
                table: "Advertisements");
        }
    }
}
