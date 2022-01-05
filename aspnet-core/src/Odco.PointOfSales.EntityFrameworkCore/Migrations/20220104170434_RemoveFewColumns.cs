using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Odco.PointOfSales.Migrations
{
    public partial class RemoveFewColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCreditOutstanding",
                table: "Finance.Payment");

            migrationBuilder.DropColumn(
                name: "OutstandingAmount",
                table: "Finance.Payment");

            migrationBuilder.RenameColumn(
                name: "InvoiceNumber",
                table: "Finance.CustomerOutstanding",
                newName: "CreatedInvoiceNumber");

            migrationBuilder.AddColumn<Guid>(
                name: "PaymentId",
                table: "Finance.CustomerOutstandingSettlements",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentId",
                table: "Finance.CustomerOutstandingSettlements");

            migrationBuilder.RenameColumn(
                name: "CreatedInvoiceNumber",
                table: "Finance.CustomerOutstanding",
                newName: "InvoiceNumber");

            migrationBuilder.AddColumn<bool>(
                name: "IsCreditOutstanding",
                table: "Finance.Payment",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "OutstandingAmount",
                table: "Finance.Payment",
                type: "decimal(18,2)",
                nullable: true);
        }
    }
}
