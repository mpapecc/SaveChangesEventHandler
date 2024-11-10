using Microsoft.EntityFrameworkCore.Diagnostics;
using SaveChangesEventHandlers.Core.Abstraction.Entities;
using SaveChangesEventHandlers.Core.Abstraction;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Text.Json;

namespace SaveChangesEventHandlers.Core.Implementation.Interceptors
{
    public class EntitesForUpdateInterceptor: IMaterializationInterceptor
    {
        public EntitesForUpdateInterceptor(SaveChangesEventDbContext context)
        {
            Context = context;
        }

        public SaveChangesEventDbContext Context { get; }

        public object InitializedInstance(MaterializationInterceptionData materializationData, object instance)
        {
            if (instance is IEntity someEntity)
            {
                string jsonString = JsonSerializer.Serialize(instance, instance.GetType());
                var aaa = JsonSerializer.Deserialize(jsonString, instance.GetType());
                Context.EntitesForUpdate.Add(instance,aaa);
            }

            return instance;
        }

        private object CreateOriginalObjectWithAllProperties(object entity)
        {
            var castedEntityEntry = entity as EntityEntry;

            var originalValues = new Dictionary<string, object?>();

            foreach (var item in castedEntityEntry.Collections)
            {
                originalValues.Add(item.Metadata.Name, item.CurrentValue);
            }

            foreach (var item in castedEntityEntry.References)
            {
                originalValues.Add(item.Metadata.Name, item.CurrentValue);
            }

            foreach (var item in castedEntityEntry.Properties)
            {
                originalValues.Add(item.Metadata.Name, item.OriginalValue);
            }

            var originalObject = Activator.CreateInstance(castedEntityEntry.Metadata.ClrType);

            foreach (var item in originalValues)
            {
                originalObject.GetType().GetProperty(item.Key).SetValue(originalObject, item.Value, null);
            }

            return originalObject;
        }
    }
}
