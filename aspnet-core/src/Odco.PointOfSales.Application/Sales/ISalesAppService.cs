using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Odco.PointOfSales.Application.Inventory.NonInventoryProducts;
using Odco.PointOfSales.Application.Inventory.StockBalances;
using Odco.PointOfSales.Application.Sales.Customers;
using Odco.PointOfSales.Application.Sales.Sales;
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

        #region Sale Header + Products
        Task<SaleDto> CreateOrUpdateSalesAsync(CreateOrUpdateSaleDto input);
        Task<PagedResultDto<SaleDto>> GetAllSalesAsync(PagedSaleResultRequestDto input);
        Task<SaleDto> GetSalesAsync(Guid saleId);
        #endregion

        #region StockBalance
        /// GroupBy SellingPrice
        Task<List<GroupBySellingPriceDto>> GetStockBalancesByProductIdAsync(Guid productId);
        #endregion

        #region NonInventoryProduct
        Task<List<NonInventoryProductDto>> GetNonInventoryProductBySaleIdAsync(Guid saleId);
        #endregion
    }
}
