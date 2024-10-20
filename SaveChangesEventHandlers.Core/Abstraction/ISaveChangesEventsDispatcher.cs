using Microsoft.EntityFrameworkCore;

namespace SaveChangesEventHandlers.Core.Abstraction
{
    public interface ISaveChangesEventsDispatcher
    {
        void DispatchBefore();
        void DispatchAfter();
        void ProccessEntitesForBeforeActions(DbContext dbContext);
        void ProccessEntitesForAfterActions(DbContext dbContext);
        int SaveChangesWithEventsDispatcher(DbContext dbContext, Func<int> saveChanges);
    }
}
