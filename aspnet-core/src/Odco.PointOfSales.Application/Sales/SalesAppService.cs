using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq;
using Abp.Linq.Extensions;
using Microsoft.EntityFrameworkCore;
using Odco.PointOfSales.Application.GeneralDto;
using Odco.PointOfSales.Application.Inventory.StockBalances;
using Odco.PointOfSales.Application.Productions.Warehouses;
using Odco.PointOfSales.Application.Sales.Customers;
using Odco.PointOfSales.Application.Sales.TemporarySalesHeaders;
using Odco.PointOfSales.Application.Sales.TemporarySalesProducts;
using Odco.PointOfSales.Core.Inventory;
using Odco.PointOfSales.Core.Productions;
using Odco.PointOfSales.Core.Sales;
using Odco.PointOfSales.Sales.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Odco.PointOfSales.Application.Sales
{
    public class SalesAppService : ApplicationService, ISalesAppService
    {
        private readonly IAsyncQueryableExecuter _asyncQueryableExecuter;
        private readonly IRepository<Customer, Guid> _customerRepository;
        private readonly IRepository<TempSalesHeader, int> _tempSalesHeaderRepository;
        private readonly IRepository<TempSalesProduct, int> _tempSaleProductRepository;
        private readonly IRepository<StockBalance, Guid> _stockBalanceRepository;
        private readonly IRepository<Warehouse, Guid> _warehouseRepository;

        public SalesAppService(IRepository<Customer, Guid> customerRepository,
            IRepository<TempSalesHeader, int> tempSalesHeaderRepository,
            IRepository<TempSalesProduct, int> tempSaleProductRepository,
            IRepository<StockBalance, Guid> stockBalanceRepository,
            IRepository<Warehouse, Guid> warehouseRepository)
        {
            _asyncQueryableExecuter = NullAsyncQueryableExecuter.Instance;
            _customerRepository = customerRepository;
            _tempSalesHeaderRepository = tempSalesHeaderRepository;
            _tempSaleProductRepository = tempSaleProductRepository;
            _stockBalanceRepository = stockBalanceRepository;
            _warehouseRepository = warehouseRepository;
        }

        #region Customer
        public async Task<CustomerDto> CreateCustomerAsync(CreateCustomerDto input)
        {
            try
            {
                var customer = ObjectMapper.Map<Customer>(input);
                var created = await _customerRepository.InsertAsync(customer);
                await CurrentUnitOfWork.SaveChangesAsync();
                return ObjectMapper.Map<CustomerDto>(created);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task DeleteCustomerAsync(EntityDto<Guid> input)
        {
            try
            {
                var customer = _customerRepository.FirstOrDefault(pt => pt.Id == input.Id); ;
                await _customerRepository.DeleteAsync(customer);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<PagedResultDto<CustomerDto>> GetAllCustomersAsync(PagedCustomerResultRequestDto input)
        {
            try
            {
                var query = _customerRepository.GetAll()
                        .WhereIf(!input.Keyword.IsNullOrWhiteSpace(),
                        x => x.FirstName.Contains(input.Keyword) || x.MiddleName.Contains(input.Keyword) || x.LastName.Contains(input.Keyword));

                var result = _asyncQueryableExecuter.ToListAsync
                    (
                        query.OrderBy(o => o.CreationTime)
                             .PageBy(input.SkipCount, input.MaxResultCount)
                    );

                var count = await _asyncQueryableExecuter.CountAsync(query);

                var resultDto = ObjectMapper.Map<List<CustomerDto>>(result.Result);
                return new PagedResultDto<CustomerDto>(count, resultDto);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<CustomerDto> GetCustomerAsync(EntityDto<Guid> input)
        {
            try
            {
                var customer = await _customerRepository.GetAllIncluding(s => s.Addresses).FirstOrDefaultAsync(pt => pt.Id == input.Id);
                return ObjectMapper.Map<CustomerDto>(customer);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<CustomerDto> UpdateCustomerAsync(CustomerDto input)
        {
            try
            {
                var customer = _customerRepository.GetAll().FirstOrDefault(pt => pt.Id == input.Id);
                ObjectMapper.Map(input, customer);
                await _customerRepository.UpdateAsync(customer);
                await CurrentUnitOfWork.SaveChangesAsync();
                return ObjectMapper.Map<CustomerDto>(customer);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<CommonKeyValuePairDto>> GetPartialCustomersAsync(string keyword)
        {
            if (string.IsNullOrEmpty(keyword))
                return new List<CommonKeyValuePairDto>();

            keyword.ToLower();

            var customers = await _customerRepository
                .GetAll()
                .Where(s => s.FirstName.ToLower().Contains(keyword) || s.MiddleName.ToLower().Contains(keyword) || s.LastName.ToLower().Contains(keyword) || s.Code.Contains(keyword))
                .Take(20)
                .ToListAsync();

            return customers.Select(s => new CommonKeyValuePairDto
            {
                Id = s.Id,
                Code = s.Code,
                Name = $"{s.FirstName} {s.MiddleName} {s.LastName}"
            }).ToList();
        }
        #endregion

        #region TemporarySales Header + Products
        /// <summary>
        /// 1. Create Temporary Sales Header & Product
        /// 2. StockBalance: Shift sales quantity to allocated quantity
        /// 3. Set Default warehouse to selected products (Line Level Products)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<TempSalesHeaderDto> CreateOrUpdateTempSalesAsync(CreateOrUpdateTempSalesHeaderDto input)
        {
            var warehouse = await _warehouseRepository.FirstOrDefaultAsync(w => w.IsActive && w.IsDefault);
            var _tempHeader = new TempSalesHeader();

            if (input.Id.HasValue)
            {
                var tempSales = await _tempSalesHeaderRepository
                    .GetAllIncluding(t => t.TempSalesProducts)
                    .FirstOrDefaultAsync(t => t.Id == input.Id);

                if (tempSales != null)
                {
                    // AQ = Allocated Qty
                    // BBQ = Book Balance Qty
                    // Actual Quantity  &   AQ
                    // 1. Existing SP   &   Input SP                Result StockBalanceTable
                    // 2.               =                           No need to do anything
                    // 3.               >                           
                    // 4.               <
                    // 5.               !

                    foreach (var existSP in tempSales.TempSalesProducts)
                    {
                        var iSP = input.TempSalesProducts.FirstOrDefault(isp => isp.StockBalanceId == existSP.StockBalanceId);

                        if (iSP != null)
                        {
                            // NOTE: IF exist in DB & exist in new request Dto
                            // 1. Get Company Summary(1) + Warehouse Summary(1) + GRN Summary(1) based on the SBID
                            // 2. Update AQ & BBQ
                            // 3. GRN Summary - Strigth forward set value because Data got from specific StockBalanceId
                            var stockBalances = await GetStockBalancesByStockBalanceIdAsync(iSP.StockBalanceId);

                            if (existSP.Quantity > iSP.Quantity)
                            {
                                foreach (var sb in stockBalances)
                                {
                                    var differences = existSP.Quantity - iSP.Quantity;
                                    sb.AllocatedQuantity = iSP.Quantity;
                                    sb.BookBalanceQuantity += differences;
                                    await _stockBalanceRepository.UpdateAsync(sb);
                                }
                            }
                            else if (existSP.Quantity < iSP.Quantity)
                            {
                                foreach (var sb in stockBalances)
                                {
                                    var differences = iSP.Quantity - existSP.Quantity;
                                    sb.AllocatedQuantity += differences;
                                    sb.BookBalanceQuantity -= differences;
                                    await _stockBalanceRepository.UpdateAsync(sb);
                                }
                            }

                            existSP.DiscountRate = iSP.DiscountRate;
                            existSP.DiscountAmount = iSP.DiscountAmount;
                            existSP.Quantity = iSP.Quantity;
                            existSP.LineTotal = iSP.LineTotal;
                            await _tempSaleProductRepository.UpdateAsync(existSP);
                        }
                        else
                        {
                            // NOTE: IF exist in DB & not exist in new request Dto
                            // 1. Remove / Delete from Line Level
                            // 2. Get Company Summary(1) + Warehouse Summary(1) + GRN Summary(1) based on the SBID
                            // 3. Update (Revert) AQ & BBQ in all 3 rows
                            // 4. GRN Summary (Soft Delete it)
                            var stockBalances = await GetStockBalancesByStockBalanceIdAsync(existSP.StockBalanceId);
                            decimal _AQ = 0; // Related to GRN summary
                            foreach (var sb in stockBalances)
                            {
                                if (sb.SequenceNumber > 0)
                                {
                                    _AQ = existSP.Quantity;

                                    sb.BookBalanceQuantity += _AQ;
                                    sb.AllocatedQuantity -= _AQ;
                                }
                                else
                                {
                                    sb.AllocatedQuantity -= _AQ;
                                    sb.BookBalanceQuantity += _AQ;
                                }
                                await _stockBalanceRepository.UpdateAsync(sb);
                                
                            }
                            await _tempSaleProductRepository.DeleteAsync(existSP);
                        }
                    }

                    // NOTE: IF not exist in DB & exist in new request Dto
                    // 1. Create a product row for existing SalesHeaderId
                    // 2. Update the record in StockBalance Table
                    var addTempSalesProductDto = new List<CreateTempSalesProductDto>();
                    foreach (var iSP in input.TempSalesProducts)
                    {
                        if (!tempSales.TempSalesProducts.Any(existSP => existSP.StockBalanceId == iSP.StockBalanceId))
                            addTempSalesProductDto.Add(iSP);
                    }

                    foreach (var tempSalesProduct in addTempSalesProductDto)
                    {
                        await CreateTempSalesProductAsync(
                            true,
                            input.Id,
                            tempSalesProduct,
                            new WarehouseDto { Id = warehouse.Id, Name = warehouse.Name, Code = warehouse.Code }
                        );
                    }

                    _tempHeader = new TempSalesHeader { Id = tempSales.Id };
                }
            }
            else
            {
                // NOTE: Create a new (TempSales) Header & Line Level Details

                foreach (var lineLevel in input.TempSalesProducts)
                {
                    await CreateTempSalesProductAsync(
                        false,
                        null,
                        lineLevel,
                        new WarehouseDto { Id = warehouse.Id, Name = warehouse.Name, Code = warehouse.Code }
                    );
                }

                var temp = ObjectMapper.Map<TempSalesHeader>(input);
                _tempHeader = await _tempSalesHeaderRepository.InsertAsync(temp);
            }

            await CurrentUnitOfWork.SaveChangesAsync();
            return ObjectMapper.Map<TempSalesHeaderDto>(_tempHeader);
        }

        public async Task<TempSalesHeaderDto> GetTempSalesAsync(int tempSalesHeaderId)
        {
            var temp = await _tempSalesHeaderRepository
                .GetAllIncluding(t => t.TempSalesProducts)
                .FirstOrDefaultAsync(t => t.Id == tempSalesHeaderId);
            return ObjectMapper.Map<TempSalesHeaderDto>(temp);
        }

        private async Task CreateTempSalesProductAsync(bool isExisting, int? existingHeaderId, CreateTempSalesProductDto lineLevel, WarehouseDto warehouse)
        {
            // Set Warehouse Details to Line level 
            lineLevel.WarehouseId = warehouse.Id;
            lineLevel.WarehouseCode = warehouse.Code;
            lineLevel.WarehouseName = warehouse.Name;

            //var stockBalances = await GetStockBalancesAsync(lineLevel.ProductId, warehouse.Id);
            var stockBalances = await GetStockBalancesAsync(lineLevel.ProductId, warehouse.Id);

            // * Increase in Allocated Qty & Decrease in Book Balance Qty
            // 1. Company Summary
            // 2. Warehouse Summary
            // 3. GRN Summary reduce (specific)
            foreach (var sb in stockBalances)
            {
                if (sb.SequenceNumber <= 0 || sb.Id == lineLevel.StockBalanceId)
                {
                    sb.AllocatedQuantity += lineLevel.Quantity;
                    sb.BookBalanceQuantity -= lineLevel.Quantity;
                    await _stockBalanceRepository.UpdateAsync(sb);
                }
            }

            if(isExisting)
            {
                var lineLevelProduct = ObjectMapper.Map<TempSalesProduct>(lineLevel);
                lineLevelProduct.TempSalesHeaderId = existingHeaderId.Value;
                await _tempSaleProductRepository.InsertAsync(lineLevelProduct);
            }
        }
        #endregion

        #region Stock Balance
        public async Task<List<ProductStockBalanceDto>> GetStockBalancesByStockBalanceIdsAsync(Guid[] stockBalanceIds)
        {
            return await _stockBalanceRepository.GetAll()
                    .Where(sb => sb.SequenceNumber > 0 && stockBalanceIds.Contains(sb.Id))
                    .Select(sb => new ProductStockBalanceDto
                    {
                        StockBalanceId = sb.Id,
                        ProductId = sb.ProductId,
                        ExpiryDate = sb.ExpiryDate,
                        BatchNumber = sb.BatchNumber,
                        AllocatedQuantity = sb.AllocatedQuantity,
                        AllocatedQuantityUnitOfMeasureUnit = sb.AllocatedQuantityUnitOfMeasureUnit,
                        BookBalanceQuantity = sb.BookBalanceQuantity,
                        BookBalanceUnitOfMeasureUnit = sb.BookBalanceUnitOfMeasureUnit,
                        CostPrice = sb.CostPrice,
                        SellingPrice = sb.SellingPrice,
                        MaximumRetailPrice = sb.MaximumRetailPrice
                    })
                    .ToListAsync();
        }

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

        /// <summary>
        /// Company summary + Warehouse summaries + GRN summaries based on given Stock balance Id
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="warehouseId"></param>
        /// <returns></returns>
        private async Task<List<StockBalance>> GetStockBalancesByStockBalanceIdAsync(Guid stockBalanceId)
        {
            var returnList = new List<StockBalance>();

            var _sb1 = await _stockBalanceRepository.FirstOrDefaultAsync(sb => sb.Id == stockBalanceId);

            var _sb2 = await _stockBalanceRepository
                .GetAll()
                .Where(sb => sb.ProductId == _sb1.ProductId
                            && sb.SequenceNumber == 0
                            && (!sb.WarehouseId.HasValue || sb.WarehouseId == _sb1.WarehouseId))
                .OrderByDescending(sb => sb.SequenceNumber)
                .ToListAsync();

            returnList.Add(_sb1);
            returnList.AddRange(_sb2);
            return returnList;
        }
        #endregion

    }
}
