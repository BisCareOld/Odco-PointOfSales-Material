using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Odco.PointOfSales.Application.Finance.Payments;
using System;
using System.Threading.Tasks;

namespace Odco.PointOfSales.Application.Finance
{
    public interface IFinanceAppService : IApplicationService
    {
        #region Payment
        Task<PaymentDto> CreatePaymentForCustomerOutstandingAsync(CreatePaymentForOutstandingDto input);
        Task<PagedResultDto<PaymentDto>> GetAllPaymentsAsync(PagedPaymentResultRequestDto input);
        Task<PaymentDto> GetPaymentAsync(Guid paymentId);
        #endregion
    }
}
