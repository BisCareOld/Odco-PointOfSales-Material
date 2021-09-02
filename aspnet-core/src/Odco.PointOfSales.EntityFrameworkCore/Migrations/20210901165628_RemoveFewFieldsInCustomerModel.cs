using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Odco.PointOfSales.Migrations
{
    public partial class RemoveFewFieldsInCustomerModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sales.Customer_Common.City_CityId",
                table: "Sales.Customer");

            migrationBuilder.DropForeignKey(
                name: "FK_Sales.Customer_Common.PersonTitle_PersonTitleId",
                table: "Sales.Customer");

            migrationBuilder.DropIndex(
                name: "IX_Sales.Customer_CityId",
                table: "Sales.Customer");

            migrationBuilder.DropColumn(
                name: "CityId",
                table: "Sales.Customer");

            migrationBuilder.AlterColumn<Guid>(
                name: "PersonTitleId",
                table: "Sales.Customer",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_Sales.Customer_Common.PersonTitle_PersonTitleId",
                table: "Sales.Customer",
                column: "PersonTitleId",
                principalTable: "Common.PersonTitle",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sales.Customer_Common.PersonTitle_PersonTitleId",
                table: "Sales.Customer");

            migrationBuilder.AlterColumn<Guid>(
                name: "PersonTitleId",
                table: "Sales.Customer",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CityId",
                table: "Sales.Customer",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Sales.Customer_CityId",
                table: "Sales.Customer",
                column: "CityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Sales.Customer_Common.City_CityId",
                table: "Sales.Customer",
                column: "CityId",
                principalTable: "Common.City",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Sales.Customer_Common.PersonTitle_PersonTitleId",
                table: "Sales.Customer",
                column: "PersonTitleId",
                principalTable: "Common.PersonTitle",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
