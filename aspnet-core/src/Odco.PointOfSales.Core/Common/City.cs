using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Odco.PointOfSales.Core.Common
{
    [Table("Common.City")]
    public class City : FullAuditedEntity<Guid>
    {
        [Required]
        [StringLength(75)]
        public string Name { get; set; }

        public Guid DistrictId { get; set; }

        public District District { get; set; }

        public bool IsActive { get; set; }
    }
}
