using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Odco.PointOfSales.Application.GeneralDto;
using Odco.PointOfSales.Application.Productions.Brands;
using Odco.PointOfSales.Application.Productions.Categories;
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
        Task<List<CommonKeyValuePairDto>> GetPartialProductsAsync(string keyword);
        #endregion

        #region Category
        Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto input);
        Task DeleteCategoryAsync(EntityDto<Guid> input);
        Task<CategoryDto> GetCategoryAsync(EntityDto<Guid> input);
        Task<PagedResultDto<CategoryDto>> GetAllCategoriesAsync(PagedCategoryResultRequestDto input);
        Task<CategoryDto> UpdateCategoryAsync(CategoryDto input);
        #endregion

        #region Brand
        Task<BrandDto> CreateBrandAsync(CreateBrandDto input);
        Task DeleteBrandAsync(EntityDto<Guid> input);
        Task<BrandDto> GetBrandAsync(EntityDto<Guid> input);
        Task<PagedResultDto<BrandDto>> GetAllBrandsAsync(PagedBrandsResultRequestDto input);
        Task<BrandDto> UpdateBrandAsync(BrandDto input);
        #endregion
    }
}
