namespace SaveChangesEventHandlers.Core.Abstraction
{
    public interface ISoftDeletableEntity : IEntity
    {
        bool IsSoftDeleted { get; set; }
    }
}
