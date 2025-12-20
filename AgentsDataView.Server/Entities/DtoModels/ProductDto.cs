using System.ComponentModel.DataAnnotations;

namespace AgentsDataView.Entities.DtoModels
{
    public class ProductDto
    {
        public int Id { get; set; }
        public int CreatedById { get; set; }
        public DateTimeOffset CreationDate { get; set; }
        
        [Required(AllowEmptyStrings = false, ErrorMessage = "کد کالا مشخص نشده است")]
        [RegularExpression(@"^\d+$", ErrorMessage = "کد کالا باید فقط شامل اعداد باشد.")]
        [MaxLength(100, ErrorMessage = "کد کالا نمی تواند بیشتر از 100 رقم باشد.")]
        public string Code { get; set; } = "";

        [Required(AllowEmptyStrings = false, ErrorMessage = "نام کالا الزامی است")]
        [MaxLength(250, ErrorMessage = "طول نام کالا نمی تواند بیش از 250 کاراکتر باشد.")]
        public string Name { get; set; } = "";

        [MaxLength(250, ErrorMessage = "طول مدل کالا نمی تواند بیش از 250 کاراکتر باشد.")]
        public string Model { get; set; } = "";
    }
}
