using Abp.AutoMapper;
using Odco.PointOfSales.Core.Productions;
using System;

namespace Odco.PointOfSales.Application.Productions.Products
{
    [AutoMapTo(typeof(ProductSearchResultDto)), AutoMapFrom(typeof(Product))]
    public class ProductSearchResultDto
    {
        public Guid Id { get; set; }

        public string BarCode { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public bool IsActive { get; set; }
    }
}
