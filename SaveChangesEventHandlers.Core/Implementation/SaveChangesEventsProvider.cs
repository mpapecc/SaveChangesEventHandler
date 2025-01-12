using Microsoft.Extensions.DependencyInjection;
using SaveChangesEventHandlers.Core.Abstraction;

namespace SaveChangesEventHandlers.Core.Implemention
{
    public class SaveChangesEventsProvider
    {
        private readonly IServiceProvider serviceProvider;

        public SaveChangesEventsProvider(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        //TODO : get only needed, not all and them filter for one
        public object GetServiceHandlerForType(Type handlerForType)
        {
            Type specificInterfaceType = typeof(ISaveChangesHandler<>).MakeGenericType(handlerForType);

            return this.serviceProvider.GetService(specificInterfaceType);
        }
    }
}
