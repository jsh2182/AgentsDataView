
namespace AgentsDataView.Entities
{
    public class CompanyUserRelation : BaseEntity<long>
    {
        public int UserId { get; set; }
        public int CompanyId { get; set; }

        public virtual SystemUser? User {get; set;}
        public virtual Company? Company { get; set; }
    }
}
