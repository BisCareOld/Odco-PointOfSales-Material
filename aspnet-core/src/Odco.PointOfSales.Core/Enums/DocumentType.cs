namespace Odco.PointOfSales.Core.Enums
{
    public enum DocumentType : byte
    {
        PurchaseOrder = 1,
        GoodsReceivedNote = 2,
        SalesOrder = 3,
        Invoice = 4,
        Receipt = 5,
        Dispatch = 6,
        StockTransfer = 7,
        StockAdjustment = 8,
        CompanyReturn = 9,
        SalesReturn = 10
    }
}
