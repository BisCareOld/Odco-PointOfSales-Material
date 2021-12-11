using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Odco.PointOfSales.Migrations
{
    public partial class RenameTempSalesHeaderToTempSaleModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sales.TempSalesProduct_Sales.TempSalesHeader_TempSalesHeaderId",
                table: "Sales.TempSalesProduct");

            migrationBuilder.DropTable(
                name: "Sales.TempSalesHeader");

            migrationBuilder.RenameColumn(
                name: "TempSalesHeaderId",
                table: "Sales.TempSalesProduct",
                newName: "TempSaleId");

            migrationBuilder.RenameIndex(
                name: "IX_Sales.TempSalesProduct_TempSalesHeaderId",
                table: "Sales.TempSalesProduct",
                newName: "IX_Sales.TempSalesProduct_TempSaleId");

            migrationBuilder.RenameColumn(
                name: "TempSalesHeaderId",
                table: "Finance.Invoice",
                newName: "TempSaleId");

            migrationBuilder.CreateTable(
                name: "Sales.TempSale",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CustomerCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    CustomerName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DiscountRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DiscountAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TaxRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TaxAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    GrossAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NetAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
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
                    table.PrimaryKey("PK_Sales.TempSale", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Sales.TempSalesProduct_Sales.TempSale_TempSaleId",
                table: "Sales.TempSalesProduct",
                column: "TempSaleId",
                principalTable: "Sales.TempSale",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sales.TempSalesProduct_Sales.TempSale_TempSaleId",
                table: "Sales.TempSalesProduct");

            migrationBuilder.DropTable(
                name: "Sales.TempSale");

            migrationBuilder.RenameColumn(
                name: "TempSaleId",
                table: "Sales.TempSalesProduct",
                newName: "TempSalesHeaderId");

            migrationBuilder.RenameIndex(
                name: "IX_Sales.TempSalesProduct_TempSaleId",
                table: "Sales.TempSalesProduct",
                newName: "IX_Sales.TempSalesProduct_TempSalesHeaderId");

            migrationBuilder.RenameColumn(
                name: "TempSaleId",
                table: "Finance.Invoice",
                newName: "TempSalesHeaderId");

            migrationBuilder.CreateTable(
                name: "Sales.TempSalesHeader",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    CustomerCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CustomerName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DeleterUserId = table.Column<long>(type: "bigint", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DiscountAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DiscountRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    GrossAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true),
                    NetAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TaxAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TaxRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sales.TempSalesHeader", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Sales.TempSalesProduct_Sales.TempSalesHeader_TempSalesHeaderId",
                table: "Sales.TempSalesProduct",
                column: "TempSalesHeaderId",
                principalTable: "Sales.TempSalesHeader",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
