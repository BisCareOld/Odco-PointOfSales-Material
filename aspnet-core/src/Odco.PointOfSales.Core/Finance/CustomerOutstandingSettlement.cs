using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Odco.PointOfSales.Core.Finance
{
    /// <summary>
    /// CustomerOutstanding : CustomerOutstandingSettlements = 1 : M
    /// This will track the payment when settling the customer outstanding against the sale
    /// </summary>
    [Table("Finance.CustomerOutstandingSettlements")]
    public class CustomerOutstandingSettlement : FullAuditedEntity<Guid>
    {
        [Required]
        public Guid CustomerOutstandingId { get; set; }

        [Required]
        public Guid CustomerId { get; set; }

        [Required]
        [StringLength(10)]
        public string Code { get; set; }

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
