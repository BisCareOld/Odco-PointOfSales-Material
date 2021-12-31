using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Odco.PointOfSales.Core.Sales
{
    /// <summary>
    /// Sale : SBSP = 1 : M
    /// SalesProduct : SBSP = 1 : M
    /// StockBalance : SBSP = 1 : M
    /// SBSP react as an intermediate class for SalesProduct & StockBalance
    /// </summary>
    [Table("Sales.StockBalancesOfSalesProduct")]
    public class StockBalancesOfSalesProduct : FullAuditedEntity<long>
    {
        public Guid SaleId { get; set; }

        public Guid SalesProductId { get; set; }

        #region StockBalance
        public Guid StockBalanceId { get; set; }

        public int SequenceNumber { get; set; }

        public Guid ProductId { get; set; }

        [Required]
        [StringLength(10)]
        public string ProductCode { get; set; }

        [Required]
        [StringLength(100)]
        public string ProductName { get; set; }

        public Guid? WarehouseId { get; set; }

        [StringLength(10)]
        public string WarehouseCode { get; set; }

        [StringLength(100)]
        public string WarehouseName { get; set; }

        public decimal CostPrice { get; set; }

        public decimal SellingPrice { get; set; }

        public decimal MaximumRetailPrice { get; set; }

        #endregion

        public decimal QuantityTaken { get; set; }

        /// <summary>
        /// The price we have offered for Customer
        /// SalesProduct => Price
        /// </summary>
        public decimal Price { get; set; }
    }
}


















