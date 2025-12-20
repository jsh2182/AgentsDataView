using System.ComponentModel.DataAnnotations;

namespace AgentsDataView.Entities
{
    public class City:BaseEntity<int>
    {
        public int ProvinceId { get; set; }
        [Required(AllowEmptyStrings =false, ErrorMessage ="کد شهر الزامی است.")]
        public int Code { get; set; }
        
        [Required(AllowEmptyStrings = false, ErrorMessage = "نام شهر الزامی است")]
        [MaxLength(25, ErrorMessage ="نام شهر حداکثر 25 کاراکتر می تواند باشد.")]
        public string CityName { get; set; } = "";

        public virtual Province? Province { get; set; }
        public virtual ICollection<Company>? Companies { get; set; }
    }
}
