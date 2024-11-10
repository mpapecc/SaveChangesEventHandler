using Microsoft.Extensions.DependencyInjection;
using SaveChangesEventHandlers.Core.Abstraction;
using SaveChangesEventHandlers.Core.Implemention;
using System.Reflection;

namespace SaveChangesEventHandlers.Core
{
    public static class RegisterSaveChangesInfrastructure
    {
        public static IServiceCollection AddSaveChangesInfrastructure(this IServiceCollection services)
        {
            RegisterAllSaveChangesHandler(services);

            services.AddScoped<SaveChangesEventsProvider>();
            services.AddScoped<ISaveChangesEventsDispatcher, SaveChangesEventsDispatcher>();
            return services;
        }

        private static void RegisterAllSaveChangesHandler(IServiceCollection services) 
        {
            AppDomain.CurrentDomain.GetAssemblies().ToList()
            .ForEach(assembly =>
            {
                assembly.GetTypes().Where(type => typeof(ISaveChangesHandlerKey).IsAssignableFrom(type) && !type.IsInterface)
                                    .ToList()
                                    .ForEach(type => services.AddScoped(typeof(ISaveChangesHandlerKey), type));
            });
        }
    }
}
