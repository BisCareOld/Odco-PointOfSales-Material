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
using Odco.PointOfSales.Application.Productions.Warehouses;
using Odco.PointOfSales.Application.Sales.Customers;
using Odco.PointOfSales.Application.Sales.Sales;
using Odco.PointOfSales.Application.Sales.SalesProducts;
using Odco.PointOfSales.Application.StockBalances;
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
        private readonly IStockBalanceAppService _stockBalanceAppService;

        public SalesAppService(IRepository<Customer, Guid> customerRepository,
            IRepository<Sale, Guid> saleRepository,
            IRepository<SalesProduct, Guid> saleProductRepository,
            IRepository<StockBalance, Guid> stockBalanceRepository,
            IRepository<Warehouse, Guid> warehouseRepository,
            IRepository<NonInventoryProduct, Guid> nonInventoryProductRepository,
            IDocumentSequenceNumberManager documentSequenceNumberManager,
            IRepository<Payment, Guid> paymentRepository,
            IRepository<StockBalancesOfSalesProduct, long> stockBalancesOfSalesProductRepository,
            IStoredProcedureAppService storedProcedureAppService,
            IStockBalanceAppService stockBalanceAppService
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
            _stockBalanceAppService = stockBalanceAppService;
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

                #region Sales Number
                if(!input.Id.HasValue)
                    input.SalesNumber = await _documentSequenceNumberManager.GetAndUpdateNextDocumentNumberAsync(DocumentType.Sales);
                #endregion

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

                    var requestDto = input;
                    requestDto.SalesProducts.Clear();
                    requestDto.NonInventoryProducts.Clear();
                    await CreatePaymentForSalesIdAsync(requestDto);
                }
                else
                {
                    input.PaymentStatus = PaymentStatus.NotPurchased;
                }
                #endregion

                #region Sale
                var sale = ObjectMapper.Map<Sale>(input);
                sale.SalesProducts.Clear();
                _saleHeader.Id = await _saleRepository.InsertOrUpdateAndGetIdAsync(sale);
                #endregion

                #region SalesProduct = InventoryProduct
                // Create / Update / Delete SalesProduct
                input.SalesProducts.ToList().ForEach(isp => {
                    isp.SaleId = _saleHeader.Id;
                    isp.SalesNumber = input.SalesNumber;
                });

                await CreateOrUpdateSalesProductsAsync(_saleHeader.Id, input.SalesNumber, input.SalesProducts.ToList());
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
                        .Where(sb => sb.SequenceNumber > 0 && sb.WarehouseId == sp.WarehouseId && sb.ProductId == sp.ProductId && sb.BookBalanceQuantity > 0 && sb.SellingPrice == sp.SellingPrice)?
                        .Select(sb => sb.BookBalanceQuantity)?
                        .Sum() ?? 0;
                }

                // Adding NonInventoryProducts
                tempDto.NonInventoryProducts = await GetNonInventoryProductBySaleIdAsync(saleId);
                return tempDto;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task CreateOrUpdateSalesProductsAsync(Guid saleId, string salesNumber, List<CreateSalesProductDto> inputSalesProducts)
        {
            var existingSPs = await _saleProductRepository.GetAll().Where(sp => sp.SaleId == saleId).ToListAsync();

            var existingSBSPs = await GetStockBalancesOfSalesProductsBySaleIdAsync(saleId);

            #region Stock Balances
            var existingTakenStockBalances = new List<StockBalance>();

            if (existingSBSPs.Any())
            {
                var sbIds = existingSBSPs.Select(sbsp => sbsp.StockBalanceId).ToList();

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
            foreach (var esp in existingSPs)
            {
                #region StockBalance Summaries (Company & Warehouse Summary)
                var sbSummaries = existingTakenStockBalances
                        .Where(sb => sb.SequenceNumber == 0 && sb.ProductId == esp.ProductId && (!sb.WarehouseId.HasValue || sb.WarehouseId == esp.WarehouseId))
                        .ToList();
                if (!sbSummaries.Any())
                {
                    sbSummaries = await _stockBalanceAppService.GetStockBalanceSummariesAsync(esp.ProductId, esp.WarehouseId.Value);
                }
                #endregion

                if (!inputSalesProducts.Any(isp => isp.Id == esp.Id))
                {
                    var eSBSPs = existingSBSPs.Where(sbsp => sbsp.SalesProductId == esp.Id);

                    // UPDATE GRN Row
                    foreach (var eSBSP in eSBSPs)
                    {
                        var _sb = existingTakenStockBalances.FirstOrDefault(sb => sb.Id == eSBSP.StockBalanceId);
                        _sb.AllocatedQuantity -= eSBSP.QuantityTaken;
                        _sb.BookBalanceQuantity += eSBSP.QuantityTaken;
                        await _stockBalanceRepository.UpdateAsync(_sb);
                    }

                    // UPDATE Company & Warehouse Summaries
                    foreach (var sb in sbSummaries)
                    {
                        sb.AllocatedQuantity -= esp.Quantity;
                        sb.BookBalanceQuantity += esp.Quantity;
                        await _stockBalanceRepository.UpdateAsync(sb);
                    }

                    // DELETE StockBalancesOfSalesProduct 
                    foreach (var sbsp in eSBSPs)
                        await _stockBalancesOfSalesProductRepository.HardDeleteAsync(sbsp);

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

                    var stockBalances = await _stockBalanceAppService.GetStockBalancesByProductIdBasedOnSellingPriceAsync(isp.ProductId, isp.WarehouseId.Value, isp.SellingPrice);

                    var stockBalanceSummaries = await _stockBalanceAppService.GetStockBalanceSummariesAsync(isp.ProductId, isp.WarehouseId.Value);

                    decimal totalQuantity = isp.Quantity;
                    decimal remainingQuantity = isp.Quantity;

                    // UPDATE GRN Row
                    foreach (var sb in stockBalances)
                    {
                        if (remainingQuantity > sb.BookBalanceQuantity)
                        {
                            // SBSP
                            await CreateStockBalancesOfSalesProductAsync(saleId, isp.Id.Value, sb, sb.BookBalanceQuantity, isp.Price);

                            remainingQuantity -= sb.BookBalanceQuantity;
                            sb.AllocatedQuantity += sb.BookBalanceQuantity;
                            sb.BookBalanceQuantity = 0;
                        }
                        else
                        {
                            sb.AllocatedQuantity += remainingQuantity;
                            sb.BookBalanceQuantity -= remainingQuantity;

                            // SBSP
                            await CreateStockBalancesOfSalesProductAsync(saleId, isp.Id.Value, sb, remainingQuantity, isp.Price);

                            remainingQuantity = 0;
                        }
                        if (remainingQuantity == 0) break;
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

                    var eSP = existingSPs.FirstOrDefault(sp => sp.Id == isp.Id.Value);

                    var eSBSPs = existingSBSPs.Where(sbsp => sbsp.SalesProductId == isp.Id.Value).ToList();

                    var existingConsumedStockBalanceIds = eSBSPs.Select(sbsp => sbsp.StockBalanceId);

                    #region StockBalance
                    // Already Consumed
                    var alreadyConsumedStockBalances = new List<StockBalance>();

                    // Other SB's which are related to selling price
                    var relatedNewStockBalances =
                        _stockBalanceRepository
                            .GetAll()
                            .Where(sb => sb.SequenceNumber > 0 &&
                                sb.ProductId == isp.ProductId &&
                                sb.WarehouseId == isp.WarehouseId &&
                                sb.SellingPrice == isp.SellingPrice &&
                                sb.BookBalanceQuantity > 0
                            ).ToList();

                    foreach (var sbId in existingConsumedStockBalanceIds)
                    {
                        var sb = await _stockBalanceAppService.GetStockBalanceByIdAsync(sbId);
                        alreadyConsumedStockBalances.Add(sb);
                    }
                    #endregion

                    #region StockBalance Summaries (Company & Warehouse Summary)
                    var sbSummaries = existingTakenStockBalances
                            .Where(sb => sb.SequenceNumber == 0 && sb.ProductId == isp.ProductId && (!sb.WarehouseId.HasValue || sb.WarehouseId == isp.WarehouseId))
                            .ToList();
                    if (!sbSummaries.Any())
                    {
                        sbSummaries = await _stockBalanceAppService.GetStockBalanceSummariesAsync(isp.ProductId, isp.WarehouseId.Value);
                    }
                    #endregion

                    if (eSP != null)
                    {
                        if (isp.Quantity != eSP.Quantity)
                        {
                            if (isp.Quantity < eSP.Quantity)
                            {
                                // Initially "reduceQuantity" will have values where it should be 0 to finish the loop
                                decimal reduceQuantity = eSP.Quantity - isp.Quantity;

                                foreach (var sb in alreadyConsumedStockBalances)
                                {
                                    var specificSB = eSBSPs.FirstOrDefault(sbsp => sbsp.StockBalanceId == sb.Id);

                                    if (reduceQuantity < specificSB.QuantityTaken)
                                    {
                                        // UPDATE SBSP
                                        specificSB.QuantityTaken -= reduceQuantity;

                                        sb.AllocatedQuantity -= reduceQuantity;
                                        sb.BookBalanceQuantity += reduceQuantity;
                                        reduceQuantity = 0;
                                    }
                                    else
                                    {
                                        sb.AllocatedQuantity -= specificSB.QuantityTaken;
                                        sb.BookBalanceQuantity += specificSB.QuantityTaken;
                                        reduceQuantity -= specificSB.QuantityTaken;

                                        // DELETE SBSP
                                        specificSB.QuantityTaken = 0;
                                        await _stockBalancesOfSalesProductRepository.HardDeleteAsync(specificSB);
                                    }
                                    if (reduceQuantity == 0) break;
                                }

                                // UPDATE Company & Warehouse Summaries
                                foreach (var sb in sbSummaries)
                                {
                                    sb.AllocatedQuantity -= eSP.Quantity - isp.Quantity;
                                    sb.BookBalanceQuantity += eSP.Quantity - isp.Quantity;
                                    await _stockBalanceRepository.UpdateAsync(sb);
                                }
                            }
                            else
                            {
                                var increasingQuantity = isp.Quantity - eSP.Quantity;

                                // Existing SB's
                                foreach (var sb in alreadyConsumedStockBalances)
                                {
                                    var specificSB = eSBSPs.FirstOrDefault(sbsp => sbsp.StockBalanceId == sb.Id);

                                    if (sb.BookBalanceQuantity > 0)
                                    {
                                        if (increasingQuantity < sb.BookBalanceQuantity)
                                        {
                                            sb.AllocatedQuantity += increasingQuantity;
                                            sb.BookBalanceQuantity -= increasingQuantity;

                                            // UPDATE SBSP
                                            specificSB.QuantityTaken += increasingQuantity;

                                            increasingQuantity = 0;
                                        }
                                        else
                                        {
                                            sb.AllocatedQuantity += sb.BookBalanceQuantity;
                                            increasingQuantity -= sb.BookBalanceQuantity;

                                            // UPDATE SBSP
                                            specificSB.QuantityTaken += sb.BookBalanceQuantity;

                                            sb.BookBalanceQuantity = 0;
                                        }
                                        if (increasingQuantity == 0) break;
                                    }
                                }

                                // New SB's
                                foreach (var nsb in relatedNewStockBalances)
                                {
                                    if (!alreadyConsumedStockBalances.Any(eSB => eSB.Id == nsb.Id))
                                    {
                                        if (increasingQuantity < nsb.BookBalanceQuantity)
                                        {
                                            await CreateStockBalancesOfSalesProductAsync(saleId, isp.Id.Value, nsb, increasingQuantity, isp.Price);

                                            //foreach (var sbSum in sbSummaries)
                                            //{
                                            //    sbSum.AllocatedQuantity += increasingQuantity;
                                            //    sbSum.BookBalanceQuantity -= increasingQuantity;
                                            //}

                                            nsb.AllocatedQuantity += increasingQuantity;
                                            nsb.BookBalanceQuantity -= increasingQuantity;
                                            increasingQuantity = 0;
                                        }
                                        else
                                        {
                                            await CreateStockBalancesOfSalesProductAsync(saleId, isp.Id.Value, nsb, nsb.BookBalanceQuantity, isp.Price);

                                            //foreach (var sbSum in sbSummaries)
                                            //{
                                            //    sbSum.AllocatedQuantity += nsb.BookBalanceQuantity;
                                            //    sbSum.BookBalanceQuantity -= nsb.BookBalanceQuantity;
                                            //}

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
                                foreach (var sbSum in sbSummaries)
                                {
                                    sbSum.AllocatedQuantity += isp.Quantity - eSP.Quantity;
                                    sbSum.BookBalanceQuantity -= isp.Quantity - eSP.Quantity;
                                    await _stockBalanceRepository.UpdateAsync(sbSum);
                                }
                            }
                        }

                        ObjectMapper.Map(isp, eSP);
                        await _saleProductRepository.UpdateAsync(eSP);
                    }
                }
            }
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
        private async Task CreatePaymentForSalesIdAsync(CreateOrUpdateSaleDto input)
        {
            try
            {
                var invoiceNumber = await _documentSequenceNumberManager.GetAndUpdateNextDocumentNumberAsync(DocumentType.Invoice);

                var payments = new List<Payment>();

                #region
                decimal remainingReceivedAmount = input.ReceivedAmount - input.BalanceAmount - (input.Outstandings?.FirstOrDefault()?.OutstandingAmount ?? 0);
                #endregion

                #region Map Payment from Payments[]

                if (input.Cashes.Any())
                {
                    decimal _amountPaid = 0;

                    // Get the total amount of cash payments
                    var totalCashAmount = input.Cashes.Select(c => c.CashAmount).Sum();
                    if(remainingReceivedAmount <= totalCashAmount) {
                        _amountPaid = remainingReceivedAmount;
                        remainingReceivedAmount = 0;
                    }
                    else {
                        _amountPaid = totalCashAmount;
                        remainingReceivedAmount -= totalCashAmount;
                    }
                    payments.Add(new Payment
                    {
                        PaidAmount = _amountPaid,
                        IsCash = true,
                        SpecificReceivedAmount = totalCashAmount,
                        SpecificBalanceAmount = totalCashAmount - _amountPaid,
                    });
                }
                foreach (var ip in input.Cheques)
                {
                    decimal _amountPaid = 0;

                    if(remainingReceivedAmount <= ip.ChequeAmount) {
                        _amountPaid = remainingReceivedAmount;
                        remainingReceivedAmount = 0;
                    }
                    else {
                        _amountPaid = ip.ChequeAmount;
                        remainingReceivedAmount -= ip.ChequeAmount;
                    }
                    payments.Add(new Payment
                    {
                        PaidAmount = _amountPaid,
                        ChequeNumber = ip.ChequeNumber,
                        ChequeReturnDate = ip.ChequeReturnDate,
                        BankId = ip.BankId,
                        Bank = ip.Bank,
                        BranchId = ip.BranchId,
                        Branch = ip.Branch,
                        IsCheque = true,
                        SpecificReceivedAmount = ip.ChequeAmount,
                        SpecificBalanceAmount = ip.ChequeAmount - _amountPaid,
                    });
                }
                foreach (var ip in input.Outstandings)
                {
                    payments.Add(new Payment
                    {
                        OutstandingAmount = ip.OutstandingAmount,
                        PaidAmount = 0,
                        IsCreditOutstanding = true,
                    });
                }
                foreach (var ip in input.DebitCards)
                {
                    payments.Add(new Payment
                    {
                        IsDebitCard = true,
                    });
                }
                foreach (var ip in input.GiftCards)
                {
                    payments.Add(new Payment
                    {
                        PaidAmount = ip.GiftCardAmount,
                        IsGiftCard = true,
                    });
                }

                foreach (var p in payments)
                {
                    p.SaleId = input.Id.Value;
                    p.CustomerId = input.CustomerId;
                    p.CustomerCode = input.CustomerCode;
                    p.CustomerName = !string.IsNullOrEmpty(input.CustomerName) ? input.CustomerName.Trim() : null;
                    p.SaleNumber = input.SalesNumber;
                    p.InvoiceNumber = invoiceNumber;
                    p.TotalReceivedAmount = input.ReceivedAmount;
                    p.TotalBalanceAmount = input.BalanceAmount;

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
