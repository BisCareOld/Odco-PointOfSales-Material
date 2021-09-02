using Microsoft.EntityFrameworkCore.Migrations;

namespace Odco.PointOfSales.Migrations
{
    public partial class AddGRNRelatedModel123 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "GoodsRecievedNumber",
                table: "Inventory.GoodsRecieved",
                newName: "GoodsReceivedNumber");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "GoodsReceivedNumber",
                table: "Inventory.GoodsRecieved",
                newName: "GoodsRecievedNumber");
        }
    }
}
