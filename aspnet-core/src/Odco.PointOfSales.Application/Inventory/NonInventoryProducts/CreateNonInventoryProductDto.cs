using System;
using System.ComponentModel.DataAnnotations;

namespace Odco.PointOfSales.Application.Inventory.NonInventoryProducts
{
    public class CreateNonInventoryProductDto
    {
        public Guid? Id { get; set; }

        public int SequenceNumber { get; set; }

        public int? TempSaleId { get; set; }

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

        /// <summary>
        ///     Warehouse Id *FK
        ///     1 : M
        ///     from which warehouse should receive the goods
        /// </summary>
        public Guid? WarehouseId { get; set; }

        [StringLength(10)]
        public string WarehouseCode { get; set; }

        [StringLength(100)]
        public string WarehouseName { get; set; }

        public decimal Quantity { get; set; }

        [StringLength(10)]
        public string QuantityUnitOfMeasureUnit { get; set; }

        #region Sales
        public decimal DiscountRate { get; set; }

        public decimal DiscountAmount { get; set; }

        public decimal LineTotal { get; set; }
        #endregion

        public decimal CostPrice { get; set; }

        public decimal SellingPrice { get; set; }

        public decimal MaximumRetailPrice { get; set; }
    }
}
