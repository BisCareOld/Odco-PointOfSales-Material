using Abp.Application.Services.Dto;

namespace Odco.PointOfSales.Application.Productions.Products
{
    public class PagedProductResultRequestDto : PagedResultRequestDto
    {
        public string Keyword { get; set; }
        public bool? IsActive { get; set; }
    }
}
