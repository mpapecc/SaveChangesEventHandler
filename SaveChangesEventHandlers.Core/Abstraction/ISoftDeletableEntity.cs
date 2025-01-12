namespace SaveChangesEventHandlers.Core.Abstraction
{
    public interface ISoftDeletableEntity
    {
        bool IsSoftDeleted { get; set; }
    }
}
