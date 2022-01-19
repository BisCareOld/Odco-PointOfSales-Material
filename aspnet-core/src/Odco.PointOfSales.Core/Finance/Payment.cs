using Abp.Domain.Entities.Auditing;
using Odco.PointOfSales.Core.Enums;
using Odco.PointOfSales.Core.Sales;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Odco.PointOfSales.Core.Finance
{
    /// <summary>
    /// Sale : Payment = 1 : M
    /// Payment : PaymentLineLevel = 1 : M
    /// Payment will generate the "InvoiceNumber"
    /// 2 Types of Payment Header
    ///     1. Sales Payment => SaleId (Exist)
    ///     2. Outstanding Payment => SaleId (Not Exist) 
    /// If IsOutstandingPaymentInvolved (True) => This Sales Payment having an outstanding payment
    /// 
    /// Identify Payment Type (Sale or Outstanding)
    ///     1. Directly from "PaymentType"
    ///     2. "SaleId" (Not Exist) =>  Outstanding else Sale 
    /// </summary>
    [Table("Finance.Payment")]
    public class Payment : FullAuditedEntity<Guid>
    {
        #region Sales
        public Guid? SaleId { get; set; }

        public Sale Sale { get; set; }

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

        /// <summary>
        /// 1 => Sales
        /// 2 => Outstanding
        /// </summary>
        public PaymentType PaymentType { get; set; }

        public bool IsOutstandingPaymentInvolved { get; set; }

        public ICollection<PaymentLineLevel> PaymentLineLevels { get; set; }

        public Payment()
        {
            PaymentLineLevels = new HashSet<PaymentLineLevel>();
        }
    }
}