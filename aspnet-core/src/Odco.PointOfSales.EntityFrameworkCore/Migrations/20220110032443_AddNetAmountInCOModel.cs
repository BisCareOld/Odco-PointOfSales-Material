using Microsoft.EntityFrameworkCore.Migrations;

namespace Odco.PointOfSales.Migrations
{
    public partial class AddNetAmountInCOModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "NetAmount",
                table: "Finance.CustomerOutstanding",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NetAmount",
                table: "Finance.CustomerOutstanding");
        }
    }
}
