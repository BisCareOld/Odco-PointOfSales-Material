using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Odco.PointOfSales.Core.Finance;
using System;
using System.ComponentModel.DataAnnotations;

namespace Odco.PointOfSales.Application.Finance.CustomerOutstandingSettlements
{
    [AutoMapTo(typeof(CustomerOutstandingSettlementDto)), AutoMapFrom(typeof(CustomerOutstandingSettlement))]
    public class CustomerOutstandingSettlementDto : EntityDto<Guid>
    {
        [Required]
        public Guid CustomerOutstandingId { get; set; }

        [Required]
        public Guid CustomerId { get; set; }

        [Required]
        [StringLength(10)]
        public string CustomerCode { get; set; }

        [Required]
        [StringLength(200)]
        public string CustomerName { get; set; }

        [Required]
        public Guid SaleId { get; set; }

        [Required]
        [StringLength(15)]
        public string SalesNumber { get; set; }

        [Required]
        public Guid PaymentId { get; set; }

        [Required]
        [StringLength(15)]
        public string InvoiceNumber { get; set; }

        /// <summary>
        /// How much paid for particular outstanding / settlement amount
        /// </summary>
        public decimal PaidAmount { get; set; }
    }
}
