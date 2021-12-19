using Microsoft.EntityFrameworkCore.Migrations;

namespace Odco.PointOfSales.Migrations
{
    public partial class RefactoringModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CashAmount",
                table: "Finance.Payment");

            migrationBuilder.DropColumn(
                name: "ChequeAmount",
                table: "Finance.Payment");

            migrationBuilder.DropColumn(
                name: "GiftCardAmount",
                table: "Finance.Payment");

            migrationBuilder.DropColumn(
                name: "OutstandingSettledAmount",
                table: "Finance.Payment");

            migrationBuilder.AddColumn<string>(
                name: "CustomerCode",
                table: "Finance.Payment",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomerName",
                table: "Finance.Payment",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PaidAmount",
                table: "Finance.Payment",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomerCode",
                table: "Finance.Payment");

            migrationBuilder.DropColumn(
                name: "CustomerName",
                table: "Finance.Payment");

            migrationBuilder.DropColumn(
                name: "PaidAmount",
                table: "Finance.Payment");

            migrationBuilder.AddColumn<decimal>(
                name: "CashAmount",
                table: "Finance.Payment",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ChequeAmount",
                table: "Finance.Payment",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "GiftCardAmount",
                table: "Finance.Payment",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "OutstandingSettledAmount",
                table: "Finance.Payment",
                type: "decimal(18,2)",
                nullable: true);
        }
    }
}
