using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Odco.PointOfSales.Application.Sales.TemporarySalesProducts;
using Odco.PointOfSales.Core.Sales;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Odco.PointOfSales.Application.Sales.TemporarySales
{
    [AutoMapTo(typeof(TempSaleDto)), AutoMapFrom(typeof(TempSale))]
    public class TempSaleDto : EntityDto<int>
    {
        public Guid? CustomerId { get; set; }

        [StringLength(10)]
        public string CustomerCode { get; set; }

        [StringLength(100)]
        public string CustomerName { get; set; }

        public decimal DiscountRate { get; set; }

        public decimal DiscountAmount { get; set; }

        public decimal TaxRate { get; set; }

        public decimal TaxAmount { get; set; }

        public decimal GrossAmount { get; set; }

        public decimal NetAmount { get; set; }

        [StringLength(100)]
        public string Remarks { get; set; }

        public bool IsActive { get; set; }

        public ICollection<TempSalesProductDto> TempSalesProducts { get; set; }

        public TempSaleDto()
        {
            TempSalesProducts = new HashSet<TempSalesProductDto>();
        }
    }
}
