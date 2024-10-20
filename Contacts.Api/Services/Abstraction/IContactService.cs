using Contacts.Api.Dtos;
using Contacts.Api.Models;

namespace Contacts.Api.Services.Abstraction
{
    public interface IContactService
    {
        List<ContactDto>? GetAll(List<Guid> tags, string? firstName = null, string? lastName = null);
        ContactDetailDto? GetById(Guid contactId);
        ContactDetailDto? Update(Guid contactId, ContactDetailDto record);
    }
}
