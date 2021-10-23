using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq;
using Abp.Linq.Extensions;
using Microsoft.EntityFrameworkCore;
using Odco.PointOfSales.Application.GeneralDto;
using Odco.PointOfSales.Application.Inventory.StockBalances;
using Odco.PointOfSales.Application.Productions.Brands;
using Odco.PointOfSales.Application.Productions.Categories;
using Odco.PointOfSales.Application.Productions.Products;
using Odco.PointOfSales.Application.Productions.Warehouses;
using Odco.PointOfSales.Core.Enums;
using Odco.PointOfSales.Core.Inventory;
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
        private readonly IRepository<StockBalance, Guid> _stockBalanceRepository;
        private readonly IRepository<Warehouse, Guid> _warehouseRepository;

        public ProductionAppService(IRepository<Product, Guid> productRepository,
            IRepository<Category, Guid> categoryRepository,
            IRepository<Brand, Guid> brandRepository,
            IRepository<StockBalance, Guid> stockBalanceRepository,
            IRepository<Warehouse, Guid> warehouseRepository)
        {
            _asyncQueryableExecuter = NullAsyncQueryableExecuter.Instance;
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _brandRepository = brandRepository;
            _stockBalanceRepository = stockBalanceRepository;
            _warehouseRepository = warehouseRepository;
        }

        #region Product
        public async Task<ProductDto> CreateProductAsync(CreateProductDto input)
        {
            try
            {
                var product = ObjectMapper.Map<Product>(input);
                var created = await _productRepository.InsertAsync(product);
                await CreateOrUpdateStockBalance(created.Id, created.Code, created.Name);
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
                await CreateOrUpdateStockBalance(updated.Id, updated.Code, updated.Name);
                await CurrentUnitOfWork.SaveChangesAsync();
                return ObjectMapper.Map<ProductDto>(updated);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<CommonKeyValuePairDto>> GetPartialProductsAsync(string keyword)
        {
            if (string.IsNullOrEmpty(keyword))
                return new List<CommonKeyValuePairDto>();

            var products = await _productRepository.GetAllListAsync();

            return products.Where(p => p.IsActive && (p.Code.Contains(keyword) || p.Name.ToLower().Contains(keyword)))
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

        public async Task<List<ProductSearchResultDto>> GetPartialProductsByTypesAsync(ProductSearchType type, string keyword)
        {
            if (string.IsNullOrEmpty(keyword))
                return new List<ProductSearchResultDto>();

            List<Product> products = new List<Product>();

            if (type == ProductSearchType.Barcode)
            {
                products = await _productRepository
                    .GetAll()
                    .Where(p => p.IsActive && p.BarCode.ToLower().Contains(keyword))
                    .OrderBy(p => p.BarCode)
                    .Take(PointOfSalesConsts.MaximumNumberOfReturnResults)
                    .ToListAsync();
            }
            else if (type == ProductSearchType.Code)
            {
                products = await _productRepository
                    .GetAll()
                    .Where(p => p.IsActive && p.Code.ToLower().Contains(keyword))
                    .OrderBy(p => p.Code)
                    .Take(PointOfSalesConsts.MaximumNumberOfReturnResults)
                    .ToListAsync();
            }
            else if (type == ProductSearchType.Name)
            {
                products = await _productRepository
                    .GetAll()
                    .Where(p => p.IsActive && p.Name.ToLower().Contains(keyword))
                    .OrderBy(p => p.Name)
                    .Take(PointOfSalesConsts.MaximumNumberOfReturnResults)
                    .ToListAsync();
            }

            return products.Select(p => new ProductSearchResultDto
            {
                Id = p.Id,
                BarCode = p.BarCode,
                Code = p.Code,
                Name = p.Name,
                IsActive = p.IsActive
            }).ToList();
        }
        #endregion

        #region Warehouse
        public async Task<WarehouseDto> CreateWarehouseAsync(CreateWarehouseDto input)
        {
            try
            {
                var warehouse = ObjectMapper.Map<Warehouse>(input);
                var created = await _warehouseRepository.InsertAsync(warehouse);
                await CurrentUnitOfWork.SaveChangesAsync();
                return ObjectMapper.Map<WarehouseDto>(created);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task DeleteWarehouseAsync(EntityDto<Guid> input)
        {
            try
            {
                var warehouse = await _warehouseRepository.FirstOrDefaultAsync(pt => pt.Id == input.Id); ;
                await _warehouseRepository.DeleteAsync(warehouse);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<WarehouseDto> GetWarehouseAsync(EntityDto<Guid> input)
        {
            try
            {
                var warehouse = await _warehouseRepository.FirstOrDefaultAsync(pt => pt.Id == input.Id);
                var returnDto = ObjectMapper.Map<WarehouseDto>(warehouse);
                return returnDto;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<PagedResultDto<WarehouseDto>> GetAllWarehousesAsync(PagedWarehouseResultRequestDto input)
        {
            try
            {
                var query = _warehouseRepository.GetAll()
                        .WhereIf(!input.Keyword.IsNullOrWhiteSpace(),
                        x => x.Name.Contains(input.Keyword));

                var result = _asyncQueryableExecuter.ToListAsync
                    (
                        query.OrderBy(o => o.CreationTime)
                             .PageBy(input.SkipCount, input.MaxResultCount)
                    );

                var count = await _asyncQueryableExecuter.CountAsync(query);

                var resultDto = ObjectMapper.Map<List<WarehouseDto>>(result.Result);
                return new PagedResultDto<WarehouseDto>(count, resultDto);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<WarehouseDto> UpdateWarehouseAsync(UpdateWarehouseDto input)
        {
            try
            {
                var warehouse = await _warehouseRepository.FirstOrDefaultAsync(pt => pt.Id == input.Id);
                ObjectMapper.Map(input, warehouse);
                var updated = await _warehouseRepository.UpdateAsync(warehouse);
                await CurrentUnitOfWork.SaveChangesAsync();
                return ObjectMapper.Map<WarehouseDto>(updated);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<CommonKeyValuePairDto>> GetAllKeyValuePairWarehousesAsync()
        {
            return await _warehouseRepository.GetAll()
                .Where(w => w.IsActive)
                .OrderBy(w => w.Name)
                .Select(w => new CommonKeyValuePairDto
                {
                    Id = w.Id,
                    Code = w.Code,
                    Name = w.Name,
                })
                .ToListAsync();
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

        #region Stock Balance
        public async Task<ResponseDto<ProductStockBalanceDto>> GetStockBalancesByProductIdAsync(Guid productId)
        {
            var warehouse = await _warehouseRepository.FirstOrDefaultAsync(w => w.IsDefault);
            if (warehouse == null) return new ResponseDto<ProductStockBalanceDto>
            {
                StatusCode = 404,
                Message = "Default warehouse is not defined",
                Items = new ProductStockBalanceDto[] { }
            };

            var stockBalances = _stockBalanceRepository.GetAll()
                .Where(sb => sb.SequenceNumber > 0 && sb.ProductId == productId && sb.WarehouseId == warehouse.Id && sb.BookBalanceQuantity > 0)
                .Select(sb => new ProductStockBalanceDto
                {
                    StockBalanceId = sb.Id,
                    ProductId = sb.ProductId,
                    ExpiryDate = sb.ExpiryDate,
                    BatchNumber = sb.BatchNumber,
                    AllocatedQuantity = sb.AllocatedQuantity,
                    AllocatedQuantityUnitOfMeasureUnit = sb.AllocatedQuantityUnitOfMeasureUnit,
                    BookBalanceQuantity = sb.BookBalanceQuantity,
                    BookBalanceUnitOfMeasureUnit = sb.BookBalanceUnitOfMeasureUnit,
                    CostPrice = sb.CostPrice,
                    SellingPrice = sb.SellingPrice,
                    MaximumRetailPrice = sb.MaximumRetailPrice
                });
            return new ResponseDto<ProductStockBalanceDto>
            {
                StatusCode = 200,
                Message = "",
                Items = stockBalances.ToArray()
            };
        }

        // Delete if not needed
        //public async Task<ProductStockBalanceDto> GetStockBalancesByStockBalaneIdsAsync(int[] ids)
        //{
        //    return _stockBalanceRepository.GetAll()
        //        .Where(sb => sb.SequenceNumber > 0 && sb.ProductId == productId && sb.WarehouseId == warehouse.Id && sb.BookBalanceQuantity > 0)
        //        .Select(sb => new ProductStockBalanceDto
        //        {
        //            StockBalanceId = sb.Id,
        //            ProductId = sb.ProductId,
        //            ExpiryDate = sb.ExpiryDate,
        //            BatchNumber = sb.BatchNumber,
        //            BookBalanceQuantity = sb.BookBalanceQuantity,
        //            BookBalanceUnitOfMeasureUnit = sb.BookBalanceUnitOfMeasureUnit,
        //            CostPrice = sb.CostPrice,
        //            SellingPrice = sb.SellingPrice,
        //            MaximumRetailPrice = sb.MaximumRetailPrice
        //        });
        //}
        #endregion

        private async Task CreateOrUpdateStockBalance(Guid productId, string productCode, string productName)
        {
            var stockBalances = _stockBalanceRepository.GetAll().Where(sb => sb.ProductId == productId);

            if (stockBalances.Count() > 0)
            {
                foreach (var sb in stockBalances)
                {
                    sb.ProductId = productId;
                    sb.ProductCode = productCode;
                    sb.ProductName = productName;
                    await _stockBalanceRepository.UpdateAsync(sb);
                }
            }
            else
            {
                // Warehouse summary
                var warehouses = await GetAllKeyValuePairWarehousesAsync();

                // Company summary
                // Initializing with a Empty (Null) warehouse
                warehouses.Add(new CommonKeyValuePairDto());

                var newStockBalances = warehouses.Select(w => new StockBalance
                {
                    SequenceNumber = 0,
                    ProductId = productId,
                    ProductCode = productCode,
                    ProductName = productName,
                    WarehouseId = w.Id,
                    WarehouseCode = w.Code,
                    WarehouseName = w.Name
                }).ToList();

                foreach (var stockBalance in newStockBalances)
                    await _stockBalanceRepository.InsertAsync(stockBalance);
            }
            await CurrentUnitOfWork.SaveChangesAsync();
        }
    }
}
