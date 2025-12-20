using AgentsDataView.Entities;

namespace AgentsDataView.Entities
{
    public class Setting:BaseEntity<int>
    {
        public required string SettingName { get; set; }
        public required string SettingValue { get; set; }
    }
}
