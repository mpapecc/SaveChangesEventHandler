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


        public SaveChangesEventsDispatcher(SaveChangesEventsProvider saveChangesEventsProvider)
        {
            this.saveChangesEventsProvider = saveChangesEventsProvider;
        }

        public async Task<int> SaveChangesWithEventsDispatcher(DbContext dbContext, Func<CancellationToken, Task<int>> saveChanges, CancellationToken cancellationToken)
        {
            ProccessEntitesForBeforeActions(dbContext);

            //TODO: suport save changes iterations
            DispatchBefore();

            ProccessEntitesForAfterActions(dbContext);

            var savedEntites = await saveChanges.Invoke(cancellationToken);

            DispatchAfter();

            return savedEntites;
        }

        public void DispatchAfter()
        {

            InvokeNewActionForEntites(this.passToAfterAdd, nameof(ISaveChangesHandler<IEntity>.AfterNewPersisted));

            InvokeUpdateActionForEntites(this.passToAfterUpdate, nameof(ISaveChangesHandler<IEntity>.AfterUpdate));
        }

        public void DispatchBefore()
        {
            InvokeNewActionForEntites(this.forAdd, nameof(ISaveChangesHandler<IEntity>.BeforeNewPersisted));

            InvokeUpdateActionForEntites(this.forUpdate, nameof(ISaveChangesHandler<IEntity>.BeforeUpdate));
        }

        public void ProccessEntitesForBeforeActions(DbContext dbContext)
        {
            this.forUpdate = dbContext.ChangeTracker.Entries().Where(e => e.State == EntityState.Modified);
            this.forDelete = dbContext.ChangeTracker.Entries().Where(e => e.State == EntityState.Deleted);
            this.forAdd = dbContext.ChangeTracker.Entries().Where(e => e.State == EntityState.Added);
        }

        public void ProccessEntitesForAfterActions(DbContext dbContext)
        {
            var passToAfterAddTempArray = new EntityEntry[1];
            var passToAfterUpdateTempArray = new EntityEntry[1];

            this.forAdd.ToList().CopyTo(passToAfterAddTempArray);
            this.forUpdate.ToList().CopyTo(passToAfterUpdateTempArray);

            var passToAfterAddTempList = passToAfterAddTempArray.ToList();
            var passToAfterUpdateTempList = passToAfterUpdateTempArray.ToList();

            this.passToAfterAdd = passToAfterAddTempList.Where(entity => entity != null);
            this.passToAfterUpdate = passToAfterUpdateTempList.Where(entity => entity != null);
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
