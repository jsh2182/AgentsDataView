using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AgentsDataView.Entities
{
    public class ProvinceConfiguration : IEntityTypeConfiguration<Province>
    {
        public void Configure(EntityTypeBuilder<Province> builder)
        {
            builder.ToTable("Provinces", "dbo");
            builder.HasKey(p => p.Id);
            //builder.Property(p => p.Id).ValueGeneratedOnAdd();
            builder.Property(p => p.ProvinceName).HasMaxLength(100);
            
            builder.HasData(
                new Province()
            {
                Id=1,
                Code = 1,
                ProvinceName = "آذربایجان شرقی",
            },
                new Province()
                {
                    Id = 2,
                    Code = 2,
                    ProvinceName = "آذربایجان غربی",
                },
                new Province()
                {
                    Id = 3,
                    Code = 3,
                    ProvinceName = "اردبیل",
                },
                new Province()
                {
                    Id = 4,
                    Code = 4,
                    ProvinceName = "اصفهان",
                },
                new Province()
                {
                    Id = 5,
                    Code = 5,
                    ProvinceName = "ایلام",
                },
                new Province()
                {
                    Id = 6,
                    Code = 6,
                    ProvinceName = "بوشهر",
                },
                new Province()
                {
                    Id = 7,
                    Code = 7,
                    ProvinceName = "تهران",
                },
                new Province()
                {
                    Id = 8,
                    Code = 8,
                    ProvinceName = "چهارمحال و بختیاری",
                },
                new Province()
                {
                    Id = 9,
                    Code = 9,
                    ProvinceName = "خراسان رضوی",
                },
                new Province()
                {
                    Id = 10,
                    Code = 10,
                    ProvinceName = "خراسان شمالی",
                },
                new Province()
                {
                    Id = 11,
                    Code = 11,
                    ProvinceName = "خراسان جنوبی",
                },
                new Province()
                {
                    Id = 12,
                    Code = 12,
                    ProvinceName = "خوزستان",
                },
                new Province()
                {
                    Id = 13,
                    Code = 13,
                    ProvinceName = "زنجان",
                },
                new Province()
                {
                    Id = 14,
                    Code = 14,
                    ProvinceName = "سمنان",
                },
                new Province()
                {
                    Id = 15,
                    Code = 15,
                    ProvinceName = "سیستان و بلوچستان",
                },
                new Province()
                {
                    Id = 16,
                    Code = 16,
                    ProvinceName = "فارس",
                },
                new Province()
                {
                    Id = 17,
                    Code = 17,
                    ProvinceName = "قزوین",
                },
                new Province()
                {
                    Id = 18,
                    Code = 18,
                    ProvinceName = "قم",
                },
                new Province()
                {
                    Id = 19,
                    Code = 19,
                    ProvinceName = "کردستان",
                },
                new Province()
                {
                    Id = 20,
                    Code = 20,
                    ProvinceName = "کرمان",
                },
                new Province()
                {
                    Id = 21,
                    Code = 21,
                    ProvinceName = "کرمانشاه",
                },
                new Province()
                {
                    Id = 22,
                    Code = 22,
                    ProvinceName = "کهگیلویه و بویراحمد",
                },
                new Province()
                {
                    Id = 23,
                    Code = 23,
                    ProvinceName = "گلستان",
                },
                new Province()
                {
                    Id = 24,
                    Code = 24,
                    ProvinceName = "گیلان",
                },
                new Province()
                {
                    Id = 25,
                    Code = 25,
                    ProvinceName = "لرستان",
                },
                new Province()
                {
                    Id = 26,
                    Code = 26,
                    ProvinceName = "مازندران",
                },
                new Province()
                {
                    Id = 27,
                    Code = 27,
                    ProvinceName = "مرکزی",
                },
                new Province()
                {
                    Id = 28,
                    Code = 28,
                    ProvinceName = "هرمزگان",
                },
                new Province()
                {
                    Id = 29,
                    Code = 29,
                    ProvinceName = "همدان",
                },
                new Province()
                {
                    Id = 30,
                    Code = 30,
                    ProvinceName = "یزد",
                },
                new Province()
                {
                    Id = 31,
                    Code = 31,
                    ProvinceName = "البرز",
                });
        }
    }
}
