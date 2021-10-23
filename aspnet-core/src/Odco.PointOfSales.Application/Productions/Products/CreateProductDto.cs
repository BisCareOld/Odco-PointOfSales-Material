using Abp.AutoMapper;
using Odco.PointOfSales.Core.Productions;
using System;
using System.ComponentModel.DataAnnotations;

namespace Odco.PointOfSales.Application.Productions.Products
{
    [AutoMapTo(typeof(Product))]
    public class CreateProductDto
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

        public Guid? CategoryId { get; set; }

        [StringLength(100)]
        public string BarCode { get; set; }

        public int? PackSize { get; set; }

        [Required]
        public decimal ReOrderLevel { get; set; }

        public decimal? DiscountRate { get; set; }

        [Required]
        public decimal ReOrderQuantity { get; set; }

        public decimal CostPrice { get; set; }

        public decimal RetailPrice { get; set; }

        public decimal WholeSalePrice { get; set; }

        public bool IsActive { get; set; }

        public Guid[] SupplierIds { get; set; }
    }
}
