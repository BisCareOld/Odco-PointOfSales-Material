using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Odco.PointOfSales.Core.Finance
{
    /// <summary>
    /// This class act/like a summary of customer outstandings
    /// Sale : CustomerOutstanding = 1 : 1/0
    /// Payment : CustomerOutstanding = 1 : 1/0
    /// CustomerOutstanding : CustomerOutstandingSettlements = 1 : M
    /// CREATE: when Payment is done in Payment screen
    /// </summary>
    [Table("Finance.CustomerOutstanding")]
    public class CustomerOutstanding : FullAuditedEntity<Guid>
    {
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

        [StringLength(15)]
        public string CreatedInvoiceNumber { get; set; }

        /// <summary>
        /// Once the value is SET, there is no edit FIXED value
        /// </summary>
        public decimal OutstandingAmount { get; set; }

        /// <summary>
        /// Initially the value will be set to "OutstandingAmount" and reduced until 0
        /// </summary>
        public decimal DueOutstandingAmount { get; set; }
    }
}