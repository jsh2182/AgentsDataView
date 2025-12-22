using System.ComponentModel.DataAnnotations;

namespace AgentsDataView.Entities.DtoModels
{
    public class UpdateMeDto
    {
        [Required(ErrorMessage = "وارد کردن نام کاربری الزامی است.")]
        [StringLength(100)]
        public string? UserName { get; set; }
        [Required(ErrorMessage = "وارد کردن نام کامل الزامی است.")]
        [StringLength(150)]
        public required string UserFullName { get; set; }
        public string? UserMobile { get; set; }
        public string Password { get; set; }
    }
}
