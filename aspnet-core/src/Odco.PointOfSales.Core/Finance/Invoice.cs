using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Odco.PointOfSales.Core.Finance
{
    // Invoice : InvoiceProduct = 1 : M
    // Invoice : Payment        = 1 : M
    [Table("Finance.Invoice")]
    public class Invoice : FullAuditedEntity<Guid>
    {
        public Invoice()
        {
            InvoiceProducts = new HashSet<InvoiceProduct>();
            Payments = new HashSet<Payment>();
        }

        public int? TempSaleId { get; set; }

        [Required]
        [StringLength(15)]
        public string InvoiceNumber { get; set; }

        [StringLength(10)]
        public string ReferenceNumber { get; set; }

        public Guid? CustomerId { get; set; }

        [StringLength(10)]
        public string CustomerCode { get; set; }

        [StringLength(100)]
        public string CustomerName { get; set; }

        public decimal DiscountRate { get; set; }

        public decimal DiscountAmount { get; set; }

        public decimal TaxRate { get; set; }

        public decimal TaxAmount { get; set; }

        public decimal GrossAmount { get; set; }

        public decimal NetAmount { get; set; }

        [StringLength(100)]
        public string Remarks { get; set; }

        public ICollection<InvoiceProduct> InvoiceProducts { get; set; }
        
        public ICollection<Payment> Payments { get; set; }
    }
}
