using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AgentsDataView.Entities
{
    public class SystemUserConfiguration : IEntityTypeConfiguration<SystemUser>
    {
        public void Configure(EntityTypeBuilder<SystemUser> builder)
        {
            builder.ToTable("SystemUser", "dbo");
            builder.HasKey(s => s.Id);
            builder.Property(s => s.Id).ValueGeneratedOnAdd();
            builder.Property(s => s.LoginInfo).HasMaxLength(1000);
            builder.Property(s => s.Password).HasMaxLength(100).IsRequired();
            builder.Property(s => s.UserFullName).HasMaxLength(350).IsRequired();
            builder.Property(s => s.UserMobile).HasMaxLength(11);
            builder.Property(s => s.UserName).HasMaxLength(50);
            builder.Property(s => s.IsActive);
            builder.HasIndex(s => s.RelatedPersonID).HasDatabaseName("IX_SystemUser_PersonID");

            //$yStem$uPer_ @dM!N#0 For Admin
            builder.HasData(new SystemUser() { UserFullName = "مدیر", UserName = "Super", Id = 1, Password= "AuyVk1xj38jg+H3ae5aCy73j+aH+ygoqPSec3xgAasA=" });
        }
    }
}
