using Abp.Domain.Entities.Auditing;
using Odco.PointOfSales.Core.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Odco.PointOfSales.Core.Common
{
    [Table("Common.Classification")]
    public class Classification : FullAuditedEntity<Guid>
    {
        [Required]
        [StringLength(25)]
        public string Name { get; set; }

        [Required]
        public ClassificationType ClassificationType { get; set; }

        public bool IsActive { get; set; }
    }
}
