using Microsoft.EntityFrameworkCore;
using SaveChangesEventHandlers.Core.Implementation.Interceptors;

namespace SaveChangesEventHandlers.Core.Abstraction
{
    public abstract class SaveChangesEventDbContext : DbContext
    {
        private readonly ISaveChangesEventsDispatcher saveChangesEventsDispatcher;
        public Dictionary<object, object> EntitesForUpdate { get; set; } = new Dictionary<object, object>();

        protected SaveChangesEventDbContext(DbContextOptions options, ISaveChangesEventsDispatcher saveChangesEventsDispatcher) : base(options)
        {
            this.saveChangesEventsDispatcher = saveChangesEventsDispatcher;
        }

        public virtual int SaveChangesWithEventHandlers()
        {
            return this.saveChangesEventsDispatcher.SaveChangesWithEventsDispatcher(this, base.SaveChanges);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.AddInterceptors(new EntitesForUpdateInterceptor(this));
        }
    }
}
