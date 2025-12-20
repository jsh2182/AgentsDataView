using AgentsDataView.Entities;

namespace AgentsDataView.Entities
{
    public class UserRole : BaseEntity<int>
    {
        public int UserId { get; set; }
        public int RoleId { get; set; }

        public virtual SystemUser? SystemUser { get; set; }
        public virtual Role? Role { get; set; }
    }
}
