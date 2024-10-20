using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SaveChangesEventHandlers.Core.Abstraction;
using SaveChangesEventsHandler.Test.TestData;
using System.Data.Common;

namespace SaveChangesEventsHandler.Test.Helpers
{
    public class TestDbContextFactory: IDisposable
    {
        private DbConnection _connection;
        private readonly ISaveChangesEventsDispatcher saveChangesEventsDispatcher;

        public TestDbContextFactory(ISaveChangesEventsDispatcher saveChangesEventsDispatcher)
        {
            this.saveChangesEventsDispatcher = saveChangesEventsDispatcher;
        }
        private DbContextOptions<TestDbContext> CreateOptions()
        {
            return new DbContextOptionsBuilder<TestDbContext>()
                .UseSqlite(_connection)
                .ConfigureWarnings(x => x.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.AmbientTransactionWarning))
                .Options;
        }

        public TestDbContext CreateContext()
        {
            if (_connection == null)
            {
                _connection = new SqliteConnection("DataSource=:memory:");
                _connection.Open();

                var options = CreateOptions();
                using (var context = new TestDbContext(options, this.saveChangesEventsDispatcher))
                {
                    context.Database.EnsureCreated();
                }
            }

            return new TestDbContext(CreateOptions(), this.saveChangesEventsDispatcher);
        }

        public void Dispose()
        {
            if (_connection != null)
            {
                _connection.Dispose();
                _connection = null;
            }
        }
    }
}
