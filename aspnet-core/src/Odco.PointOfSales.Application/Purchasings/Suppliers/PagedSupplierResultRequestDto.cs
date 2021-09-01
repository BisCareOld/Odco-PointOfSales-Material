using Abp.Application.Services.Dto;

namespace Odco.PointOfSales.Application.Purchasings.Suppliers
{
    public class PagedSupplierResultRequestDto : PagedResultRequestDto
    {
        public string Keyword { get; set; }
        public bool? IsActive { get; set; }
    }
}
