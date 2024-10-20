using Microsoft.Extensions.DependencyInjection;
using SaveChangesEventHandlers.Core;
using SaveChangesEventHandlers.Core.Abstraction;
using SaveChangesEventHandlers.Core.Implemention;
using SaveChangesEventsHandler.Test.TestData;

namespace SaveChangesEventsHandler.Test.Tests
{
    [TestClass]
    public class RegisterSaveChangesInfrastructureTests
    {
        public static ServiceProvider serviceProvider;

        [ClassInitialize]
        public static void ClassInitialize(TestContext _)
        {
            var serviceCollection = new ServiceCollection();
            var services = serviceCollection.AddSaveChangesInfrastructure();
            serviceProvider = serviceCollection.BuildServiceProvider();
        }

        [TestMethod]
        public void test_SaveChangesEventsProvider_is_registred()
        {
            var service = serviceProvider.GetService<SaveChangesEventsProvider>();
            Assert.IsNotNull(service);
        }

        [TestMethod]
        public void test_SaveChangesEventsDispatcher_is_registred()
        {
            var service = serviceProvider.GetService<ISaveChangesEventsDispatcher>();
            Assert.IsNotNull(service);
        }

        [TestMethod]
        public void test_TestModelSaveChangesHandler_is_registred()
        {
            var service = serviceProvider.GetServices<ISaveChangesHandlerKey>();
            Assert.IsTrue(service.First() is ISaveChangesHandler<TestModel>);
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            serviceProvider?.Dispose();
        }
    }
}