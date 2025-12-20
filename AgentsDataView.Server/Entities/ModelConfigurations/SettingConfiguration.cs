using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AgentsDataView.Entities
{
    public class SettingConfiguration : IEntityTypeConfiguration<Setting>
    {
        void IEntityTypeConfiguration<Setting>.Configure(EntityTypeBuilder<Setting> builder)
        {
            builder.ToTable("Settings", "dbo");
            builder.HasKey(x => x.Id);
            builder.HasIndex(s => s.SettingName).HasDatabaseName("IX_Setting_Name").IsUnique();
            builder.HasData(new Setting() { Id = 1, SettingName="LastUpdateDate", SettingValue="" });
            builder.HasData(new Setting() { Id = 2, SettingName = "AdvertisingLink", SettingValue="" });
        }
    }
}
