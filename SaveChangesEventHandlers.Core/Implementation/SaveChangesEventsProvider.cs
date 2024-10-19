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

        public Dictionary<Type, ISaveChangesHandlerKey> GetServiceHandlers()
        {
            return this.serviceProvider.GetServices<ISaveChangesHandlerKey>().ToDictionary(s => s.HandlerForType());
        }


        //TODO : get only needed, not all and them filter for one
        public ISaveChangesHandlerKey? GetServiceHandlerForType(Type handlerForType)
        {
            return this.serviceProvider.GetServices<ISaveChangesHandlerKey>().Where(s => s.HandlerForType() == handlerForType).FirstOrDefault();
        }
    }
}
