using Contacts.Api.Dtos;
using Contacts.Api.Models;

namespace Contacts.Api.Services.Abstraction
{
    public interface IContactService
    {
        Task<List<ContactDto>?> GetAll(List<Guid> tags, string? firstName = null, string? lastName = null);
        Task<ContactDetailDto?> GetById(Guid contactId);
        Task<ContactDetailDto?> Update(Guid contactId, ContactDetailDto record);
    }
}
