using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Odco.PointOfSales.Core.Common;
using Odco.PointOfSales.Core.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace Odco.PointOfSales.Application.Purchasings.PurchaseOrders
{
    [AutoMapTo(typeof(PurchaseOrderDto)), AutoMapFrom(typeof(PurchaseOrder))]
    public class PurchaseOrderDto : EntityDto<Guid>
    {
        [Required]
        [StringLength(15)]
        public string PurchaseOrderNumber { get; set; }

        [StringLength(15)]
        public string ReferenceNumber { get; set; }

        public DateTime? ExpectedDeliveryDate { get; set; }

        [Required]
        public Guid SupplierId { get; set; }

        [Required]
        [StringLength(10)]
        public string SupplierCode { get; set; }

        [Required]
        [StringLength(100)]
        public string SupplierName { get; set; }

        public decimal TaxRate { get; set; }

        public decimal TaxAmount { get; set; }

        public decimal DiscountRate { get; set; }

        public decimal DiscountAmount { get; set; }

        public decimal GrossAmount { get; set; }
        
        public decimal NetAmount { get; set; }

        public PurchaseOrderStatus Status { get; set; }

        [StringLength(100)]
        public string Remarks { get; set; }
    }
}
