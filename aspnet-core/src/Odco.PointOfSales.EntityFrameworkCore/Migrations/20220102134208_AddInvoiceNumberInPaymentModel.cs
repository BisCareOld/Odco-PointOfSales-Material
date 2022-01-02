using Microsoft.EntityFrameworkCore.Migrations;

namespace Odco.PointOfSales.Migrations
{
    public partial class AddInvoiceNumberInPaymentModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "InvoiceNumber",
                table: "Finance.Payment",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InvoiceNumber",
                table: "Finance.Payment");
        }
    }
}
