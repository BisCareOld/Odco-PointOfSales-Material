using System;
using System.ComponentModel.DataAnnotations;

namespace Odco.PointOfSales.Application.Inventory.StockBalances
{
    /// <summary>
    /// Based on SellingPrice
    /// </summary>
    public class GroupBySellingPriceDto
    {
        /// <summary>
        /// Comma seperated Guids
        /// </summary>
        // public Guid[] StockBalanceIds { get; set; }
        public Guid ProductId { get; set; }

        public Guid? WarehouseId { get; set; }

        [StringLength(10)]
        public string WarehouseCode { get; set; }

        [StringLength(100)]
        public string WarehouseName { get; set; }

        public decimal TotalBookBalanceQuantity { get; set; }

        public decimal SellingPrice { get; set; }

        public bool IsSelected { get; set; }
    }
}
