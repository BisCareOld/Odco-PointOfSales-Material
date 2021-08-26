using Microsoft.EntityFrameworkCore;
using Abp.Zero.EntityFrameworkCore;
using Odco.PointOfSales.Authorization.Roles;
using Odco.PointOfSales.Authorization.Users;
using Odco.PointOfSales.MultiTenancy;
using Odco.PointOfSales.Core.Productions;

namespace Odco.PointOfSales.EntityFrameworkCore
{
    public class PointOfSalesDbContext : AbpZeroDbContext<Tenant, Role, User, PointOfSalesDbContext>
    {
        /* Define a DbSet for each entity of the application */
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<Brand> Brands { get; set; }
        public virtual DbSet<Category> Categories { get; set; }

        public PointOfSalesDbContext(DbContextOptions<PointOfSalesDbContext> options)
            : base(options)
        {

        }

        //protected override void OnModelCreating(DbModelBuilder modelBuilder)
        //{
        //    base.OnModelCreating(modelBuilder);

        //    modelBuilder.Entity<Prod>().HasRequired(c => c.SalesPerson).WithMany(s => s.SalesPersonSalesAreas).WillCascadeOnDelete(false);
            


        //}
    }
}
