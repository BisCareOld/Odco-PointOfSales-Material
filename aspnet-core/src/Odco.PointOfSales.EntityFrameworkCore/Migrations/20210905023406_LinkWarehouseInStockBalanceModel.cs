using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Odco.PointOfSales.Migrations
{
    public partial class LinkWarehouseInStockBalanceModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "Production.Warehouse",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "WarehouseCode",
                table: "Inventory.StockBalance",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "WarehouseId",
                table: "Inventory.StockBalance",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WarehouseName",
                table: "Inventory.StockBalance",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Code",
                table: "Production.Warehouse");

            migrationBuilder.DropColumn(
                name: "WarehouseCode",
                table: "Inventory.StockBalance");

            migrationBuilder.DropColumn(
                name: "WarehouseId",
                table: "Inventory.StockBalance");

            migrationBuilder.DropColumn(
                name: "WarehouseName",
                table: "Inventory.StockBalance");
        }
    }
}
