using Microsoft.EntityFrameworkCore;
using Abp.Zero.EntityFrameworkCore;
using Odco.PointOfSales.Authorization.Roles;
using Odco.PointOfSales.Authorization.Users;
using Odco.PointOfSales.MultiTenancy;

namespace Odco.PointOfSales.EntityFrameworkCore
{
    public class PointOfSalesDbContext : AbpZeroDbContext<Tenant, Role, User, PointOfSalesDbContext>
    {
        /* Define a DbSet for each entity of the application */
        
        public PointOfSalesDbContext(DbContextOptions<PointOfSalesDbContext> options)
            : base(options)
        {
        }
    }
}
