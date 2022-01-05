using Microsoft.EntityFrameworkCore.Migrations;

namespace Odco.PointOfSales.Migrations
{
    public partial class AddSoldQuantity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "SoldQuantity",
                table: "Inventory.StockBalance",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "SoldQuantityUnitOfMeasureUnit",
                table: "Inventory.StockBalance",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SoldQuantity",
                table: "Inventory.StockBalance");

            migrationBuilder.DropColumn(
                name: "SoldQuantityUnitOfMeasureUnit",
                table: "Inventory.StockBalance");
        }
    }
}
