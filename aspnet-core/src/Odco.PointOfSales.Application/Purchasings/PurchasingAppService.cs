using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq;
using Abp.Linq.Extensions;
using Microsoft.EntityFrameworkCore;
using Odco.PointOfSales.Application.Common.SequenceNumbers;
using Odco.PointOfSales.Application.GeneralDto;
using Odco.PointOfSales.Application.Purchasings.PurchaseOrders;
using Odco.PointOfSales.Application.Purchasings.Suppliers;
using Odco.PointOfSales.Core.Common;
using Odco.PointOfSales.Core.Enums;
using Odco.PointOfSales.Core.Inventory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Odco.PointOfSales.Application.Purchasings
{
    public class PurchasingAppService : ApplicationService, IPurchasingAppService
    {
        private readonly IAsyncQueryableExecuter _asyncQueryableExecuter;
        private readonly IDocumentSequenceNumberManager _documentSequenceNumberManager;
        private readonly IRepository<Supplier, Guid> _supplierRepository;
        private readonly IRepository<PurchaseOrder, Guid> _purchaseOrderRepository;
        private readonly IRepository<StockBalance, Guid> _stockBalanceRepository;

        public PurchasingAppService(IDocumentSequenceNumberManager documentSequenceNumberManager, 
            IRepository<Supplier, Guid> supplierRepository,
            IRepository<PurchaseOrder, Guid> purchaseOrderRepository,
            IRepository<StockBalance, Guid> stockBalanceRepository)
        {
            _asyncQueryableExecuter = NullAsyncQueryableExecuter.Instance;
            _documentSequenceNumberManager = documentSequenceNumberManager;
            _supplierRepository = supplierRepository;
            _purchaseOrderRepository = purchaseOrderRepository;
            _stockBalanceRepository = stockBalanceRepository;
        }


        #region Supplier
        public async Task<SupplierDto> CreateSupplierAsync(CreateSupplierDto input)
        {
            try
            {
                var supplier = ObjectMapper.Map<Supplier>(input);
                var created = await _supplierRepository.InsertAsync(supplier);
                await CurrentUnitOfWork.SaveChangesAsync();
                return ObjectMapper.Map<SupplierDto>(created);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task DeleteSupplierAsync(EntityDto<Guid> input)
        {
            try
            {
                var supplier = _supplierRepository.FirstOrDefault(pt => pt.Id == input.Id); ;
                await _supplierRepository.DeleteAsync(supplier);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<PagedResultDto<SupplierDto>> GetAllSuppliersAsync(PagedSupplierResultRequestDto input)
        {
            try
            {
                var query = _supplierRepository.GetAll()
                        .WhereIf(!input.Keyword.IsNullOrWhiteSpace(),
                        x => x.FirstName.Contains(input.Keyword) || x.LastName.Contains(input.Keyword) || x.MiddleName.Contains(input.Keyword));

                var result = _asyncQueryableExecuter.ToListAsync
                    (
                        query.OrderBy(o => o.CreationTime)
                             .PageBy(input.SkipCount, input.MaxResultCount)
                    );

                var count = await _asyncQueryableExecuter.CountAsync(query);

                var resultDto = ObjectMapper.Map<List<SupplierDto>>(result.Result);
                return new PagedResultDto<SupplierDto>(count, resultDto);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<SupplierDto> GetSupplierAsync(EntityDto<Guid> input)
        {
            try
            {
                var company = _supplierRepository.GetAllIncluding(s => s.Addresses).FirstOrDefault(pt => pt.Id == input.Id);
                return ObjectMapper.Map<SupplierDto>(company);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<SupplierDto> UpdateSupplierAsync(SupplierDto input)
        {
            try
            {
                var supplier = _supplierRepository.GetAll().FirstOrDefault(pt => pt.Id == input.Id);
                ObjectMapper.Map(input, supplier);
                await _supplierRepository.UpdateAsync(supplier);
                await CurrentUnitOfWork.SaveChangesAsync();
                return ObjectMapper.Map<SupplierDto>(supplier);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<CommonKeyValuePairDto>> GetPartialSuppliersAsync(string keyword)
        {
            if (string.IsNullOrEmpty(keyword))
                return new List<CommonKeyValuePairDto>();

            keyword.ToLower();

            var suppliers = await _supplierRepository
                .GetAll()
                .Where(s => s.IsActive && (s.FirstName.ToLower().Contains(keyword) || s.MiddleName.ToLower().Contains(keyword) || s.LastName.ToLower().Contains(keyword) || s.Code.Contains(keyword)))
                .Take(PointOfSalesConsts.MaximumNumberOfReturnResults)
                .ToListAsync();

            return suppliers.Select(s => new CommonKeyValuePairDto
            {
                Id = s.Id,
                Code = s.Code,
                Name = $"{s.FirstName} {s.MiddleName} {s.LastName}"
            }).ToList();
        }
        #endregion

        #region Purchase Order
        public async Task<PurchaseOrderDto> CreatePurchaseOrderAsync(CreatePurchaseOrderDto headerLevel)
        {
            try
            {
                headerLevel.PurchaseOrderNumber = 
                    await _documentSequenceNumberManager.GetAndUpdateNextDocumentNumberAsync(DocumentType.PurchaseOrder);

                var purchaseOrder = ObjectMapper.Map<PurchaseOrder>(headerLevel);

                var created = await _purchaseOrderRepository.InsertAsync(purchaseOrder);

                // Update Stock Balance Table
                foreach (var lineLevel in created.PurchaseOrderProducts)
                {
                    lineLevel.ReferenceNumber = headerLevel.ReferenceNumber;

                    // Define Created PO number (Document Sequent number) again
                    lineLevel.PurchaseOrderNumber = headerLevel.PurchaseOrderNumber;

                    // Two results should be generated
                    // 1. StockBalance with a warehouse
                    // 2. StockBalance without a warehouse
                    var stockBalances = await GetStockBalancesAsync(lineLevel.ProductId, lineLevel.WarehouseId);

                    // Company & Warehouse summary
                    foreach (var sb in stockBalances)
                    {
                        sb.OnOrderQuantity = sb.OnOrderQuantity + lineLevel.OrderQuantity;
                        sb.PurchaseOrderNumber = lineLevel.PurchaseOrderNumber;

                        await _stockBalanceRepository.UpdateAsync(sb);
                    }
                }

                await CurrentUnitOfWork.SaveChangesAsync();

                return new PurchaseOrderDto { Id = created.Id, PurchaseOrderNumber = created.PurchaseOrderNumber};
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Company & Warehouse summary
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="warehouseId"></param>
        /// <returns></returns>
        private async Task<List<StockBalance>> GetStockBalancesAsync(Guid productId, Guid warehouseId)
        {
            return await _stockBalanceRepository
                .GetAll()
                .Where(sb => sb.SequenceNumber == 0 
                    && sb.ProductId == productId
                    && (sb.WarehouseId.HasValue || sb.WarehouseId == warehouseId))
                .ToListAsync();
        }
        #endregion


    }
}
