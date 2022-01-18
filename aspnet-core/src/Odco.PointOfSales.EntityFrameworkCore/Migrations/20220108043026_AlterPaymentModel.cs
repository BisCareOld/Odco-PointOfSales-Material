using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Odco.PointOfSales.Migrations
{
    public partial class AlterPaymentModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Finance.Payment_Sales.Sale_SaleId",
                table: "Finance.Payment");

            migrationBuilder.DropColumn(
                name: "Bank",
                table: "Finance.Payment");

            migrationBuilder.DropColumn(
                name: "BankId",
                table: "Finance.Payment");

            migrationBuilder.DropColumn(
                name: "Branch",
                table: "Finance.Payment");

            migrationBuilder.DropColumn(
                name: "BranchId",
                table: "Finance.Payment");

            migrationBuilder.DropColumn(
                name: "ChequeNumber",
                table: "Finance.Payment");

            migrationBuilder.DropColumn(
                name: "ChequeReturnDate",
                table: "Finance.Payment");

            migrationBuilder.DropColumn(
                name: "IsCash",
                table: "Finance.Payment");

            migrationBuilder.DropColumn(
                name: "IsCheque",
                table: "Finance.Payment");

            migrationBuilder.DropColumn(
                name: "IsDebitCard",
                table: "Finance.Payment");

            migrationBuilder.DropColumn(
                name: "PaidAmount",
                table: "Finance.Payment");

            migrationBuilder.DropColumn(
                name: "SaleNumber",
                table: "Finance.Payment");

            migrationBuilder.DropColumn(
                name: "SpecificBalanceAmount",
                table: "Finance.Payment");

            migrationBuilder.RenameColumn(
                name: "SpecificReceivedAmount",
                table: "Finance.Payment",
                newName: "TotalPaidAmount");

            migrationBuilder.RenameColumn(
                name: "IsGiftCard",
                table: "Finance.Payment",
                newName: "IsOutstandingPaymentInvolved");

            migrationBuilder.RenameColumn(
                name: "CustomerPhoneNumber",
                table: "Finance.Payment",
                newName: "SalesNumber");

            migrationBuilder.RenameColumn(
                name: "Code",
                table: "Finance.CustomerOutstandingSettlements",
                newName: "CustomerCode");

            migrationBuilder.RenameColumn(
                name: "Code",
                table: "Finance.CustomerOutstanding",
                newName: "CustomerCode");

            migrationBuilder.AlterColumn<Guid>(
                name: "SaleId",
                table: "Finance.Payment",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.CreateTable(
                name: "Finance.PaymentLineLevel",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PaymentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    InvoiceNumber = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    SaleId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SalesNumber = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CustomerCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    CustomerName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    ChequeNumber = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: true),
                    BankId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Bank = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    BranchId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Branch = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ChequeReturnDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SpecificReceivedAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SpecificBalanceAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaidAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsCash = table.Column<bool>(type: "bit", nullable: false),
                    IsCheque = table.Column<bool>(type: "bit", nullable: false),
                    IsDebitCard = table.Column<bool>(type: "bit", nullable: false),
                    IsGiftCard = table.Column<bool>(type: "bit", nullable: false),
                    IsOutstandingPaymentInvolved = table.Column<bool>(type: "bit", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeleterUserId = table.Column<long>(type: "bigint", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Finance.PaymentLineLevel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Finance.PaymentLineLevel_Finance.Payment_PaymentId",
                        column: x => x.PaymentId,
                        principalTable: "Finance.Payment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Finance.PaymentLineLevel_PaymentId",
                table: "Finance.PaymentLineLevel",
                column: "PaymentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Finance.Payment_Sales.Sale_SaleId",
                table: "Finance.Payment",
                column: "SaleId",
                principalTable: "Sales.Sale",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Finance.Payment_Sales.Sale_SaleId",
                table: "Finance.Payment");

            migrationBuilder.DropTable(
                name: "Finance.PaymentLineLevel");

            migrationBuilder.RenameColumn(
                name: "TotalPaidAmount",
                table: "Finance.Payment",
                newName: "SpecificReceivedAmount");

            migrationBuilder.RenameColumn(
                name: "SalesNumber",
                table: "Finance.Payment",
                newName: "CustomerPhoneNumber");

            migrationBuilder.RenameColumn(
                name: "IsOutstandingPaymentInvolved",
                table: "Finance.Payment",
                newName: "IsGiftCard");

            migrationBuilder.RenameColumn(
                name: "CustomerCode",
                table: "Finance.CustomerOutstandingSettlements",
                newName: "Code");

            migrationBuilder.RenameColumn(
                name: "CustomerCode",
                table: "Finance.CustomerOutstanding",
                newName: "Code");

            migrationBuilder.AlterColumn<Guid>(
                name: "SaleId",
                table: "Finance.Payment",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Bank",
                table: "Finance.Payment",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "BankId",
                table: "Finance.Payment",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Branch",
                table: "Finance.Payment",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "BranchId",
                table: "Finance.Payment",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ChequeNumber",
                table: "Finance.Payment",
                type: "nvarchar(25)",
                maxLength: 25,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ChequeReturnDate",
                table: "Finance.Payment",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsCash",
                table: "Finance.Payment",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsCheque",
                table: "Finance.Payment",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDebitCard",
                table: "Finance.Payment",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "PaidAmount",
                table: "Finance.Payment",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "SaleNumber",
                table: "Finance.Payment",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "SpecificBalanceAmount",
                table: "Finance.Payment",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddForeignKey(
                name: "FK_Finance.Payment_Sales.Sale_SaleId",
                table: "Finance.Payment",
                column: "SaleId",
                principalTable: "Sales.Sale",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
