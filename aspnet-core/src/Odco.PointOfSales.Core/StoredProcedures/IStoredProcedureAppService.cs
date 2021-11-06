using Abp.Domain.Repositories;
using Odco.PointOfSales.Core.Inventory;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Odco.PointOfSales.Core.StoredProcedures
{
    public interface IStoredProcedureAppService: IRepository<StockBalance, Guid>
    {
        Task<List<StockBalance>> GetStockBalancesByStockBalanceId(Guid[] stockBalanceIds);
    }
}
