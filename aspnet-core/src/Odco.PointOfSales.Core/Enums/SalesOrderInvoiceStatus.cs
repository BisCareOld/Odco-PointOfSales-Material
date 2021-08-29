namespace Odco.PointOfSales.Core.Enums
{
    // Invoice Line level
    public enum SalesOrderInvoiceStatus : byte
    {
        Draft = 1,
        PartiallyInvoiced = 2,
        FullyInvoiced = 3,
        FullyDispatched,
        PartiallyReturned
    }
}
