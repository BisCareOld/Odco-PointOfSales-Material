using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Odco.PointOfSales.Core.Common
{
    /// <summary>
    /// Sri Lanka have 22 districs
    /// </summary>
    [Table("Common.District")]
    public class District : FullAuditedEntity<Guid>
    {
        [Required]
        [StringLength(75)]
        public string Name { get; set; }

        public bool IsActive { get; set; }

        public ICollection<City> Cities { get; set; }

        public District()
        {
            Cities = new HashSet<City>();
        }
    }
}
