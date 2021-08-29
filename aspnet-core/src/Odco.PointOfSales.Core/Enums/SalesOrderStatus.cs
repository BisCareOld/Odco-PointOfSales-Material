namespace Odco.PointOfSales.Core.Enums
{
    public enum SalesOrderStatus : byte
    {
        Draft = 1,
        PendingApproval = 2,
        Rejected = 3,
        Approved = 4,
        PartiallyDispatched = 5,
        FullyDispatched = 6,
        PartiallyReturned = 7,
        FullyReturned = 8
    }
}
