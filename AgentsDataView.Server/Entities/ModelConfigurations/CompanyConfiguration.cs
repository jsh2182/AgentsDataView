using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AgentsDataView.Entities
{
    public class CompanyConfiguration : IEntityTypeConfiguration<Company>
    {
        public void Configure(EntityTypeBuilder<Company> builder)
        {
            builder.ToTable("Companies", "dbo");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id).ValueGeneratedOnAdd();
            builder.HasIndex(c => c.ProvinceId).HasDatabaseName("IX_Company_ProvinceId");

            builder.HasOne(c => c.Province)
                .WithMany(p => p.Companies)
                .HasForeignKey(c=>c.ProvinceId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(c=>c.City)
                .WithMany(c=>c.Companies)
                .HasForeignKey(c=>c.CityId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
