
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Odco.PointOfSales.Application.Finance.PaymentLineLevels;
using Odco.PointOfSales.Core.Finance;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Odco.PointOfSales.Application.Finance.Payments
{
    [AutoMapTo(typeof(PaymentDto)), AutoMapFrom(typeof(Payment))]
    public class PaymentDto : EntityDto<Guid>
    {
        #region Sales
        public Guid? SaleId { get; set; }

        [StringLength(15)]
        public string SalesNumber { get; set; }

        /// <summary>
        /// Similar to PaymentNumber = InvoiceNumber
        /// Exist: When creating a Payment
        /// Sale : InvoiceNumber = 1 : M
        /// </summary>
        [Required]
        [StringLength(15)]
        public string InvoiceNumber { get; set; }
        #endregion

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

        public decimal TotalPaidAmount { get; set; }
        #endregion

        [StringLength(100)]
        public string Remarks { get; set; }

        public bool IsOutstandingPaymentInvolved { get; set; }

        #region ABP 
        public DateTime CreationTime { get; set; }
        #endregion

        public ICollection<PaymentLineLevelDto> PaymentLineLevels { get; set; }

        public PaymentDto()
        {
            PaymentLineLevels = new HashSet<PaymentLineLevelDto>();
        }

    }
}
