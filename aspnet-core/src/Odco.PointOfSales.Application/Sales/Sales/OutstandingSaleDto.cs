using System;

namespace Odco.PointOfSales.Application.Sales.Sales
{
    public class OutstandingSaleDto
    {
        public bool IsSelected { get; set; }

        public Guid SaleId { get; set; }

        public string SalesNumber { get; set; }

        public decimal NetAmount { get; set; }              //  100

        public decimal DueOutstandingAmount { get; set; }   //  100 ->  0

        public decimal? EnteredAmount { get; set; }         // Check null or 0
    }
}
