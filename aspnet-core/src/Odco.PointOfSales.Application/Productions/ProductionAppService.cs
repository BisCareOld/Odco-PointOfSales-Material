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
        private readonly IRepository<NonInventoryProduct, Guid> _nonInventoryProductRepository;

        public ProductionAppService(IRepository<Product, Guid> productRepository,
            IRepository<Category, Guid> categoryRepository,
            IRepository<Brand, Guid> brandRepository,
            IRepository<StockBalance, Guid> stockBalanceRepository,
            IRepository<Warehouse, Guid> warehouseRepository,
            IRepository<NonInventoryProduct, Guid> nonInventoryProductRepository)
        {
            _asyncQueryableExecuter = NullAsyncQueryableExecuter.Instance;
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _brandRepository = brandRepository;
            _stockBalanceRepository = stockBalanceRepository;
            _warehouseRepository = warehouseRepository;
            _nonInventoryProductRepository = nonInventoryProductRepository;
        }

        #region Product
        public async Task<ProductDto> CreateProductAsync(CreateProductDto input)
        {
            try
            {
                var product = ObjectMapper.Map<Product>(input);
                var created = await _productRepository.InsertAsync(product);
                await CreateOrUpdateStockBalance(created.Id, created.Code, created.Name);
                await CreateOrUpdateNonInventoryProductSummaryAsync(created.Id, created.Code, created.Name);
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
                await CreateOrUpdateNonInventoryProductSummaryAsync(updated.Id, updated.Code, updated.Name);
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
                CreateOrUpdateWarehouseSummariesInNonInventoryProductAndStockBalanceAsync(created.Id, created.Code, created.Name);
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
                CreateOrUpdateWarehouseSummariesInNonInventoryProductAndStockBalanceAsync(updated.Id, updated.Code, updated.Name);
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
            return await _warehouseRepository
                .GetAll()
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
                    WarehouseId = sb.WarehouseId,
                    WarehouseCode = sb.WarehouseCode,
                    WarehouseName = sb.WarehouseName,
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

        public async Task<ProductStockBalanceDto> GetRecentlyCreatedGoodsReceivedNoteAsync(Guid productId)
        {
            return await _stockBalanceRepository
                .GetAll()
                .Where(sb => sb.SequenceNumber > 0 && sb.ProductId == productId)
                .OrderByDescending(sb => sb.SequenceNumber)
                .Select(sb => new ProductStockBalanceDto
                {
                    StockBalanceId = sb.Id,
                    ProductId = sb.ProductId,
                    WarehouseId = sb.WarehouseId,
                    WarehouseCode = sb.WarehouseCode,
                    WarehouseName = sb.WarehouseName,
                    ExpiryDate = sb.ExpiryDate,
                    BatchNumber = sb.BatchNumber,
                    AllocatedQuantity = sb.AllocatedQuantity,
                    AllocatedQuantityUnitOfMeasureUnit = sb.AllocatedQuantityUnitOfMeasureUnit,
                    BookBalanceQuantity = sb.BookBalanceQuantity,
                    BookBalanceUnitOfMeasureUnit = sb.BookBalanceUnitOfMeasureUnit,
                    CostPrice = sb.CostPrice,
                    SellingPrice = sb.SellingPrice,
                    MaximumRetailPrice = sb.MaximumRetailPrice,
                    IsSelected = true,
                })
                .FirstOrDefaultAsync();
        }

        private async Task CreateOrUpdateStockBalance(Guid productId, string productCode, string productName)
        {
            var existStockBalances = _stockBalanceRepository.GetAll().Where(sb => sb.ProductId == productId);

            var warehouses = await GetAllKeyValuePairWarehousesAsync();

            #region Creating or Updating Company Summary
            if (!existStockBalances.Any(n => !n.WarehouseId.HasValue))
            {
                // Create
                await _stockBalanceRepository.InsertAsync(new StockBalance
                {
                    Id = Guid.NewGuid(),
                    SequenceNumber = 0,
                    ProductId = productId,
                    ProductCode = productCode,
                    ProductName = productName,
                    WarehouseId = null,
                    WarehouseCode = null,
                    WarehouseName = null
                });
            }
            else
            {
                // Update
                var update = existStockBalances.FirstOrDefault(n => !n.WarehouseId.HasValue);
                update.ProductCode = productCode;
                update.ProductName = productName;
                await _stockBalanceRepository.UpdateAsync(update);
            }
            #endregion

            #region Creating or Updating Warehouse Summaries
            foreach (var w in warehouses)
            {
                // Creating or Updating Warehouse Summaries
                if (!existStockBalances.Any(n => n.WarehouseId.HasValue && n.WarehouseId == w.Id))
                {
                    // Create
                    await _stockBalanceRepository.InsertAsync(new StockBalance
                    {
                        Id = Guid.NewGuid(),
                        SequenceNumber = 0,
                        ProductId = productId,
                        ProductCode = productCode,
                        ProductName = productName,
                        WarehouseId = w.Id,
                        WarehouseCode = w.Code,
                        WarehouseName = w.Name,
                    });
                }
                else
                {
                    // Update
                    var update = existStockBalances.FirstOrDefault(n => n.WarehouseId.HasValue && n.WarehouseId == w.Id);
                    update.ProductCode = productCode;
                    update.ProductName = productName;
                    await _stockBalanceRepository.UpdateAsync(update);
                }
            }
            #endregion
        }

        #endregion

        #region NonInventoryProduct
        private async Task CreateOrUpdateNonInventoryProductSummaryAsync(Guid productId, string productCode, string productName)
        {
            var existNonInventoryProducts = await _nonInventoryProductRepository.GetAll().Where(n => n.SequenceNumber == 0 && n.ProductId == productId).ToListAsync();

            var warehouses = await GetAllKeyValuePairWarehousesAsync();

            #region Company Summary
            // Creating or Updating Company Summaries
            if (!existNonInventoryProducts.Any(n => !n.WarehouseId.HasValue))
            {
                // Create
                await _nonInventoryProductRepository.InsertAsync(new NonInventoryProduct
                {
                    Id = Guid.NewGuid(),
                    SequenceNumber = 0,
                    TempSaleId = 0,
                    ProductId = productId,
                    ProductCode = productCode,
                    ProductName = productName,
                    WarehouseId = null,
                    WarehouseCode = null,
                    WarehouseName = null,
                    Quantity = 0,
                    QuantityUnitOfMeasureUnit = null,
                    DiscountRate = 0,
                    DiscountAmount = 0,
                    LineTotal = 0,
                    CostPrice = 0,
                    SellingPrice = 0,
                    MaximumRetailPrice = 0,
                });
            }
            else
            {
                // Update
                var update = existNonInventoryProducts.FirstOrDefault(n => !n.WarehouseId.HasValue);
                update.ProductName = productName;
                update.ProductCode = productCode;
                await _nonInventoryProductRepository.UpdateAsync(update);
            }
            #endregion

            #region Warehouse Summaries
            foreach (var w in warehouses)
            {
                // Creating or Updating Warehouse Summaries
                if (!existNonInventoryProducts.Any(n => n.WarehouseId.HasValue && n.WarehouseId == w.Id))
                {
                    // Create
                    await _nonInventoryProductRepository.InsertAsync(new NonInventoryProduct
                    {
                        Id = Guid.NewGuid(),
                        SequenceNumber = 0,
                        TempSaleId = 0,
                        ProductId = productId,
                        ProductCode = productCode,
                        ProductName = productName,
                        WarehouseId = w.Id,
                        WarehouseCode = w.Code,
                        WarehouseName = w.Name,
                        Quantity = 0,
                        QuantityUnitOfMeasureUnit = null,
                        DiscountRate = 0,
                        DiscountAmount = 0,
                        LineTotal = 0,
                        CostPrice = 0,
                        SellingPrice = 0,
                        MaximumRetailPrice = 0,
                    });
                }
                else
                {
                    // Update
                    var update = existNonInventoryProducts.FirstOrDefault(n => n.WarehouseId.HasValue && n.WarehouseId == w.Id);
                    update.ProductName = productName;
                    update.ProductCode = productCode;
                    await _nonInventoryProductRepository.UpdateAsync(update);
                }
            }
            #endregion

        }
        #endregion

        private async Task CreateOrUpdateWarehouseSummariesInNonInventoryProductAndStockBalanceAsync(Guid warehouseId, string warehouseCode, string warehouseName)
        {
            var products = await _productRepository.GetAll().ToListAsync();

            var existNonInventoryProducts = await _nonInventoryProductRepository.GetAll().Where(n => n.SequenceNumber == 0 && n.WarehouseId.HasValue).ToListAsync();

            var existStockBalances = await _stockBalanceRepository.GetAll().Where(n => n.SequenceNumber == 0 && n.WarehouseId.HasValue).ToListAsync();

            foreach (var p in products)
            {
                #region Creating or Updating NonInventoryProduct
                if (!existNonInventoryProducts.Any(n => n.ProductId == p.Id && n.WarehouseId == warehouseId))
                {
                    // Create
                    _nonInventoryProductRepository.InsertAsync(new NonInventoryProduct
                    {
                        Id = Guid.NewGuid(),
                        SequenceNumber = 0,
                        TempSaleId = 0,
                        ProductId = p.Id,
                        ProductCode = p.Code,
                        ProductName = p.Name,
                        WarehouseId = warehouseId,
                        WarehouseCode = warehouseCode,
                        WarehouseName = warehouseName,
                        Quantity = 0,
                        QuantityUnitOfMeasureUnit = null,
                        DiscountRate = 0,
                        DiscountAmount = 0,
                        LineTotal = 0,
                        CostPrice = 0,
                        SellingPrice = 0,
                        MaximumRetailPrice = 0,
                    });

                }
                else
                {
                    // Update
                    var update = existNonInventoryProducts.FirstOrDefault(n => n.ProductId == p.Id && n.WarehouseId == warehouseId);
                    update.WarehouseCode = warehouseCode;
                    update.WarehouseName = warehouseName;
                    _nonInventoryProductRepository.UpdateAsync(update);
                }
                #endregion

                #region Creating or Updating StockBalance
                if (!existStockBalances.Any(n => n.ProductId == p.Id && n.WarehouseId == warehouseId))
                {
                    // Create
                    _stockBalanceRepository.InsertAsync(new StockBalance
                    {
                        Id = Guid.NewGuid(),
                        SequenceNumber = 0,
                        ProductId = p.Id,
                        ProductCode = p.Code,
                        ProductName = p.Name,
                        WarehouseId = warehouseId,
                        WarehouseCode = warehouseCode,
                        WarehouseName = warehouseName,
                    });

                }
                else
                {
                    // Update
                    var update = existStockBalances.FirstOrDefault(n => n.ProductId == p.Id && n.WarehouseId == warehouseId);
                    update.WarehouseCode = warehouseCode;
                    update.WarehouseName = warehouseName;
                    _stockBalanceRepository.UpdateAsync(update);
                }
                #endregion
            }
        }
    }
}
