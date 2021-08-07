using System.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace Odco.PointOfSales.EntityFrameworkCore
{
    public static class PointOfSalesDbContextConfigurer
    {
        public static void Configure(DbContextOptionsBuilder<PointOfSalesDbContext> builder, string connectionString)
        {
            builder.UseSqlServer(connectionString);
        }

        public static void Configure(DbContextOptionsBuilder<PointOfSalesDbContext> builder, DbConnection connection)
        {
            builder.UseSqlServer(connection);
        }
    }
}
