using System.ComponentModel.DataAnnotations;

namespace AgentsDataView.Entities
{
    public class Province:BaseEntity<int>
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "کد استان الزامی است.")]
        public int Code { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "نام استان الزامی است.")]
        public string ProvinceName { get; set; } = "";

        public virtual ICollection<City>? Cities { get; set; }
        public virtual ICollection<Company>?  Companies { get; set; }
    }
}
