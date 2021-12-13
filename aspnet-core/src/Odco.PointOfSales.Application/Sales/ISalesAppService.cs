using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Odco.PointOfSales.Application.Inventory.NonInventoryProducts;
using Odco.PointOfSales.Application.Inventory.StockBalances;
using Odco.PointOfSales.Application.Sales.Customers;
using Odco.PointOfSales.Application.Sales.TemporarySales;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Odco.PointOfSales.Application.Sales
{
    public interface ISalesAppService : IApplicationService
    {
        #region Customer
        Task<CustomerDto> CreateCustomerAsync(CreateCustomerDto input);
        Task DeleteCustomerAsync(EntityDto<Guid> input);
        Task<PagedResultDto<CustomerDto>> GetAllCustomersAsync(PagedCustomerResultRequestDto input);
        Task<CustomerDto> GetCustomerAsync(EntityDto<Guid> input);
        Task<CustomerDto> UpdateCustomerAsync(CustomerDto input);
        Task<List<CustomerSearchResultDto>> GetPartialCustomersAsync(string keyword);
        #endregion

        #region TemporarySales Header + Products
        Task<TempSaleDto> CreateOrUpdateTempSalesAsync(CreateOrUpdateTempSaleDto input);
        Task<PagedResultDto<TempSaleDto>> GetAllTempSalesAsync(PagedTempSaleResultRequestDto input);
        Task<TempSaleDto> GetTempSalesAsync(int tempSaleId);
        #endregion

        #region StockBalance
        Task<List<ProductStockBalanceDto>> GetStockBalancesByStockBalanceIdsAsync(Guid[] stockBalanceIds);
        #endregion

        #region NonInventoryProduct
        Task<List<NonInventoryProductDto>> GetNonInventoryProductByTempSaleIdAsync(int tempSaleId);
        #endregion
    }
}
