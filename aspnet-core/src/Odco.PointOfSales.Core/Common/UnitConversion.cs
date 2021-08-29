using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Odco.PointOfSales.Core.Common
{
    [Table("Common.UnitConversion")]
    public class UnitConversion : FullAuditedEntity<Guid>
    {
        public Guid FromUnitOfMeasureId { get; set; }

        // public virtual UnitOfMeasure FromUnitOfMeasure { get; set; }

        public Guid ToUnitOfMeasureId { get; set; }

        // public virtual UnitOfMeasure ToUnitOfMeasure { get; set; }

        [Required]
        public decimal ConvertionFactor { get; set; }
    }
}
