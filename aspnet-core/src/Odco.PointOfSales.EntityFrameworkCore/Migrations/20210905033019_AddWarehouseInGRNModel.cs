using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Odco.PointOfSales.Migrations
{
    public partial class AddWarehouseInGRNModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "WarehouseCode",
                table: "Inventory.GoodsRecievedProduct",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "WarehouseId",
                table: "Inventory.GoodsRecievedProduct",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "WarehouseName",
                table: "Inventory.GoodsRecievedProduct",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WarehouseCode",
                table: "Inventory.GoodsRecievedProduct");

            migrationBuilder.DropColumn(
                name: "WarehouseId",
                table: "Inventory.GoodsRecievedProduct");

            migrationBuilder.DropColumn(
                name: "WarehouseName",
                table: "Inventory.GoodsRecievedProduct");
        }
    }
}
