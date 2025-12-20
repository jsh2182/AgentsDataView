using AgentsDataView.Common.Utilities;
using System.ComponentModel.DataAnnotations;

namespace AgentsDataView.Entities.DtoModels
{
    public class UserDto : IValidatableObject
    {
        public int CompanyId { get; set; }
        public int Id { get; set; }
        [Required(ErrorMessage="وارد کردن نام کاربری الزامی است.")]
        [StringLength(100)]
        public string? UserName { get; set; }
        [Required(ErrorMessage="وارد کردن نام کامل الزامی است.")]
        [StringLength(150)]
        public required string UserFullName { get; set; }
        public string? UserMobile { get; set; }
        public string Password { get; set; }
        public long? RelatedPersonID { get; set; }
        public bool IsActive { get; set; } = true;
        public string? LoginInfo { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (UserMobile?.IsValidMobileNumber() == false)
                {
                yield return new ValidationResult("شماره همراه معتبر نیست", [nameof(UserMobile)]);
            }
            if(CompanyId < 1)
            {
                yield return new ValidationResult("شناسه شرکت باید بزرگتر از صفر باشد.", [nameof(CompanyId)]);
            }
        }
    }
}
