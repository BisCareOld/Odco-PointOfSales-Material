using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Odco.PointOfSales.Migrations
{
    public partial class AddPurchaseOrderModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CanceledEmployeeCode",
                table: "Purchasing.PurchaseOrderProduct");

            migrationBuilder.DropColumn(
                name: "CanceledEmployeeId",
                table: "Purchasing.PurchaseOrderProduct");

            migrationBuilder.DropColumn(
                name: "CanceledEmployeeName",
                table: "Purchasing.PurchaseOrderProduct");

            migrationBuilder.DropColumn(
                name: "ApprovalApprovalEmployeeCode",
                table: "Purchasing.PurchaseOrder");

            migrationBuilder.DropColumn(
                name: "ApprovalApprovalEmployeeName",
                table: "Purchasing.PurchaseOrder");

            migrationBuilder.DropColumn(
                name: "ApprovalEmployeeId",
                table: "Purchasing.PurchaseOrder");

            migrationBuilder.DropColumn(
                name: "BillingAddressCity",
                table: "Purchasing.PurchaseOrder");

            migrationBuilder.DropColumn(
                name: "BillingAddressId",
                table: "Purchasing.PurchaseOrder");

            migrationBuilder.DropColumn(
                name: "BillingAddressLine1",
                table: "Purchasing.PurchaseOrder");

            migrationBuilder.DropColumn(
                name: "BillingAddressLine2",
                table: "Purchasing.PurchaseOrder");

            migrationBuilder.DropColumn(
                name: "BillingAddressPostalCode",
                table: "Purchasing.PurchaseOrder");

            migrationBuilder.DropColumn(
                name: "OnbehalfOfEmployeeCode",
                table: "Purchasing.PurchaseOrder");

            migrationBuilder.DropColumn(
                name: "OnbehalfOfEmployeeId",
                table: "Purchasing.PurchaseOrder");

            migrationBuilder.DropColumn(
                name: "OnbehalfOfEmployeeName",
                table: "Purchasing.PurchaseOrder");

            migrationBuilder.RenameColumn(
                name: "UnitPrice",
                table: "Purchasing.PurchaseOrderProduct",
                newName: "FreeQuantity");

            migrationBuilder.RenameColumn(
                name: "TotalAmount",
                table: "Purchasing.PurchaseOrder",
                newName: "GrossAmount");

            migrationBuilder.AlterColumn<string>(
                name: "Remarks",
                table: "Purchasing.PurchaseOrderProduct",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CostPrice",
                table: "Purchasing.PurchaseOrderProduct",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "ReferenceNumber",
                table: "Purchasing.PurchaseOrderProduct",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WarehouseCode",
                table: "Purchasing.PurchaseOrderProduct",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "WarehouseId",
                table: "Purchasing.PurchaseOrderProduct",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "WarehouseName",
                table: "Purchasing.PurchaseOrderProduct",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CostPrice",
                table: "Purchasing.PurchaseOrderProduct");

            migrationBuilder.DropColumn(
                name: "ReferenceNumber",
                table: "Purchasing.PurchaseOrderProduct");

            migrationBuilder.DropColumn(
                name: "WarehouseCode",
                table: "Purchasing.PurchaseOrderProduct");

            migrationBuilder.DropColumn(
                name: "WarehouseId",
                table: "Purchasing.PurchaseOrderProduct");

            migrationBuilder.DropColumn(
                name: "WarehouseName",
                table: "Purchasing.PurchaseOrderProduct");

            migrationBuilder.RenameColumn(
                name: "FreeQuantity",
                table: "Purchasing.PurchaseOrderProduct",
                newName: "UnitPrice");

            migrationBuilder.RenameColumn(
                name: "GrossAmount",
                table: "Purchasing.PurchaseOrder",
                newName: "TotalAmount");

            migrationBuilder.AlterColumn<string>(
                name: "Remarks",
                table: "Purchasing.PurchaseOrderProduct",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CanceledEmployeeCode",
                table: "Purchasing.PurchaseOrderProduct",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CanceledEmployeeId",
                table: "Purchasing.PurchaseOrderProduct",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CanceledEmployeeName",
                table: "Purchasing.PurchaseOrderProduct",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApprovalApprovalEmployeeCode",
                table: "Purchasing.PurchaseOrder",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApprovalApprovalEmployeeName",
                table: "Purchasing.PurchaseOrder",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ApprovalEmployeeId",
                table: "Purchasing.PurchaseOrder",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BillingAddressCity",
                table: "Purchasing.PurchaseOrder",
                type: "nvarchar(75)",
                maxLength: 75,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "BillingAddressId",
                table: "Purchasing.PurchaseOrder",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "BillingAddressLine1",
                table: "Purchasing.PurchaseOrder",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BillingAddressLine2",
                table: "Purchasing.PurchaseOrder",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BillingAddressPostalCode",
                table: "Purchasing.PurchaseOrder",
                type: "nvarchar(7)",
                maxLength: 7,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "OnbehalfOfEmployeeCode",
                table: "Purchasing.PurchaseOrder",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "OnbehalfOfEmployeeId",
                table: "Purchasing.PurchaseOrder",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OnbehalfOfEmployeeName",
                table: "Purchasing.PurchaseOrder",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }
    }
}
