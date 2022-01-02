using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Odco.PointOfSales.Core.Inventory
{
    /// <summary>
    /// 
    /// OPERATIONS
    ///      SequenceNo  |   Warehouse
    ///          0              NULL        =>      Company Summary
    ///          0              Exist       =>      Warehouse Summart
    ///          1 =<           Exist       =>      NonInventory / Sales
    ///          
    /// CRUD
    ///     CREATE  =   Sales -> Payment
    ///     UPDATE  =   Payment -> Sales    (Alter Qty)
    ///     DELETE  =   Payment ->  Sales   (Remove Product)
    /// 
    /// RELATIONSHIPS
    ///     NonInventoryProduct : TempSales = 1 : 1
    ///     
    /// </summary>
    [Table("Inventory.NonInventoryProduct")]
    public class NonInventoryProduct : FullAuditedEntity<Guid>
    {
        /// <summary>
        /// Company Summary: 0
        /// Warehouse Summaries: 0
        /// Sales: 1
        /// </summary>
        public int SequenceNumber { get; set; }

        public Guid? SaleId { get; set; }

        [StringLength(15)]
        public string SalesNumber { get; set; }

        [StringLength(15)]
        public string InvoiceNumber { get; set; }

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

        #region Calculation By Sales
        public decimal DiscountRate { get; set; }

        public decimal DiscountAmount { get; set; }

        public decimal LineTotal { get; set; }

        #endregion

        public decimal CostPrice { get; set; }

        public decimal SellingPrice { get; set; }

        public decimal MaximumRetailPrice { get; set; }

        /// <summary>
        /// Actual Price given for the Customer
        /// </summary>
        public decimal Price { get; set; }
    }
}

