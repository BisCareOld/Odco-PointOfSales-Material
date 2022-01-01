using Abp.Application.Services;
using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Odco.PointOfSales.Application.Inventory.StockBalances;
using Odco.PointOfSales.Core.Inventory;
using Odco.PointOfSales.Core.Productions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Odco.PointOfSales.Application.StockBalances
{
    public class StockBalanceAppService : ApplicationService, IStockBalanceAppService
    {
        private readonly IRepository<StockBalance, Guid> _stockBalanceRepository;
        private readonly IRepository<Warehouse, Guid> _warehouseRepository;
        private readonly IRepository<Product, Guid> _productRepository;

        public StockBalanceAppService(IRepository<StockBalance, Guid> stockBalanceRepository,
            IRepository<Warehouse, Guid> warehouseRepository,
            IRepository<Product, Guid> productRepository)
        {
            _stockBalanceRepository = stockBalanceRepository;
            _warehouseRepository = warehouseRepository;
            _productRepository = productRepository;
        }

        /// <summary>
        ///     Sync Product with warehouses in Stock Balance table
        ///     Creates missing summaries such as company summaries and warehouse summaries
        /// </summary>
        /// <returns></returns>
        public async Task<bool> SyncStockBalancesAsync()
        {
            var products = _productRepository.GetAll().ToList();

            var warehouses = await _warehouseRepository.GetAll().Where(w => w.IsActive).ToListAsync();

            // Company summary + Warehouse summary
            var stockBalances = _stockBalanceRepository.GetAll().Where(sb => sb.SequenceNumber == 0).ToList();

            var createStockBalances = new List<StockBalance>();

            foreach (var p in products)
            {
                foreach (var w in warehouses)
                    // Check Warehouse summary
                    if (!stockBalances.Any(sb => sb.ProductId == p.Id && sb.WarehouseId == w.Id))
                        createStockBalances.Add(new StockBalance
                        {
                            SequenceNumber = 0,
                            ProductId = p.Id,
                            ProductCode = p.Code,
                            ProductName = p.Name,
                            WarehouseId = w.Id,
                            WarehouseCode = w.Code,
                            WarehouseName = w.Name
                        });

                // Check Company summary
                if (!stockBalances.Any(sb => sb.ProductId == p.Id && !sb.WarehouseId.HasValue))
                    createStockBalances.Add(new StockBalance
                    {
                        SequenceNumber = 0,
                        ProductId = p.Id,
                        ProductCode = p.Code,
                        ProductName = p.Name,
                        WarehouseId = null,
                        WarehouseCode = null,
                        WarehouseName = null
                    });
            }

            foreach (var sb in createStockBalances) await _stockBalanceRepository.InsertAsync(sb);

            await CurrentUnitOfWork.SaveChangesAsync();

            return true;
        }

        /// <summary>
        /// GroupBy SellingPrice
        /// If selling prices are equal then sum the BBQ and get the related SB Ids
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        public async Task<List<GroupBySellingPriceDto>> GetStockBalancesByProductIdGroupBySellingPriceAsync(Guid productId)
        {
            try
            {
                var warehouse = await GetWarehouseAsync(null);

                var _stockBalances = await _stockBalanceRepository
                    .GetAll()
                    .Where(sb =>
                        sb.SequenceNumber > 0 &&
                        sb.BookBalanceQuantity > 0 &&
                        sb.ProductId == productId &&
                        sb.WarehouseId == warehouse.Id
                    )
                    .ToListAsync();

                var stockBalanceGroupBy = _stockBalances.GroupBy(sb => sb.SellingPrice).ToList();

                var returnDto = new List<GroupBySellingPriceDto>();

                /// NOTE: If selling prices are equal then sum the BBQ and get the related SB Ids
                foreach (var y in stockBalanceGroupBy)
                {
                    var a = y.FirstOrDefault();

                    returnDto.Add(new GroupBySellingPriceDto
                    {
                        ProductId = a.ProductId,
                        WarehouseId = a.WarehouseId,
                        WarehouseCode = a.WarehouseCode,
                        WarehouseName = a.WarehouseName,
                        TotalBookBalanceQuantity = y.Select(sb => sb.BookBalanceQuantity).Sum(),
                        SellingPrice = y.Key,   // SellingPrice
                        IsSelected = false,
                    });
                }

                return returnDto;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Getting the stockBalances based on the SellingPrices
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="warehouseId"></param>
        /// <param name="sellingPrice"></param>
        /// <returns></returns>
        public async Task<List<StockBalance>> GetStockBalancesByProductIdBasedOnSellingPriceAsync(Guid productId, Guid warehouseId, decimal sellingPrice)
        {
            return await _stockBalanceRepository
                    .GetAll()
                    .Where(sb => sb.SequenceNumber > 0 &&
                        sb.ProductId == productId &&
                        sb.WarehouseId == warehouseId &&
                        sb.SellingPrice == sellingPrice &&
                        sb.BookBalanceQuantity > 0)
                    .ToListAsync();
        }

        /// <summary>
        /// Company summary + Warehouse summaries + GRN Summaries
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="warehouseId"></param>
        /// <returns></returns>
        public async Task<List<StockBalance>> GetStockBalancesAsync(Guid productId, Guid warehouseId)
        {
            return await _stockBalanceRepository
                .GetAll()
                .Where(sb => sb.ProductId == productId && (!sb.WarehouseId.HasValue || sb.WarehouseId == warehouseId))
                .ToListAsync();
        }

        /// <summary>
        /// Company summary + Warehouse summaries
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="warehouseId"></param>
        /// <returns></returns>
        public async Task<List<StockBalance>> GetStockBalanceSummariesAsync(Guid productId, Guid warehouseId)
        {
            return await _stockBalanceRepository
                .GetAll()
                .Where(sb => sb.SequenceNumber == 0 && sb.ProductId == productId && (!sb.WarehouseId.HasValue || sb.WarehouseId == warehouseId))
                .ToListAsync();
        }

        /// <summary>
        /// Company summary + Warehouse summaries + GRN summaries based on given Stock balance Id
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="warehouseId"></param>
        /// <returns></returns>
        public async Task<List<StockBalance>> GetStockBalancesByStockBalanceIdAsync(Guid stockBalanceId, Guid productId, Guid warehouseId)
        {
            return await _stockBalanceRepository.GetAll()
                .Where(sb => sb.Id == stockBalanceId ||
                (sb.SequenceNumber == 0 && sb.ProductId == productId && sb.WarehouseId == warehouseId) ||
                (sb.SequenceNumber == 0 && sb.ProductId == productId && !sb.WarehouseId.HasValue)
            ).ToListAsync();
        }

        public async Task<StockBalance> GetStockBalanceByIdAsync(Guid stockBalanceId)
        {
            return await _stockBalanceRepository.FirstOrDefaultAsync(sb => sb.Id == stockBalanceId);
        }

        #region Warehouse
        private async Task<Warehouse> GetWarehouseAsync(Guid? warehouseId)
        {
            if (warehouseId.HasValue)
                return _warehouseRepository.FirstOrDefault(w => w.Id == warehouseId);
            return _warehouseRepository.FirstOrDefault(w => w.IsDefault);
        }
        #endregion
    }
}
