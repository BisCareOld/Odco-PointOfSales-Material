using Microsoft.EntityFrameworkCore;
using Abp.Zero.EntityFrameworkCore;
using Odco.PointOfSales.Authorization.Roles;
using Odco.PointOfSales.Authorization.Users;
using Odco.PointOfSales.MultiTenancy;
using Odco.PointOfSales.Core.Productions;
using Odco.PointOfSales.Core.Common;
using Odco.PointOfSales.Core.IntermediateEntities;
using Odco.PointOfSales.Sales.Common;
using Odco.PointOfSales.Core.Inventory;
using Odco.PointOfSales.Core.Sales;
using Odco.PointOfSales.Core.Finance;

namespace Odco.PointOfSales.EntityFrameworkCore
{
    public class PointOfSalesDbContext : AbpZeroDbContext<Tenant, Role, User, PointOfSalesDbContext>
    {
        /* Define a DbSet for each entity of the application */
        #region Common
        public virtual DbSet<Address> Addresses { get; set; }
        public virtual DbSet<City> Cities { get; set; }
        public virtual DbSet<Classification> Classifications { get; set; }
        public virtual DbSet<District> Districts { get; set; }
        public virtual DbSet<DocumentSequenceNumber> DocumentSequenceNumbers { get; set; }
        public virtual DbSet<PersonTitle> PersonTitles { get; set; }
        public virtual DbSet<UnitConversion> UnitConversions { get; set; }
        public virtual DbSet<UnitOfMeasure> UnitOfMeasures { get; set; }
        public virtual DbSet<Bank> Banks { get; set; }
        public virtual DbSet<BankBranch> BankBranches { get; set; }
        #endregion

        #region IntermediateEntities
        public virtual DbSet<ProductPriceGroup> ProductPriceGroups { get; set; }
        public virtual DbSet<SupplierProduct> SupplierProducts { get; set; }
        #endregion

        #region Productions
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<Brand> Brands { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Warehouse> Warehouses { get; set; }
        #endregion

        #region Purchasings
        public virtual DbSet<PurchaseOrder> PurchaseOrders { get; set; }
        public virtual DbSet<PurchaseOrderProduct> GetPurchaseOrderProducts { get; set; }
        public virtual DbSet<Supplier> Suppliers { get; set; }
        #endregion

        #region Sales
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<PriceGroup> PriceGroups { get; set; }
        public virtual DbSet<Sale> TempSale { get; set; }
        public virtual DbSet<InventorySalesProduct> TempSalesProducts { get; set; }
        public virtual DbSet<StockBalancesOfInventorySalesProduct> StockBalancesOfSalesProducts { get; set; }

        #endregion

        #region Inventory
        public virtual DbSet<StockBalance> StockBalances { get; set; }
        public virtual DbSet<GoodsReceived> GoodsReceivedTransactions { get; set; }
        public virtual DbSet<GoodsReceivedProduct> GoodsReceivedProducts { get; set; }
        public virtual DbSet<NonInventoryProduct> NonInventoryProducts { get; set; }
        #endregion

        #region Finance
        public virtual DbSet<Payment> Payments { get; set; }
        public virtual DbSet<CustomerOutstanding> CustomerOutstandings { get; set; }
        public virtual DbSet<CustomerOutstandingSettlement> CustomerOutstandingSettlements { get; set; }
        #endregion

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
