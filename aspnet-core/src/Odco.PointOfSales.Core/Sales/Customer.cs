using Abp.Domain.Entities.Auditing;
using Odco.PointOfSales.Core.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Odco.PointOfSales.Sales.Common
{
    [Table("Sales.Customer")]
    public partial class Customer : FullAuditedEntity<Guid>
    {
        [Required]
        [StringLength(10)]
        public string Code { get; set; }

        /// <summary>
        /// Mr, Mrs etc..
        /// </summary>
        [Required]
        public Guid PersonTitleId { get; set; }

        public PersonTitle PersonTitle { get; set; }

        /// <summary>
        /// Customer Name
        /// </summary>
        [Required]
        [StringLength(100)]
        public string FirstName { get; set; }

        [StringLength(100)]
        public string MiddleName { get; set; }

        [StringLength(100)]
        public string LastName { get; set; }

        /// <summary>
        /// Town Entity is changed into City
        /// </summary>
        [Required]
        public Guid CityId { get; set; }

        public City City { get; set; }

        [Required]
        [StringLength(15)]
        public string ContactNumber1 { get; set; }

        [StringLength(15)]
        public string ContactNumber2 { get; set; }

        [StringLength(15)]
        public string ContactNumber3 { get; set; }

        [StringLength(100)]
        public string Email { get; set; }

        /// <summary>
        /// Count of outstanding invoices 
        /// </summary>
        public int? NoOfInvoices { get; set; }

        /// <summary>
        /// Define the amount of rupees
        /// </summary>
        [Column(TypeName = "money")]
        public decimal? CreditLimit { get; set; }

        /// <summary>
        /// 1 - 30 days => drop down list
        /// </summary>
        public int? CreditPeriod { get; set; }

        /// <summary>
        /// 0 - 100
        /// </summary>
        public decimal? DiscountRate { get; set; }

        public Guid? ClassificationId { get; set; }

        public Classification Classification { get; set; }

        public Guid? PriceGroupId { get; set; }

        public PriceGroup PriceGroup { get; set; }

        public bool IsActive { get; set; }

        public ICollection<Address> Addresses { get; set; }

        public Customer()
        {
            Addresses = new HashSet<Address>();
        }
    }
}
