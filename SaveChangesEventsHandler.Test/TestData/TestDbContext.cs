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

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await this.saveChangesEventsDispatcher.SaveChangesWithEventsDispatcher(this, base.SaveChangesAsync, cancellationToken);
        }

        public DbSet<TestModel> TestModels { get; set; }
    }
}
