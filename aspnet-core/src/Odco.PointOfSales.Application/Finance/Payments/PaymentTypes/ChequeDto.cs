using System;
using System.ComponentModel.DataAnnotations;

namespace Odco.PointOfSales.Application.Finance.Payments.PaymentTypes
{
    public class ChequeDto
    {
        [StringLength(25)]
        public string ChequeNumber { get; set; }

        public Guid BankId { get; set; }

        [StringLength(100)]
        public string Bank { get; set; }

        public Guid? BranchId { get; set; }

        [StringLength(100)]
        public string Branch { get; set; }

        public DateTime ChequeReturnDate { get; set; }

        public decimal ChequeAmount { get; set; }
    }
}
