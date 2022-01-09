using Odco.PointOfSales.Application.Finance.Payments.PaymentTypes;
using Odco.PointOfSales.Application.Sales.Sales;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Odco.PointOfSales.Application.Finance.Payments
{
    public class CreatePaymentForOutstandingDto
    {
        public CreatePaymentForOutstandingDto()
        {
            OutstandingSales = new HashSet<OutstandingSaleDto>();
            //PaymentDtos
            Cashes = new HashSet<CashDto>();
            Cheques = new HashSet<ChequeDto>();
            DebitCards = new HashSet<DebitCardDto>();
            GiftCards = new HashSet<GiftCardDto>();
        }

        #region Customer
        public Guid? CustomerId { get; set; }

        [StringLength(10)]
        public string CustomerCode { get; set; }

        [StringLength(150)]
        public string CustomerName { get; set; }
        #endregion

        #region Based on Customer Payment, Getting Summary of Payment
        public decimal TotalReceivedAmount { get; set; }

        public decimal TotalBalanceAmount { get; set; }

        // PaidAmount = TotalReceivedAmount - TotalBalanceAmount
        #endregion

        public bool IsOutstandingPaymentInvolved { get; set; }

        public ICollection<OutstandingSaleDto> OutstandingSales { get; set; }

        #region Payment Types
        public ICollection<CashDto> Cashes { get; set; }
        public ICollection<ChequeDto> Cheques { get; set; }
        public ICollection<DebitCardDto> DebitCards { get; set; }
        public ICollection<GiftCardDto> GiftCards { get; set; }
        #endregion
    }
}
