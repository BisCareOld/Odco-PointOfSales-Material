using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Odco.PointOfSales.Core.Finance;
using System;
using System.ComponentModel.DataAnnotations;

namespace Odco.PointOfSales.Application.Finance.InvoiceProducts
{
    [AutoMapTo(typeof(InvoiceProductDto)), AutoMapFrom(typeof(InvoiceProduct))]
    public class InvoiceProductDto : EntityDto<Guid>
    {
        public Guid InvoiceId { get; set; }

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
