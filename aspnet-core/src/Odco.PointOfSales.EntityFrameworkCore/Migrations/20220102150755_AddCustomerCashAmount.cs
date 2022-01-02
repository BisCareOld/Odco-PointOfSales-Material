using Microsoft.EntityFrameworkCore.Migrations;

namespace Odco.PointOfSales.Migrations
{
    public partial class AddCustomerCashAmount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "BalanceAmount",
                table: "Sales.Sale",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ReceivedAmount",
                table: "Sales.Sale",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BalanceAmount",
                table: "Sales.Sale");

            migrationBuilder.DropColumn(
                name: "ReceivedAmount",
                table: "Sales.Sale");
        }
    }
}
