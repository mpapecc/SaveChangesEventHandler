using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace SaveChangesEventHandlers.Core.Abstraction
{
    public interface ISaveChangesEventsDispatcher
    {
        void DispatchBefore(List<EntityEntry> entites, SaveChangesEventDbContext dbContext);
        void DispatchAfter(Dictionary<EntityState, List<EntityEntry>> entitiesPerState , SaveChangesEventDbContext dbContext);
        int SaveChangesWithEventsDispatcher(SaveChangesEventDbContext dbContext, Func<int> saveChanges);
    }
}
