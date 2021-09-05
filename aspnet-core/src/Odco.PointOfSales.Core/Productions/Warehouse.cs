using Abp.Domain.Entities.Auditing;
using Odco.PointOfSales.Core.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Odco.PointOfSales.Core.Productions
{
    [Table("Production.Warehouse")]
    public class Warehouse : FullAuditedEntity<Guid>
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(100)]
        public string Description { get; set; }

        [StringLength(50)]
        public string ContactPersonName { get; set; }

        [StringLength(15)]
        public string ContactNumber { get; set; }

        public bool IsActive { get; set; }

        public ICollection<Address> Addresses { get; set; }

        public Warehouse()
        {
            Addresses = new HashSet<Address>();
        }
    }
}
