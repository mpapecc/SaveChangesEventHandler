using SaveChangesEventHandlers.Core.Abstraction.Entities;

namespace SaveChangesEventsHandler.Test.TestData.Entites
{
    public class TestModel : BaseEntity
    {
        public string FirstName { get; set; }
        public List<TestModelNavigation> TestModelNavigations { get; set; }
    }
}
