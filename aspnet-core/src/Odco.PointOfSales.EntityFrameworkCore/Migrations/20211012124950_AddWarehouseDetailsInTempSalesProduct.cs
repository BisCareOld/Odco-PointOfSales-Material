using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Odco.PointOfSales.Migrations
{
    public partial class AddWarehouseDetailsInTempSalesProduct : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "WarehouseCode",
                table: "Sales.TempSalesProduct",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "WarehouseId",
                table: "Sales.TempSalesProduct",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WarehouseName",
                table: "Sales.TempSalesProduct",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WarehouseCode",
                table: "Sales.TempSalesProduct");

            migrationBuilder.DropColumn(
                name: "WarehouseId",
                table: "Sales.TempSalesProduct");

            migrationBuilder.DropColumn(
                name: "WarehouseName",
                table: "Sales.TempSalesProduct");
        }
    }
}
