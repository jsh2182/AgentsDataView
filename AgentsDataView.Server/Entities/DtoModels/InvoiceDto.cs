using System.ComponentModel.DataAnnotations;

namespace AgentsDataView.Entities.DtoModels
{
    public class InvoiceDto : IValidatableObject
    {
        public long Id { get; set; }
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
        /// رسید و حواله انبار شناسه مشتری ندارند.
        /// </summary>
        public long CustomerId { get; set; }
        /// <summary>
        /// دوره مالی
        /// </summary>
        public int FinancialPeriodId { get; set; }

        public string InvoiceDescription { get; set; } = "";
        /// <summary>
        /// تاریخ فاکتور که از کاربر گرفته می شود.
        /// (به صورت UTC)
        /// </summary>
        public DateTimeOffset InvoiceDate { get; set; }
        /// <summary>
        /// شماره فاکتور از کاربر گرفته می شود.
        /// </summary>
        [Range(1, int.MaxValue, ErrorMessage = $"شماره فاکتور باید عددی بین 1 و 2,147,483,647 باشد.")]
        public int InvoiceNumber { get; set; }
        /// <summary>
        /// تنها فاکتورهایی در گزارش ها نمایش داده می شود که وضعیت آنها تایید شده باشد<br/>
        /// وضعیت پیش نویس زمانی تبدیل به تایید می شود که کاربری دکمه بروزرسانی داده را سمت مرورگر کلیک کند.
        /// </summary>
        public BaseInfoes.InvoiceStates InvoiceState { get; set; } = BaseInfoes.InvoiceStates.Approved;
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
        public List<InvoiceDetailDto> InvoiceDetails { get; set; } = [];

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var minDate = new DateTimeOffset(2010, 1, 1, 0, 0, 0, TimeSpan.Zero);

            if (InvoiceDate < minDate)
            {
                yield return new ValidationResult(
                    $"تاریخ باید بزرگتر از {minDate:yyyy-MM-dd} باشد.",
                    [nameof(InvoiceDate)]);
            }
            if (InvoiceDate.Offset != TimeSpan.Zero)
            {
                yield return new ValidationResult("تاریخ باید UTC باشد", [nameof(InvoiceDate)]);
            }
            if (InvoiceDetails.Count == 0)
            {
                yield return new ValidationResult("ثبت فاکتور بدون ردیف امکانپذیر نیست.", [nameof(InvoiceDetails)]);
            }
            if(FinancialPeriodId < 1)
            {
                yield return new ValidationResult("شناسه دوره مالی باید بزرگتر از صفر باشد.", [nameof(FinancialPeriodId)]);
            }
            var sumNetPrice = InvoiceDetails.Sum(d => d.NetPrice);
            bool hasDiff = sumNetPrice - DiscountValue + TaxValue + ExtraCosts == InvoiceNetPrice;
            if (hasDiff)
            {
                yield return new ValidationResult("جمع خالص فاکتور باید برابر با جمع خالص ردیفها - تخفیف فاکتور +سایر هزینه ها+ ارزش افزوده فاکتور باشد", [nameof(InvoiceNetPrice)]);
            }
        }
    }
}
