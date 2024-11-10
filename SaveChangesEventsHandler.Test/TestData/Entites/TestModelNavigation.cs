using SaveChangesEventHandlers.Core.Abstraction.Entities;

namespace SaveChangesEventsHandler.Test.TestData.Entites
{
    public class TestModelNavigation : BaseEntity
    {
        public string LastName { get; set; }
        public TestModel TestModel { get; set; }
        public int TestModelId { get; set; }
    }
}
