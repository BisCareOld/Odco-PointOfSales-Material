using Abp.Domain.Entities.Auditing;
using Odco.PointOfSales.Sales.Common;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Odco.PointOfSales.Core.Common
{
    [Table("Common.Address")]
    public class Address : FullAuditedEntity<Guid>
    {
        [StringLength(200)]
        public string AddressLine1 { get; set; }

        [StringLength(200)]
        public string AddressLine2 { get; set; }

        public Guid DistrictId { get; set; }

        public Guid CityId { get; set; }

        public City City { get; set; }

        [StringLength(10)]
        public string PostalCode { get; set; }

        public bool IsDefault { get; set; }

        public Guid? SupplierId { get; set; }

        public Supplier Supplier { get; set; }

        public Guid? CustomerId { get; set; }

        public Customer Customer { get; set; }
    }
}
