using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Odco.PointOfSales.Application.Inventory.GoodsReceiveNotes;
using System.Threading.Tasks;

namespace Odco.PointOfSales.Application.Inventory
{
    public interface IInventoryAppService : IApplicationService
    {
        #region Goods Receive Product
        Task<GoodsReceivedDto> CreateGoodsReceivedNoteAsync(CreateGoodsReceivedDto input);
        Task<PagedResultDto<GoodsReceivedDto>> GetAllGoodsReceivedProductsAsync(PagedGRNResultRequestDto input);
        #endregion

        #region Stock Balance
        Task<bool> SyncStockBalancesAsync();
        #endregion
    }
}
