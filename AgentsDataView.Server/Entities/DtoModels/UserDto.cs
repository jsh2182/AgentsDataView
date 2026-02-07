using AgentsDataView.Common.Utilities;
using System.ComponentModel.DataAnnotations;

namespace AgentsDataView.Entities.DtoModels
{
    public class UserDto : IValidatableObject
    {
        public int Id { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage="وارد کردن نام کاربری الزامی است.")]
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
        public int[]? CompanyIds { get; set; }
        /// <summary>
        /// برای وقتی که کاربر قصد دارد ارتباط را با انتخاب استان ایجاد کند.
        /// </summary>
        public int[]? ProvinceIds { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (UserMobile?.IsValidMobileNumber() == false)
                {
                yield return new ValidationResult("شماره همراه معتبر نیست", [nameof(UserMobile)]);
            }
            if(!(CompanyIds?.Length > 0) && !(ProvinceIds?.Length >0))
            {
                yield return new ValidationResult("کاربر باید دست کم به یک شرکت یا استان متصل باشد.", [nameof(CompanyIds)]);
            }
        }
    }
}
