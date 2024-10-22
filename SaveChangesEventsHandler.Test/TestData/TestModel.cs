using SaveChangesEventHandlers.Core.Abstraction;

namespace SaveChangesEventsHandler.Test.TestData
{
    public class TestModel : IEntity
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public ICollection<TestModelNavigation> TestModelNavigations { get; set; }
    }


    public class TestModelNavigation : IEntity
    {
        public int Id { get; set; }
        public string LastName { get; set; }
        public TestModel TestModel { get; set; }
        public int TestModelId { get; set; }
    }


}
