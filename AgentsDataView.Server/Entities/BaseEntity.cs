namespace AgentsDataView.Entities
{
    public interface IEntity
    {

    }
    public abstract class BaseEntity<TKey>:IEntity
    {
        public required TKey Id { get; set; }
    }
}
