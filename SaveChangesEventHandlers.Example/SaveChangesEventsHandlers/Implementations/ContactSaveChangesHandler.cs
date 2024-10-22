using SaveChangesEventHandlers.Example.Models;
using SaveChangesEventHandlers.Example.Repositories.Abstraction;
using SaveChangesEventHandlers.Example.Services.Abstraction;
using SaveChangesEventHandlers.Core.Abstraction;

namespace SaveChangesEventHandlers.Example.SaveChangesEventsHandlers.Implementations
{
    public class ContactSaveChangesHandler : ISaveChangesHandler<Contact>
    {
        private readonly ILogger<ContactSaveChangesHandler> logger;
        private readonly IBaseRepository<Contact> contactRepository;
        private readonly IBaseRepository<Email> emailRepository;

        private readonly IContactService contactService;

        public ContactSaveChangesHandler(
            ILogger<ContactSaveChangesHandler> logger, 
            IBaseRepository<Contact> contactRepository,
            IBaseRepository<Email> emailRepository,
            IContactService contactService)
        {
            this.logger = logger;
            this.contactRepository = contactRepository;
            this.emailRepository = emailRepository;
            this.contactService = contactService;
        }
        public void AfterNewPersisted(Contact entity)
        {
            var email = new Email()
            {
                Value = "alooo@bremacinieee.com",
                ContactId = entity.Id,
            };
            this.emailRepository.Add(email);
            this.logger.LogInformation("after contact is persisted");
        }

        public void BeforeNewPersisted(Contact entity)
        {
            if (entity.Address.Contains("Sucidar"))
                throw new InvalidDataException("Address cna not be sucidar");
            entity.FirstName = "alo bree";

            var email = new Email() 
            {
                Value = "alooo@breeee.com",
                ContactId = entity.Id,
            };
            this.emailRepository.Add(email);    
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

        public void BeforeDelete(Contact entity)
        {
            this.logger.LogInformation("before contact is deleted".ToUpper());
        }

        public void AfterDelete(Contact entity)
        {
            this.logger.LogInformation("after contact is deleted".ToUpper());
        }
    }
}
