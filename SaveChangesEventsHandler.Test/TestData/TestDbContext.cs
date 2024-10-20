using Microsoft.EntityFrameworkCore;
using SaveChangesEventHandlers.Core.Abstraction;

namespace SaveChangesEventsHandler.Test.TestData
{
    public class TestDbContext:DbContext
    {
        private readonly ISaveChangesEventsDispatcher saveChangesEventsDispatcher;

        public TestDbContext(DbContextOptions<TestDbContext> options, ISaveChangesEventsDispatcher saveChangesEventsDispatcher) : base(options)
        {
            this.saveChangesEventsDispatcher = saveChangesEventsDispatcher;
        }

        public override int SaveChanges()
        {
            return this.saveChangesEventsDispatcher.SaveChangesWithEventsDispatcher(this, base.SaveChanges);
        }

        public DbSet<TestModel> TestModels { get; set; }
    }
}
