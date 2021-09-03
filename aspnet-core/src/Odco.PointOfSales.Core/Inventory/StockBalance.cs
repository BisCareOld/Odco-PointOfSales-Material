using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Odco.PointOfSales.Core.Inventory
{
    [Table("Inventory.StockBalance")]
    public class StockBalance : FullAuditedEntity<Guid>
    {
        public int SequenceNumber { get; set; }

        /// <summary>
        ///     Product *FK
        /// </summary>
        public Guid ProductId { get; set; }

        /// <summary>
        ///     10 Digits Auto Generated
        /// </summary>
        [Required]
        [StringLength(10)]
        public string ProductCode { get; set; }

        /// <summary>
        ///     Getting the product descriptions
        /// </summary>
        [Required]
        [StringLength(100)]
        public string ProductName { get; set; }

        public DateTime? ExpiryDate { get; set; }

        [StringLength(15)]
        public string BatchNumber { get; set; }

        public decimal ReceivedQuantity { get; set; }

        [StringLength(10)]
        public string ReceivedQuantityUnitOfMeasureUnit { get; set; }

        /// <summary>
        ///     Based on System quantity
        ///     13, 2
        /// </summary>
        public decimal BookBalanceQuantity { get; set; }

        /// <summary>
        ///     UnitofMeasure => Unit
        /// </summary>
        [StringLength(10)]
        public string BookBalanceUnitOfMeasureUnit { get; set; }

        public decimal OnOrderQuantity { get; set; }

        [StringLength(10)]
        public string OnOrderQuantityUnitOfMeasureUnit { get; set; }

        public decimal AllocatedQuantity { get; set; }

        [StringLength(10)]
        public string AllocatedQuantityUnitOfMeasureUnit { get; set; }

        public decimal CostPrice { get; set; }

        public decimal SellingPrice { get; set; }

        public decimal MaximumRetailPrice { get; set; }

        /// <summary>
        ///     GRN Number
        /// </summary>
        [StringLength(10)]
        public string GoodsRecievedNumber { get; set; }
    }
}





















