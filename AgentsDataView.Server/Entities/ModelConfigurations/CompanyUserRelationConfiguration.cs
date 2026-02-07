using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AgentsDataView.Entities
{
    public class CompanyUserRelationConfiguration : IEntityTypeConfiguration<CompanyUserRelation>
    {
        public void Configure(EntityTypeBuilder<CompanyUserRelation> builder)
        {
            builder.ToTable("CompanyUserRelations", "dbo");
            builder.HasKey(x => x.Id);

            builder.HasIndex(c => new { c.UserId, c.CompanyId })
                .HasDatabaseName("IX_CompanyUserRelations_User_Comp").IsUnique();

            builder.HasOne(c => c.Company)
                .WithMany(c => c.CompanyUserRelations)
                .HasForeignKey(c => c.CompanyId);

            builder.HasOne(c => c.User)
                .WithMany(u => u.CompanyUserRelations)
                .HasForeignKey(c => c.UserId);

        }
    }
}
