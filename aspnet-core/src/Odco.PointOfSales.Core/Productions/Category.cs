using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Odco.PointOfSales.Core.Productions
{
    [Table("Production.Category")]
    public class Category : FullAuditedEntity<Guid>
    {
        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        public bool IsActive { get; set; }

        public virtual ICollection<Product> Products { get; set; }

        public Category()
        {
            Products = new HashSet<Product>();
        }
    }
}
