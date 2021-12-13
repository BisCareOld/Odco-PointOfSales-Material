using Abp.Application.Services.Dto;

namespace Odco.PointOfSales.Application.Sales.Sales
{
    public class PagedSaleResultRequestDto : PagedResultRequestDto
    {
        public string Keyword { get; set; }
        public bool? IsActive { get; set; }
    }
}
