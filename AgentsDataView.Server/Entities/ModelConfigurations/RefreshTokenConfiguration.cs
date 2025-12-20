using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AgentsDataView.Entities.ModelConfigurations
{
    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.ToTable("RefreshTokens", "dbo");
            builder.HasKey(r => r.Id);
            builder.Property(r => r.Id).ValueGeneratedOnAdd();

            builder.HasOne(r=>r.SystemUser)
                .WithMany(s=>s.RefreshTokens)
                .HasForeignKey(r=>r.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();


        }
    }
}
