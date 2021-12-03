using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Odco.PointOfSales.Core.Common
{
    [Table("Common.Bank")]
    public class Bank : FullAuditedEntity<Guid>
    {
        public Bank()
        {
            BankBranches = new HashSet<BankBranch>();
        }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        public bool IsActive { get; set; }

        public ICollection<BankBranch> BankBranches { get; set; }

    }
}
