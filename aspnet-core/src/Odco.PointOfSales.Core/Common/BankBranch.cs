using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Odco.PointOfSales.Core.Common
{
    [Table("Common.BankBranch")]
    public class BankBranch : FullAuditedEntity<Guid>
    {
        public Guid BankId { get; set; }

        public Bank Bank { get; set; }

        [Required]
        [StringLength(150)]
        public string Name { get; set; }

        [StringLength(400)]
        public string Address { get; set; }

        [StringLength(15)]
        public string ContactNumber1 { get; set; }

        [StringLength(15)]
        public string ContactNumber2 { get; set; }

        [StringLength(15)]
        public string ContactNumber3 { get; set; }

        [StringLength(15)]
        public string FaxNumber { get; set; }

        public Guid? DistrictId { get; set; }
    }
}
