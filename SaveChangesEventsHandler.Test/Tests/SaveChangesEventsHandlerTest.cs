using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SaveChangesEventHandlers.Core;
using SaveChangesEventHandlers.Core.Abstraction;
using SaveChangesEventsHandler.Test.Helpers;
using SaveChangesEventsHandler.Test.TestData;

namespace SaveChangesEventsHandler.Test.Tests
{
    [TestClass]
    public class SaveChangesEventsHandlerTest
    {

        [TestMethod]
        public async Task test_BeforeNewPersisted()
        {
            var serviceCollection = new ServiceCollection();
            var services = serviceCollection.AddSaveChangesInfrastructure();
            var serviceProvider = serviceCollection.BuildServiceProvider();

            using (var factory = new TestDbContextFactory(serviceProvider.GetRequiredService<ISaveChangesEventsDispatcher>()))
            {
                using (var context = factory.CreateContext())
                {
                    var testModel = new TestModel() { FirstName = nameof(test_BeforeNewPersisted) };
                    context.TestModels.Add(testModel);
                    context.SaveChanges();
                }

                using (var context = factory.CreateContext())
                {
                    var count = await context.TestModels.CountAsync();
                    Assert.AreEqual(1, count);

                    var u = await context.TestModels.FirstOrDefaultAsync(testModel => testModel.FirstName == nameof(ISaveChangesHandler<IEntity>.BeforeNewPersisted));
                    Assert.IsNotNull(u);
                }
            }
        }

        [TestMethod]
        public async Task test_BeforeUpdate()
        {
            var serviceCollection = new ServiceCollection();
            var services = serviceCollection.AddSaveChangesInfrastructure();
            var serviceProvider = serviceCollection.BuildServiceProvider();

            using (var factory = new TestDbContextFactory(serviceProvider.GetRequiredService<ISaveChangesEventsDispatcher>()))
            {
                using (var context = factory.CreateContext())
                {
                    var testModel = new TestModel() { FirstName = nameof(test_BeforeUpdate) };
                    context.TestModels.Add(testModel);

                    context.SaveChanges();

                    var u = await context.TestModels.FirstOrDefaultAsync(testModel => testModel.FirstName == nameof(ISaveChangesHandler<IEntity>.BeforeNewPersisted));
                    u.FirstName = nameof(test_BeforeUpdate);
                    context.Update(u);

                    context.SaveChanges();
                }

                using (var context = factory.CreateContext())
                {
                    var count = await context.TestModels.CountAsync();
                    Assert.AreEqual(1, count);

                    var u = await context.TestModels.FirstOrDefaultAsync(testModel => testModel.FirstName == nameof(ISaveChangesHandler<IEntity>.BeforeUpdate));
                    Assert.IsNotNull(u);
                }
            }
        }

    }
}