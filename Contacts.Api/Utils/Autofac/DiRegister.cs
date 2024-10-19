using Autofac;
using Contacts.Api.Models;
using Contacts.Api.Repositories.Abstraction;
using Contacts.Api.Repositories.Implementation;
using Contacts.Api.Services.Abstraction;
using Contacts.Api.Services.Implementation;
using Contacts.Api.Utils.Mappings.Abstraction;
using Contacts.Api.Utils.Mappings.Implementation;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SaveChangesEventHandlers.Core.Abstraction;
using SaveChangesEventHandlers.Core.Implemention;

namespace Contacts.Api.Utils.Autofac
{
    public class DiRegister:Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterGeneric(typeof(BaseRepository<>)).As(typeof(IBaseRepository<>)).InstancePerLifetimeScope();

            //RegisterSaveChangesEventsInfrastructure(builder);
            builder.RegisterType<ContactService>().As<IContactService>().AsImplementedInterfaces();
            builder.RegisterType<ContactValidator>().As<IValidator<Contact>>().SingleInstance();
            builder.RegisterType<CustomMap>().As<ICustomMap>().SingleInstance();

        }

        private static void RegisterSaveChangesEventsInfrastructure(ContainerBuilder builder)
        {
            System.Reflection.Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => typeof(ISaveChangesHandlerKey).IsAssignableFrom(t) && !t.IsInterface)
                .ToList()
                .ForEach(t => builder.RegisterType(t).As<ISaveChangesHandlerKey>().InstancePerLifetimeScope());

            builder.RegisterType<SaveChangesEventsProvider>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<SaveChangesEventsDispatcher>().As<ISaveChangesEventsDispatcher>().InstancePerLifetimeScope();
        }
    }
}
