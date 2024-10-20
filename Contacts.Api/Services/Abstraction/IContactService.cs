using SaveChangesEventHandlers.Example.Dtos;
using SaveChangesEventHandlers.Example.Models;

namespace SaveChangesEventHandlers.Example.Services.Abstraction
{
    public interface IContactService
    {
        List<ContactDto>? GetAll(List<Guid> tags, string? firstName = null, string? lastName = null);
        ContactDetailDto? GetById(Guid contactId);
        ContactDetailDto? Update(Guid contactId, ContactDetailDto record);
    }
}
