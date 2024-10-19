using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using SaveChangesEventHandlers.Core.Abstraction;
using SaveChangesEventHandlers.Core.Implemention;

namespace SaveChangesEventHandlers.Core
{
    public static class RegisterSaveChangesInfrastructure
    {
        public static IServiceCollection AddSaveChangesInfrastructure(this IServiceCollection services)
        {
            AppDomain.CurrentDomain.GetAssemblies().ToList()
            .ForEach(a => 
            {
                a.GetTypes().Where(t => typeof(ISaveChangesHandlerKey).IsAssignableFrom(t) && !t.IsInterface)
                .ToList()
                .ForEach(h => services.AddScoped(typeof(ISaveChangesHandlerKey), h));
            });

            services.AddScoped<SaveChangesEventsProvider>();
            services.AddScoped<ISaveChangesEventsDispatcher, SaveChangesEventsDispatcher>();
            return services;
        }
    }
}
