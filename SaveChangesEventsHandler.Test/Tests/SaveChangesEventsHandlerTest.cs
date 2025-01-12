using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SaveChangesEventHandlers.Core;
using SaveChangesEventHandlers.Core.Abstraction;
using SaveChangesEventsHandler.Test.Helpers;
using SaveChangesEventsHandler.Test.TestData.Entites;

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
                    context.SaveChangesWithEventHandlers();
                }

                using (var context = factory.CreateContext())
                {
                    var count = await context.TestModels.CountAsync();
                    Assert.AreEqual(1, count);

                    var u = await context.TestModels.FirstOrDefaultAsync(testModel => testModel.FirstName == nameof(ISaveChangesHandler<object>.BeforeNewPersisted));
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

                    context.SaveChangesWithEventHandlers();
                }

                using (var context = factory.CreateContext())
                {
                    var u = await context.TestModels.FirstOrDefaultAsync(testModel => testModel.FirstName == nameof(ISaveChangesHandler<object>.BeforeNewPersisted));
                    u.FirstName = nameof(TestModel);
                    u.TestModelNavigations = new List<TestModelNavigation>()
                        {
                            new TestModelNavigation() { LastName = "Mile"}
                        };

                    context.TestModels.Update(u);

                    context.SaveChangesWithEventHandlers();
                }

                using (var context = factory.CreateContext())
                {
                    var count = await context.TestModels.CountAsync();
                    Assert.AreEqual(1, count);

                    var u = await context.TestModels.FirstOrDefaultAsync(testModel => testModel.FirstName == nameof(ISaveChangesHandler<object>.BeforeUpdate));
                    Assert.IsNotNull(u);
                }
            }
        }

        [TestMethod]
        public async Task test_BeforeNewPersistedWithNavigationCollection()
        {
            var serviceCollection = new ServiceCollection();
            var services = serviceCollection.AddSaveChangesInfrastructure();
            var serviceProvider = serviceCollection.BuildServiceProvider();

            using (var factory = new TestDbContextFactory(serviceProvider.GetRequiredService<ISaveChangesEventsDispatcher>()))
            {
                using (var context = factory.CreateContext())
                {
                    var testModel = new TestModel()
                    {
                        FirstName = nameof(test_BeforeNewPersisted),
                        TestModelNavigations = new List<TestModelNavigation>()
                        {
                            new TestModelNavigation() { LastName = "Mile"},
                            new TestModelNavigation() { LastName = "Milanezi"}
                        }
                    };

                    context.TestModels.Add(testModel);
                    context.SaveChangesWithEventHandlers();
                }

                using (var context = factory.CreateContext())
                {

                    var count = await context.TestModels.CountAsync();
                    Assert.AreEqual(1, count);

                    var u = await context.TestModels.FirstOrDefaultAsync(testModel => testModel.FirstName == nameof(ISaveChangesHandler<object>.BeforeNewPersisted));
                    Assert.IsNotNull(u);
                }
            }
        }
    }
}