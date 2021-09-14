using Abp.Application.Services.Dto;

namespace Odco.PointOfSales.Application.Productions.Warehouses
{
    public class PagedWarehouseResultRequestDto : PagedResultRequestDto
    {
        public string Keyword { get; set; }
        public bool? IsActive { get; set; }
    }
}
