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

        public SalesAppService(IRepository<Customer, Guid> customerRepository,
            IRepository<Sale, Guid> saleRepository,
            IRepository<SalesProduct, Guid> saleProductRepository,
            IRepository<StockBalance, Guid> stockBalanceRepository,
            IRepository<Warehouse, Guid> warehouseRepository,
            IRepository<NonInventoryProduct, Guid> nonInventoryProductRepository,
            IDocumentSequenceNumberManager documentSequenceNumberManager,
            IRepository<Payment, Guid> paymentRepository
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
                var salesNumber = string.Empty;

                if (input.Id.HasValue)
                {
                    var existingSale = await _saleRepository
                        .GetAllIncluding(t => t.SalesProducts)
                        .FirstOrDefaultAsync(t => t.Id == input.Id.Value);

                    // NOTE: Update existing Sale & Line Level Details (InventoryProduct & NonInventoryProduct)
                    if (existingSale != null)
                    {
                        // NOTE:
                        // 1. Make Payment
                        // 2. Getting the SalesNumber
                        if (input.Cashes.Any() || input.Cheques.Any() || input.Outstandings.Any() || input.DebitCards.Any() || input.GiftCards.Any())
                        {
                            salesNumber = await _documentSequenceNumberManager.GetAndUpdateNextDocumentNumberAsync(DocumentType.Sales);
                            input.SalesNumber = salesNumber;
                            input.SalesProducts.ToList().ForEach(sp => sp.SalesNumber = salesNumber);
                            await CreatePaymentForSalesIdAsync(input.Id.Value, salesNumber, input.CustomerId, input.CustomerCode, input.CustomerName, input.Cashes.ToArray(), input.Cheques.ToArray(), input.Outstandings.ToArray(), input.DebitCards.ToArray(), input.GiftCards.ToArray());
                        }

                        // AQ = Allocated Qty
                        // BBQ = Book Balance Qty
                        // Actual Quantity  &   AQ
                        // 1. Existing SP   &   Input SP                Result StockBalanceTable
                        // 2.               =                           No need to do anything
                        // 3.               >                           
                        // 4.               <
                        // 5.               !

                        foreach (var existSP in existingSale.SalesProducts)
                        {
                            var iSP = input.SalesProducts.FirstOrDefault(isp => isp.StockBalanceId == existSP.StockBalanceId);

                            if (iSP != null)
                            {
                                // NOTE: IF exist in DB & exist in new request Dto
                                // 1. Get Company Summary(1) + Warehouse Summary(1) + GRN Summary(1) based on the SBID
                                // 2. Update AQ & BBQ
                                // 3. GRN Summary - Strigth forward set value because Data got from specific StockBalanceId
                                var stockBalances = await GetStockBalancesByStockBalanceIdAsync(iSP.StockBalanceId, iSP.ProductId, iSP.WarehouseId.Value);

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

                                existSP.SalesNumber = salesNumber;
                                existSP.Price = iSP.Price;
                                existSP.DiscountRate = iSP.DiscountRate;
                                existSP.DiscountAmount = iSP.DiscountAmount;
                                existSP.Quantity = iSP.Quantity;
                                existSP.LineTotal = iSP.LineTotal;
                                await _saleProductRepository.UpdateAsync(existSP);
                            }
                            else
                            {
                                // NOTE: IF exist in DB & not exist in new request Dto
                                // 1. Remove / Delete from Line Level
                                // 2. Get Company Summary(1) + Warehouse Summary(1) + GRN Summary(1) based on the SBID
                                // 3. Update (Revert) AQ & BBQ in all 3 rows
                                // 4. GRN Summary (Soft Delete it)
                                var stockBalances = await GetStockBalancesByStockBalanceIdAsync(existSP.StockBalanceId, existSP.ProductId, existSP.WarehouseId.Value);
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
                                await _saleProductRepository.DeleteAsync(existSP);
                            }
                        }

                        // NOTE: IF not exist in DB & exist in new request Dto
                        // 1. Create a product row for existing TempSaleId
                        // 2. Update the record in StockBalance Table
                        var addTempSalesProductDto = new List<CreateSalesProductDto>();
                        foreach (var iSP in input.SalesProducts)
                        {
                            if (!existingSale.SalesProducts.Any(existSP => existSP.StockBalanceId == iSP.StockBalanceId))
                                addTempSalesProductDto.Add(iSP);
                        }

                        foreach (var tempSalesProduct in addTempSalesProductDto)
                        {
                            await CreateSalesProductAsync(
                                true,
                                input.Id,
                                tempSalesProduct,
                                new WarehouseDto { Id = warehouse.Id, Name = warehouse.Name, Code = warehouse.Code }
                            );
                        }

                        existingSale.SalesNumber = salesNumber;
                        existingSale.ReferenceNumber = input.ReferenceNumber;
                        existingSale.CustomerId = input.CustomerId;
                        existingSale.CustomerCode = input.CustomerCode;
                        existingSale.CustomerName = input.CustomerName;
                        existingSale.DiscountRate = input.DiscountRate;
                        existingSale.DiscountAmount = input.DiscountAmount;
                        existingSale.TaxRate = input.TaxRate;
                        existingSale.TaxAmount = input.TaxAmount;
                        existingSale.GrossAmount = input.GrossAmount;
                        existingSale.NetAmount = input.NetAmount;
                        existingSale.Remarks = input.Remarks;
                        existingSale.IsActive = input.IsActive;

                        await _saleRepository.UpdateAsync(existingSale);
                        _saleHeader = new Sale { Id = existingSale.Id };
                    }
                }
                else
                {
                    // NOTE: Create a new Sale & Line Level Details (InventoryProduct & NonInventoryProduct)
                    foreach (var lineLevel in input.SalesProducts)
                    {
                        await CreateSalesProductAsync(
                            false,
                            null,
                            lineLevel,
                            new WarehouseDto { Id = warehouse.Id, Name = warehouse.Name, Code = warehouse.Code }
                        );
                    }

                    var sale = ObjectMapper.Map<Sale>(input);
                    _saleHeader.Id = await _saleRepository.InsertAndGetIdAsync(sale);
                }

                // Create / Update / Delete NonInventoryProduct
                await CreateOrUpdateNonInventoryProductAsync(_saleHeader.Id, salesNumber, input.NonInventoryProducts.ToList());

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
            var temp = await _saleRepository
                .GetAllIncluding(t => t.SalesProducts)
                .FirstOrDefaultAsync(t => t.Id == saleId);
            var tempDto = ObjectMapper.Map<SaleDto>(temp);

            // Adding NonInventoryProducts
            tempDto.NonInventoryProducts = await GetNonInventoryProductBySaleIdAsync(saleId);
            return tempDto;
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
                if (sb.SequenceNumber <= 0 || sb.Id == lineLevel.StockBalanceId)
                {
                    sb.AllocatedQuantity += lineLevel.Quantity;
                    sb.BookBalanceQuantity -= lineLevel.Quantity;
                    await _stockBalanceRepository.UpdateAsync(sb);
                }
            }

            if (isExisting)
            {
                var lineLevelProduct = ObjectMapper.Map<SalesProduct>(lineLevel);
                lineLevelProduct.SaleId = existingSaleId.Value;
                await _saleProductRepository.InsertAsync(lineLevelProduct);
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
                        WarehouseId = sb.WarehouseId,
                        WarehouseCode = sb.WarehouseCode,
                        WarehouseName = sb.WarehouseName,
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
                            MaximumRetailPrice = input_nip.MaximumRetailPrice
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
                    p.CustomerId = customerId;
                    p.CustomerCode = customerCode;
                    p.CustomerCode = customerName;
                    //p.CustomerPhoneNumber = input.Customer
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
        #endregion
    }
}
