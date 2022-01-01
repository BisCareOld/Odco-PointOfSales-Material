using Abp.Application.Services;
using Odco.PointOfSales.Application.Inventory.StockBalances;
using Odco.PointOfSales.Core.Inventory;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Odco.PointOfSales.Application.StockBalances
{
    public interface IStockBalanceAppService : IApplicationService
    {
        Task<bool> SyncStockBalancesAsync();

        Task<List<GroupBySellingPriceDto>> GetStockBalancesByProductIdGroupBySellingPriceAsync(Guid productId);

        Task<List<StockBalance>> GetStockBalancesByProductIdBasedOnSellingPriceAsync(Guid productId, Guid warehouseId, decimal sellingPrice);

        Task<List<StockBalance>> GetStockBalancesAsync(Guid productId, Guid warehouseId);

        Task<List<StockBalance>> GetStockBalanceSummariesAsync(Guid productId, Guid warehouseId);

        Task<List<StockBalance>> GetStockBalancesByStockBalanceIdAsync(Guid stockBalanceId, Guid productId, Guid warehouseId);

        Task<StockBalance> GetStockBalanceByIdAsync(Guid stockBalanceId);
    }
}
