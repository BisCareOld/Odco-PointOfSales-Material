using Abp.Domain.Entities.Auditing;
using Odco.PointOfSales.Sales.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Odco.PointOfSales.Core.Sales
{
    [Table("Sales.TempSale")]
    public class TempSale : FullAuditedEntity<int>
    {
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

        public bool IsActive { get; set; }

        public ICollection<TempSalesProduct> TempSalesProducts { get; set; }

        public TempSale()
        {
            TempSalesProducts = new HashSet<TempSalesProduct>();
        }
    }
}
