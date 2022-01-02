using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Odco.PointOfSales.Application.Inventory.NonInventoryProducts;
using Odco.PointOfSales.Application.Sales.SalesProducts;
using Odco.PointOfSales.Core.Enums;
using Odco.PointOfSales.Core.Sales;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Odco.PointOfSales.Application.Sales.Sales
{
    [AutoMapTo(typeof(SaleDto)), AutoMapFrom(typeof(Sale))]
    public class SaleDto : EntityDto<Guid>
    {
        [StringLength(15)]
        public string SalesNumber { get; set; }

        [StringLength(15)]
        public string InvoiceNumber { get; set; }

        [StringLength(15)]
        public string ReferenceNumber { get; set; }

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

        public PaymentStatus PaymentStatus { get; set; }

        public bool IsActive { get; set; }

        public ICollection<SalesProductDto> SalesProducts { get; set; }
        
        public ICollection<NonInventoryProductDto> NonInventoryProducts { get; set; }

        public SaleDto()
        {
            SalesProducts = new HashSet<SalesProductDto>();

            NonInventoryProducts = new HashSet<NonInventoryProductDto>();

        }
    }
}
