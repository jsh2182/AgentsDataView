namespace AgentsDataView.Entities
{

    public class Invoice : BaseEntity<long>
    {

        /// <summary>
        /// شناسه شرکت باید همان شناسه ای باشد که از طریق Api ثبت شرکت دریافت شده است
        /// </summary>
        public int CompanyId { get; set; }
        /// <summary>
        /// تاریخ ثبت فاکتور در Api‏
        /// </summary>
        public DateTimeOffset CreationDate { get; set; }
        /// <summary>
        /// شناسه کاربر ثبت کننده
        /// </summary>
        public int CreatedById { get; set; }
        /// <summary>
        /// شناسه مشتری که فاکتور به نام او صادر شده است.<br/>
        /// </summary>
        public long CustomerId { get; set; }
        /// <summary>
        /// دوره مالی
        /// </summary>
        public int FinancialPeriodId { get; set; }

        public string InvoiceDescription { get; set; } = "";
        /// <summary>
        /// تاریخ فاکتور که از کاربر گرفته می شود.
        /// </summary>
        public DateTimeOffset InvoiceDate { get; set; }
        /// <summary>
        /// شماره فاکتور از کاربر گرفته می شود.
        /// </summary>
        public int InvoiceNumber { get; set; }
        /// <summary>
        /// تنها فاکتورهایی در گزارش ها نمایش داده می شود که وضعیت آنها تایید شده باشد<br/>
        /// وضعیت پیش نویس زمانی تبدیل به تایید می شود که کاربری دکمه بروزرسانی داده را سمت مرورگر کلیک کند.
        /// </summary>
        public BaseInfoes.InvoiceStates InvoiceState { get; set; } = BaseInfoes.InvoiceStates.Draft;
        public required BaseInfoes.InvoiceTypes InvoiceType { get; set; }
        /// <summary>
        /// شناسه آخرین کاربری که فاکتور را بروزرسانی کرده است.<br/>
        /// می تواند شناسه کاربری باشد که دکمه بروزرسانی را کلیک کرده یا شناسه کاربری که فاکتور ویرایش شده را به Api فرستاده است.
        /// </summary>
        public int? UpdatedById { get; set; }
        /// <summary>
        /// تاریخ  آخرین بروزرسانی .<br/>
        /// می تواند تاریخ آخرین بروزرسانی از سمت داده های شرکت باشد یا تاریخ آخرین بروزرسانی از سمت کاربر نهایی
        /// </summary>
        public DateTimeOffset? UpdateDate { get; set; }
        /// <summary>
        /// فاکتوری که از سمت Api حذف می شود به طور منطقی اینجا حذف خواهد شد.<br/>
        /// هنگامی که کاربر دکمه بروزرسانی را روی مرورگر کلیک کرد این فاکتورها به صورت دائمی حذف خواهند شد
        /// </summary>
        public bool SignedForDelete { get; set; }
        /// <summary>
        /// نام کاربری سمت کلاینت(سیستمی که Api از روی آن فراخوانی شده)‏
        /// </summary>
        public string ClientUserName { get; set; } = "";

        /// <summary>
        /// ارزش افزوده کل 
        /// </summary>
        public decimal TaxValue { get; set; }
        /// <summary>
        /// تخفیف کل
        /// </summary>
        public decimal DiscountValue { get; set; }
        /// <summary>
        /// سایر هزینه ها
        /// </summary>
        public decimal ExtraCosts { get; set; }
        /// <summary>
        /// مبلغ نهایی فاکتور
        /// </summary>
        public decimal InvoiceNetPrice { get; set; }

        public virtual Company? Company { get; set; }
        public virtual SystemUser? CreatedBy { get; set; }
        public virtual SystemUser? UpdatedBy { get; set; }
        public virtual FinancialPeriod? FinancialPeriod { get; set; }
        public virtual ICollection<InvoiceDetail>? InvoiceDetails { get; set; }
    }
}
