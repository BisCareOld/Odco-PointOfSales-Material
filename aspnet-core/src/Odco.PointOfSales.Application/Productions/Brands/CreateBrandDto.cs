using Abp.AutoMapper;
using Odco.PointOfSales.Core.Productions;
using System.ComponentModel.DataAnnotations;

namespace Odco.PointOfSales.Application.Productions.Brands
{
    [AutoMapTo(typeof(Brand))]
    public class CreateBrandDto
    {
        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        public bool IsActive { get; set; }
    }
}
