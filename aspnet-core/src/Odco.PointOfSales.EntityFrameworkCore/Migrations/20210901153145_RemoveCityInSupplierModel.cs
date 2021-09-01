using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Odco.PointOfSales.Migrations
{
    public partial class RemoveCityInSupplierModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Purchasing.Supplier_Common.City_CityId",
                table: "Purchasing.Supplier");

            migrationBuilder.DropForeignKey(
                name: "FK_Purchasing.Supplier_Common.PersonTitle_PersonTitleId",
                table: "Purchasing.Supplier");

            migrationBuilder.DropIndex(
                name: "IX_Purchasing.Supplier_CityId",
                table: "Purchasing.Supplier");

            migrationBuilder.DropColumn(
                name: "CityId",
                table: "Purchasing.Supplier");

            migrationBuilder.AlterColumn<Guid>(
                name: "PersonTitleId",
                table: "Purchasing.Supplier",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_Purchasing.Supplier_Common.PersonTitle_PersonTitleId",
                table: "Purchasing.Supplier",
                column: "PersonTitleId",
                principalTable: "Common.PersonTitle",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Purchasing.Supplier_Common.PersonTitle_PersonTitleId",
                table: "Purchasing.Supplier");

            migrationBuilder.AlterColumn<Guid>(
                name: "PersonTitleId",
                table: "Purchasing.Supplier",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CityId",
                table: "Purchasing.Supplier",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Purchasing.Supplier_CityId",
                table: "Purchasing.Supplier",
                column: "CityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Purchasing.Supplier_Common.City_CityId",
                table: "Purchasing.Supplier",
                column: "CityId",
                principalTable: "Common.City",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Purchasing.Supplier_Common.PersonTitle_PersonTitleId",
                table: "Purchasing.Supplier",
                column: "PersonTitleId",
                principalTable: "Common.PersonTitle",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
