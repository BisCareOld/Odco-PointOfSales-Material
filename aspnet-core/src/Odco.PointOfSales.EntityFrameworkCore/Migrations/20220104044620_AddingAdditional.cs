using Microsoft.EntityFrameworkCore.Migrations;

namespace Odco.PointOfSales.Migrations
{
    public partial class AddingAdditional : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ReceivedAmount",
                table: "Finance.Payment",
                newName: "TotalReceivedAmount");

            migrationBuilder.RenameColumn(
                name: "BalanceAmount",
                table: "Finance.Payment",
                newName: "TotalBalanceAmount");

            migrationBuilder.AddColumn<decimal>(
                name: "SpecificBalanceAmount",
                table: "Finance.Payment",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "SpecificReceivedAmount",
                table: "Finance.Payment",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SpecificBalanceAmount",
                table: "Finance.Payment");

            migrationBuilder.DropColumn(
                name: "SpecificReceivedAmount",
                table: "Finance.Payment");

            migrationBuilder.RenameColumn(
                name: "TotalReceivedAmount",
                table: "Finance.Payment",
                newName: "ReceivedAmount");

            migrationBuilder.RenameColumn(
                name: "TotalBalanceAmount",
                table: "Finance.Payment",
                newName: "BalanceAmount");
        }
    }
}
