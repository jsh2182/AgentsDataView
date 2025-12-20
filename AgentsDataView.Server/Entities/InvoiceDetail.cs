using System.ComponentModel.DataAnnotations;

namespace AgentsDataView.Entities
{
    public enum Currencies
    {
        /// <summary>
        /// ریال ایران
        /// </summary>
        IRR,
        /// <summary>
        /// دلار آمریکا
        /// </summary>
        USD, 
        /// <summary>
        /// یورو
        /// </summary>
        EUR, 
        /// <summary>
        /// پوند بریتانیا
        /// </summary>
        GBP, 
        /// <summary>
        /// فرانک سوئیس
        /// </summary>
        CHF,
        /// <summary>
        ///  ین ژاپن
        /// </summary>
        JPY,
        /// <summary>
        /// یوآن چین
        /// </summary>
        CNY, 
        /// <summary>
        ///  دلار کانادا
        /// </summary>
        CAD,
        /// <summary>
        /// دلار استرالیا
        /// </summary>
        AUD, 
        /// <summary>
        ///  دلار نیوزیلند
        /// </summary>
        NZD,
        /// <summary>
        ///  دلار سنگاپور
        /// </summary>
        SGD,
        /// <summary>
        ///  وون کره جنوبی
        /// </summary>
        KRW,
        /// <summary>
        /// روپیه هند
        /// </summary>
        INR, 
        /// <summary>
        /// رئال برزیل
        /// </summary>
        BRL, 
        /// <summary>
        /// پزو مکزیک
        /// </summary>
        MXN, 
        /// <summary>
        /// راند آفریقای جنوبی
        /// </summary>
        ZAR, 
        /// <summary>
        /// روبل روسیه
        /// </summary>
        RUB, 
        /// <summary>
        /// درهم امارات متحده عربی
        /// </summary>
        AED, 
        /// <summary>
        ///  ریال سعودی
        /// </summary>
        SAR,
        /// <summary>
        /// ریال قطر
        /// </summary>
        QAR, 
        /// <summary>
        /// دینار کویت
        /// </summary>
        KWD, 
        /// <summary>
        /// دینار بحرین
        /// </summary>
        BHD, 
        /// <summary>
        /// ریال عمان
        /// </summary>
        OMR, 
        /// <summary>
        /// دینار اردن
        /// </summary>
        JOD, 
        /// <summary>
        /// پوند مصر
        /// </summary>
        EGP, 
        /// <summary>
        ///  درهم مراکش
        /// </summary>
        MAD,
        /// <summary>
        /// دینار تونس
        /// </summary>
        TND
    }
    public class InvoiceDetail : BaseEntity<long>
    {
        public long InvoiceId { get; set; }
        public int ProductId { get; set; }

        [Range(1, 1_000_000, ErrorMessage ="تعداد باید عددی بین 1 و 1,000,000 باشد.")]
        public decimal Quantity { get; set; } = 1;

        [Range(0, 100_000_000_000, ErrorMessage = "بهای واحد باید عددی بین 0 و 100,000,000,000 باشد.")]
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; }

        [Range(0,100, ErrorMessage ="درصد ارزش افزوده باید عددی بین 0 و 100 باشد.")]
        public decimal TaxPercent { get; set; }
        [Range(0, 9_9999_999_999_999.9999, ErrorMessage = "مبلغ ارزش افزوده باید عددی بین 0 و 9,9999,999,999,999.9999 باشد.")]
        public decimal TaxValue { get; set; }

        [Range(0,100, ErrorMessage ="درصد تخفیف باید عددی بین 0 و 100 باشد.")]
        public decimal DiscountPercent { get; set; }
        [Range(0,9_9999_999_999_999.9999, ErrorMessage = "مبلغ تخفیف باید عددی بین 0 و 9,9999,999,999,999.9999 باشد.")]
        public decimal DiscountValue { get; set; }

        /// <summary>
        /// بهای خالص: بهای کل پس از ارزش افزوده و تخفیف
        /// </summary>
        public decimal NetPrice { get; }
        /// <summary>
        /// 
        /// </summary>
        public decimal CostOfGoodsSold { get; set; }

        /// <summary>
        /// برای همگام سازی با تاخیر هر ردیف فاکتوری که ثبت می شود به صورت پیش نویس ثبت خواهد شد.<br/>
        /// وقتی کاربر در مرورگر دکمه بروزرسانی را کلیک کند این فیلد مقدار False خواهد گرفت.<br/>
        /// تنها ردیفهایی در گزارش نمایش داده می شوند که پیش نویس نباشند.
        /// </summary>


        public string DetailDescription { get; set; } = "";
        public Currencies Currency { get; set; } = Currencies.IRR;

        public virtual Product? Product { get; set; }
        public virtual Invoice? Invoice { get; set; }

    }
}
