using System;
using System.ComponentModel.DataAnnotations;

namespace Odco.PointOfSales.Application.Inventory.StockBalances
{
    public class ProductStockBalanceDto
    {
        public Guid StockBalanceId { get; set; }

        public Guid ProductId { get; set; }

        public Guid? WarehouseId { get; set; }

        [StringLength(10)]
        public string WarehouseCode { get; set; }

        [StringLength(100)]
        public string WarehouseName { get; set; }

        public DateTime? ExpiryDate { get; set; }

        [StringLength(15)]
        public string BatchNumber { get; set; }

        public decimal AllocatedQuantity { get; set; }

        [StringLength(10)]
        public string AllocatedQuantityUnitOfMeasureUnit { get; set; }

        public decimal BookBalanceQuantity { get; set; }

        [StringLength(10)]
        public string BookBalanceUnitOfMeasureUnit { get; set; }

        public decimal CostPrice { get; set; }

        public decimal SellingPrice { get; set; }

        public decimal MaximumRetailPrice { get; set; }

        public bool IsSelected { get; set; }
    }
}

