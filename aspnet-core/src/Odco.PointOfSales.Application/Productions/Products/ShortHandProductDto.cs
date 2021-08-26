using Abp.AutoMapper;
using Odco.PointOfSales.Core.Productions;
using System;
using System.ComponentModel.DataAnnotations;

namespace Odco.PointOfSales.Application.Productions.Products
{
    [AutoMapTo(typeof(ShortHandProductDto)), AutoMapFrom(typeof(Product))]
    public class ShortHandProductDto
    {
        public Guid Id { get; set; }

        [Required]
        [StringLength(15)]
        public string Code { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        public bool IsActive { get; set; }
    }
}
