using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AgentsDataView.Entities
{
    public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
    {
        public void Configure(EntityTypeBuilder<Invoice> builder)
        {
            builder.ToTable("Invoices", "dbo");
            builder.HasKey(i => i.Id);
            builder.Property(i => i.Id).ValueGeneratedOnAdd();
            builder.Property(i => i.InvoiceDescription).HasMaxLength(1000);
            builder.Property(i => i.ExtraCosts).HasPrecision(18, 4);
            builder.Property(i=>i.TaxValue).HasPrecision(18, 4);
            builder.Property(i=>i.DiscountValue).HasPrecision(18, 4);
            builder.Property(i=>i.InvoiceNetPrice).HasPrecision(24, 4);
            builder.HasIndex(i => new { i.CompanyId, i.FinancialPeriodId, i.InvoiceType, i.InvoiceNumber}).HasDatabaseName("IX_Invoice_Period_Comp_Type_Number").IsUnique();
            builder.HasIndex(i => i.InvoiceType).HasDatabaseName("IX_Invoice_Type");

            builder.HasOne(i => i.Company)
                .WithMany(c => c.Invoices)
                .HasForeignKey(i => i.CompanyId)
                .IsRequired();
            builder.HasOne(i => i.CreatedBy)
                .WithMany(c => c.Invoices_CreatedBy)
                .HasForeignKey(i => i.CreatedById)
                .IsRequired();
            builder.HasOne(i => i.UpdatedBy)
                .WithMany(u => u.Invoices_UpdatedBy)
                .HasForeignKey(i => i.UpdatedById);
            builder.HasOne(i => i.FinancialPeriod)
                .WithMany(f => f.Invoices)
                .HasForeignKey(i => i.FinancialPeriodId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();
        }
    }
}
