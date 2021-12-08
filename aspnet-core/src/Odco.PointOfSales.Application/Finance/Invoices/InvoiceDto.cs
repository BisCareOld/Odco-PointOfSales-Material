using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Odco.PointOfSales.Application.Finance.InvoiceProducts;
using Odco.PointOfSales.Core.Finance;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Odco.PointOfSales.Application.Finance.Invoices
{
    [AutoMapTo(typeof(InvoiceDto)), AutoMapFrom(typeof(Invoice))]
    public class InvoiceDto : EntityDto<Guid>
    {
        public InvoiceDto()
        {
            InvoiceProducts = new HashSet<InvoiceProductDto>();
        }

        public int? TempSaleId { get; set; }

        [Required]
        [StringLength(15)]
        public string InvoiceNumber { get; set; }

        [StringLength(10)]
        public string ReferenceNumber { get; set; }

        public Guid? CustomerId { get; set; }

        [StringLength(10)]
        public string CustomerCode { get; set; }

        [StringLength(100)]
        public string CustomerName { get; set; }

        public decimal DiscountRate { get; set; }

        public decimal DiscountAmount { get; set; }

        public decimal TaxRate { get; set; }

        public decimal TaxAmount { get; set; }

        public decimal GrossAmount { get; set; }

        public decimal NetAmount { get; set; }

        [StringLength(100)]
        public string Remarks { get; set; }

        public ICollection<InvoiceProductDto> InvoiceProducts { get; set; }
    }
}
