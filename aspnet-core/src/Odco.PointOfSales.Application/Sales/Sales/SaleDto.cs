using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Odco.PointOfSales.Application.Inventory.NonInventorySalesProducts;
using Odco.PointOfSales.Application.Sales.InventorySalesProducts;
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

        #region ABP
        public DateTime CreationTime { get; set; }
        #endregion

        public ICollection<InventorySalesProductDto> InventorySalesProducts { get; set; }
        
        public ICollection<NonInventorySalesProductDto> NonInventorySalesProducts { get; set; }

        public SaleDto()
        {
            InventorySalesProducts = new HashSet<InventorySalesProductDto>();

            NonInventorySalesProducts = new HashSet<NonInventorySalesProductDto>();

        }
    }
}
