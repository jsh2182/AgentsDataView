using System.ComponentModel.DataAnnotations;

namespace AgentsDataView.Entities.DtoModels
{
    public class FinancialPeriodDto : IValidatableObject
    {
        public int CompanyId { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "نام دوره مالی الزامی است.")]
        [MaxLength(100, ErrorMessage = "طول عنوان دوره نمی تواند بیشتر از 100 کاراکتر باشد.")]
        public string Title { get; set; } = "";

        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset? EndDate { get; set; }
        public int Id { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var minDate = new DateTimeOffset(2010, 1, 1, 0, 0, 0, TimeSpan.Zero);

            if (StartDate < minDate)
            {
                yield return new ValidationResult(
                    $"تاریخ باید بزرگتر از {minDate:yyyy-MM-dd} باشد.",
                    [nameof(StartDate)]);
            }
            if (StartDate.Offset != TimeSpan.Zero)
            {
                yield return new ValidationResult("تاریخ باید UTC باشد", [nameof(StartDate)]);
            }
            if (EndDate != null && EndDate.Value.Offset != TimeSpan.Zero)
            {
                yield return new ValidationResult("تاریخ باید UTC باشد", [nameof(EndDate)]);
            }
            if (StartDate >= EndDate)
            {
                yield return new ValidationResult("تاریخ پایان دوره باید بزرگتر از تاریخ آغاز دوره مالی باشد.", [nameof(EndDate)]);
            }
            if (CompanyId < 1)
            {
                yield return new ValidationResult("شناسه شرکت باید بزرگتر از صفر باشد.", [nameof(CompanyId)]);
            }
        }
    }
}
