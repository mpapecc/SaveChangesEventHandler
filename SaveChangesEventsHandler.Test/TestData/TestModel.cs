using SaveChangesEventHandlers.Core.Abstraction;

namespace SaveChangesEventsHandler.Test.TestData
{
    public class TestModel:IEntity
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
    }
}
