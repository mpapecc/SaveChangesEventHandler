using SaveChangesEventHandlers.Core.Abstraction;
using SaveChangesEventHandlers.Core.Abstraction.Entities;
using SaveChangesEventsHandler.Test.TestData.Entites;

namespace SaveChangesEventsHandler.Test.TestData
{
    public class TestModelSaveChangesHandler : ISaveChangesHandler<TestModel>
    {

        public void AfterDelete(TestModel entity)
        {
        }

        public void AfterNewPersisted(TestModel entity)
        {
        }

        public void AfterUpdate(TestModel oldEntity, TestModel newEntity)
        {
        }

        public void BeforeDelete(TestModel entity)
        {
        }

        public void BeforeNewPersisted(TestModel entity)
        {
            entity.FirstName = nameof(ISaveChangesHandler<IEntity>.BeforeNewPersisted);
            entity.TestModelNavigations.Add(new TestModelNavigation() { LastName = "from handler" });
        }

        public void BeforeUpdate(TestModel oldEntity, TestModel newEntity)
        {
            newEntity.FirstName = nameof(ISaveChangesHandler<IEntity>.BeforeUpdate);
        }

        public Type HandlerForType() => typeof(TestModel);
    }
}
