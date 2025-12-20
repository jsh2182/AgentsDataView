namespace AgentsDataView.Entities
{
    public static class BaseInfoes
    {
        public enum InvoiceTypes
        {
            /// <summary>
            /// پیش فاکتور فروش
            /// </summary>
            PreSale,
            /// <summary>
            /// فاکتور فروش
            /// </summary>
            Sales,
            /// <summary>
            ///برگشت از فروش
            /// </summary>
            SalesReturn,
            /// <summary>
            /// پیش فاکتور خرید
            /// </summary>
            PrePurchase,
            /// <summary>
            /// فاکتور خرید
            /// </summary>
            Purchase,
            /// <summary>
            /// برگشت از خرید
            /// </summary>
            PurchaseReturn,
            /// <summary>
            /// رسید انبار
            /// </summary>
            GoodsReceipt,
            /// <summary>
            /// حواله انبار
            /// </summary>
            GoodsIssue,
            /// <summary>
            /// انتقال بین انبارها
            /// </summary>
            StockTransfer,
            /// <summary>
            /// رسید تولید در جریان
            /// </summary>
            WIPProductionReceipt,
            /// <summary>
            /// رسید تولید پیشرفته
            /// </summary>
            AdvancedProductionReceipt,
            /// <summary>
            /// رسید تولید درجریان تکمیلی
            /// </summary>
            WIPCompletionReceipt,
            /// <summary>
            /// رسید تولید پیشرفته تکمیلی
            /// </summary>
            AdvancedProductionCompletionReceipt,
            /// <summary>
            /// رسید اول دوره
            /// </summary>
            BeginingInventoryReceipt,
            /// <summary>
            /// تعدیلات اضافی انبار
            /// </summary>
            InventoryAdjustment_Increase,
            /// <summary>
            /// تعدیلات کسری انبار
            /// </summary>
            InventoryAdjustment_Decrease
        }

        public enum InvoiceStates
        {
            Draft,
            Approved,
        }

        public static readonly Dictionary<InvoiceTypes, string> PInvoiceTypes = new()
        {
            { InvoiceTypes.GoodsIssue,"حواله انبار" },
            { InvoiceTypes.GoodsReceipt , "رسید انبار" },
            { InvoiceTypes.PrePurchase , "پیش فاکتور خرید" },
            { InvoiceTypes.PreSale , "پیش فاکتور فروش" },
            { InvoiceTypes.Purchase , "فاکتور خرید" },
            { InvoiceTypes.PurchaseReturn , "برگشت از خرید" },
            { InvoiceTypes.Sales , "فاکتور فروش" },
            { InvoiceTypes.SalesReturn , "برگشت از فروش" },
            { InvoiceTypes.StockTransfer , "انتقال بین انبارها" },
             { InvoiceTypes.WIPProductionReceipt,"رسید تولید در جریان" },
             { InvoiceTypes.AdvancedProductionReceipt, "رسید تولید پیشرفته" },
             { InvoiceTypes.WIPCompletionReceipt,"رسید تولید درجریان تکمیلی" },
             { InvoiceTypes.AdvancedProductionCompletionReceipt,"رسید تولید پیشرفته تکمیلی" },
             { InvoiceTypes.BeginingInventoryReceipt,"رسید اول دوره" },
             { InvoiceTypes.InventoryAdjustment_Increase,"تعدیلات اضافی انبار" },
             { InvoiceTypes.InventoryAdjustment_Decrease,"تعدیلات کسری انبار" }
        };

    }
}
