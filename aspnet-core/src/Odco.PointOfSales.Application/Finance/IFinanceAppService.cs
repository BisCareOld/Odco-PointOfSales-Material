using Abp.Application.Services;
using Odco.PointOfSales.Application.Finance.Invoices;
using System.Threading.Tasks;

namespace Odco.PointOfSales.Application.Finance
{
    public interface IFinanceAppService : IApplicationService
    {
        //Task<InvoiceDto> CreateInvoiceAsync(CreateInvoiceDto input);
    }
}
