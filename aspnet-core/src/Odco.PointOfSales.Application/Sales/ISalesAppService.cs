using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Odco.PointOfSales.Application.GeneralDto;
using Odco.PointOfSales.Application.Sales.Customers;
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
        Task<List<CommonKeyValuePairDto>> GetPartialCustomersAsync(string keyword);
        #endregion
    }
}
