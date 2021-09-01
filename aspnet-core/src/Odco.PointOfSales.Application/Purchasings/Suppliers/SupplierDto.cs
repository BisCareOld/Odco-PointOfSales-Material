using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Odco.PointOfSales.Core.Common;
using System;
using System.ComponentModel.DataAnnotations;

namespace Odco.PointOfSales.Application.Purchasings.Suppliers
{
    [AutoMapTo(typeof(SupplierDto)), AutoMapFrom(typeof(Supplier))]
    public class SupplierDto : EntityDto<Guid>
    {
        [Required]
        [StringLength(10)]
        public string Code { get; set; }

        [Required]
        [StringLength(100)]
        public string BusinessName { get; set; }

        [Required]
        public Guid CityId { get; set; }

        [Required]
        public Guid PersonTitleId { get; set; }

        [Required]
        [StringLength(100)]
        public string FirstName { get; set; }

        [StringLength(100)]
        public string MiddleName { get; set; }

        [StringLength(100)]
        public string LastName { get; set; }

        [Required]
        [StringLength(15)]
        public string ContactNumber1 { get; set; }

        [StringLength(15)]
        public string ContactNumber2 { get; set; }

        [StringLength(15)]
        public string ContactNumber3 { get; set; }

        [StringLength(100)]
        public string Email { get; set; }

        public int? NoOfInvoices { get; set; }

        public decimal? CreditLimit { get; set; }

        public int? CreditPeriod { get; set; }

        public decimal? DiscountRate { get; set; }

        public Guid? ClassificationId { get; set; }

        public Guid? PriceGroupId { get; set; }

        public bool IsActive { get; set; }
    }
}
