using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using SaveChangesEventHandlers.Core.Abstraction;
using SaveChangesEventHandlers.Core.Abstraction.Entities;
using System.Transactions;
using SaveChangesEventHandlers.Core.Extensions;

namespace SaveChangesEventHandlers.Core.Implemention
{
    public class SaveChangesEventsDispatcher : ISaveChangesEventsDispatcher
    {
        private Dictionary<EntityState, List<EntityEntry>> entitesForAfterProcessing { get; set; } = new()
        {
            {EntityState.Added, new() },
            {EntityState.Modified, new() },
            {EntityState.Deleted, new() }
        };

        private Dictionary<object, EntityState> processedEntities { get; set; } = [];

        private readonly SaveChangesEventsProvider saveChangesEventsProvider;

        public SaveChangesEventsDispatcher(SaveChangesEventsProvider saveChangesEventsProvider)
        {
            this.saveChangesEventsProvider = saveChangesEventsProvider;
        }

        public int SaveChangesWithEventsDispatcher(SaveChangesEventDbContext dbContext, Func<int> saveChanges)
        {
            using(var scope = new TransactionScope())
            {
                //dbContext.ChangeTracker.DetectChanges();

                var shouldTriggerSaveChangesAgain = false;
                var shouldTriggerBeforeDispatcherAgain = false;
                var savedEntites = 0;
                var entities = new List<EntityEntry>();

                do
                {
                    shouldTriggerBeforeDispatcherAgain = false;
                    shouldTriggerSaveChangesAgain = false;

                    do
                    {
                        entities = dbContext.ChangeTracker.Entries()
                            .FilterUnprocessedEntries(this.processedEntities)
                            .SetEmptyEntityEntryCollectionProperties()
                            .ToList();

                        foreach (var item in entities)
                        {
                            var aas = dbContext.EntitesForUpdate.TryGetValue(item.Entity, out object originalVallue);

                            var eee = originalVallue;

                        }

                        dbContext.ChangeTracker.DetectChanges();

                        shouldTriggerBeforeDispatcherAgain = false;

                        if (entities.Any())
                        {
                            shouldTriggerBeforeDispatcherAgain = true;

                            DispatchBefore(entities, dbContext);
                        }

                        ProcessEntriesForAfterActions(entities);

                    } while (shouldTriggerBeforeDispatcherAgain);

                    savedEntites += saveChanges.Invoke();

                    DispatchAfter(this.entitesForAfterProcessing, dbContext);

                    ClearEntitesForAfterActions();

                    shouldTriggerSaveChangesAgain = dbContext.ChangeTracker.Entries().FilterUnprocessedEntries(this.processedEntities).Any();

                } while (shouldTriggerSaveChangesAgain);

                scope.Complete();

                return savedEntites;
            }
        }

        public void DispatchBefore(List<EntityEntry> entites, SaveChangesEventDbContext dbContext)
        {
            if (entites.Any(e => e.State == EntityState.Added))
            {
                InvokeNewActionForEntites(entites.Where(e => e.State == EntityState.Added), nameof(ISaveChangesHandler<IEntity>.BeforeNewPersisted));
            }

            if (entites.Any(e => e.State == EntityState.Modified))
            {
                InvokeUpdateActionForEntites(entites.Where(e => e.State == EntityState.Modified), nameof(ISaveChangesHandler<IEntity>.BeforeUpdate), dbContext);
            }

            if (entites.Any(e => e.State == EntityState.Deleted))
            {
                InvokeDeleteActionForEntites(entites.Where(e => e.State == EntityState.Deleted), nameof(ISaveChangesHandler<IEntity>.BeforeDelete));
            }

            entites.ToList().ForEach(e => this.processedEntities.Add(e.Entity, e.State));
        }

        private void ProcessEntriesForAfterActions(List<EntityEntry> entities)
        {
            var processedEntitiesTempForAfter = new EntityEntry[entities.Count];

            entities.ToList().CopyTo(processedEntitiesTempForAfter);

            entities.GroupBy(e => e.State).ToList().ForEach(es =>
            {
                switch (es.Key)
                {
                    case EntityState.Deleted:
                    case EntityState.Modified:
                    case EntityState.Added:
                        this.entitesForAfterProcessing[es.Key].AddRange(es);
                        break;
                    default:
                        break;
                }
            });
        }

        public void DispatchAfter(Dictionary<EntityState, List<EntityEntry>> entitesPerState,SaveChangesEventDbContext dbContext)
        {
            if (entitesPerState[EntityState.Added].Any())
            {
                InvokeNewActionForEntites(entitesPerState[EntityState.Added], nameof(ISaveChangesHandler<IEntity>.AfterNewPersisted));
            }

            if (entitesPerState[EntityState.Modified].Any())
            {
                InvokeUpdateActionForEntites(entitesPerState[EntityState.Modified], nameof(ISaveChangesHandler<IEntity>.AfterUpdate), dbContext);
            }

            if (entitesPerState[EntityState.Deleted].Any())
            {
                InvokeDeleteActionForEntites(entitesPerState[EntityState.Deleted], nameof(ISaveChangesHandler<IEntity>.AfterDelete));
            }
        }

        private void ClearEntitesForAfterActions()
        {
            foreach (var kvp in this.entitesForAfterProcessing)
            {
                kvp.Value.Clear();
            }
        }

        private void InvokeNewActionForEntites(IEnumerable<EntityEntry> entities, string methodName)
        {
            foreach (var item in entities.GroupBy(e => e.Metadata.ClrType))
            {
                var handler = this.saveChangesEventsProvider.GetServiceHandlerForType(item.Key);

                if (handler == null)
                {
                    continue;
                }

                foreach (var entity in item)
                {
                    object[] arguments = [entity.Entity];

                    var method = handler.GetType().GetMethod(methodName);
                    
                    method?.Invoke(handler, arguments);
                }
            }
        }

        private void InvokeDeleteActionForEntites(IEnumerable<EntityEntry> entities, string methodName)
        {
            foreach (var item in entities.GroupBy(e => e.Metadata.ClrType))
            {
                var handler = this.saveChangesEventsProvider.GetServiceHandlerForType(item.Key);

                if (handler == null)
                {
                    continue;
                }

                foreach (var entity in item)
                {
                    var newValues = entity.Entity;

                    if (newValues is ISoftDeletableEntity)
                    {
                        ISoftDeletableEntity a = newValues as ISoftDeletableEntity;
                        a.IsSoftDeleted = true;
                        entity.CurrentValues.SetValues(a);
                        entity.State = EntityState.Modified;
                    }

                    object[] arguments = [newValues];

                    var method = handler.GetType().GetMethod(methodName);
                    method?.Invoke(handler, arguments);

                    entity.CurrentValues.SetValues(arguments[0]);
                }
            }
        }

        private void InvokeUpdateActionForEntites(IEnumerable<EntityEntry> entities, string methodName, SaveChangesEventDbContext dbContext)
        {
            foreach (var item in entities.GroupBy(e => e.Metadata.ClrType))
            {
                var handler = this.saveChangesEventsProvider.GetServiceHandlerForType(item.Key);

                if (handler == null)
                {
                    continue;
                }

                foreach (var entity in item)
                {
                    var oldValuesFull = entity.GetOriginalObject(dbContext);

                    object[] arguments = [oldValuesFull, entity.Entity];

                    var method = handler.GetType().GetMethod(methodName);
                    method?.Invoke(handler, arguments);

                    entity.CurrentValues.SetValues(arguments[1]);
                }
            }
        }
    }
}
