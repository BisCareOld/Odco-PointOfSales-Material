namespace Odco.PointOfSales.Core.Enums
{
    public enum DocumentType : byte
    {
        PurchaseOrder = 1,
        GoodsReceivedNote = 2,
        SalesOrder = 3,
        Sales = 4,
        Invoice = 5,            // Payment
        Receipt = 6,
        Dispatch = 7,
        StockTransfer = 8,
        StockAdjustment = 9,
        CompanyReturn = 10,
        SalesReturn = 11
    }
}
