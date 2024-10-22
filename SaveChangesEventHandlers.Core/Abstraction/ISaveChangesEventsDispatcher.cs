using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace SaveChangesEventHandlers.Core.Abstraction
{
    public interface ISaveChangesEventsDispatcher
    {
        void DispatchBefore(List<EntityEntry> entites);
        void DispatchAfter(Dictionary<EntityState, List<EntityEntry>> entitiesPerState );
        int SaveChangesWithEventsDispatcher(DbContext dbContext, Func<int> saveChanges);
    }
}
