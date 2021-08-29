using Abp.Domain.Entities.Auditing;
using Odco.PointOfSales.Core.IntermediateEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Odco.PointOfSales.Core.Productions
{
    [Table("Production.Product")]
    public class Product : FullAuditedEntity<Guid>
    {
        [Required]
        [StringLength(15)]
        public string Code { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(100)]
        public string Description { get; set; }

        public Guid? BrandId { get; set; }

        public Brand Brand { get; set; }

        /// <summary>
        /// Product : Category = 1 : M // No need this relationship
        /// Product : SubCategory = 1 : M
        /// In UI you should show Categories, Once a category is selected subcategories should be rendered you should store selected SubCategory Only
        /// </summary>
        public Guid? CategoryId { get; set; }

        public Category Category { get; set; }

        [StringLength(100)]
        public string BarCode { get; set; }

        /// <summary>
        /// Eg: A cigaratte pack has 20 cigarattes
        /// </summary>
        public int? PackSize { get; set; }

        /// <summary>
        /// Minimum Stock Level => Once the quantity is below the reorderlevel it should notify for the users
        /// </summary>
        [Required]
        public decimal ReOrderLevel { get; set; }

        /// <summary>
        /// Percentage 1 - 100
        /// </summary>
        public decimal? DiscountRate { get; set; }

        /// <summary>
        /// Once the stock has been reached to reorderlevel => this value will be the quantity in Purchase Order
        /// Could be 0
        /// </summary>
        [Required]
        public decimal ReOrderQuantity { get; set; }

        public bool IsActive { get; set; }

        public virtual ICollection<SupplierProduct> SupplierProducts { get; set; }

        public virtual ICollection<ProductPriceGroup> ProductPriceGroups { get; set; }

        public Product()
        {
            SupplierProducts = new HashSet<SupplierProduct>();

            ProductPriceGroups = new HashSet<ProductPriceGroup>();
        }
    }
}
