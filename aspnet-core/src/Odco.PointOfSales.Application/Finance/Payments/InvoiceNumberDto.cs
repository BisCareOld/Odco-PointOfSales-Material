using System;

namespace Odco.PointOfSales.Application.Finance.Payments
{
    public class InvoiceNumberDto
    {
        public Guid PaymentId { get; set; }

        public string InvoiceNumber { get; set; }

        /// <summary>
        /// Sales => 1
        /// Outstanding => 2
        /// </summary>
        public byte PaymentType { get; set; }
    }
}
