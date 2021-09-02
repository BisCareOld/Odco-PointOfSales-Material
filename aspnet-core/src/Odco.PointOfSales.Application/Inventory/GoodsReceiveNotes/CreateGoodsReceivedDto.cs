using Abp.AutoMapper;
using Odco.PointOfSales.Application.Inventory.GoodsReceivedProducts;
using Odco.PointOfSales.Core.Inventory;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Transactions;

namespace Odco.PointOfSales.Application.Inventory.GoodsReceiveNotes
{
    [AutoMapTo(typeof(GoodsReceived))]
    public class CreateGoodsReceivedDto
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

        public ICollection<CreateGoodsReceivedProductDto> GoodsReceivedProducts { get; set; }

        public CreateGoodsReceivedDto()
        {
            GoodsReceivedProducts = new HashSet<CreateGoodsReceivedProductDto>();
        }
    }
}
