using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Odco.PointOfSales.Core.Productions;
using System;
using System.ComponentModel.DataAnnotations;

namespace Odco.PointOfSales.Application.Productions.Categories
{
    [AutoMapTo(typeof(CategoryDto)), AutoMapFrom(typeof(Category))]
    public class CategoryDto : EntityDto<Guid>
    {
        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        public bool IsActive { get; set; }
    }
}
