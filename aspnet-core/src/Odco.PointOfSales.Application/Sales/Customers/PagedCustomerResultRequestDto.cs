using Abp.Application.Services.Dto;

namespace Odco.PointOfSales.Application.Sales.Customers
{
    public class PagedCustomerResultRequestDto : PagedResultRequestDto
    {
        public string Keyword { get; set; }
        public bool? IsActive { get; set; }
    }
}