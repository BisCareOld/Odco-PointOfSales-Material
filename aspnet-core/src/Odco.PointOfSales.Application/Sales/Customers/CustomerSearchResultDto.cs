using System;

namespace Odco.PointOfSales.Application.Sales.Customers
{
    public class CustomerSearchResultDto
    {
        public Guid Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public string ContactNumber1 { get; set; }

        public string ContactNumber2 { get; set; }

        public string ContactNumber3 { get; set; }
        
        public bool IsActive { get; set; }
    }
}