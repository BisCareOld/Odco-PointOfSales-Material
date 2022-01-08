using Abp.Application.Services;
using Abp.Domain.Repositories;
using Abp.Linq;
using Odco.PointOfSales.Application.Common.SequenceNumbers;
using Odco.PointOfSales.Core.Finance;
using Odco.PointOfSales.Core.Sales;
using System;

namespace Odco.PointOfSales.Application.Finance
{
    public class FinanceAppService : ApplicationService, IFinanceAppService
    {
        private readonly IAsyncQueryableExecuter _asyncQueryableExecuter;
        private readonly IRepository<Payment, Guid> _paymentRepository;
        private readonly IRepository<Sale, Guid> _saleRepository;
        private readonly IDocumentSequenceNumberManager _documentSequenceNumberManager;

        public FinanceAppService(
            IRepository<Payment, Guid> paymentRepository,
            IRepository<Sale, Guid> saleRepository,
            IDocumentSequenceNumberManager documentSequenceNumberManager)
        {
            _asyncQueryableExecuter = NullAsyncQueryableExecuter.Instance;
            _paymentRepository = paymentRepository;
            _saleRepository = saleRepository;
            _documentSequenceNumberManager = documentSequenceNumberManager;
        }

        

    }
}
