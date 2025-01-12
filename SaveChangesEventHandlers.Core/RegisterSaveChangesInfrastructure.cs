using Microsoft.Extensions.DependencyInjection;
using SaveChangesEventHandlers.Core.Abstraction;
using SaveChangesEventHandlers.Core.Implemention;

namespace SaveChangesEventHandlers.Core
{
    public static class RegisterSaveChangesInfrastructure
    {
        public static IServiceCollection AddSaveChangesInfrastructure(this IServiceCollection services)
        {
            var supportedTypes = RegisterAllSaveChangesHandler(services);

            services.AddScoped<SaveChangesEventsProvider>();
            services.AddScoped<ISaveChangesEventsDispatcher>(x =>
                new SaveChangesEventsDispatcher(x.GetRequiredService<SaveChangesEventsProvider>(), supportedTypes)
            );
            return services;
        }

        private static IEnumerable<Type> RegisterAllSaveChangesHandler(IServiceCollection services)
        {
            var supportedTypes = new List<Type>();

            AppDomain.CurrentDomain.GetAssemblies().ToList()
            .ForEach(assembly =>
            {
                assembly.GetTypes().Where(type => typeof(ISaveChangesHandlerKey).IsAssignableFrom(type) && !type.IsInterface)
                                    .ToList()
                                    .ForEach(type =>
                                    {
                                        Type specificInterfaceType = typeof(ISaveChangesHandler<>).MakeGenericType(type.GetGenericType());
                                        services.AddScoped(specificInterfaceType, type);
                                        supportedTypes.Add(type.GetGenericType());
                                    });
            });

            return supportedTypes;
        }

        private static Type GetGenericType(this Type type)
        {
            var eventHandlerInterface = type.GetInterfaces().FirstOrDefault(i => i.GetGenericTypeDefinition() == typeof(ISaveChangesHandler<>));
            var eventHandlerForType = eventHandlerInterface.GenericTypeArguments;

            return eventHandlerForType.FirstOrDefault();
        }

        public static bool IsPoco(object obj)
        {
            if (obj == null) return false;

            Type type = obj.GetType();

            // Check if the type is a system type
            if (type.Namespace == null || type.Namespace.StartsWith("System"))
                return false;

            // Optionally exclude Microsoft or generated proxies (e.g., EF, ASP.NET Core)
            if (type.Namespace.StartsWith("Microsoft"))
                return false;

            // Check if it's a class (exclude structs, enums, etc.)
            if (!type.IsClass)
                return false;

            // It's a user-defined POCO if we reach here
            return true;
        }
    }
}

