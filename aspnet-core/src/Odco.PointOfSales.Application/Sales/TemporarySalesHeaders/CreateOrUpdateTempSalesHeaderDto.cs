using Abp.AutoMapper;
using Odco.PointOfSales.Application.Sales.TemporarySalesProducts;
using Odco.PointOfSales.Core.Sales;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Odco.PointOfSales.Application.Sales.TemporarySalesHeaders
{
    [AutoMapTo(typeof(TempSalesHeader))]
    public class CreateOrUpdateTempSalesHeaderDto
    {
        /// <summary>
        /// Id Exist: Update
        /// Id Not Exist: Create
        /// </summary>
        public int? Id { get; set; }
        
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

        public bool IsActive { get; set; }

        public ICollection<CreateTempSalesProductDto> TempSalesProducts { get; set; }

        public CreateOrUpdateTempSalesHeaderDto()
        {
            TempSalesProducts = new HashSet<CreateTempSalesProductDto>();
        }
    }
}
