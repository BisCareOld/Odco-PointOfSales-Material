using Abp.Domain.Entities.Auditing;
using Odco.PointOfSales.Core.Enums;
using Odco.PointOfSales.Core.IntermediateEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Odco.PointOfSales.Sales.Common
{
    [Table("Sales.PriceGroup")]
    public class PriceGroup : FullAuditedEntity<Guid>
    {
        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        /// <summary>
        /// Rename Enum => ProductPriceGroup to PriceGroupCategory
        /// The values should be selling and cost
        /// </summary>
        [Required]
        public PriceGroupCategory PriceGroupCategory { get; set; }

        public virtual ICollection<ProductPriceGroup> ProductPriceGroups { get; set; }

        public PriceGroup()
        {
            ProductPriceGroups = new HashSet<ProductPriceGroup>();
        }
    }
}
