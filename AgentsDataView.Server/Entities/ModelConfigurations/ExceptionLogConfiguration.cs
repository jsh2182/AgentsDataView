using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AgentsDataView.Entities
{
    public class ExceptionLogConfiguration : IEntityTypeConfiguration<ExceptionLog>
    {
        public void Configure(EntityTypeBuilder<ExceptionLog> builder)
        {
            builder.ToTable("ExceptionLog", "dbo");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).ValueGeneratedOnAdd();
            builder.Property(e => e.MachineName).IsRequired().HasMaxLength(50);
            builder.Property(e => e.Message).IsRequired();
            builder.Property(e=>e.HttpAction).HasMaxLength(250);
            builder.Property(e=>e.CallSite).HasMaxLength(500);
            builder.Property(e=>e.RequestedURL).HasMaxLength(500);
        }
    }
}
