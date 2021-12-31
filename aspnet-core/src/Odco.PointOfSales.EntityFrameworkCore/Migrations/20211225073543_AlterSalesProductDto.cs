using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Odco.PointOfSales.Migrations
{
    public partial class AlterSalesProductDto : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BatchNumber",
                table: "Sales.SalesProduct");

            migrationBuilder.DropColumn(
                name: "BookBalanceQuantity",
                table: "Sales.SalesProduct");

            migrationBuilder.DropColumn(
                name: "BookBalanceUnitOfMeasureUnit",
                table: "Sales.SalesProduct");

            migrationBuilder.DropColumn(
                name: "CostPrice",
                table: "Sales.SalesProduct");

            migrationBuilder.DropColumn(
                name: "ExpiryDate",
                table: "Sales.SalesProduct");

            migrationBuilder.DropColumn(
                name: "IsSelected",
                table: "Sales.SalesProduct");

            migrationBuilder.DropColumn(
                name: "MaximumRetailPrice",
                table: "Sales.SalesProduct");

            migrationBuilder.DropColumn(
                name: "StockBalanceId",
                table: "Sales.SalesProduct");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BatchNumber",
                table: "Sales.SalesProduct",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "BookBalanceQuantity",
                table: "Sales.SalesProduct",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "BookBalanceUnitOfMeasureUnit",
                table: "Sales.SalesProduct",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CostPrice",
                table: "Sales.SalesProduct",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpiryDate",
                table: "Sales.SalesProduct",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsSelected",
                table: "Sales.SalesProduct",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "MaximumRetailPrice",
                table: "Sales.SalesProduct",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<Guid>(
                name: "StockBalanceId",
                table: "Sales.SalesProduct",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }
    }
}
