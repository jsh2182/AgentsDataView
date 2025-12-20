using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AgentsDataView.Entities
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Products", "dbo");
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id).ValueGeneratedOnAdd();
            builder.Property(p => p.Name);
            builder.Property(p => p.Code).HasMaxLength(250);
            // چون جستجو بیشتر روی شرکت است باید شتاسه شرکت اول قرار بگیرد
            // این باعث می شود جستجوهایی که فقط شرکت را فیلتر می کنند از ایندکس استفاده کنند.
            // علاوه بر آن همانطور که مشخص است جستجوهایی که هردوی این ستونها را فیلتر می کنند نیز از ایندکس بهره می برند
            // اما جستجویی که فقط کد کالا را فیلتر می کند سودی از ایندکس نخواهد برد زیرا کد کالا اولین ستون ایندکس نیست.
            // البته در ایجا نیازی به فیلتر تنها روی کد کالا نیست.
            builder.HasIndex(p => new { p.CompanyId, p.Code }).HasDatabaseName("IX_Products_CompanyId_Code").IsUnique();
            builder.HasIndex(p => new { p.Code, p.Name }).HasDatabaseName("IX_Products_Code_Name");

            builder.HasOne(p => p.Company)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CompanyId)
                .IsRequired();
            builder.HasOne(p => p.CreatedBy)
                .WithMany(s => s.Products)
                .HasForeignKey(p => p.CreatedById)
                .IsRequired();
        }
    }
}
