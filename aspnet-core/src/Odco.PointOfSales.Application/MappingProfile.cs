using AutoMapper;
using Odco.PointOfSales.Application.Inventory.GoodsReceiveNotes;
using Odco.PointOfSales.Application.Productions.Brands;
using Odco.PointOfSales.Application.Productions.Categories;
using Odco.PointOfSales.Application.Purchasings.Suppliers;
using Odco.PointOfSales.Application.Sales.TemporarySales;
using Odco.PointOfSales.Application.Sales.TemporarySalesProducts;
using Odco.PointOfSales.Core.Common;
using Odco.PointOfSales.Core.Inventory;
using Odco.PointOfSales.Core.Productions;
using Odco.PointOfSales.Core.Sales;
using Odco.PointOfSales.Sales.Common;

namespace Odco.PointOfSales.Application
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CategoryDto, Category>();
            CreateMap<Category, CategoryDto>();

            CreateMap<BrandDto, Brand>();
            CreateMap<Brand, BrandDto>();

            CreateMap<SupplierDto, Supplier>();
            CreateMap<Supplier, SupplierDto>();

            CreateMap<GoodsReceivedDto, GoodsReceived>();
            CreateMap<GoodsReceived, GoodsReceivedDto>();

            CreateMap<TempSaleDto, TempSale>();
            CreateMap<TempSale, TempSaleDto>();

            CreateMap<TempSalesProductDto, TempSalesProduct>();
            CreateMap<TempSalesProduct, TempSalesProductDto>();
        }
    }
}
