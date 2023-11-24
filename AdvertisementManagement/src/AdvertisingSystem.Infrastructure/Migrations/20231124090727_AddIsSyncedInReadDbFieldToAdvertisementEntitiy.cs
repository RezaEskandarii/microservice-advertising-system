using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdvertisingSystem.Infrastructure.Migrations
{
    public partial class AddIsSyncedInReadDbFieldToAdvertisementEntitiy : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSyncedInReadDb",
                table: "Advertisements",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSyncedInReadDb",
                table: "Advertisements");
        }
    }
}
