using Contacts.Api.Repositories.Abstraction;
using Contacts.Api.Models;
using SaveChangesEventHandlers.Core.Abstraction;

namespace Contacts.Api.SaveChangesEventsHandlers.Implementations
{
    public class EmailSaveChangesHandler : ISaveChangesHandler<Email>
    {
        private readonly ILogger<Email> logger;

        public EmailSaveChangesHandler(ILogger<Email> logger)
        {
            this.logger = logger;
        }

        public void AfterNewPersisted(Email entity)
        {
            this.logger.LogInformation("after email is persisted");
        }

        public void AfterUpdate(Email oldEntity, Email newEntity)
        {
            this.logger.LogInformation("after email is persisted");
        }

        public void BeforeNewPersisted(Email entity)
        {
            this.logger.LogInformation("after email is persisted");
        }

        public void BeforeUpdate(Email oldEntity, Email newEntity)
        {
            this.logger.LogInformation("after email is persisted");
        }

        public Type HandlerForType() => typeof(Email);
    }
}
