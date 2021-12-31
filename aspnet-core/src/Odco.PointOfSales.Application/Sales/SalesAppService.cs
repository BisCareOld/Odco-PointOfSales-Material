using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq;
using Abp.Linq.Extensions;
using Microsoft.EntityFrameworkCore;
using Odco.PointOfSales.Application.Common.SequenceNumbers;
using Odco.PointOfSales.Application.Finance.Payments.PaymentTypes;
using Odco.PointOfSales.Application.Inventory.NonInventoryProducts;
using Odco.PointOfSales.Application.Inventory.StockBalances;
using Odco.PointOfSales.Application.Productions.Warehouses;
using Odco.PointOfSales.Application.Sales.Customers;
using Odco.PointOfSales.Application.Sales.Sales;
using Odco.PointOfSales.Application.Sales.SalesProducts;
using Odco.PointOfSales.Core.Enums;
using Odco.PointOfSales.Core.Finance;
using Odco.PointOfSales.Core.Inventory;
using Odco.PointOfSales.Core.Productions;
using Odco.PointOfSales.Core.Sales;
using Odco.PointOfSales.Core.StoredProcedures;
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
        private readonly IRepository<Sale, Guid> _saleRepository;
        private readonly IRepository<SalesProduct, Guid> _saleProductRepository;
        private readonly IRepository<StockBalance, Guid> _stockBalanceRepository;
        private readonly IRepository<Warehouse, Guid> _warehouseRepository;
        private readonly IRepository<NonInventoryProduct, Guid> _nonInventoryProductRepository;
        private readonly IDocumentSequenceNumberManager _documentSequenceNumberManager;
        private readonly IRepository<Payment, Guid> _paymentRepository;
        private readonly IRepository<StockBalancesOfSalesProduct, long> _stockBalancesOfSalesProductRepository;
        private readonly IStoredProcedureAppService _storedProcedureAppService;

        public SalesAppService(IRepository<Customer, Guid> customerRepository,
            IRepository<Sale, Guid> saleRepository,
            IRepository<SalesProduct, Guid> saleProductRepository,
            IRepository<StockBalance, Guid> stockBalanceRepository,
            IRepository<Warehouse, Guid> warehouseRepository,
            IRepository<NonInventoryProduct, Guid> nonInventoryProductRepository,
            IDocumentSequenceNumberManager documentSequenceNumberManager,
            IRepository<Payment, Guid> paymentRepository,
            IRepository<StockBalancesOfSalesProduct, long> stockBalancesOfSalesProductRepository,
            IStoredProcedureAppService storedProcedureAppService
            )
        {
            _asyncQueryableExecuter = NullAsyncQueryableExecuter.Instance;
            _customerRepository = customerRepository;
            _saleRepository = saleRepository;
            _saleProductRepository = saleProductRepository;
            _stockBalanceRepository = stockBalanceRepository;
            _warehouseRepository = warehouseRepository;
            _nonInventoryProductRepository = nonInventoryProductRepository;
            _documentSequenceNumberManager = documentSequenceNumberManager;
            _paymentRepository = paymentRepository;
            _stockBalancesOfSalesProductRepository = stockBalancesOfSalesProductRepository;
            _storedProcedureAppService = storedProcedureAppService;
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

        public async Task<List<CustomerSearchResultDto>> GetPartialCustomersAsync(string keyword)
        {
            if (string.IsNullOrEmpty(keyword))
                return new List<CustomerSearchResultDto>();

            keyword.ToLower();

            var customers = _customerRepository
                .GetAll()
                .Where(s => s.Code.Contains(keyword) || s.FirstName.ToLower().Contains(keyword) ||
                    (string.IsNullOrWhiteSpace(s.MiddleName) && s.MiddleName.ToLower().Contains(keyword)) ||
                    (string.IsNullOrWhiteSpace(s.LastName) && s.LastName.ToLower().Contains(keyword)) ||
                    s.ContactNumber1.Contains(keyword) || s.ContactNumber2.Contains(keyword) || s.ContactNumber3.Contains(keyword)
                );

            return await customers.OrderBy(c => c.FirstName).Take(20).Select(c => new CustomerSearchResultDto
            {
                Id = c.Id,
                Code = c.Code,
                Name = $"{c.FirstName} {c.MiddleName} {c.LastName}",
                ContactNumber1 = c.ContactNumber1,
                ContactNumber2 = c.ContactNumber2,
                ContactNumber3 = c.ContactNumber3,
                IsActive = c.IsActive
            }).ToListAsync();
        }
        #endregion

        #region Sales Header + Products
        /// <summary>
        /// 1. Create Sales Header & Product
        /// 2. StockBalance: Shift sales quantity to allocated quantity
        /// 3. Set Default warehouse to selected products (Line Level Products)
        /// 4. NonInventoryProduct: Adding a quantity is missed
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<SaleDto> CreateOrUpdateSalesAsync(CreateOrUpdateSaleDto input)
        {
            try
            {
                var warehouse = await _warehouseRepository.FirstOrDefaultAsync(w => w.IsActive && w.IsDefault);
                var _saleHeader = new Sale();

                #region Payment
                // NOTE:
                // 1. Make Payment
                // 2. Getting the SalesNumber
                if (input.Id.HasValue && (input.Cashes.Any() || input.Cheques.Any() || input.Outstandings.Any() || input.DebitCards.Any() || input.GiftCards.Any()))
                {
                    decimal? t = 0;
                    t += input.Cashes?.Select(p => p.CashAmount)?.Sum();
                    t += input.Cheques?.Select(p => p.ChequeAmount)?.Sum();
                    t += input.GiftCards?.Select(p => p.GiftCardAmount)?.Sum();

                    input.PaymentStatus = await GetPaymentStatusBySaleId(input.Id.Value, input.NetAmount, t.Value);

                    input.SalesNumber = await _documentSequenceNumberManager.GetAndUpdateNextDocumentNumberAsync(DocumentType.Sales);
                    input.SalesProducts.ToList().ForEach(sp => sp.SalesNumber = input.SalesNumber);
                    await CreatePaymentForSalesIdAsync(input.Id.Value, input.SalesNumber, input.CustomerId, input.CustomerCode, input.CustomerName, input.Cashes.ToArray(), input.Cheques.ToArray(), input.Outstandings.ToArray(), input.DebitCards.ToArray(), input.GiftCards.ToArray());
                }
                else
                {
                    input.PaymentStatus = PaymentStatus.NotPurchased;
                }
                #endregion

                #region Sale
                var s = ObjectMapper.Map<Sale>(input);
                s.SalesProducts.Clear();
                _saleHeader.Id = await _saleRepository.InsertOrUpdateAndGetIdAsync(s);
                //if (!input.Id.HasValue)
                //{
                //    _saleHeader.Id = await _saleRepository.InsertAndGetIdAsync(s);
                //}
                //else
                //{
                //    _saleHeader.Id = await _saleRepository.InsertOrUpdateAndGetIdAsync(s);
                //}
                #endregion

                #region SalesProduct = InventoryTransaction
                input.SalesProducts.ToList().ForEach(isp =>
                {
                    isp.SaleId = _saleHeader.Id;
                });

                await CreateOrUpdateSalesProductsAsync(_saleHeader.Id, input.SalesNumber, input.SalesProducts.ToList());
                #endregion

                #region SalesProduct = InventoryProduct
                //if (input.Id.HasValue)
                //{
                //    // 1. Get the Existing Sales & SalesProduct
                //    // 2. If Null create it
                //    // 3. If Not Null
                //    // 4. Get All StockBalancesOfSalesProduct using SaleId
                //    // 5. 
                //    // 
                //    // 
                //    // 
                //    // 
                //    // 

                //    var existingSale = await _saleRepository
                //        .GetAllIncluding(t => t.SalesProducts)
                //        .FirstOrDefaultAsync(t => t.Id == input.Id.Value);

                //    if (existingSale != null)
                //    {
                //        var existingStockBalancesOfSalesProducts = await _stockBalancesOfSalesProductRepository
                //            .GetAll()
                //            .Where(sbsp => sbsp.SaleId == existingSale.Id)
                //            .ToListAsync();

                //        var stockBalanceIdsOfSBSP = existingStockBalancesOfSalesProducts.Select(sbsp => sbsp.StockBalanceId)?.ToArray();

                //        // Company Summaries, Warehouse Summaries & GRN Rows 
                //        var stockBalances = new List<StockBalance>();

                //        if (stockBalanceIdsOfSBSP.Any())
                //        {
                //            stockBalances = await _storedProcedureAppService.GetStockBalancesByStockBalanceId(stockBalanceIdsOfSBSP);
                //        }

                //        foreach (var esp in existingSale.SalesProducts)
                //        {
                //            var isp = input.SalesProducts.FirstOrDefault(i => i.Id == esp.Id);

                //            if (isp != null)
                //            {
                //                var esbsp = existingStockBalancesOfSalesProducts.Where(sbsp => sbsp.SalesProductId == isp.Id).ToList();

                //                if (esbsp.Any())
                //                {
                //                    var quantityTaken = esbsp.Select(sbsp => sbsp.QuantityTaken).Sum();

                //                    if (quantityTaken != isp.Quantity)
                //                    {
                //                        // Quantity has been reduced in the new request
                //                        if (quantityTaken > isp.Quantity)
                //                        {
                //                            var quantityDifferentiation = quantityTaken - isp.Quantity;
                //                            var reducedQuantity = quantityDifferentiation;

                //                            foreach (var sbsp in esbsp)
                //                            {
                //                                var sbs = stockBalances
                //                                    .Where(sb => sb.Id == sbsp.StockBalanceId ||
                //                                        (sb.SequenceNumber == 0 && sb.ProductId == sbsp.ProductId && sb.WarehouseId == sbsp.WarehouseId) ||
                //                                        (sb.SequenceNumber == 0 && sb.ProductId == sbsp.ProductId && !sb.WarehouseId.HasValue))
                //                                    .ToList();

                //                                if (sbsp.QuantityTaken > reducedQuantity)
                //                                {
                //                                    await ASD(sbs, -1 * reducedQuantity, false);
                //                                    sbsp.QuantityTaken -= reducedQuantity;
                //                                    reducedQuantity = 0;
                //                                }
                //                                else
                //                                {
                //                                    await ASD(sbs, -1 * sbsp.QuantityTaken, false);
                //                                    reducedQuantity -= sbsp.QuantityTaken;
                //                                    sbsp.QuantityTaken = 0;
                //                                }

                //                                if (reducedQuantity == 0)
                //                                    break;
                //                            }
                //                        }
                //                        else
                //                        {
                //                            // Quantity has been increased in the new request

                //                        }
                //                    }

                //                }
                //                else
                //                {
                //                    // Update StockBalance BBQ reduce
                //                    // Create StockBalancesOfSalesProduct 

                //                    var quantity = isp.Quantity;
                //                    var takenQuantity = 0;

                //                    var _stockBalances = _stockBalanceRepository
                //                        .GetAll()
                //                        .Where(sb => sb.ProductId == isp.ProductId &&
                //                            (sb.SequenceNumber == 0 && sb.WarehouseId == isp.WarehouseId) ||
                //                            (sb.SequenceNumber == 0 && !sb.WarehouseId.HasValue) ||
                //                            (sb.SequenceNumber > 0 && sb.WarehouseId == isp.WarehouseId && sb.BookBalanceQuantity > 0 && sb.SellingPrice == isp.SellingPrice))
                //                        .ToList();

                //                    var _grnRows = _stockBalances.Where(sb => sb.SequenceNumber > 0).ToList();

                //                    foreach (var sb in _grnRows)
                //                    {
                //                        if (takenQuantity < sb.BookBalanceQuantity)
                //                        {
                //                            sb.BookBalanceQuantity -= takenQuantity;
                //                            takenQuantity = 0;
                //                            await _stockBalanceRepository.UpdateAsync(sb);
                //                        }
                //                        else
                //                        {

                //                        }
                //                    }



                //                }


                //                ObjectMapper.Map(isp, esp);
                //                await _saleProductRepository.UpdateAsync(esp);
                //            }
                //            else
                //            {
                //                // Create a Line Level (SalesProduct)
                //                // Create Data for StockBalancesOfSalesProduct linked to Line Level (SalesProduct)
                //            }
                //        }
                //    }
                //    else
                //    {
                //        // Create Header & Line Level (SalesProduct)
                //        // Create Data for StockBalancesOfSalesProduct linked to Line Level (SalesProduct)
                //    }
                //}
                //else
                //{
                //    // Create Header & Line Level (SalesProduct)
                //    // Create Data for StockBalancesOfSalesProduct linked to Line Level (SalesProduct)
                //}
                #endregion

                #region SalesProduct = InventoryProduct
                //if (input.Id.HasValue)
                //{
                //    var existingSale = await _saleRepository
                //        .GetAllIncluding(t => t.SalesProducts)
                //        .FirstOrDefaultAsync(t => t.Id == input.Id.Value);

                //    // NOTE: Update existing Sale & Line Level Details (InventoryProduct & NonInventoryProduct)
                //    if (existingSale != null)
                //    {
                //        // AQ = Allocated Qty
                //        // BBQ = Book Balance Qty
                //        // Actual Quantity  &   AQ
                //        // 1. Existing SP   &   Input SP                Result StockBalanceTable
                //        // 2.               =                           No need to do anything
                //        // 3.               >                           
                //        // 4.               <
                //        // 5.               !

                //        foreach (var existSP in existingSale.SalesProducts)
                //        {
                //            var iSP = input.SalesProducts.FirstOrDefault(isp => isp.StockBalanceId == existSP.StockBalanceId);

                //            if (iSP != null)
                //            {
                //                // NOTE: IF exist in DB & exist in new request Dto
                //                // 1. Get Company Summary(1) + Warehouse Summary(1) + GRN Summary(1) based on the SBID
                //                // 2. Update AQ & BBQ
                //                // 3. GRN Summary - Strigth forward set value because Data got from specific StockBalanceId
                //                var stockBalances = await GetStockBalancesByStockBalanceIdAsync(iSP.StockBalanceId, iSP.ProductId, iSP.WarehouseId.Value);

                //                if (existSP.Quantity > iSP.Quantity)
                //                {
                //                    foreach (var sb in stockBalances)
                //                    {
                //                        var differences = existSP.Quantity - iSP.Quantity;
                //                        sb.AllocatedQuantity = iSP.Quantity;
                //                        sb.BookBalanceQuantity += differences;
                //                        await _stockBalanceRepository.UpdateAsync(sb);
                //                    }
                //                }
                //                else if (existSP.Quantity < iSP.Quantity)
                //                {
                //                    foreach (var sb in stockBalances)
                //                    {
                //                        var differences = iSP.Quantity - existSP.Quantity;
                //                        sb.AllocatedQuantity += differences;
                //                        sb.BookBalanceQuantity -= differences;
                //                        await _stockBalanceRepository.UpdateAsync(sb);
                //                    }
                //                }

                //                existSP.SalesNumber = input.SalesNumber;
                //                existSP.Price = iSP.Price;
                //                existSP.DiscountRate = iSP.DiscountRate;
                //                existSP.DiscountAmount = iSP.DiscountAmount;
                //                existSP.Quantity = iSP.Quantity;
                //                existSP.LineTotal = iSP.LineTotal;
                //                await _saleProductRepository.UpdateAsync(existSP);
                //            }
                //            else
                //            {
                //                // NOTE: IF exist in DB & not exist in new request Dto
                //                // 1. Remove / Delete from Line Level
                //                // 2. Get Company Summary(1) + Warehouse Summary(1) + GRN Summary(1) based on the SBID
                //                // 3. Update (Revert) AQ & BBQ in all 3 rows
                //                // 4. GRN Summary (Soft Delete it)
                //                var stockBalances = await GetStockBalancesByStockBalanceIdAsync(existSP.StockBalanceId, existSP.ProductId, existSP.WarehouseId.Value);
                //                decimal _AQ = 0; // Related to GRN summary
                //                foreach (var sb in stockBalances)
                //                {
                //                    if (sb.SequenceNumber > 0)
                //                    {
                //                        _AQ = existSP.Quantity;

                //                        sb.BookBalanceQuantity += _AQ;
                //                        sb.AllocatedQuantity -= _AQ;
                //                    }
                //                    else
                //                    {
                //                        sb.AllocatedQuantity -= _AQ;
                //                        sb.BookBalanceQuantity += _AQ;
                //                    }
                //                    await _stockBalanceRepository.UpdateAsync(sb);

                //                }
                //                await _saleProductRepository.DeleteAsync(existSP);
                //            }
                //        }

                //        // NOTE: IF not exist in DB & exist in new request Dto
                //        // 1. Create a product row for existing TempSaleId
                //        // 2. Update the record in StockBalance Table
                //        var addTempSalesProductDto = new List<CreateSalesProductDto>();
                //        foreach (var iSP in input.SalesProducts)
                //        {
                //            if (!existingSale.SalesProducts.Any(existSP => existSP.StockBalanceId == iSP.StockBalanceId))
                //                addTempSalesProductDto.Add(iSP);
                //        }

                //        foreach (var tempSalesProduct in addTempSalesProductDto)
                //        {
                //            await CreateSalesProductAsync(
                //                true,
                //                input.Id,
                //                tempSalesProduct,
                //                new WarehouseDto { Id = warehouse.Id, Name = warehouse.Name, Code = warehouse.Code }
                //            );
                //        }

                //        existingSale.SalesNumber = input.SalesNumber;
                //        existingSale.ReferenceNumber = input.ReferenceNumber;
                //        existingSale.CustomerId = input.CustomerId;
                //        existingSale.CustomerCode = input.CustomerCode;
                //        existingSale.CustomerName = input.CustomerName;
                //        existingSale.DiscountRate = input.DiscountRate;
                //        existingSale.DiscountAmount = input.DiscountAmount;
                //        existingSale.TaxRate = input.TaxRate;
                //        existingSale.TaxAmount = input.TaxAmount;
                //        existingSale.GrossAmount = input.GrossAmount;
                //        existingSale.NetAmount = input.NetAmount;
                //        existingSale.Remarks = input.Remarks;
                //        existingSale.PaymentStatus = input.PaymentStatus;
                //        existingSale.IsActive = input.IsActive;

                //        await _saleRepository.UpdateAsync(existingSale);
                //        _saleHeader = new Sale { Id = existingSale.Id };
                //    }
                //}
                //else
                //{
                //    // NOTE: Create a new Sale & Line Level Details (InventoryProduct & NonInventoryProduct)
                //    foreach (var lineLevel in input.SalesProducts)
                //    {
                //        await CreateSalesProductAsync(
                //            false,
                //            null,
                //            lineLevel,
                //            new WarehouseDto { Id = warehouse.Id, Name = warehouse.Name, Code = warehouse.Code }
                //        );
                //    }

                //    var sale = ObjectMapper.Map<Sale>(input);
                //    _saleHeader.Id = await _saleRepository.InsertAndGetIdAsync(sale);
                //}
                #endregion

                #region NonInventoryProduct
                // Create / Update / Delete NonInventoryProduct
                await CreateOrUpdateNonInventoryProductAsync(_saleHeader.Id, input.SalesNumber, input.NonInventoryProducts.ToList());
                #endregion

                await CurrentUnitOfWork.SaveChangesAsync();
                return new SaleDto { Id = _saleHeader.Id }; //ObjectMapper.Map<TempSaleDto>(_tempHeader);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<PagedResultDto<SaleDto>> GetAllSalesAsync(PagedSaleResultRequestDto input)
        {
            try
            {
                var query = _saleRepository.GetAll()
                        .WhereIf(!input.Keyword.IsNullOrWhiteSpace(),
                        x => x.CustomerName.Contains(input.Keyword));

                var result = _asyncQueryableExecuter.ToListAsync
                    (
                        query.OrderByDescending(o => o.CreationTime)
                             .PageBy(input.SkipCount, input.MaxResultCount)
                    );

                var count = await _asyncQueryableExecuter.CountAsync(query);

                var resultDto = ObjectMapper.Map<List<SaleDto>>(result.Result);
                return new PagedResultDto<SaleDto>(count, resultDto);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<SaleDto> GetSalesAsync(Guid saleId)
        {
            try
            {
                var temp = await _saleRepository
                    .GetAllIncluding(t => t.SalesProducts)
                    .FirstOrDefaultAsync(t => t.Id == saleId);
                var tempDto = ObjectMapper.Map<SaleDto>(temp);

                var stockBalancesOfSalesProducts = await _stockBalancesOfSalesProductRepository
                    .GetAll()
                    .Where(sbsp => sbsp.SaleId == saleId)
                    .ToListAsync();

                foreach (var sp in tempDto.SalesProducts)
                {
                    sp.ReceivedQuantity = stockBalancesOfSalesProducts
                        .Where(sbsp => sbsp.ProductId == sp.ProductId && sbsp.SellingPrice == sp.SellingPrice)?
                        .Select(sbsp => sbsp.QuantityTaken)?
                        .Sum() ?? 0;

                    sp.TotalBookBalanceQuantity = _stockBalanceRepository
                        .GetAll()
                        .Where(sb => sb.SequenceNumber > 0 && sb.WarehouseId == sp.WarehouseId && sb.ProductId == sp.ProductId && sb.BookBalanceQuantity > 0)?
                        .Select(sb => sb.BookBalanceQuantity)?
                        .Sum() ?? 0;
                }

                //if (stockBalancesOfSalesProducts.Any())
                //{
                //    foreach (var sp in tempDto.SalesProducts)
                //    {
                //        stockBalancesOfSalesProducts
                //                        .Where(sb => sb.SalesProductId == sp.Id)?
                //                        .Select(sb => sb.StockBalanceId)?
                //                        .ToArray();
                //    }
                //}

                // Adding NonInventoryProducts
                tempDto.NonInventoryProducts = await GetNonInventoryProductBySaleIdAsync(saleId);
                return tempDto;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task CreateSalesProductAsync(bool isExisting, Guid? existingSaleId, CreateSalesProductDto lineLevel, WarehouseDto warehouse)
        {
            // Set Warehouse Details to Line level 
            lineLevel.WarehouseId = warehouse.Id;
            lineLevel.WarehouseCode = warehouse.Code;
            lineLevel.WarehouseName = warehouse.Name;

            var stockBalances = await GetStockBalancesAsync(lineLevel.ProductId, warehouse.Id);

            // * Increase in Allocated Qty & Decrease in Book Balance Qty
            // 1. Company Summary
            // 2. Warehouse Summary
            // 3. GRN Summary reduce (specific)
            foreach (var sb in stockBalances)
            {
                //if (sb.SequenceNumber <= 0 || sb.Id == lineLevel.StockBalanceId)
                //{
                //    sb.AllocatedQuantity += lineLevel.Quantity;
                //    sb.BookBalanceQuantity -= lineLevel.Quantity;
                //    await _stockBalanceRepository.UpdateAsync(sb);
                //}
            }

            if (isExisting)
            {
                var lineLevelProduct = ObjectMapper.Map<SalesProduct>(lineLevel);
                lineLevelProduct.SaleId = existingSaleId.Value;
                await _saleProductRepository.InsertAsync(lineLevelProduct);
            }
        }

        private async Task CreateOrUpdateSalesProductsAsync(Guid saleId, string salesNumber, List<CreateSalesProductDto> inputSalesProducts)
        {
            var existingSalesProducts = await _saleProductRepository.GetAll().Where(sp => sp.SaleId == saleId).ToListAsync();

            var existingSBSP = await GetStockBalancesOfSalesProductsBySaleIdAsync(saleId);

            #region Stock Balances
            var existingTakenStockBalances = new List<StockBalance>();

            if (existingSBSP.Any())
            {
                var sbIds = existingSBSP.Select(sbsp => sbsp.StockBalanceId).ToList();

                if (sbIds.Count > 0)
                {
                    // Take all the SBId's and retrive there details with Company & Warehouse Summaries (Unique Id)
                    existingTakenStockBalances = await _storedProcedureAppService.GetStockBalancesByStockBalanceId(sbIds.ToArray());
                }
            }
            #endregion

            /////
            //// NOTE:
            //// HARD DELETE => SalesProducts & SBSP
            //// UNDO => StockBalance
            /////
            foreach (var esp in existingSalesProducts)
            {
                var q = esp.Quantity; // Can deal with multiple SB's
                var s = esp.SellingPrice;

                if (!inputSalesProducts.Any(isp => isp.Id == esp.Id))
                {
                    var eSBSP = existingSBSP.Where(sbsp => sbsp.SalesProductId == esp.Id);

                    // UPDATE GRN Row
                    foreach (var esbsp in eSBSP)
                    {
                        var sb = existingTakenStockBalances.FirstOrDefault(sb => sb.Id == esbsp.StockBalanceId);
                        sb.AllocatedQuantity -= q;
                        sb.BookBalanceQuantity += q;
                    }

                    // UPDATE Company Summary & Warehouse Summary
                    var sbSummaries = existingTakenStockBalances.Where(sb => sb.SequenceNumber == 0 && sb.ProductId == esp.ProductId && (!sb.WarehouseId.HasValue || sb.WarehouseId == esp.WarehouseId.Value));
                    foreach (var sb in sbSummaries)
                    {
                        sb.AllocatedQuantity -= q;
                        sb.BookBalanceQuantity += q;
                    }

                    // DELETE StockBalancesOfSalesProduct 
                    foreach (var sbsp in eSBSP)
                    {
                        await _stockBalancesOfSalesProductRepository.HardDeleteAsync(sbsp);
                    }

                    await _saleProductRepository.HardDeleteAsync(esp);
                }
            }

            /////
            //// NOTE:
            //// CREATE / UPDATE => SalesProducts
            //// INCREASE / DECREASE => StockBalance & SBSP
            /////
            foreach (var isp in inputSalesProducts)
            {
                if (!isp.Id.HasValue)
                {
                    isp.Id = Guid.NewGuid();

                    var stockBalances = _stockBalanceRepository
                        .GetAll()
                        .Where(sb => sb.SequenceNumber > 0 &&
                            sb.ProductId == isp.ProductId &&
                            sb.WarehouseId == sb.WarehouseId.Value &&
                            sb.SellingPrice == isp.SellingPrice &&
                            sb.BookBalanceQuantity > 0);

                    var stockBalanceSummaries = _stockBalanceRepository
                        .GetAll()
                        .Where(sb => sb.SequenceNumber == 0 &&
                            sb.ProductId == isp.ProductId &&
                            (
                                !sb.WarehouseId.HasValue &&
                                sb.WarehouseId == sb.WarehouseId.Value
                            ));

                    var totalQuantity = isp.Quantity;
                    decimal takenQuantity = 0;

                    // UPDATE GRN Row
                    foreach (var sb in stockBalances)
                    {
                        if (takenQuantity > sb.BookBalanceQuantity)
                        {
                            // SBSP
                            await CreateStockBalancesOfSalesProductAsync(saleId, isp.Id.Value, sb, sb.BookBalanceQuantity, isp.Price);

                            takenQuantity += sb.BookBalanceQuantity;
                            sb.AllocatedQuantity += sb.BookBalanceQuantity;
                            sb.BookBalanceQuantity -= 0;
                        }
                        else
                        {
                            var remainingQuantity = totalQuantity - takenQuantity;

                            takenQuantity += remainingQuantity;
                            sb.AllocatedQuantity += remainingQuantity;
                            sb.BookBalanceQuantity -= remainingQuantity;

                            // SBSP
                            await CreateStockBalancesOfSalesProductAsync(saleId, isp.Id.Value, sb, remainingQuantity, isp.Price);
                        }
                    }

                    // UPDATE Company & Warehouse Summaries
                    foreach (var sb in stockBalanceSummaries)
                    {
                        sb.AllocatedQuantity += totalQuantity;
                        sb.BookBalanceQuantity -= totalQuantity;
                    }

                    // ----- CREATE -----
                    var createDto = ObjectMapper.Map<SalesProduct>(isp);
                    await _saleProductRepository.InsertAsync(createDto);
                }
                else
                {
                    // ----- UPDATE -----

                    var existingSP = existingSalesProducts.FirstOrDefault(sp => sp.Id == isp.Id.Value);

                    var eSBSP = existingSBSP.Where(sbsp => sbsp.SalesProductId == isp.Id.Value);

                    var existingConsumedStockBalanceIds = eSBSP.Select(sbsp => sbsp.StockBalanceId);

                    #region StockBalance

                    // Already Consumed                     
                    var alreadyConsumedStockBalances = new List<StockBalance>();

                    // Other SB's which is related to selling price 
                    var relatedNewStockBalances =
                        _stockBalanceRepository
                            .GetAll()
                            .Where(sb => sb.SequenceNumber > 0 &&
                                sb.ProductId == isp.ProductId &&
                                sb.WarehouseId == isp.WarehouseId &&
                                sb.SellingPrice == isp.SellingPrice &&
                                sb.BookBalanceQuantity > 0
                            );

                    foreach (var sbId in existingConsumedStockBalanceIds)
                    {
                        var sb = _stockBalanceRepository.FirstOrDefault(sb => sb.Id == sbId);
                        // relatedStockBalances.Add(sb);
                        alreadyConsumedStockBalances.Add(sb);
                    }
                    #endregion

                    if (existingSP != null && isp.Quantity != existingSP.Quantity)
                    {
                        if (isp.Quantity < existingSP.Quantity)
                        {
                            // Initially "reduceQuantity" will have values where it should be 0 to finish the loop
                            decimal reduceQuantity = existingSP.Quantity - isp.Quantity;

                            foreach (var sb in alreadyConsumedStockBalances)
                            {
                                var specificSB = eSBSP.FirstOrDefault(sbsp => sbsp.StockBalanceId == sb.Id);

                                if (reduceQuantity < specificSB.QuantityTaken)
                                {
                                    sb.AllocatedQuantity -= reduceQuantity;
                                    sb.BookBalanceQuantity += reduceQuantity;
                                    reduceQuantity = 0;
                                }
                                else
                                {
                                    sb.AllocatedQuantity -= specificSB.QuantityTaken;
                                    sb.BookBalanceQuantity += specificSB.QuantityTaken;
                                    reduceQuantity -= specificSB.QuantityTaken;
                                }
                                if (reduceQuantity == 0) break;
                            }

                            /// NOT YET Implemented 
                            /// Update Company & Warehouse Summaries
                            /// Updated in SBSP

                        }
                        else
                        {
                            var increasingQuantity = isp.Quantity - existingSP.Quantity;

                            // Existing SB's
                            foreach (var sb in alreadyConsumedStockBalances)
                            {
                                var specificSB = eSBSP.FirstOrDefault(sbsp => sbsp.StockBalanceId == sb.Id);

                                if (increasingQuantity < sb.BookBalanceQuantity)
                                {
                                    sb.AllocatedQuantity += increasingQuantity;
                                    sb.BookBalanceQuantity -= increasingQuantity;
                                    specificSB.QuantityTaken -= increasingQuantity;
                                    increasingQuantity = 0;
                                }
                                else
                                {
                                    sb.AllocatedQuantity += sb.BookBalanceQuantity;
                                    increasingQuantity -= sb.BookBalanceQuantity;
                                    specificSB.QuantityTaken += sb.BookBalanceQuantity;
                                    sb.BookBalanceQuantity = 0;
                                }
                                if (increasingQuantity == 0) break;
                            }

                            // New SB's
                            foreach (var nsb in relatedNewStockBalances)
                            {
                                if(!alreadyConsumedStockBalances.Any(eSB => eSB.Id == nsb.Id))
                                {
                                    var sbSummaries = _stockBalanceRepository.GetAll().Where(sb => sb.SequenceNumber == 0 && sb.ProductId == nsb.ProductId && (!sb.WarehouseId.HasValue || sb.WarehouseId == nsb.WarehouseId));

                                    if (increasingQuantity < nsb.BookBalanceQuantity)
                                    {
                                        await CreateStockBalancesOfSalesProductAsync(saleId, isp.Id.Value, nsb, increasingQuantity, isp.Price);

                                        foreach (var sbSum in sbSummaries)
                                        {
                                            sbSum.AllocatedQuantity += increasingQuantity;
                                            sbSum.BookBalanceQuantity -= increasingQuantity;
                                        }

                                        nsb.AllocatedQuantity += increasingQuantity;
                                        nsb.BookBalanceQuantity -= increasingQuantity;
                                        increasingQuantity = 0;
                                    }
                                    else
                                    {
                                        await CreateStockBalancesOfSalesProductAsync(saleId, isp.Id.Value, nsb, nsb.BookBalanceQuantity, isp.Price);

                                        foreach (var sbSum in sbSummaries)
                                        {
                                            sbSum.AllocatedQuantity += nsb.BookBalanceQuantity;
                                            sbSum.BookBalanceQuantity -= nsb.BookBalanceQuantity;
                                        }

                                        nsb.AllocatedQuantity += nsb.BookBalanceQuantity;
                                        increasingQuantity -= nsb.BookBalanceQuantity;
                                        nsb.BookBalanceQuantity = 0;
                                    }
                                    if (increasingQuantity == 0) break;
                                }
                            }

                            /// NOT YET Implemented 
                            /// Update Company & Warehouse Summaries
                            /// Create in SBSP
                        }

                        ObjectMapper.Map(isp, existingSP);
                        await _saleProductRepository.UpdateAsync(existingSP);
                    }
                }
            }
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sbs">Company Summary (1), Warehouse Summary (1), GRN Rows (1)</param>
        /// <param name="quantity">Can have Positive or Negative Values</param>
        /// <param name="isAffectInBookBalanceQuantity">True BookBalanceQuantity; False AllocatedQuantity</param>
        /// <returns></returns>
        private async Task ASD(List<StockBalance> sbs, decimal quantity, bool isAffectInBookBalanceQuantity)
        {
            if (isAffectInBookBalanceQuantity)
            {
                foreach (var sb in sbs)
                {
                    sb.BookBalanceQuantity += quantity;
                    await _stockBalanceRepository.UpdateAsync(sb);
                }
            }
            else
            {
                foreach (var sb in sbs)
                {
                    sb.AllocatedQuantity += quantity;
                    await _stockBalanceRepository.UpdateAsync(sb);
                }
            }
        }

        #region Stock Balance
        /// <summary>
        /// GroupBy SellingPrice
        /// If selling prices are equal then sum the BBQ and get the related SB Ids
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        public async Task<List<GroupBySellingPriceDto>> GetStockBalancesByProductIdAsync(Guid productId)
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
        private async Task<List<StockBalance>> GetStockBalancesByStockBalanceIdAsync(Guid stockBalanceId, Guid productId, Guid warehouseId)
        {
            return await _stockBalanceRepository.GetAll()
                .Where(sb => sb.Id == stockBalanceId ||
                (sb.SequenceNumber == 0 && sb.ProductId == productId && sb.WarehouseId == warehouseId) ||
                (sb.SequenceNumber == 0 && sb.ProductId == productId && !sb.WarehouseId.HasValue)
            ).ToListAsync();
        }

        #endregion

        #region NonInventoryProduct
        public async Task<List<NonInventoryProductDto>> GetNonInventoryProductBySaleIdAsync(Guid saleId)
        {
            return await _nonInventoryProductRepository
                .GetAll()
                .Where(n => n.SaleId == saleId)
                .Select(n => new NonInventoryProductDto
                {
                    Id = n.Id,
                    SequenceNumber = n.SequenceNumber,
                    SaleId = n.SaleId,
                    ProductId = n.ProductId,
                    ProductCode = n.ProductCode,
                    ProductName = n.ProductName,
                    WarehouseId = n.WarehouseId,
                    WarehouseCode = n.WarehouseCode,
                    WarehouseName = n.WarehouseName,
                    Quantity = n.Quantity,
                    QuantityUnitOfMeasureUnit = n.QuantityUnitOfMeasureUnit,
                    DiscountRate = n.DiscountRate,
                    DiscountAmount = n.DiscountAmount,
                    LineTotal = n.LineTotal,
                    CostPrice = n.CostPrice,
                    SellingPrice = n.SellingPrice,
                    MaximumRetailPrice = n.MaximumRetailPrice,
                    Price = n.Price
                })
                .ToListAsync();
        }

        private async Task CreateOrUpdateNonInventoryProductAsync(Guid saleId, string salesNumber, List<CreateNonInventoryProductDto> nonInventoryProducts)
        {
            #region Explanation
            //------- Existing -----------Input Req--------------------------
            // 1.        YES                 NO          =>  DELETE
            // 2.        NO                  YES         =>  CREATE
            // 3.        YES                 YES         =>  UPDATE
            #endregion

            var existingNIPs = await _nonInventoryProductRepository.GetAll().Where(n => n.SaleId == saleId).ToListAsync();

            foreach (var exist_nips in existingNIPs)
            {
                // DELETE
                if (!nonInventoryProducts.Any(n => n.Id == exist_nips.Id))
                {
                    // Company Summary & Warehouse Summary
                    var nonInventoryProductSummaries1 = await GetNonInventoryProductSummariesAsync(exist_nips.ProductId, exist_nips.WarehouseId.Value);

                    var remove = existingNIPs.FirstOrDefault(n => n.Id == exist_nips.Id);
                    if (remove != null)
                    {
                        await UpdateNonInventoryProductSummariesAsync(nonInventoryProductSummaries1, remove.Quantity, 0);

                        await _nonInventoryProductRepository.HardDeleteAsync(remove);
                    }
                }
            }

            foreach (var input_nip in nonInventoryProducts)
            {
                // Company Summary & Warehouse Summary
                var nonInventoryProductSummaries2 = await GetNonInventoryProductSummariesAsync(input_nip.ProductId, input_nip.WarehouseId.Value);

                if (!input_nip.Id.HasValue)
                {
                    // CREATE
                    if (!existingNIPs.Any(n => n.Id == input_nip.Id))
                    {
                        await UpdateNonInventoryProductSummariesAsync(nonInventoryProductSummaries2, 0, input_nip.Quantity);

                        await _nonInventoryProductRepository.InsertAsync(new NonInventoryProduct
                        {
                            SequenceNumber = 1,
                            SaleId = saleId,
                            SalesNumber = salesNumber,
                            ProductId = input_nip.ProductId,
                            ProductCode = input_nip.ProductCode,
                            ProductName = input_nip.ProductName,
                            WarehouseId = input_nip.WarehouseId,
                            WarehouseCode = input_nip.WarehouseCode,
                            WarehouseName = input_nip.WarehouseName,
                            Quantity = input_nip.Quantity,
                            QuantityUnitOfMeasureUnit = null,
                            DiscountRate = input_nip.DiscountRate,
                            DiscountAmount = input_nip.DiscountAmount,
                            LineTotal = input_nip.LineTotal,
                            CostPrice = input_nip.CostPrice,
                            SellingPrice = input_nip.SellingPrice,
                            MaximumRetailPrice = input_nip.MaximumRetailPrice,
                            Price = input_nip.Price
                        });
                    }
                }
                else
                {
                    // UPDATE
                    var updatedDto = existingNIPs.FirstOrDefault(n => n.Id == input_nip.Id);
                    if (updatedDto != null)
                    {
                        if (updatedDto.Quantity <= input_nip.Quantity)
                            await UpdateNonInventoryProductSummariesAsync(nonInventoryProductSummaries2, updatedDto.Quantity, input_nip.Quantity);
                        else
                            await UpdateNonInventoryProductSummariesAsync(nonInventoryProductSummaries2, updatedDto.Quantity, input_nip.Quantity);

                        updatedDto.SequenceNumber = 1;
                        updatedDto.SaleId = saleId;
                        updatedDto.SalesNumber = salesNumber;
                        updatedDto.ProductId = input_nip.ProductId;
                        updatedDto.ProductCode = input_nip.ProductCode;
                        updatedDto.ProductName = input_nip.ProductName;
                        updatedDto.WarehouseId = input_nip.WarehouseId;
                        updatedDto.WarehouseCode = input_nip.WarehouseCode;
                        updatedDto.WarehouseName = input_nip.WarehouseName;
                        updatedDto.Quantity = input_nip.Quantity;
                        updatedDto.QuantityUnitOfMeasureUnit = null;
                        updatedDto.DiscountRate = input_nip.DiscountRate;
                        updatedDto.DiscountAmount = input_nip.DiscountAmount;
                        updatedDto.LineTotal = input_nip.LineTotal;
                        updatedDto.CostPrice = input_nip.CostPrice;
                        updatedDto.SellingPrice = input_nip.SellingPrice;
                        updatedDto.MaximumRetailPrice = input_nip.MaximumRetailPrice;
                        updatedDto.Price = input_nip.Price;
                        await _nonInventoryProductRepository.UpdateAsync(updatedDto);
                    }
                }
            }
        }

        private async Task<List<NonInventoryProduct>> GetNonInventoryProductSummariesAsync(Guid ProductId, Guid WarehouseId)
        {
            return await _nonInventoryProductRepository
                        .GetAll()
                        .Where(n => n.SequenceNumber == 0
                            && n.ProductId == ProductId
                            && (n.WarehouseId == WarehouseId || !n.WarehouseId.HasValue))
                        .ToListAsync();
        }

        private async Task UpdateNonInventoryProductSummariesAsync(List<NonInventoryProduct> updateDtos, decimal previousQuantity, decimal NewQuantity)
        {
            // PrevQty  |   NewQty      |   Diff = (PrevQty - NewQty)
            //     +           -               +
            //     -           +               -
            //     =           =               =       

            // Calculate the differences & Make the differentiate value to positive
            var differentiateQuantity = previousQuantity - NewQuantity;
            differentiateQuantity = Math.Abs(differentiateQuantity);

            // Based on the comparison set positive or nagative value for "differentiateQuantity"
            if (previousQuantity >= NewQuantity)
                differentiateQuantity = (-1) * differentiateQuantity;
            else
                differentiateQuantity = (+1) * differentiateQuantity;

            foreach (var summary in updateDtos)
            {
                // Might have positive or negative values in "differentiateQuantity"
                // Minus will decrease from existing value
                // positive will increase from existing value 
                summary.Quantity += differentiateQuantity;

                await _nonInventoryProductRepository.UpdateAsync(summary);
            }
        }
        #endregion

        #region Payment
        private async Task CreatePaymentForSalesIdAsync(Guid saleId, string saleNumber, Guid? customerId, string customerCode, string customerName, CashDto[] cashes, ChequeDto[] cheques, CustomerCreditOutstandingDto[] outstandings, DebitCardDto[] debitCards, GiftCardDto[] giftCards)
        {
            try
            {
                var payments = new List<Payment>();

                #region Map Payment from Payments[]

                if (cashes.Any())
                {
                    // Get the total amount of cash payments
                    var totalCashAmount = cashes.Select(c => c.CashAmount).Sum();
                    payments.Add(new Payment
                    {
                        PaidAmount = totalCashAmount,
                        IsCash = true,
                    });
                }
                foreach (var ip in cheques)
                {
                    payments.Add(new Payment
                    {
                        PaidAmount = ip.ChequeAmount,
                        ChequeNumber = ip.ChequeNumber,
                        ChequeReturnDate = ip.ChequeReturnDate,
                        BankId = ip.BankId,
                        Bank = ip.Bank,
                        BranchId = ip.BranchId,
                        Branch = ip.Branch,
                        IsCheque = true,
                    });
                }
                foreach (var ip in outstandings)
                {
                    payments.Add(new Payment
                    {
                        OutstandingAmount = ip.OutstandingAmount,
                        PaidAmount = 0,
                        IsCreditOutstanding = true,
                    });
                }
                foreach (var ip in debitCards)
                {
                    payments.Add(new Payment
                    {
                        IsDebitCard = true,
                    });
                }
                foreach (var ip in giftCards)
                {
                    payments.Add(new Payment
                    {
                        PaidAmount = ip.GiftCardAmount,
                        IsGiftCard = true,
                    });
                }

                foreach (var p in payments)
                {
                    p.SaleId = saleId;
                    p.CustomerId = customerId;
                    p.CustomerCode = customerCode;
                    p.CustomerName = customerName.Trim();
                    p.SaleNumber = saleNumber;

                    await _paymentRepository.InsertAsync(p);
                }
                #endregion
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        private async Task<PaymentStatus> GetPaymentStatusBySaleId(Guid saleId, decimal netAmount, decimal totalPayingAmount)
        {
            // 1. Get the total of Previously Paid Amount
            decimal previouslyPaidAmount = 0;
            var prevPayments = _paymentRepository.GetAll().Where(p => p.SaleId == saleId);
            if (prevPayments.Any()) previouslyPaidAmount = prevPayments.Select(p => p.PaidAmount).Sum();

            var _totalPayingAmount = previouslyPaidAmount + totalPayingAmount;

            if (netAmount <= _totalPayingAmount)
                return PaymentStatus.Completed;
            return PaymentStatus.PartiallyPaid;
        }
        #endregion

        #region Warehouse
        private async Task<Warehouse> GetWarehouseAsync(Guid? warehouseId)
        {
            if (warehouseId.HasValue)
                return _warehouseRepository.FirstOrDefault(w => w.Id == warehouseId);
            return _warehouseRepository.FirstOrDefault(w => w.IsDefault);
        }
        #endregion

        #region StockBalanceOfSalesProduct
        private async Task CreateStockBalancesOfSalesProductAsync(Guid saleId, Guid salesProductId, StockBalance stockBalance, decimal quantity, decimal soldPrice)
        {
            await _stockBalancesOfSalesProductRepository.InsertAsync(new StockBalancesOfSalesProduct
            {
                SaleId = saleId,
                SalesProductId = salesProductId,
                StockBalanceId = stockBalance.Id,
                SequenceNumber = stockBalance.SequenceNumber,
                ProductId = stockBalance.ProductId,
                ProductCode = stockBalance.ProductCode,
                ProductName = stockBalance.ProductName,
                WarehouseId = stockBalance.WarehouseId,
                WarehouseCode = stockBalance.WarehouseCode,
                WarehouseName = stockBalance.WarehouseName,
                CostPrice = stockBalance.CostPrice,
                SellingPrice = stockBalance.SellingPrice,
                MaximumRetailPrice = stockBalance.MaximumRetailPrice,
                QuantityTaken = quantity,
                Price = soldPrice,
            });
        }

        private async Task<List<StockBalancesOfSalesProduct>> GetStockBalancesOfSalesProductsBySaleIdAsync(Guid saleId)
        {
            return await _stockBalancesOfSalesProductRepository.GetAll().Where(sbsp => sbsp.SaleId == saleId).ToListAsync();
        }
        #endregion
    }
}
