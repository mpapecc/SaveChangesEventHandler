using SaveChangesEventHandlers.Core.Abstraction;
using SaveChangesEventsHandler.Test.TestData.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaveChangesEventsHandler.Test.TestData
{
    internal class TestModelNavigationSaveChangesHandler : ISaveChangesHandler<TestModelNavigation>
    {
        public TestModelNavigationSaveChangesHandler()
        {
            
        }
        public void AfterDelete(TestModelNavigation entity)
        {
        }

        public void AfterNewPersisted(TestModelNavigation entity)
        {
        }

        public void AfterUpdate(TestModelNavigation oldEntity, TestModelNavigation newEntity)
        {
        }

        public void BeforeDelete(TestModelNavigation entity)
        {
        }

        public void BeforeNewPersisted(TestModelNavigation entity)
        {
            entity.LastName = "editeddd";
        }

        public void BeforeUpdate(TestModelNavigation oldEntity, TestModelNavigation newEntity)
        {
        }

        public Type HandlerForType() => typeof(TestModelNavigation);
    }
}
