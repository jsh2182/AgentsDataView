using System.ComponentModel.DataAnnotations;

namespace AgentsDataView.Entities
{
    public class SystemUser : BaseEntity<int>
    {
        
        [Required(AllowEmptyStrings =false, ErrorMessage ="نام کاربری الزامی است")]
        public string UserName { get; set; } = "";
        public string? Password { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "نام کاربر الزامی است")]
        public string UserFullName { get; set; } = "";
        public string? UserMobile { get; set; }
        public long? RelatedPersonID { get; set; }
        public bool IsActive { get; set; } = true;
        public string? LoginInfo { get; set; }
        public long? OrganizationRoleID { get; set; }
        public DateTimeOffset? LastLoginDate { get; set; }
        /// <summary>
        /// برای کاربرانی که هنگام ساخت شرکت، ساخته می شوند. این کاربران در فهرست کاربران پنل نمایش داده نخواهند شد
        /// </summary>
        public bool IsApiUser { get; set; }

        public virtual Company? Company { get; set; }
        public virtual ICollection<Product>? Products { get; set; }
        public virtual ICollection<Invoice>? Invoices_CreatedBy { get; set; }
        public virtual ICollection<Invoice>? Invoices_UpdatedBy { get; set; }
        public virtual ICollection<RefreshToken>? RefreshTokens { get; set; }
        public virtual ICollection<UserRole>? UserRoles { get; set; }
        public virtual ICollection<CompanyUserRelation>? CompanyUserRelations { get; set; }

    }
}
