using Abp.AutoMapper;
using Odco.PointOfSales.Core.Productions;
using System.ComponentModel.DataAnnotations;

namespace Odco.PointOfSales.Application.Productions.Categories
{
    [AutoMapTo(typeof(Category))]
    public class CreateCategoryDto
    {
        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        public bool IsActive { get; set; }
    }
}
