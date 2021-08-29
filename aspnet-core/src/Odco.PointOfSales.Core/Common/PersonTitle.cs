using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Odco.PointOfSales.Core.Common
{
    [Table("Common.PersonTitle")]
    public class PersonTitle : FullAuditedEntity<Guid>
    {
        [Required]
        [StringLength(15)]
        public string Title { get; set; }

        public bool IsActive { get; set; }
    }
}
