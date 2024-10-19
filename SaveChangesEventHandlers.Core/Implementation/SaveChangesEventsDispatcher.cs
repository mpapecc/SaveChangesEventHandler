using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using SaveChangesEventHandlers.Core.Abstraction;
using System.Reflection;

namespace SaveChangesEventHandlers.Core.Implemention
{
    public class SaveChangesEventsDispatcher : ISaveChangesEventsDispatcher
    {
        private readonly SaveChangesEventsProvider saveChangesEventsProvider;
        public Dictionary<Type, ISaveChangesHandlerKey> saveChangesEvents = new();

        private EntityEntry[] forUpdate { get; set; } = new EntityEntry[1];
        private EntityEntry[] forDelete { get; set; } = new EntityEntry[1] ;
        private EntityEntry[] forAdd { get; set; } = new EntityEntry[1] ;

        public SaveChangesEventsDispatcher(SaveChangesEventsProvider saveChangesEventsProvider)
        {
            this.saveChangesEventsProvider = saveChangesEventsProvider;
        }

        public void DispatchAfter()
        {

            InvokeNewActionForEntites(this.forAdd.ToList(), nameof(ISaveChangesHandler<IEntity>.AfterNewPersisted));

            InvokeUpdateActionForEntites(this.forUpdate.ToList(), nameof(ISaveChangesHandler<IEntity>.AfterUpdate));
        }

        public void DispatchBefore()
        {
            InvokeNewActionForEntites(this.forAdd.ToList(), nameof(ISaveChangesHandler<IEntity>.BeforeNewPersisted));

            InvokeUpdateActionForEntites(this.forUpdate.ToList(), nameof(ISaveChangesHandler<IEntity>.BeforeUpdate));
        }

        public void GetChangeEntites(DbContext dbContext)
        {
            dbContext.ChangeTracker.Entries().Where(e => e.State == EntityState.Modified).ToList().CopyTo(this.forUpdate);
            dbContext.ChangeTracker.Entries().Where(e => e.State == EntityState.Deleted).ToList().CopyTo(this.forDelete);
            dbContext.ChangeTracker.Entries().Where(e => e.State == EntityState.Added).ToList().CopyTo(this.forAdd);
        }

        public void InvokeNewActionForEntites(IEnumerable<EntityEntry> entities, string methodName)
        {
            if (entities.First() != null)
            {
                foreach (var item in entities.GroupBy(e => e.Metadata.ClrType))
                {
                    var handler = this.saveChangesEventsProvider.GetServiceHandlerForType(item.Key);

                    if (handler != null)
                    {
                        foreach (var entity in item)
                        {
                            var underlyingObject = entity.CurrentValues.ToObject();

                            var method = handler.GetType().GetMethod(methodName);
                            method?.Invoke(handler, new object[] { underlyingObject });
                        }
                    }
                }
            }
        }

        public void InvokeUpdateActionForEntites(IEnumerable<EntityEntry> entities, string methodName)
        {
            if (entities.First() != null)
            {
                foreach (var item in entities.GroupBy(e => e.Metadata.ClrType))
                {
                    var handler = this.saveChangesEventsProvider.GetServiceHandlerForType(item.Key);

                    if (handler != null)
                    {
                        foreach (var entity in item)
                        {

                            var newValues = entity.CurrentValues.ToObject();
                            var oldValues = entity.GetDatabaseValues().ToObject();

                            CheckValues(entity.CurrentValues, entity.OriginalValues);

                            var method = handler.GetType().GetMethod(methodName);
                            method?.Invoke(handler, new object[] { oldValues, newValues });
                        }
                    }
                }
            }
        }

        private void CheckValues(PropertyValues current, PropertyValues newValues)
        {

        }


        public async Task<int> SaveChangesWithEventsDispatcher(DbContext dbContext, Func<CancellationToken, Task<int>> saveChanges, CancellationToken cancellationToken)
        {
            GetChangeEntites(dbContext);
            DispatchBefore();
            var savedEntites = await saveChanges.Invoke(cancellationToken);
            DispatchAfter();
            return savedEntites;
        }
    }
}
