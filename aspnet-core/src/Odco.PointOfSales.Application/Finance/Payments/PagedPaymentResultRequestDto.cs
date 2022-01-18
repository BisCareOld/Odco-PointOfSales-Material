using Abp.Application.Services.Dto;

namespace Odco.PointOfSales.Application.Finance.Payments
{
    public class PagedPaymentResultRequestDto : PagedResultRequestDto
    {
        public string Keyword { get; set; }
        public bool? IsActive { get; set; }
    }
}