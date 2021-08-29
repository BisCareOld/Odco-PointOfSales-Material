using Abp.Domain.Entities.Auditing;
using Odco.PointOfSales.Core.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Odco.PointOfSales.Core.Common
{
    [Table("Purchasing.PurchaseOrderProduct")]
    public class PurchaseOrderProduct : FullAuditedEntity<Guid>
    {
        public Guid PurchaseOrderId { get; set; }

        public PurchaseOrder PurchaseOrder { get; set; }

        /// <summary>
        /// Comming from PurchaseOrder Code 
        /// </summary>
        [Required]
        [StringLength(15)]
        public string PurchaseOrderNumber { get; set; }

        /// <summary>
        /// Line number of products starts from 1 - ......
        /// </summary>
        public int SequenceNo { get; set; }

        /// <summary>
        /// Product *FK
        /// 1 : M
        /// </summary>
        public Guid ProductId { get; set; }

        /// <summary>
        /// 10 Digits Auto Generated
        /// </summary>
        [Required]
        [StringLength(10)]
        public string ProductCode { get; set; }

        /// <summary>
        /// Getting the product descriptions
        /// </summary>
        [StringLength(100)]
        public string ProductName { get; set; }

        /// <summary>
        /// Entering price for single product
        /// </summary>
        public decimal UnitPrice { get; set; }

        public decimal OrderQuantity { get; set; }

        public decimal DiscountRate { get; set; }

        public decimal DiscountAmount { get; set; }

        public PurchaseOrderProductStatus Status { get; set; }

        /// <summary>
        /// Price * Quantity - Discount
        /// </summary>
        public decimal LineTotal { get; set; }

        [StringLength(50)]
        public string Remarks { get; set; }

        public Guid? CanceledEmployeeId { get; set; }

        public string CanceledEmployeeCode { get; set; }

        public string CanceledEmployeeName { get; set; }
    }
}
