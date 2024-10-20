using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using SaveChangesEventHandlers.Core.Abstraction;
using System.Reflection;
using System.Transactions;

namespace SaveChangesEventHandlers.Core.Implemention
{
    public class SaveChangesEventsDispatcher : ISaveChangesEventsDispatcher
    {
        private readonly SaveChangesEventsProvider saveChangesEventsProvider;
        public Dictionary<Type, ISaveChangesHandlerKey> saveChangesEvents = new();

        private IEnumerable<EntityEntry> forUpdate { get; set; } = new List<EntityEntry>();
        private IEnumerable<EntityEntry> forDelete { get; set; } = new List<EntityEntry>() ;
        private IEnumerable<EntityEntry> forAdd { get; set; } = new List<EntityEntry>() ;
        private IEnumerable<EntityEntry> passToAfterAdd { get; set; } = new List<EntityEntry>();
        private IEnumerable<EntityEntry> passToAfterUpdate { get; set; } = new List<EntityEntry>();
        private IEnumerable<EntityEntry> passToAfterDelete { get; set; } = new List<EntityEntry>();

        public SaveChangesEventsDispatcher(SaveChangesEventsProvider saveChangesEventsProvider)
        {
            this.saveChangesEventsProvider = saveChangesEventsProvider;
        }

        public int SaveChangesWithEventsDispatcher(DbContext dbContext, Func<int> saveChanges)
        {
            using(var scope = new TransactionScope())
            {
                ProccessEntitesForBeforeActions(dbContext);

                //TODO: suport save changes iterations
                DispatchBefore();

                ProccessEntitesForAfterActions(dbContext);

                var savedEntites = saveChanges.Invoke();

                DispatchAfter();
                scope.Complete();
                return savedEntites;
            }
        }

        public void DispatchAfter()
        {

            InvokeNewActionForEntites(this.passToAfterAdd, nameof(ISaveChangesHandler<IEntity>.AfterNewPersisted));

            InvokeUpdateActionForEntites(this.passToAfterUpdate, nameof(ISaveChangesHandler<IEntity>.AfterUpdate));

            InvokeNewActionForEntites(this.passToAfterDelete, nameof(ISaveChangesHandler<IEntity>.AfterDelete));
        }

        public void DispatchBefore()
        {
            InvokeNewActionForEntites(this.forAdd, nameof(ISaveChangesHandler<IEntity>.BeforeNewPersisted));

            InvokeUpdateActionForEntites(this.forUpdate, nameof(ISaveChangesHandler<IEntity>.BeforeUpdate));

            InvokeNewActionForEntites(this.forDelete, nameof(ISaveChangesHandler<IEntity>.BeforeDelete));
        }

        public void ProccessEntitesForBeforeActions(DbContext dbContext)
        {
            this.forUpdate = dbContext.ChangeTracker.Entries().Where(e => e.State == EntityState.Modified);
            this.forDelete = dbContext.ChangeTracker.Entries().Where(e => e.State == EntityState.Deleted);
            this.forAdd = dbContext.ChangeTracker.Entries().Where(e => e.State == EntityState.Added);
        }

        public void ProccessEntitesForAfterActions(DbContext dbContext)
        {
            var passToAfterAddTempArray = new EntityEntry[this.forAdd.Count()];
            var passToAfterUpdateTempArray = new EntityEntry[this.forUpdate.Count()];
            var passToAfterDeleteTempArray = new EntityEntry[this.forDelete.Count()];

            this.forAdd.ToList().CopyTo(passToAfterAddTempArray);
            this.forUpdate.ToList().CopyTo(passToAfterUpdateTempArray);
            this.forDelete.ToList().CopyTo(passToAfterDeleteTempArray);

            var passToAfterAddTempList = passToAfterAddTempArray.ToList();
            var passToAfterUpdateTempList = passToAfterUpdateTempArray.ToList();
            var passToAfterDeleteTempList = passToAfterUpdateTempArray.ToList();

            this.passToAfterAdd = passToAfterAddTempList.Where(entity => entity != null);
            this.passToAfterUpdate = passToAfterUpdateTempList.Where(entity => entity != null);
            this.passToAfterDelete = passToAfterDeleteTempList.Where(entity => entity != null);
        }

        public void InvokeNewActionForEntites(IEnumerable<EntityEntry> entities, string methodName)
        {

            if (entities != null)
            {
                foreach (var item in entities.GroupBy(e => e.Metadata.ClrType))
                {
                    var handler = this.saveChangesEventsProvider.GetServiceHandlerForType(item.Key);

                    if (handler != null)
                    {
                        foreach (var entity in item)
                        {
                            var newValues = entity.CurrentValues.ToObject();

                            object[] arguments = new object[] { newValues };

                            var method = handler.GetType().GetMethod(methodName);
                            method?.Invoke(handler, arguments);

                            entity.CurrentValues.SetValues(arguments[0]);
                        }
                    }
                }
            }
        }

        public void InvokeUpdateActionForEntites(IEnumerable<EntityEntry> entities, string methodName)
        {
            if (entities != null)
            {
                foreach (var item in entities.GroupBy(e => e.Metadata.ClrType))
                {
                    var handler = this.saveChangesEventsProvider.GetServiceHandlerForType(item.Key);

                    if (handler != null)
                    {
                        foreach (var entity in item)
                        {
                            var newValues = entity.CurrentValues.ToObject();
                            var oldValues = entity.GetDatabaseValues().ToObject(); // find way not to check on db

                            object[] arguments = new object[] { oldValues, newValues };

                            var method = handler.GetType().GetMethod(methodName);
                            method?.Invoke(handler, arguments);

                            entity.CurrentValues.SetValues(arguments[1]);
                        }
                    }
                }
            }
        }
    }
}
