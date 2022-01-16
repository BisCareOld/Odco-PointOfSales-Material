using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq;
using Abp.Linq.Extensions;
using Odco.PointOfSales.Application.Common.SequenceNumbers;
using Odco.PointOfSales.Application.Finance.Payments;
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
        private readonly IRepository<Payment, Guid> _paymentRepository;
        private readonly IRepository<Sale, Guid> _saleRepository;
        private readonly IDocumentSequenceNumberManager _documentSequenceNumberManager;
        private readonly IRepository<CustomerOutstanding, Guid> _customerOutstandingRepository;
        private readonly IRepository<CustomerOutstandingSettlement, Guid> _customerOutstandingSettlementRepository;

        public FinanceAppService(
            IRepository<Payment, Guid> paymentRepository,
            IRepository<Sale, Guid> saleRepository,
            IDocumentSequenceNumberManager documentSequenceNumberManager,
            IRepository<CustomerOutstanding, Guid> customerOutstandingRepository,
            IRepository<CustomerOutstandingSettlement, Guid> customerOutstandingSettlementRepository
            )
        {
            _asyncQueryableExecuter = NullAsyncQueryableExecuter.Instance;
            _paymentRepository = paymentRepository;
            _saleRepository = saleRepository;
            _documentSequenceNumberManager = documentSequenceNumberManager;
            _customerOutstandingRepository = customerOutstandingRepository;
            _customerOutstandingSettlementRepository = customerOutstandingSettlementRepository;
        }

        #region Payment
        public async Task<PaymentDto> CreatePaymentForCustomerOutstandingAsync(CreatePaymentForOutstandingDto input)
        {
            // 1. Map to 
            //      Payment
            //      PaymentLineLevel
            //      CustomerOutstanding
            //      CustomerOutstandingSettlement
            //      Update Sale => Payment Status

            var invoiceNumber = string.Empty;

            bool isAnySalesChecked = input.OutstandingSales.Any(os => os.IsSelected);

            if (input.Cashes.Any() || input.Cheques.Any() || input.DebitCards.Any() || input.GiftCards.Any())
                invoiceNumber = await _documentSequenceNumberManager.GetAndUpdateNextDocumentNumberAsync(DocumentType.Invoice);

            #region Payment
            var payment = new Payment();
            payment.Id = Guid.NewGuid();
            payment.SaleId = null;
            payment.SalesNumber = null;
            payment.InvoiceNumber = invoiceNumber;
            payment.CustomerId = input.CustomerId;
            payment.CustomerCode = input.CustomerCode;
            payment.CustomerName = !string.IsNullOrEmpty(input.CustomerName) ? input.CustomerName.Trim() : null;
            payment.TotalReceivedAmount = input.TotalReceivedAmount;
            payment.TotalBalanceAmount = input.TotalBalanceAmount;
            payment.TotalPaidAmount = input.TotalReceivedAmount - input.TotalBalanceAmount;
            payment.Remarks = input.Remarks;
            payment.IsOutstandingPaymentInvolved = true;
            #endregion

            #region Payment Line Level
            decimal remainingPaidAmount1 = input.TotalReceivedAmount - input.TotalBalanceAmount;

            if (input.Cashes.Any())
            {
                decimal _amountPaid = 0;

                // Get the total amount of cash payments
                var totalCashAmount = input.Cashes.Select(c => c.CashAmount).Sum();
                if (remainingPaidAmount1 <= totalCashAmount)
                {
                    _amountPaid = remainingPaidAmount1;
                    remainingPaidAmount1 = 0;
                }
                else
                {
                    _amountPaid = totalCashAmount;
                    remainingPaidAmount1 -= totalCashAmount;
                }
                payment.PaymentLineLevels.Add(new PaymentLineLevel
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

                if (remainingPaidAmount1 <= ip.ChequeAmount)
                {
                    _amountPaid = remainingPaidAmount1;
                    remainingPaidAmount1 = 0;
                }
                else
                {
                    _amountPaid = ip.ChequeAmount;
                    remainingPaidAmount1 -= ip.ChequeAmount;
                }
                payment.PaymentLineLevels.Add(new PaymentLineLevel
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
            foreach (var ip in input.DebitCards)
            {
                payment.PaymentLineLevels.Add(new PaymentLineLevel
                {
                    IsDebitCard = true,
                });
            }
            foreach (var ip in input.GiftCards)
            {
                payment.PaymentLineLevels.Add(new PaymentLineLevel
                {
                    PaidAmount = ip.GiftCardAmount,
                    IsGiftCard = true,
                });
            }

            foreach (var pll in payment.PaymentLineLevels)
            {
                pll.InvoiceNumber = invoiceNumber;

                pll.SaleId = null;
                pll.SalesNumber = null;

                pll.CustomerId = input.CustomerId;
                pll.CustomerCode = input.CustomerCode;
                pll.CustomerName = !string.IsNullOrEmpty(input.CustomerName) ? input.CustomerName.Trim() : null;

                pll.SpecificReceivedAmount = pll.SpecificReceivedAmount;
                pll.SpecificBalanceAmount = pll.SpecificBalanceAmount;

                pll.PaidAmount = pll.PaidAmount;
                pll.IsCash = pll.IsCash;
                pll.IsCheque = pll.IsCheque;
                pll.IsDebitCard = pll.IsDebitCard;
                pll.IsGiftCard = pll.IsGiftCard;
                pll.IsOutstandingPaymentInvolved = pll.IsOutstandingPaymentInvolved;
            }

            await _paymentRepository.InsertAsync(payment);
            #endregion

            #region CustomerOutstanding & CustomerOutstandingSettlements
            var inputOutstandingSales = isAnySalesChecked ?
                                        input.OutstandingSales.Where(os => os.IsSelected).ToList() :
                                        input.OutstandingSales.ToList();
            
            var paramA = inputOutstandingSales.Select(os => os.SaleId).ToArray();
            var sales = GetSalesQueryBySaleIdsAsync(paramA).Result.ToList();

            var customerOutstandings = GetCustomerOutstandingsQueryBySaleIdAsync(input.CustomerId.Value).Result.ToList();

            decimal remainingPaidAmount2 = input.TotalReceivedAmount - input.TotalBalanceAmount;

            foreach (var ios in inputOutstandingSales)
            {
                var sale = sales.FirstOrDefault(s => s.Id == ios.SaleId);

                var customerOutstanding = customerOutstandings.FirstOrDefault(co => co.SaleId == ios.SaleId);

                var cos = new CustomerOutstandingSettlement
                {
                    CustomerOutstandingId = customerOutstanding.Id,
                    CustomerId = customerOutstanding.CustomerId,
                    CustomerCode = customerOutstanding.CustomerCode,
                    CustomerName = customerOutstanding.CustomerName,
                    SaleId = customerOutstanding.SaleId,
                    SalesNumber = customerOutstanding.SalesNumber,
                    PaymentId = payment.Id,
                    InvoiceNumber = invoiceNumber,
                    PaidAmount = 0      // Value set on below code
                };

                if (remainingPaidAmount2 < customerOutstanding.DueOutstandingAmount)
                {
                    sale.PaymentStatus = PaymentStatus.PartiallyPaid;

                    cos.PaidAmount = remainingPaidAmount2;
                    await CreateCustomerOutstandingSettlementAsync(cos);

                    customerOutstanding.DueOutstandingAmount -= remainingPaidAmount2;
                    remainingPaidAmount2 = 0;
                }
                else
                {
                    sale.PaymentStatus = PaymentStatus.Completed;

                    cos.PaidAmount = customerOutstanding.DueOutstandingAmount;
                    await CreateCustomerOutstandingSettlementAsync(cos);

                    remainingPaidAmount2 -= customerOutstanding.DueOutstandingAmount;
                    customerOutstanding.DueOutstandingAmount = 0;
                }
                if (remainingPaidAmount2 == 0) break;
            }
            #endregion

            return new PaymentDto { Id = payment.Id, InvoiceNumber = invoiceNumber };
        }

        public async Task<PagedResultDto<PaymentDto>> GetAllPaymentsAsync(PagedPaymentResultRequestDto input)
        {
            try
            {
                var query = _paymentRepository.GetAll()
                        .WhereIf(!input.Keyword.IsNullOrWhiteSpace(),
                        x => x.CustomerName.Contains(input.Keyword));

                var result = _asyncQueryableExecuter.ToListAsync
                    (
                        query.OrderByDescending(o => o.CreationTime)
                             .PageBy(input.SkipCount, input.MaxResultCount)
                    );

                var count = await _asyncQueryableExecuter.CountAsync(query);

                var resultDto = ObjectMapper.Map<List<PaymentDto>>(result.Result);
                return new PagedResultDto<PaymentDto>(count, resultDto);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        private async Task<IQueryable<Sale>> GetSalesQueryBySaleIdsAsync(Guid[] saleIds)
        {
            return _saleRepository.GetAll().Where(s => saleIds.Any(sId => sId == s.Id));
        }

        private async Task<IQueryable<CustomerOutstanding>> GetCustomerOutstandingsQueryBySaleIdAsync(Guid customerId)
        {
            return _customerOutstandingRepository.GetAll().Where(co => co.CustomerId == customerId);
        }

        private async Task CreateCustomerOutstandingSettlementAsync(CustomerOutstandingSettlement cos)
        {
            await _customerOutstandingSettlementRepository.InsertAsync(cos);
        }

    }
}
