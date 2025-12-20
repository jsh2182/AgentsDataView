namespace AgentsDataView.Entities
{
    public class Role:BaseEntity<int>
    {
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public virtual ICollection<UserRole>? UserRoles { get; set; }
    }
}
