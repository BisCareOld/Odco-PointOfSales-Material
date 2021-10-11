using System;
using System.ComponentModel.DataAnnotations;

namespace Odco.PointOfSales.Application.Inventory.StockBalances
{
    public class ProductStockBalanceDto
    {
        public Guid StockBalanceId { get; set; }

        public Guid ProductId { get; set; }

        public DateTime? ExpiryDate { get; set; }

        [StringLength(15)]
        public string BatchNumber { get; set; }

        public decimal BookBalanceQuantity { get; set; }

        [StringLength(10)]
        public string BookBalanceUnitOfMeasureUnit { get; set; }

        public decimal CostPrice { get; set; }

        public decimal SellingPrice { get; set; }

        public decimal MaximumRetailPrice { get; set; }

        public bool IsSelected { get; set; }
    }
}

