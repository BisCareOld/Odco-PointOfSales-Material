using Abp.Application.Services;
using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Odco.PointOfSales.Application.Common.SequenceNumbers;
using Odco.PointOfSales.Application.Inventory.GoodsReceiveNotes;
using Odco.PointOfSales.Core.Enums;
using Odco.PointOfSales.Core.Inventory;
using Odco.PointOfSales.Core.Productions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Odco.PointOfSales.Application.Inventory
{
    public class InventoryAppService : ApplicationService, IInventoryAppService
    {
        private readonly IRepository<GoodsReceived, Guid> _goodsReceivedRepository;
        private readonly IRepository<StockBalance, Guid> _stockBalanceRepository;
        private readonly IDocumentSequenceNumberManager _documentSequenceNumberManager;
        private readonly IRepository<Product, Guid> _productRepository;
        private readonly IRepository<Warehouse, Guid> _warehouseRepository;

        public InventoryAppService(IRepository<GoodsReceived, Guid> goodsReceivedRepository,
            IRepository<StockBalance, Guid> stockBalanceRepository,
            IDocumentSequenceNumberManager documentSequenceNumberManager,
            IRepository<Product, Guid> productRepository,
            IRepository<Warehouse, Guid> warehouseRepository)
        {
            _goodsReceivedRepository = goodsReceivedRepository;
            _stockBalanceRepository = stockBalanceRepository;
            _documentSequenceNumberManager = documentSequenceNumberManager;
            _productRepository = productRepository;
            _warehouseRepository = warehouseRepository;
        }

        #region Goods Received Notes
        public async Task<GoodsReceivedDto> CreateGoodsReceivedNoteAsync(CreateGoodsReceivedDto input)
        {
            try
            {
                input.GoodsReceivedNumber = await _documentSequenceNumberManager.GetAndUpdateNextDocumentNumberAsync(DocumentType.GoodsReceivedNote);

                var transaction = ObjectMapper.Map<GoodsReceived>(input);

                var created = await _goodsReceivedRepository.InsertAsync(transaction);

                created.TransactionStatus = TransactionStatus.Approved;

                foreach (var lineLevel in created.GoodsReceivedProducts)
                {
                    lineLevel.GoodsRecievedNumber = created.GoodsReceivedNumber;

                    var stockBalances = await GetStockBalancesAsync(lineLevel.ProductId, lineLevel.WarehouseId);

                    /// Creating a new row in StockBalance table
                    var newStockBalance = new StockBalance
                    {
                        SequenceNumber = stockBalances.Select(sb => sb.SequenceNumber).Max() + 1, // Get the heigest value + increase
                        ProductId = lineLevel.ProductId,
                        ProductCode = lineLevel.ProductCode,
                        ProductName = lineLevel.ProductName,
                        BatchNumber = lineLevel.BatchNumber,
                        ExpiryDate = lineLevel.ExpiryDate,
                        BookBalanceQuantity = lineLevel.Quantity + lineLevel.FreeQuantity,
                        BookBalanceUnitOfMeasureUnit = null,
                        OnOrderQuantity = 0,
                        OnOrderQuantityUnitOfMeasureUnit = null,
                        AllocatedQuantity = 0,
                        AllocatedQuantityUnitOfMeasureUnit = null,
                        ReceivedQuantity = lineLevel.Quantity,
                        ReceivedQuantityUnitOfMeasureUnit = null,
                        CostPrice = lineLevel.CostPrice,
                        SellingPrice = lineLevel.SellingPrice,
                        MaximumRetailPrice = lineLevel.MaximumRetailPrice,
                        GoodsRecievedNumber = created.GoodsReceivedNumber
                    };

                    await _stockBalanceRepository.InsertAsync(newStockBalance);

                    // Company Summary + Warehouse Summary
                    var stockSummaries = stockBalances.Where(sb => sb.SequenceNumber == 0);

                    foreach (var stockSummary in stockSummaries)
                    {
                        stockSummary.BookBalanceQuantity += lineLevel.Quantity + lineLevel.FreeQuantity;
                        stockSummary.ReceivedQuantity += lineLevel.Quantity + lineLevel.FreeQuantity;

                        await _stockBalanceRepository.UpdateAsync(stockSummary);
                    }
                }

                await CurrentUnitOfWork.SaveChangesAsync();
                return ObjectMapper.Map<GoodsReceivedDto>(created);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region Stock Balance
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
        #endregion

        /// <summary>
        /// Company summary + Warehouse summaries + GRN summaries
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="warehouseId"></param>
        /// <returns></returns>
        private async Task<List<StockBalance>> GetStockBalancesAsync(Guid productId, Guid warehouseId)
        {
            return await _stockBalanceRepository
                .GetAll()
                .Where(sb => sb.ProductId == productId && (!sb.WarehouseId.HasValue || sb.WarehouseId == warehouseId))
                .ToListAsync();
        }
    }
}
