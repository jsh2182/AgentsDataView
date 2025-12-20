using System.ComponentModel.DataAnnotations;

namespace AgentsDataView.Entities.DtoModels
{
    public class SettingDto
    {
        [Required(AllowEmptyStrings = false, ErrorMessage ="نام تنظیمات الزامی است")]
        public string SettingName { get; set; } = string.Empty;
        public string SettingValue { get; set; } = string.Empty;
    }
}
