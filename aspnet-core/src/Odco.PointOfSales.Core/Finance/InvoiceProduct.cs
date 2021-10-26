using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Odco.PointOfSales.Core.Finance
{
    [Table("Finance.InvoiceProduct")]
    public class InvoiceProduct : FullAuditedEntity<Guid>
    {
        public Guid InvoiceId { get; set; }

        public Invoice Invoice { get; set; }

        [StringLength(15)]
        public string InvoiceNumber { get; set; }

        #region StockBalance
        public Guid StockBalanceId { get; set; }

        public Guid ProductId { get; set; }

        [Required]
        [StringLength(10)]
        public string ProductCode { get; set; }

        [Required]
        [StringLength(100)]
        public string ProductName { get; set; }

        public Guid WarehouseId { get; set; }

        [StringLength(10)]
        public string WarehouseCode { get; set; }

        [StringLength(100)]
        public string WarehouseName { get; set; }

        public decimal CostPrice { get; set; }

        public decimal SellingPrice { get; set; }

        public decimal MaximumRetailPrice { get; set; }
        #endregion

        public decimal Quantity { get; set; }

        /// <summary>
        /// Actual Price given for the Customer
        /// </summary>
        public decimal Price { get; set; }

        public decimal DiscountRate { get; set; }

        public decimal DiscountAmount { get; set; }

        public decimal LineTotal { get; set; }

        [StringLength(100)]
        public string Remarks { get; set; }
    }
}
