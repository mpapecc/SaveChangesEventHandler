using Microsoft.EntityFrameworkCore;
using SaveChangesEventHandlers.Core.Abstraction;
using SaveChangesEventsHandler.Test.TestData.Entites;
using SaveChangesEventsHandler.Test.TestData.EntityConfigurations;

namespace SaveChangesEventsHandler.Test.TestData
{
    public class TestDbContext: SaveChangesEventDbContext
    {
        public TestDbContext(DbContextOptions<TestDbContext> options, ISaveChangesEventsDispatcher saveChangesEventsDispatcher) : base(options, saveChangesEventsDispatcher)
        {
        }

        public DbSet<TestModel> TestModels { get; set; }
        public DbSet<TestModelNavigation> TestModelNavigations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new TestModelEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new TestModelNavigationEntityTypeConfiguration());
            base.OnModelCreating(modelBuilder);
        }
    }
}
