using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Odco.PointOfSales.Core.Inventory;
using System;
using System.ComponentModel.DataAnnotations;

namespace Odco.PointOfSales.Application.Inventory.GoodsReceivedProducts
{
    [AutoMapTo(typeof(GoodsReceivedProductDto)), AutoMapFrom(typeof(GoodsReceivedProduct))]
    public class GoodsReceivedProductDto : EntityDto<Guid>
    {
        public Guid GoodsRecievedId { get; set; }

        /// <summary>
        ///     GRN sequence Number
        /// </summary>
        [Required]
        [StringLength(15)]
        public string GoodsRecievedNumber { get; set; }

        /// <summary>
        ///     1, 2, 3, etc...
        /// </summary>
        [Required]
        public int SequenceNumber { get; set; }

        public Guid ProductId { get; set; }

        [Required]
        [StringLength(10)]
        public string ProductCode { get; set; }

        [Required]
        [StringLength(100)]
        public string ProductName { get; set; }

        public Guid WarehouseId { get; set; }

        [Required]
        [StringLength(10)]
        public string WarehouseCode { get; set; }

        [Required]
        [StringLength(100)]
        public string WarehouseName { get; set; }

        public DateTime? ExpiryDate { get; set; }

        [StringLength(15)]
        public string BatchNumber { get; set; }

        public decimal Quantity { get; set; }

        public decimal FreeQuantity { get; set; }

        public decimal CostPrice { get; set; }

        public decimal SellingPrice { get; set; }

        public decimal MaximumRetailPrice { get; set; }

        public decimal DiscountRate { get; set; }

        public decimal DiscountAmount { get; set; }

        public decimal LineTotal { get; set; }
    }
}
