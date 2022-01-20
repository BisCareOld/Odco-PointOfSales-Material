using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Odco.PointOfSales.Application.Finance.CustomerOutstandingSettlements;
using Odco.PointOfSales.Application.Finance.Payments;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Odco.PointOfSales.Application.Finance
{
    public interface IFinanceAppService : IApplicationService
    {
        #region Payment
        Task<PaymentDto> CreatePaymentForCustomerOutstandingAsync(CreatePaymentForOutstandingDto input);
        Task<PagedResultDto<PaymentDto>> GetAllPaymentsAsync(PagedPaymentResultRequestDto input);
        Task<PaymentDto> GetPaymentAsync(Guid paymentId);
        Task<List<InvoiceNumberDto>> GetAllInvoiceNumbersBySaleIdAsync(Guid saleId);
        #endregion

        #region CustomerOutstandingSettlements
        Task<List<CustomerOutstandingSettlementDto>> GetCustomerOutstandingSettlementsByPaymentIdAsync(Guid paymentId);
        #endregion
    }
}
