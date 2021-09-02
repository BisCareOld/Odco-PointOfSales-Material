using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Odco.PointOfSales.Application.GeneralDto;
using Odco.PointOfSales.Application.Purchasings.Suppliers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Odco.PointOfSales.Application.Purchasings
{
    public interface IPurchasingAppService : IApplicationService
    {
        #region Supplier
        Task<SupplierDto> CreateSupplierAsync(CreateSupplierDto input);
        Task DeleteSupplierAsync(EntityDto<Guid> input);
        Task<PagedResultDto<SupplierDto>> GetAllSuppliersAsync(PagedSupplierResultRequestDto input);
        Task<SupplierDto> GetSupplierAsync(EntityDto<Guid> input);
        Task<SupplierDto> UpdateSupplierAsync(SupplierDto input);
        Task<List<CommonKeyValuePairDto>> GetPartialSuppliersAsync(string keyword);
        #endregion
    }
}
