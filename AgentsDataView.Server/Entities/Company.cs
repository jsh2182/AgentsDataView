using System.ComponentModel.DataAnnotations;

namespace AgentsDataView.Entities
{
    public class Company : BaseEntity<int>
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "نام شرکت الزامی است")]
        [MaxLength(200, ErrorMessage = "نام شرکت حداکثر می تواند 200 کاراکتر داشته باشد")]
        public string Name { get; set; } = "";
        
        [Required(AllowEmptyStrings =false, ErrorMessage ="کد شرکت الزامی است.")]
        [MaxLength(100, ErrorMessage = "کدشرکت حداکثر می تواند 100 کاراکتر داشته باشد")]
        public string Code { get; set; } ="";
        public int? ProvinceId { get; set; }
        public int? CityId { get; set; }
        
        [MaxLength(100,ErrorMessage ="کدپستی حداکثر می تواند 100 کاراکتر داشته باشد")]
        public string PostalCode { get; set; } = "";
        
        [MaxLength(100, ErrorMessage = "نشانی حداکثر می تواند 1000 کاراکتر داشته باشد")]
        public string Address { get; set; } = "";

        [MaxLength(100, ErrorMessage = "شماره ثبت حداکثر می تواند 100 کاراکتر داشته باشد")]
        public string RegistrationNumber { get; set; } = "";

        [MaxLength(100, ErrorMessage = "شماره تلفن حداکثر می تواند 100 کاراکتر داشته باشد")]
        public string PhoneNumber { get; set; } = "";

        public DateTimeOffset CreationDate { get; set; } = DateTimeOffset.UtcNow;

        public DateTimeOffset? ValidTo { get; set; }

        public virtual Province? Province { get; set; }
        public virtual City? City { get; set; }
        public virtual ICollection<Product>? Products { get; set; }
        public virtual ICollection<Invoice>? Invoices { get; set; }
        public virtual ICollection<FinancialPeriod>? FinancialPeriods { get; set; }
        public virtual ICollection<SystemUser>? SystemUsers { get; set; }



    }
}
