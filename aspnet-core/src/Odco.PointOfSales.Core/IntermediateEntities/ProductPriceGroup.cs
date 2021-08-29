using Abp.Domain.Entities.Auditing;
using Odco.PointOfSales.Core.Productions;
using Odco.PointOfSales.Sales.Common;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Odco.PointOfSales.Core.IntermediateEntities
{
    [Table("Sales.ProductPriceGroup")]
    public class ProductPriceGroup : FullAuditedEntity<Guid>
    {
        public Guid ProductId { get; set; }

        public Product Product { get; set; }

        public Guid PriceGroupId { get; set; }

        public PriceGroup PriceGroup { get; set; }
    }
}
