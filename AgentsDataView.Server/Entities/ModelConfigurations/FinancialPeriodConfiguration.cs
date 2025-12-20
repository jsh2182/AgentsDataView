using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AgentsDataView.Entities
{
    public class FinancialPeriodConfiguration : IEntityTypeConfiguration<FinancialPeriod>
    {
        public void Configure(EntityTypeBuilder<FinancialPeriod> builder)
        {
            builder.ToTable("FinancialPeriods","dbo");
            builder.HasKey(f => f.Id);
            builder.Property(f => f.Id).ValueGeneratedOnAdd();
            builder.HasOne(f => f.Company)
                .WithMany(c => c.FinancialPeriods)
                .HasForeignKey(c => c.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
