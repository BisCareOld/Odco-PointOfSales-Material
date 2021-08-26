using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Odco.PointOfSales.Core.Productions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Odco.PointOfSales.Application.Productions.Products
{
    [AutoMapTo(typeof(ProductDto)), AutoMapFrom(typeof(Product))]
    public class ProductDto : EntityDto<Guid>
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

        public bool IsActive { get; set; }

        //public ICollection<SupplierDto> Suppliers { get; set; }

        //public ProductDto()
        //{
        //    Suppliers = new HashSet<SupplierDto>();
        //}
    }
}
