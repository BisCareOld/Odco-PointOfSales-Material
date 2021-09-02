using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Odco.PointOfSales.Core.Inventory;
using System;
using System.ComponentModel.DataAnnotations;
using System.Transactions;

namespace Odco.PointOfSales.Application.Inventory.GoodsReceiveNotes
{
    [AutoMapTo(typeof(GoodsReceivedDto)), AutoMapFrom(typeof(GoodsReceived))]
    public class GoodsReceivedDto : EntityDto<Guid>
    {
        [Required]
        [StringLength(15)]
        public string GoodsReceivedNumber { get; set; }

        [StringLength(15)]
        public string ReferenceNumber { get; set; }

        public Guid SupplierId { get; set; }

        [StringLength(10)]
        public string SupplierCode { get; set; }

        [StringLength(100)]
        public string SupplierName { get; set; }

        public decimal DiscountRate { get; set; }

        public decimal DiscountAmount { get; set; }

        public decimal TaxRate { get; set; }

        public decimal TaxAmount { get; set; }

        public decimal GrossAmount { get; set; }

        public decimal NetAmount { get; set; }

        [Required]
        public TransactionStatus TransactionStatus { get; set; }

        [StringLength(100)]
        public string Remarks { get; set; }
    }
}
