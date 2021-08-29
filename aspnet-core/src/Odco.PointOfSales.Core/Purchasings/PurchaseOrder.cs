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
        [Range(0, 999.99)]
        public decimal TaxRate { get; set; }

        [Range(0, 99999999.99)]
        public decimal TaxAmount { get; set; }

        /// <summary>
        /// Percentage = Overall Total (0 - 100)%
        /// </summary>
        [Range(0, 999.99)]
        public decimal DiscountRate { get; set; }

        /// <summary>
        /// Discount amount for bill => Overall Total
        /// </summary>
        [Range(0, 99999999.99)]
        public decimal DiscountAmount { get; set; }

        [Required]
        [Range(0, 99999999.99)]
        public decimal TotalAmount { get; set; }

        [Required]
        [Range(0, 99999999.99)]
        public decimal NetAmount { get; set; }

        /// <summary>
        /// Create => Open, Update will be happen in GRN
        /// </summary>
        [Required]
        public PurchaseOrderStatus Status { get; set; }

        /// <summary>
        /// Supplier Address
        /// </summary>
        public Guid BillingAddressId { get; set; }

        [Required]
        [StringLength(200)]
        public string BillingAddressLine1 { get; set; }

        [Required]
        [StringLength(200)]
        public string BillingAddressLine2 { get; set; }

        [Required]
        [StringLength(75)]
        public string BillingAddressCity { get; set; }

        [Required]
        [StringLength(7)]
        public string BillingAddressPostalCode { get; set; }

        /// <summary>
        /// Quatation Referance Number
        /// Should be editable
        /// </summary>
        [StringLength(15)]
        public string ReferenceNumber { get; set; }

        [StringLength(100)]
        public string Remarks { get; set; }

        /// <summary>
        /// Not Required For th moment
        /// </summary>
        public Guid? OnbehalfOfEmployeeId { get; set; }

        /// <summary>
        /// Not Required For th moment
        /// </summary>
        [StringLength(10)]
        public string OnbehalfOfEmployeeCode { get; set; }

        /// <summary>
        /// Not Required For th moment
        /// </summary>
        [StringLength(100)]
        public string OnbehalfOfEmployeeName { get; set; }

        /// <summary>
        /// The employee who has approved this PO
        /// </summary>
        public Guid? ApprovalEmployeeId { get; set; }

        /// <summary>
        /// Not Required For th moment
        /// </summary>
        [StringLength(10)]
        public string ApprovalApprovalEmployeeCode { get; set; }

        /// <summary>
        /// Not Required For th moment
        /// </summary>
        [StringLength(100)]
        public string ApprovalApprovalEmployeeName { get; set; }

        public virtual ICollection<PurchaseOrderProduct> PurchaseOrderProducts { get; set; }

        public PurchaseOrder()
        {
            PurchaseOrderProducts = new HashSet<PurchaseOrderProduct>();
        }
    }
}
