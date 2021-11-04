using Abp.Application.Services;
using Abp.Domain.Repositories;
using Abp.Linq;
using Odco.PointOfSales.Application.Common.SequenceNumbers;
using Odco.PointOfSales.Application.Finance.Invoices;
using Odco.PointOfSales.Core.Enums;
using Odco.PointOfSales.Core.Finance;
using Odco.PointOfSales.Core.Sales;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Odco.PointOfSales.Application.Finance
{
    public class FinanceAppService : ApplicationService, IFinanceAppService
    {
        private readonly IAsyncQueryableExecuter _asyncQueryableExecuter;
        private readonly IRepository<Invoice, Guid> _invoiceRepository;
        private readonly IRepository<InvoiceProduct, Guid> _invoiceProductRepository;
        private readonly IRepository<Payment, Guid> _paymentRepository;
        private readonly IRepository<TempSalesHeader, int> _tempSalesHeaderRepository;
        private readonly IDocumentSequenceNumberManager _documentSequenceNumberManager;

        public FinanceAppService(IRepository<Invoice, Guid> invoiceRepository,
            IRepository<InvoiceProduct, Guid> invoiceProductRepository,
            IRepository<Payment, Guid> paymentRepository,
            IRepository<TempSalesHeader, int> tempSalesHeaderRepository,
            IDocumentSequenceNumberManager documentSequenceNumberManager)
        {
            _asyncQueryableExecuter = NullAsyncQueryableExecuter.Instance;
            _invoiceRepository = invoiceRepository;
            _invoiceProductRepository = invoiceProductRepository;
            _paymentRepository = paymentRepository;
            _tempSalesHeaderRepository = tempSalesHeaderRepository;
            _documentSequenceNumberManager = documentSequenceNumberManager;
        }

        public async Task<InvoiceDto> CreateInvoiceAsync(CreateInvoiceDto input)
        {
            var invoiceProducts = new List<InvoiceProduct>();
            var payments = new List<Payment>();

            var tempSalesHeader = _tempSalesHeaderRepository
                .GetAllIncluding(tsh => tsh.TempSalesProducts)
                .FirstOrDefault(tsh => tsh.Id == input.TempSalesHeaderId);

            var invoiceNumber = await _documentSequenceNumberManager.GetAndUpdateNextDocumentNumberAsync(DocumentType.Invoice);

            #region Map TempSalesProduct = InvoiceProduct (LineLevel)
            foreach (var tsp in tempSalesHeader.TempSalesProducts)
            {
                if (tsp.IsActive)
                {
                    invoiceProducts.Add(new InvoiceProduct
                    {
                        // InvoiceId = 
                        // InvoiceNumber = 
                        StockBalanceId = tsp.StockBalanceId,
                        ProductId = tsp.ProductId,
                        ProductCode = tsp.Code,
                        ProductName = tsp.Name,
                        WarehouseId = tsp.WarehouseId.Value,
                        WarehouseCode = tsp.WarehouseCode,
                        WarehouseName = tsp.WarehouseName,
                        CostPrice = tsp.CostPrice,
                        SellingPrice = tsp.SellingPrice,
                        MaximumRetailPrice = tsp.MaximumRetailPrice,
                        Quantity = tsp.Quantity,
                        Price = tsp.SellingPrice,
                        DiscountRate = tsp.DiscountRate,
                        DiscountAmount = tsp.DiscountAmount,
                        LineTotal = tsp.LineTotal,
                        // Remarks = 
                    });
                }
            }
            #endregion

            #region Map Payment from Payments[]
            foreach (var ip in input.Cashes)
            {
                payments.Add(new Payment
                {
                    CashAmount = ip.CashAmount,
                    IsCash = true,
                });
            }
            foreach (var ip in input.Cheques)
            {
                payments.Add(new Payment
                {
                    ChequeAmount = ip.ChequeAmount,
                    ChequeNumber = ip.ChequeNumber,
                    ChequeReturnDate = ip.ChequeReturnDate,
                    BankId = ip.BankId,
                    Bank = ip.Bank,
                    BranchId = ip.BranchId,
                    Branch = ip.Branch,
                    IsCheque = true,
                });
            }
            foreach (var ip in input.Outstandings)
            {
                payments.Add(new Payment
                {
                    OutstandingAmount = ip.OutstandingAmount,
                    OutstandingSettledAmount = ip.OutstandingAmount,
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
                    GiftCardAmount = ip.GiftCardAmount,
                    IsGiftCard = true,
                });
            }

            foreach(var p in payments)
            {
                p.CustomerId = input.CustomerId;
                //p.CustomerPhoneNumber = input.Customer
                p.InvoiceNumber = invoiceNumber;
            }
            #endregion

            #region Map TempSalesHeader = Invoice (HeaderLevel)
            var invoice = new Invoice
            {
                TempSalesHeaderId = input.TempSalesHeaderId,
                // InvoiceNumber = input.
                // ReferenceNumber = tempSalesHeader.Refer
                CustomerId = input.CustomerId,
                CustomerCode = input.CustomerCode,
                CustomerName = input.CustomerName,
                DiscountRate = input.DiscountRate,
                DiscountAmount = input.DiscountAmount,
                TaxRate = input.TaxRate,
                TaxAmount = input.TaxAmount,
                GrossAmount = input.GrossAmount,
                NetAmount = input.NetAmount,
                //Remarks = input.Rema
                InvoiceProducts = invoiceProducts,
                Payments = payments
            };
            #endregion

            var created = await _invoiceRepository.InsertAsync(invoice);
            
            tempSalesHeader.IsActive = false;
            await _tempSalesHeaderRepository.UpdateAsync(tempSalesHeader);

            await CurrentUnitOfWork.SaveChangesAsync();
            return ObjectMapper.Map<InvoiceDto>(created);
        }
    }
}
