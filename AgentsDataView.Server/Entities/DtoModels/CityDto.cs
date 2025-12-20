using System.ComponentModel.DataAnnotations;

namespace AgentsDataView.Entities.DtoModels
{
    public class CityDto
    {
        public int Id { get; set; }
        public int ProvinceId { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "کد شهر الزامی است.")]
        public int Code { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "نام شهر الزامی است")]
        [MaxLength(25, ErrorMessage = "نام شهر حداکثر 25 کاراکتر می تواند باشد.")]
        public string CityName { get; set; } = "";
    }
}
