using Abp.Domain.Entities.Auditing;
using Odco.PointOfSales.Core.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Odco.PointOfSales.Core.Finance
{
    /// <summary>
    /// Payment : PaymentLineLevel = 1 : M
    /// 2 Types of Payment Line level
    ///     1. Sales Payment => SaleId (Exist) Or IsOutstandingPaymentInvolved (False)
    ///     2. Outstanding Payment => SaleId (Not Exist) Or IsOutstandingPaymentInvolved (True)
    /// </summary>
    [Table("Finance.PaymentLineLevel")]
    public class PaymentLineLevel : FullAuditedEntity<Guid>
    {
        #region Payment *Foreign Key
        public Guid? PaymentId { get; set; }
        
        public Payment Payment { get; set; }

        public PaymentType? PaymentType { get; set; }   // Just for a reference

        [StringLength(15)]
        public string InvoiceNumber { get; set; }
        #endregion

        #region Sales = Getting from Payment
        public Guid? SaleId { get; set; }

        [StringLength(15)]
        public string SalesNumber { get; set; }
        #endregion

        #region Customer = Getting from Payment
        public Guid? CustomerId { get; set; }

        [StringLength(10)]
        public string CustomerCode { get; set; }

        [StringLength(150)]
        public string CustomerName { get; set; }
        #endregion

        #region Cash

        #endregion

        #region Cheque
        [StringLength(25)]
        public string ChequeNumber { get; set; }

        public Guid? BankId { get; set; }

        [StringLength(100)]
        public string Bank { get; set; }

        public Guid? BranchId { get; set; }

        [StringLength(100)]
        public string Branch { get; set; }

        public DateTime? ChequeReturnDate { get; set; }
        #endregion

        #region Debit Card

        #endregion

        #region Gift Card

        #endregion

        #region Based on Customer Payment
        public decimal SpecificReceivedAmount { get; set; }

        public decimal SpecificBalanceAmount { get; set; }
        #endregion

        public decimal PaidAmount { get; set; }

        public bool IsCash { get; set; }

        public bool IsCheque { get; set; }

        public bool IsDebitCard { get; set; }

        public bool IsGiftCard { get; set; }

        public bool IsOutstandingPaymentInvolved { get; set; }
    }
}
