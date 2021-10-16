using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Odco.PointOfSales.Sales.Common;
using System;
using System.ComponentModel.DataAnnotations;

namespace Odco.PointOfSales.Application.Sales.TemporarySalesProducts
{
    [AutoMapTo(typeof(TempSalesProductDto)), AutoMapFrom(typeof(TempSalesProduct))]
    public class TempSalesProductDto : EntityDto<int>
    {
        #region TemporarySalesHeader
        public int TempSalesHeaderId { get; set; }
        #endregion

        #region Product
        public Guid ProductId { get; set; }

        [StringLength(15)]
        public string BarCode { get; set; }

        [Required]
        [StringLength(15)]
        public string Code { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        #endregion

        #region StockBalance
        public Guid StockBalanceId { get; set; }

        public DateTime? ExpiryDate { get; set; }

        [StringLength(15)]
        public string BatchNumber { get; set; }

        public Guid? WarehouseId { get; set; }

        [StringLength(10)]
        public string WarehouseCode { get; set; }

        [StringLength(100)]
        public string WarehouseName { get; set; }

        /// <summary>
        /// Store when the time being
        /// </summary>
        public decimal BookBalanceQuantity { get; set; }

        [StringLength(10)]
        public string BookBalanceUnitOfMeasureUnit { get; set; }

        public decimal CostPrice { get; set; }

        public decimal SellingPrice { get; set; }

        public decimal MaximumRetailPrice { get; set; }

        public bool IsSelected { get; set; }
        #endregion

        #region Sales
        public decimal DiscountRate { get; set; }

        public decimal DiscountAmount { get; set; }

        /// <summary>
        /// Needed / Allocated Quantity
        /// </summary>
        public decimal Quantity { get; set; }

        public decimal LineTotal { get; set; }
        #endregion

        public bool IsActive { get; set; }
    }
}
