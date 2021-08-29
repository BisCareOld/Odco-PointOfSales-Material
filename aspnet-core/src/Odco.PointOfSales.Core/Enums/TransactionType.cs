namespace Odco.PointOfSales.Core.Enums
{
    public enum TransactionType : byte
    {
        GRN = 1,
        IssueNote = 2,
        GoodsReturn = 3,
        StockAdjustmant = 4,
        StockTransfer = 5,
        SalesInvoice = 6 // Invoice without having 'Sales Order'
    }
}
