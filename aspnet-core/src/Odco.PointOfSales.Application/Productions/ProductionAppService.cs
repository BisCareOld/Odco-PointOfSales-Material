using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq;
using Abp.Linq.Extensions;
using Microsoft.EntityFrameworkCore;
using Odco.PointOfSales.Application.GeneralDto;
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

        public ProductionAppService(IRepository<Product, Guid> productRepository)
        {
            _asyncQueryableExecuter = NullAsyncQueryableExecuter.Instance;
            _productRepository = productRepository;
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
    }
}
