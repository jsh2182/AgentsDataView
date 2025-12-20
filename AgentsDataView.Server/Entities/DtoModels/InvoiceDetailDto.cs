using System.ComponentModel.DataAnnotations;

namespace AgentsDataView.Entities.DtoModels
{
    public class InvoiceDetailDto:IValidatableObject
    {
        public long InvoiceId { get; set; }
        public string ProductCode { get; set; } = "";
        public int ProductId { get; set; }
        public string ProductName { get; set; } = "";
        public string ProductModel { get; set; } = "";

        [Range(1, 1_000_000, ErrorMessage = "تعداد باید عددی بین 1 و 1,000,000 باشد.")]
        public decimal Quantity { get; set; } = 1;

        [Range(0, 100_000_000_000, ErrorMessage = "بهای واحد باید عددی بین 0 و 100,000,000,000 باشد.")]
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; }

        [Range(0, 100, ErrorMessage = "درصد ارزش افزوده باید عددی بین 0 و 100 باشد.")]
        public decimal TaxPercent { get; set; }
       
        [Range(0, 9_9999_999_999_999.9999, ErrorMessage = "مبلغ ارزش افزوده باید عددی بین 0 و 9,9999,999,999,999.9999 باشد.")]
        public decimal TaxValue { get; set; }

        [Range(0, 100, ErrorMessage = "درصد تخفیف باید عددی بین 0 و 100 باشد.")]
        public decimal DiscountPercent { get; set; }
        [Range(0, 9_9999_999_999_999.9999, ErrorMessage = "مبلغ تخفیف باید عددی بین 0 و 9,9999,999,999,999.9999 باشد.")]
        public decimal DiscountValue { get; set; }

        /// <summary>
        /// بهای خالص: بهای کل پس از ارزش افزوده و تخفیف
        /// </summary>
        public decimal NetPrice { get; }
        /// <summary>
        /// بهای تمام شده کالای فروش رفته
        /// </summary>
        public decimal CostOfGoodsSold { get; set; }

        public string DetailDescription { get; set; } = "";
        public Currencies Currency { get; set; } = Currencies.IRR;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if(TaxValue > (UnitPrice * Quantity - DiscountValue))
            {
                yield return new ValidationResult("بهای ارزش افزوده نمی تواند بزرگتر از بهای کل منهای تخفیف  باشد.", [nameof(TaxValue)]);
            }
            if (DiscountValue > (UnitPrice * Quantity))
            {
                yield return new ValidationResult("بهای تخفیف نمی تواند بزرگتر از بهای کل  باشد.", [nameof(TaxValue)]);
            }
            if(ProductId < 1)
            {
                yield return new ValidationResult("شناسه ارتباطی کالا باید بزرگتر از صفر باشد", [nameof(ProductId)]);   
            }
        }
    }
}
