using Abp.Domain.Entities.Auditing;
using Odco.PointOfSales.Core.Common;
using Odco.PointOfSales.Core.Productions;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Odco.PointOfSales.Core.IntermediateEntities
{
    [Table("Sales.SupplierProduct")]
    public class SupplierProduct : FullAuditedEntity<Guid>
    {
        public Guid ProductId { get; set; }

        public Product Product { get; set; }

        public Guid SupplierId { get; set; }

        public Supplier Supplier { get; set; }
    }
}
