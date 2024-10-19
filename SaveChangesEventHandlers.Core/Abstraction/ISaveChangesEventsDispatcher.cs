using Microsoft.EntityFrameworkCore;

namespace SaveChangesEventHandlers.Core.Abstraction
{
    public interface ISaveChangesEventsDispatcher
    {
        void DispatchBefore();
        void DispatchAfter();
        void GetChangeEntites(DbContext dbContext);
        Task<int> SaveChangesWithEventsDispatcher(DbContext dbContext, Func<CancellationToken, Task<int>> saveChanges, CancellationToken cancellationToken);
    }
}
