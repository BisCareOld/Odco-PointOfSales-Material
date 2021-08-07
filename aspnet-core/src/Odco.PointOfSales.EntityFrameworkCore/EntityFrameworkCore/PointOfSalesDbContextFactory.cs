using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Odco.PointOfSales.Configuration;
using Odco.PointOfSales.Web;

namespace Odco.PointOfSales.EntityFrameworkCore
{
    /* This class is needed to run "dotnet ef ..." commands from command line on development. Not used anywhere else */
    public class PointOfSalesDbContextFactory : IDesignTimeDbContextFactory<PointOfSalesDbContext>
    {
        public PointOfSalesDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<PointOfSalesDbContext>();
            var configuration = AppConfigurations.Get(WebContentDirectoryFinder.CalculateContentRootFolder());

            PointOfSalesDbContextConfigurer.Configure(builder, configuration.GetConnectionString(PointOfSalesConsts.ConnectionStringName));

            return new PointOfSalesDbContext(builder.Options);
        }
    }
}
