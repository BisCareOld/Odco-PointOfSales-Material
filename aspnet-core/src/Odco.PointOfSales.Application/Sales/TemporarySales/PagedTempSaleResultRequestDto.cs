using Abp.Application.Services.Dto;

namespace Odco.PointOfSales.Application.Sales.TemporarySales
{
    public class PagedTempSaleResultRequestDto : PagedResultRequestDto
    {
        public string Keyword { get; set; }
        public bool? IsActive { get; set; }
    }
}
