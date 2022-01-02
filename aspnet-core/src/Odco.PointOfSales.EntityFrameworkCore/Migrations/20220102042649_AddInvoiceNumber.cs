using Microsoft.EntityFrameworkCore.Migrations;

namespace Odco.PointOfSales.Migrations
{
    public partial class AddInvoiceNumber : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "InvoiceNumber",
                table: "Sales.SalesProduct",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InvoiceNumber",
                table: "Sales.Sale",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InvoiceNumber",
                table: "Inventory.NonInventoryProduct",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InvoiceNumber",
                table: "Finance.Payment",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InvoiceNumber",
                table: "Sales.SalesProduct");

            migrationBuilder.DropColumn(
                name: "InvoiceNumber",
                table: "Sales.Sale");

            migrationBuilder.DropColumn(
                name: "InvoiceNumber",
                table: "Inventory.NonInventoryProduct");

            migrationBuilder.DropColumn(
                name: "InvoiceNumber",
                table: "Finance.Payment");
        }
    }
}
