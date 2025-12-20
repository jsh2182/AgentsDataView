using AgentsDataView.Server.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AgentsDataView.Entities
{
    public class InvoiceDetailConfiguration : IEntityTypeConfiguration<InvoiceDetail>
    {
        public void Configure(EntityTypeBuilder<InvoiceDetail> builder)
        {
            builder.ToTable("InvoiceDetails", "dbo");
            builder.HasKey(d => d.Id);
            builder.Property(d => d.Id).ValueGeneratedOnAdd();
            builder.Property(d => d.DetailDescription).HasMaxLength(300);
            builder.Property(d => d.Quantity).HasPrecision(14, 2);
            builder.Property(d=>d.UnitPrice).HasPrecision(18, 4);
            builder.Property(d => d.TotalPrice).HasPrecision(22,4).HasComputedColumnSql("[Quantity] * [UnitPrice]", true);
            builder.Property(d => d.TaxPercent).HasPrecision(5, 2);
            builder.Property(d => d.TaxValue).HasPrecision(18, 4);
            builder.Property(d => d.DiscountPercent).HasPrecision(5, 2);
            builder.Property(d=>d.DiscountValue).HasPrecision(18, 4);
            builder.Property(d => d.NetPrice).HasPrecision(22, 4).HasComputedColumnSql("[Quantity] * [UnitPrice] - [DiscountValue] + [TaxValue]");

            builder.HasOne(d => d.Invoice)
                .WithMany(i => i.InvoiceDetails)
                .HasForeignKey(d => d.InvoiceId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
            builder.HasOne(d=>d.Product)
                .WithMany(p=>p.InvoiceDetails)
                .HasForeignKey(d=>d.ProductId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();
        }
    }
}
