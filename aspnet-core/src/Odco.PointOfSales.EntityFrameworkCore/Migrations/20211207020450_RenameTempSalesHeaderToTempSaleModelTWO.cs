using Microsoft.EntityFrameworkCore.Migrations;

namespace Odco.PointOfSales.Migrations
{
    public partial class RenameTempSalesHeaderToTempSaleModelTWO : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TempSalesId",
                table: "Inventory.NonInventoryProduct",
                newName: "TempSaleId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TempSaleId",
                table: "Inventory.NonInventoryProduct",
                newName: "TempSalesId");
        }
    }
}
