using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq;
using Abp.Linq.Extensions;
using Microsoft.EntityFrameworkCore;
using Odco.PointOfSales.Application.GeneralDto;
using Odco.PointOfSales.Application.Productions.Brands;
using Odco.PointOfSales.Application.Productions.Categories;
using Odco.PointOfSales.Application.Productions.Products;
using Odco.PointOfSales.Core.Productions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Odco.PointOfSales.Application.Productions
{
    public class ProductionAppService : ApplicationService, IProductionAppService
    {
        private readonly IAsyncQueryableExecuter _asyncQueryableExecuter;
        private readonly IRepository<Product, Guid> _productRepository;
        private readonly IRepository<Category, Guid> _categoryRepository;
        private readonly IRepository<Brand, Guid> _brandRepository;

        public ProductionAppService(IRepository<Product, Guid> productRepository, 
            IRepository<Category, Guid> categoryRepository,
            IRepository<Brand, Guid> brandRepository)
        {
            _asyncQueryableExecuter = NullAsyncQueryableExecuter.Instance;
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _brandRepository = brandRepository;
        }

        #region Product
        public async Task<ProductDto> CreateProductAsync(CreateProductDto input)
        {
            try
            {
                var product = ObjectMapper.Map<Product>(input);
                var created = await _productRepository.InsertAsync(product);
                await CurrentUnitOfWork.SaveChangesAsync();
                return ObjectMapper.Map<ProductDto>(created);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task DeleteProductAsync(EntityDto<Guid> input)
        {
            try
            {
                var product = await _productRepository.FirstOrDefaultAsync(pt => pt.Id == input.Id); ;
                await _productRepository.DeleteAsync(product);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ProductDto> GetProductAsync(EntityDto<Guid> input)
        {
            try
            {
                var product = await _productRepository.FirstOrDefaultAsync(pt => pt.Id == input.Id);
                var returnDto = ObjectMapper.Map<ProductDto>(product);
                return returnDto;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<PagedResultDto<ProductDto>> GetAllProductsAsync(PagedProductResultRequestDto input)
        {
            try
            {
                var query = _productRepository.GetAll()
                        .WhereIf(!input.Keyword.IsNullOrWhiteSpace(),
                        x => x.Name.Contains(input.Keyword));

                var result = _asyncQueryableExecuter.ToListAsync
                    (
                        query.OrderBy(o => o.CreationTime)
                             .PageBy(input.SkipCount, input.MaxResultCount)
                    );

                var count = await _asyncQueryableExecuter.CountAsync(query);

                var resultDto = ObjectMapper.Map<List<ProductDto>>(result.Result);
                return new PagedResultDto<ProductDto>(count, resultDto);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ProductDto> UpdateProductAsync(UpdateProductDto input)
        {
            try
            {
                var product = await _productRepository.FirstOrDefaultAsync(pt => pt.Id == input.Id);
                ObjectMapper.Map(input, product);
                var updated = await _productRepository.UpdateAsync(product);
                await CurrentUnitOfWork.SaveChangesAsync();
                return ObjectMapper.Map<ProductDto>(updated);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<CommonKeyValuePairDto>> GetPartialProductsAsync(string keyword, Guid? supplierId)
        {
            if (string.IsNullOrEmpty(keyword))
                return new List<CommonKeyValuePairDto>();

            var products  = await _productRepository.GetAllListAsync();

            return products.Where(p => p.Code.Contains(keyword) || p.Name.ToLower().Contains(keyword))
                .OrderBy(p => p.Name)
                .Take(PointOfSalesConsts.MaximumNumberOfReturnResults)
                .Select(p => new CommonKeyValuePairDto
                {
                    Id = p.Id,
                    Code = p.Code,
                    Name = p.Name,
                })
                .ToList();
        }
        #endregion

        #region Category
        public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto input)
        {
            try
            {
                var category = ObjectMapper.Map<Category>(input);
                var created = await _categoryRepository.InsertAsync(category);
                await CurrentUnitOfWork.SaveChangesAsync();
                return ObjectMapper.Map<CategoryDto>(created);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task DeleteCategoryAsync(EntityDto<Guid> input)
        {
            try
            {
                var category = _categoryRepository.GetAll().FirstOrDefault(pt => pt.Id == input.Id); ;
                await _categoryRepository.DeleteAsync(category);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<PagedResultDto<CategoryDto>> GetAllCategoriesAsync(PagedCategoryResultRequestDto input)
        {
            try
            {
                var query = _categoryRepository.GetAll()
                        .WhereIf(!input.Keyword.IsNullOrWhiteSpace(),
                        x => x.Name.Contains(input.Keyword));

                var result = _asyncQueryableExecuter.ToListAsync
                    (
                        query.OrderBy(o => o.CreationTime)
                             .PageBy(input.SkipCount, input.MaxResultCount)
                    );

                var count = await _asyncQueryableExecuter.CountAsync(query);

                var resultDto = ObjectMapper.Map<List<CategoryDto>>(result.Result);
                return new PagedResultDto<CategoryDto>(count, resultDto);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<CategoryDto> GetCategoryAsync(EntityDto<Guid> input)
        {
            try
            {
                var category = await _categoryRepository.FirstOrDefaultAsync(pt => pt.Id == input.Id);
                return ObjectMapper.Map<CategoryDto>(category);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<CategoryDto> UpdateCategoryAsync(CategoryDto input)
        {
            try
            {
                var category = _categoryRepository.GetAll().FirstOrDefault(pt => pt.Id == input.Id);
                ObjectMapper.Map(input, category);
                await _categoryRepository.UpdateAsync(category);
                await CurrentUnitOfWork.SaveChangesAsync();
                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<CommonKeyValuePairDto>> GetAllCategoriesKeyValuePairAsync()
        {
            return await _categoryRepository.GetAll().Select(c => new CommonKeyValuePairDto
            {
                Id = c.Id,
                Name = c.Name
            }).ToListAsync();
        }
        #endregion

        #region Brand
        public async Task<BrandDto> CreateBrandAsync(CreateBrandDto input)
        {
            try
            {
                var brand = ObjectMapper.Map<Brand>(input);
                var created = await _brandRepository.InsertAsync(brand);
                await CurrentUnitOfWork.SaveChangesAsync();
                return ObjectMapper.Map<BrandDto>(created);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task DeleteBrandAsync(EntityDto<Guid> input)
        {
            try
            {
                var brand = _brandRepository.GetAll().FirstOrDefault(pt => pt.Id == input.Id); ;
                await _brandRepository.DeleteAsync(brand);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<PagedResultDto<BrandDto>> GetAllBrandsAsync(PagedBrandsResultRequestDto input)
        {
            try
            {
                var query = _brandRepository.GetAll()
                        .WhereIf(!input.Keyword.IsNullOrWhiteSpace(),
                        x => x.Name.Contains(input.Keyword));

                var result = _asyncQueryableExecuter.ToListAsync
                    (
                        query.OrderBy(o => o.CreationTime)
                             .PageBy(input.SkipCount, input.MaxResultCount)
                    );

                var count = await _asyncQueryableExecuter.CountAsync(query);

                var resultDto = ObjectMapper.Map<List<BrandDto>>(result.Result);
                return new PagedResultDto<BrandDto>(count, resultDto);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<BrandDto> GetBrandAsync(EntityDto<Guid> input)
        {
            try
            {
                var brand = await _brandRepository.FirstOrDefaultAsync(pt => pt.Id == input.Id);
                return ObjectMapper.Map<BrandDto>(brand);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<BrandDto> UpdateBrandAsync(BrandDto input)
        {
            try
            {
                var brand = _brandRepository.GetAll().FirstOrDefault(pt => pt.Id == input.Id);
                ObjectMapper.Map(input, brand);
                await _brandRepository.UpdateAsync(brand);
                await CurrentUnitOfWork.SaveChangesAsync();
                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<CommonKeyValuePairDto>> GetAllBrandsKeyValuePairAsync()
        {
            return await _categoryRepository.GetAll().Select(c => new CommonKeyValuePairDto
            {
                Id = c.Id,
                Name = c.Name
            }).ToListAsync();
        }
        #endregion
    }
}
