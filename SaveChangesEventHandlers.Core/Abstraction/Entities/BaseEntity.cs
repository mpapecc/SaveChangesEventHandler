namespace SaveChangesEventHandlers.Core.Abstraction.Entities
{
    public abstract class BaseEntity : IEntity
    {
        public int Id { get; set; }
    }
}
