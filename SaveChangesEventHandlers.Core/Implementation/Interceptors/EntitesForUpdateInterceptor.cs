using Microsoft.EntityFrameworkCore.Diagnostics;
using SaveChangesEventHandlers.Core.Abstraction;
using System.Text.Json;

namespace SaveChangesEventHandlers.Core.Implementation.Interceptors
{
    public class EntitesForUpdateInterceptor : IMaterializationInterceptor
    {
        public readonly SaveChangesEventDbContext Context;
        public readonly IEnumerable<Type> SupportedTypes;

        public EntitesForUpdateInterceptor(
            SaveChangesEventDbContext context,
            IEnumerable<Type> suportedTypes)
        {
            Context = context;
            SupportedTypes = suportedTypes;
        }

        public object InitializedInstance(MaterializationInterceptionData materializationData, object instance)
        {
            if (SupportedTypes.Any(x => x.Equals(instance.GetType())))
            {
                string entityAsString = JsonSerializer.Serialize(instance, instance.GetType());
                var copyOfEntity = JsonSerializer.Deserialize(entityAsString, instance.GetType());
                Context.EntitesForUpdate.Add(instance, copyOfEntity);
            }

            return instance;
        }
    }
}
