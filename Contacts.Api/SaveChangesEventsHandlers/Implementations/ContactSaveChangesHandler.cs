using Contacts.Api.Models;
using Contacts.Api.Repositories.Abstraction;
using Contacts.Api.Services.Abstraction;
using SaveChangesEventHandlers.Core.Abstraction;

namespace Contacts.Api.SaveChangesEventsHandlers.Implementations
{
    public class ContactSaveChangesHandler : ISaveChangesHandler<Contact>
    {
        private readonly ILogger<ContactSaveChangesHandler> logger;
        private readonly IBaseRepository<Contact> contactRepository;
        private readonly IContactService contactService;

        public ContactSaveChangesHandler(
            ILogger<ContactSaveChangesHandler> logger, 
            IBaseRepository<Contact> contactRepository,
            IContactService contactService)
        {
            this.logger = logger;
            this.contactRepository = contactRepository;
            this.contactService = contactService;
        }
        public void AfterNewPersisted(Contact entity)
        {
            this.logger.LogInformation("after contact is persisted");
        }

        public void BeforeNewPersisted(Contact entity)
        {
            if (entity.Address.Contains("Sucidar"))
                throw new InvalidDataException("Address cna not be sucidar");

            var a = this.contactRepository.GetAll();
            this.logger.LogInformation(a.First().FirstName);
            this.logger.LogInformation("before contact is persisted");
        }

        public void AfterUpdate(Contact oldEntity, Contact newEntity)
        {
            this.logger.LogInformation("after contact is updated");
        }

        public void BeforeUpdate(Contact oldEntity, Contact newEntity)
        {
            this.logger.LogInformation("before contact is updated");
        }

        public Type HandlerForType() => typeof(Contact);
    }
}
