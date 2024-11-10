using Microsoft.EntityFrameworkCore.Diagnostics;
using SaveChangesEventHandlers.Core.Abstraction.Entities;
using SaveChangesEventHandlers.Core.Abstraction;
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
                string entityAsString = JsonSerializer.Serialize(instance, instance.GetType());
                var copyOfEntity = JsonSerializer.Deserialize(entityAsString, instance.GetType());
                Context.EntitesForUpdate.Add(instance, copyOfEntity);
            }

            return instance;
        }
    }
}
