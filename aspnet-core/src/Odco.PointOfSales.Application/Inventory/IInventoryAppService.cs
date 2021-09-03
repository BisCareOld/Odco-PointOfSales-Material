using Abp.Application.Services;
using Odco.PointOfSales.Application.Inventory.GoodsReceiveNotes;
using System.Threading.Tasks;

namespace Odco.PointOfSales.Application.Inventory
{
    public interface IInventoryAppService : IApplicationService
    {
        Task<GoodsReceivedDto> CreateGoodsReceivedNoteAsync(CreateGoodsReceivedDto input);
    }
}
