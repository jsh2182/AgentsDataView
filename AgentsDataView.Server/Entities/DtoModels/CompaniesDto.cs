using AgentsDataView.Common.Utilities;
using System.ComponentModel.DataAnnotations;

namespace AgentsDataView.Entities.DtoModels
{
    public class CompaniesDto
    {
        public int? Id { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "نام شرکت الزامی است")]
        [MaxLength(200, ErrorMessage = "نام شرکت حداکثر می تواند 200 کاراکتر داشته باشد")]
        public string Name { get; set; } = "";

        [Required(AllowEmptyStrings = false, ErrorMessage = "کد شرکت الزامی است.")]
        [MaxLength(100, ErrorMessage = "کدشرکت حداکثر می تواند 100 کاراکتر داشته باشد")]
        public string Code { get; set; } = "";
        public int? ProvinceId { get; set; }
        public int? CityId { get; set; }

        [MaxLength(100, ErrorMessage = "کدپستی حداکثر می تواند 100 کاراکتر داشته باشد")]
        public string PostalCode { get; set; } = "";

        [MaxLength(100, ErrorMessage = "نشانی حداکثر می تواند 1000 کاراکتر داشته باشد")]
        public string Address { get; set; } = "";

        [MaxLength(100, ErrorMessage = "شماره ثبت حداکثر می تواند 100 کاراکتر داشته باشد")]
        public string RegistrationNumber { get; set; } = "";

        [MaxLength(100, ErrorMessage = "شماره تلفن حداکثر می تواند 100 کاراکتر داشته باشد")]
        public string PhoneNumber { get; set; } = "";
        public string? CityName { get; set; }
        public string? ProvinceName { get; set; }
        public DateTimeOffset? MaxInvoiceCreationDate { get; set; }
        public DateTimeOffset? MaxInvoiceDate { get; set; }
        public string PMaxInvoiceCreationDate => MaxInvoiceCreationDate.HasValue ? MaxInvoiceCreationDate.Value.ToLocalTime().Date.ToPersian():"";
        public string PMaxInvoiceDate => MaxInvoiceDate.HasValue ? MaxInvoiceDate.Value.ToLocalTime().Date.ToPersian() : "";
    }
}
