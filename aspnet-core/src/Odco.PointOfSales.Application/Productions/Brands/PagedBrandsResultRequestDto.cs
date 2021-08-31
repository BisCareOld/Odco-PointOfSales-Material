using Abp.Application.Services.Dto;

namespace Odco.PointOfSales.Application.Productions.Brands
{
    public class PagedBrandsResultRequestDto : PagedResultRequestDto
    {
        public string Keyword { get; set; }
        public bool? IsActive { get; set; }
    }
}
