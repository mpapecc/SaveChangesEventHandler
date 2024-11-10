using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using SaveChangesEventHandlers.Core.Abstraction.Entities;
using SaveChangesEventHandlers.Core.Abstraction;

namespace SaveChangesEventHandlers.Core.Extensions
{
    public static class EntityEntryExtensions
    {
        public static IEnumerable<EntityEntry> FilterUnprocessedEntries(this IEnumerable<EntityEntry> entries, Dictionary<object, EntityState> processedEntities)
        {
            return entries.Where(e => e.State != EntityState.Detached && e.State != EntityState.Unchanged)
                          .Where(e => !processedEntities.ContainsKey(e.Entity));
        }

        public static object CreateOriginalObjectWithAllProperties(this EntityEntry entityEntry)
        {
            var originalValues = new Dictionary<string, object?>();

            foreach (var item in entityEntry.Collections)
            {
                originalValues.Add(item.Metadata.Name, item.CurrentValue);
            }

            foreach (var item in entityEntry.References)
            {
                originalValues.Add(item.Metadata.Name, item.CurrentValue);
            }

            foreach (var item in entityEntry.Properties)
            {
                originalValues.Add(item.Metadata.Name, item.OriginalValue);
            }

            var originalObject = Activator.CreateInstance(entityEntry.Metadata.ClrType);

            foreach (var item in originalValues)
            {
                originalObject.GetType().GetProperty(item.Key).SetValue(originalObject, item.Value, null);
            }

            return originalObject;
        }

        public static object GetOriginalObject(this EntityEntry entityEntry, SaveChangesEventDbContext dbContext)
        {
            return dbContext.EntitesForUpdate[entityEntry.Entity];
        }

        public static IEnumerable<EntityEntry> SetEmptyEntityEntryCollectionProperties(this IEnumerable<EntityEntry> entityEntries)
        {

            foreach(var entry in entityEntries)
            {
                foreach (var property in entry.Collections)
                {
                    //if collection property does not have any values it will be passed on as null
                    // and trying to add item in EventHandlers will throw error
                    if (property.CurrentValue is null)
                    {
                        property.CurrentValue = Activator.CreateInstance(property.Metadata.ClrType) as IEnumerable<BaseEntity>;
                    }
                }
            }

            return entityEntries;
        }
    }
}
