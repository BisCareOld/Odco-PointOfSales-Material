
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Odco.PointOfSales.Core.Finance;
using System;
using System.ComponentModel.DataAnnotations;

namespace Odco.PointOfSales.Application.Finance.Payments
{
    [AutoMapTo(typeof(PaymentDto)), AutoMapFrom(typeof(Payment))]
    public class PaymentDto : EntityDto<Guid>
    {
        #region Sales
        public Guid SaleId { get; set; }

        [Required]
        [StringLength(15)]
        public string SaleNumber { get; set; }
        #endregion

        public Guid? CustomerId { get; set; }

        [StringLength(15)]
        public string CustomerPhoneNumber { get; set; }

        #region Cash
        public decimal? CashAmount { get; set; }
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

        public decimal? ChequeAmount { get; set; }
        #endregion

        #region Credit Outstanding
        public decimal? OutstandingAmount { get; set; }

        public decimal? OutstandingSettledAmount { get; set; }
        #endregion

        #region Debit Card

        #endregion

        #region Gift Card
        public decimal? GiftCardAmount { get; set; }
        #endregion

        public bool IsCash { get; set; }

        public bool IsCheque { get; set; }

        public bool IsCreditOutstanding { get; set; }

        public bool IsDebitCard { get; set; }

        public bool IsGiftCard { get; set; }
    }
}
