using Microsoft.EntityFrameworkCore.Migrations;
using Odco.PointOfSales.StoredProcedures;

namespace Odco.PointOfSales.Migrations
{
    public partial class Add_SP_sPGetStockBalancesByStockBalanceId : Migration
    {
        private StoredProcedureHelper spHelper = new StoredProcedureHelper();

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var sp = spHelper.GetSP("spGetStockBalancesByStockBalanceId.sql");
            migrationBuilder.Sql(sp);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE spGetStockBalancesByStockBalanceId");
        }
    }
}
