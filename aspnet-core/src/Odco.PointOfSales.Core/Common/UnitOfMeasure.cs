using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Odco.PointOfSales.Core.Common
{
    [Table("Common.UnitOfMeasure")]
    public class UnitOfMeasure : FullAuditedEntity<Guid>
    {
        [Required]
        [StringLength(10)]
        public string Unit { get; set; }

        [Required]
        [StringLength(50)]
        public string Description { get; set; }

        //public ICollection<UnitConversion> UnitConversions { get; set; }

        public UnitOfMeasure()
        {
            //UnitConversions = new HashSet<UnitConversion>();
        }
    }
}
