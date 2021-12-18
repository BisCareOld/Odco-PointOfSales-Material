using Microsoft.EntityFrameworkCore.Migrations;

namespace Odco.PointOfSales.Migrations
{
    public partial class AddPaymentStatusEnumInSales : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte>(
                name: "PaymentStatus",
                table: "Sales.Sale",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentStatus",
                table: "Sales.Sale");
        }
    }
}
