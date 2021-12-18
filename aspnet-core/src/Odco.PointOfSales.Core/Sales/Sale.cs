using Abp.Domain.Entities.Auditing;
using Odco.PointOfSales.Core.Enums;
using Odco.PointOfSales.Core.Finance;
using Odco.PointOfSales.Sales.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Odco.PointOfSales.Core.Sales
{
    // Sale : SalesProduct = 1: M
    // Sale : NonInventoryProduct = 1 : M
    // Sale : Payment = 1 : M
    [Table("Sales.Sale")]
    public class Sale : FullAuditedEntity<Guid>
    {
        /// <summary>
        /// Exist: When Payment is carried out
        /// </summary>
        [StringLength(15)]
        public string SalesNumber { get; set; }

        [StringLength(15)]
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

        public PaymentStatus PaymentStatus { get; set; }

        public bool IsActive { get; set; }

        public ICollection<SalesProduct> SalesProducts { get; set; }

        public ICollection<Payment> Payments { get; set; }

        public Sale()
        {
            SalesProducts = new HashSet<SalesProduct>();

            Payments = new HashSet<Payment>();
        }
    }
}
