using SaveChangesEventHandlers.Core.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaveChangesEventsHandler.Test.TestData
{
    public class TestModelSaveChangesHandler : ISaveChangesHandler<TestModel>
    {
        public void AfterNewPersisted(TestModel entity)
        {
        }

        public void AfterUpdate(TestModel oldEntity, TestModel newEntity)
        {
        }

        public void BeforeNewPersisted(TestModel entity)
        {
            entity.FirstName = nameof(ISaveChangesHandler<IEntity>.BeforeNewPersisted);
        }

        public void BeforeUpdate(TestModel oldEntity, TestModel newEntity)
        {
            newEntity.FirstName = nameof(ISaveChangesHandler<IEntity>.BeforeUpdate);
        }

        public Type HandlerForType() => typeof(TestModel);
    }
}
