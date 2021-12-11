using Abp.AutoMapper;
using Odco.PointOfSales.Application.Finance.Payments.PaymentTypes;
using Odco.PointOfSales.Core.Finance;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Odco.PointOfSales.Application.Finance.Invoices
{
    [AutoMapTo(typeof(Invoice))]
    public class CreateInvoiceDto
    {
        public CreateInvoiceDto()
        {
            Cashes = new HashSet<CashDto>();
            Cheques = new HashSet<ChequeDto>();
            Outstandings = new HashSet<CustomerCreditOutstandingDto>();
            DebitCards = new HashSet<DebitCardDto>();
            GiftCards = new HashSet<GiftCardDto>();
        }

        public int? TempSaleId { get; set; }

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

        #region Payment Types
        public ICollection<CashDto> Cashes { get; set; }
        public ICollection<ChequeDto> Cheques { get; set; }
        public ICollection<CustomerCreditOutstandingDto> Outstandings { get; set; }
        public ICollection<DebitCardDto> DebitCards { get; set; }
        public ICollection<GiftCardDto> GiftCards { get; set; }
        #endregion
    }
}
