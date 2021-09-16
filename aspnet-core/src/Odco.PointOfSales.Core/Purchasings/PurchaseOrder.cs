using Abp.Domain.Entities.Auditing;
using Odco.PointOfSales.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Odco.PointOfSales.Core.Common
{
    [Table("Purchasing.PurchaseOrder")]
    public class PurchaseOrder : FullAuditedEntity<Guid>
    {
        /// <summary>
        /// Purchase Order Number
        /// Should be Auto Generate
        /// Should be index
        /// </summary>
        [Required]
        [StringLength(15)]
        public string PurchaseOrderNumber { get; set; }

        /// <summary>
        /// Quatation Referance Number
        /// Should be editable
        /// </summary>
        [StringLength(15)]
        public string ReferenceNumber { get; set; }

        /// <summary>
        /// PO delivery Date
        /// </summary>
        public DateTime? ExpectedDeliveryDate { get; set; }

        /// <summary>
        /// FK Supplier 
        /// PurchaseOrder : Supplier =  1: M
        /// </summary>
        [Required]
        public Guid SupplierId { get; set; }

        [Required]
        [StringLength(10)]
        public string SupplierCode { get; set; }

        [Required]
        [StringLength(100)]
        public string SupplierName { get; set; }

        /// <summary>
        /// Percentage (0 - 100)
        /// </summary>
        public decimal TaxRate { get; set; }

        public decimal TaxAmount { get; set; }

        /// <summary>
        /// Percentage = Overall Total (0 - 100)%
        /// </summary>
        public decimal DiscountRate { get; set; }

        /// <summary>
        /// Discount amount for bill => Overall Total
        /// </summary>
        public decimal DiscountAmount { get; set; }

        [Required]
        public decimal GrossAmount { get; set; }

        [Required]
        public decimal NetAmount { get; set; }

        /// <summary>
        /// Create => Open, Update will be happen in GRN
        /// </summary>
        public PurchaseOrderStatus Status { get; set; }

        [StringLength(100)]
        public string Remarks { get; set; }

        public virtual ICollection<PurchaseOrderProduct> PurchaseOrderProducts { get; set; }

        public PurchaseOrder()
        {
            PurchaseOrderProducts = new HashSet<PurchaseOrderProduct>();
        }
    }
}
