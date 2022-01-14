using Abp.Application.Services;
using Odco.PointOfSales.Application.Finance.Payments;
using System.Threading.Tasks;

namespace Odco.PointOfSales.Application.Finance
{
    public interface IFinanceAppService : IApplicationService
    {
        #region Payment
        Task<PaymentDto> CreatePaymentForCustomerOutstandingAsync(CreatePaymentForOutstandingDto input);
        #endregion
    }
}
