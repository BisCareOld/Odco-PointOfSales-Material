using Abp.Application.Services;
using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Odco.PointOfSales.Application.Common.SequenceNumbers;
using Odco.PointOfSales.Application.Inventory.GoodsReceiveNotes;
using Odco.PointOfSales.Core.Enums;
using Odco.PointOfSales.Core.Inventory;
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

        public InventoryAppService(IRepository<GoodsReceived, Guid> goodsReceivedRepository,
            IRepository<StockBalance, Guid> stockBalanceRepository,
            IDocumentSequenceNumberManager documentSequenceNumberManager)
        {
            _goodsReceivedRepository = goodsReceivedRepository;
            _stockBalanceRepository = stockBalanceRepository;
            _documentSequenceNumberManager = documentSequenceNumberManager;
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

                    var stockBalances = await GetStockBalancesAsync(lineLevel.ProductId);

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

                    var stockSummary = stockBalances.FirstOrDefault(sb => sb.SequenceNumber == 0);

                    if (stockSummary != null)
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

        /// <summary>
        /// Company summary + GRN summaries
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        private async Task<List<StockBalance>> GetStockBalancesAsync(Guid productId)
        {
            return await _stockBalanceRepository
                .GetAll()
                .Where(sb => sb.ProductId == productId)
                .ToListAsync();
        }
    }
}
