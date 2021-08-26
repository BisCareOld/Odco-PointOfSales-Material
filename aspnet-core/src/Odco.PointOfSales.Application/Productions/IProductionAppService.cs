using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Odco.PointOfSales.Application.GeneralDto;
using Odco.PointOfSales.Application.Productions.Products;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Odco.PointOfSales.Application.Productions
{
    public interface IProductionAppService : IApplicationService
    {
        #region Product
        Task<ProductDto> CreateProductAsync(CreateProductDto input);
        Task DeleteProductAsync(EntityDto<Guid> input);
        Task<ProductDto> GetProductAsync(EntityDto<Guid> input);
        Task<PagedResultDto<ProductDto>> GetAllProductsAsync(PagedProductResultRequestDto input);
        Task<ProductDto> UpdateProductAsync(UpdateProductDto input);
        Task<List<CommonKeyValuePairDto>> GetPartialProductsAsync(string keyword, Guid? supplierId);
        #endregion
    }
}
