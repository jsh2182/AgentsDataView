using AgentsDataView.Server.Entities;
using System.ComponentModel.DataAnnotations;

namespace AgentsDataView.Entities
{
    public class Product : BaseEntity<int>
    {
        public int CompanyId { get; set; }
        public int CreatedById { get; set; }
        public DateTimeOffset CreationDate { get; set; } = DateTimeOffset.UtcNow;
        public string Code { get; set; } = "";
       
        [Required(AllowEmptyStrings = false, ErrorMessage = "نام کالا الزامی است")]
        [MaxLength(250, ErrorMessage ="طول نام کالا نمی تواند بیش از 250 کاراکتر باشد.")]
        public string Name { get; set; } = "";

        [MaxLength(250, ErrorMessage = "طول مدل کالا نمی تواند بیش از 250 کاراکتر باشد.")]
        public string Model { get; set; } = "";

        public virtual Company? Company { get; set; }
        public virtual SystemUser? CreatedBy { get; set; }

        public virtual ICollection<InvoiceDetail>?  InvoiceDetails { get; set; }

    }
}
