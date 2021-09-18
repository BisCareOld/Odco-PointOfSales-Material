using Abp.Application.Services.Dto;

namespace Odco.PointOfSales.Application.Inventory.GoodsReceiveNotes
{
    public class PagedGRNResultRequestDto : PagedResultRequestDto
    {
        public string Keyword { get; set; }
        public bool? IsActive { get; set; }
    }
}
