using Microsoft.EntityFrameworkCore;
using SaveChangesEventHandlers.Core.Implementation.Interceptors;

namespace SaveChangesEventHandlers.Core.Abstraction
{
    public abstract class SaveChangesEventDbContext : DbContext
    {
        private readonly ISaveChangesEventsDispatcher saveChangesEventsDispatcher;
        public Dictionary<object, object> EntitesForUpdate { get; set; } = new Dictionary<object, object>();

        protected SaveChangesEventDbContext(
            DbContextOptions options,
            ISaveChangesEventsDispatcher saveChangesEventsDispatcher) : base(options)
        {
            this.saveChangesEventsDispatcher = saveChangesEventsDispatcher;
        }

        protected int SaveChangesWithEventHandlers(SaveChangesEventDbContext context)
        {
            return this.saveChangesEventsDispatcher.SaveChangesWithEventsDispatcher(context, base.SaveChanges);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.AddInterceptors(new EntitesForUpdateInterceptor(this, saveChangesEventsDispatcher.SuportedTypes));
        }
    }
}
