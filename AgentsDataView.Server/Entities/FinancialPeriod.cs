using System.ComponentModel.DataAnnotations;

namespace AgentsDataView.Entities
{
    public class FinancialPeriod:BaseEntity<int>
    {
        public int CompanyId { get; set; }
        [Required(AllowEmptyStrings =false, ErrorMessage ="نام دوره مالی الزامی است.")]
        [MaxLength(100, ErrorMessage ="طول عنوان دوره نمی تواند بیشتر از 100 کاراکتر باشد.")]
        public string Title { get; set; } = "";

        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset? EndDate { get; set; }

        public virtual Company? Company { get; set; }
        public virtual ICollection<Invoice>? Invoices { get; set; }
    }
}
