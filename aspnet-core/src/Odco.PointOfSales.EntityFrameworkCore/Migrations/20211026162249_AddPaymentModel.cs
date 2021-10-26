using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Odco.PointOfSales.Migrations
{
    public partial class AddPaymentModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Finance.Payment",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InvoiceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InvoiceNumber = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CustomerPhoneNumber = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    CashAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ChequeNumber = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: true),
                    BankId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Bank = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    BranchId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Branch = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ChequeReturnDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ChequeAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    OutstandingAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    OutstandingSettledAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    GiftCardAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    IsCash = table.Column<bool>(type: "bit", nullable: false),
                    IsCheque = table.Column<bool>(type: "bit", nullable: false),
                    IsCreditOutstanding = table.Column<bool>(type: "bit", nullable: false),
                    IsDebitCard = table.Column<bool>(type: "bit", nullable: false),
                    IsGiftCard = table.Column<bool>(type: "bit", nullable: false),
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
                    table.PrimaryKey("PK_Finance.Payment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Finance.Payment_Finance.Invoice_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "Finance.Invoice",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Finance.Payment_InvoiceId",
                table: "Finance.Payment",
                column: "InvoiceId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Finance.Payment");
        }
    }
}
