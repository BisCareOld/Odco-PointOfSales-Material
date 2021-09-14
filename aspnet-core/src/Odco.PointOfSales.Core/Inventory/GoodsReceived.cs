using Abp.Domain.Entities.Auditing;
using Odco.PointOfSales.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Odco.PointOfSales.Core.Inventory
{
    [Table("Inventory.GoodsRecieved")]
    public class GoodsReceived : FullAuditedEntity<Guid>
    {
        /// <summary>
        ///     GRN Number => Sequence Number
        /// </summary>
        [Required]
        [StringLength(15)]
        public string GoodsReceivedNumber { get; set; }

        /// <summary>
        ///     Should be unique but not implemented (Unique, Nullable and Index)
        /// </summary>
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

        public ICollection<GoodsReceivedProduct> GoodsReceivedProducts { get; set; }

        public GoodsReceived()
        {
            GoodsReceivedProducts = new HashSet<GoodsReceivedProduct>();
        }

    }
}