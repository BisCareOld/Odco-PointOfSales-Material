using Abp.Application.Services.Dto;

namespace Odco.PointOfSales.Application.Productions.Categories
{
    public class PagedCategoryResultRequestDto : PagedResultRequestDto
    {
        public string Keyword { get; set; }
        public bool? IsActive { get; set; }
    }
}

