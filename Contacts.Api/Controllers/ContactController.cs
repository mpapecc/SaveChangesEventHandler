using Contacts.Api.Dtos;
using Contacts.Api.Models;
using Contacts.Api.Repositories.Abstraction;
using Contacts.Api.Services.Abstraction;
using Contacts.Api.Utils.Mappings;
using Contacts.Api.Utils.Mappings.Abstraction;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Contacts.Api.Controllers
{

    public class ContactController :BaseController<Contact,ContactDto,ContactDetailDto>
    {

        private readonly IContactService _contactService;
        private readonly IBaseRepository<Contact> contactRepository;
        private readonly DataContext _dataContext;

        public ContactController(
            IBaseRepository<Contact> contactRepository,
            IContactService contactService,
            ICustomMap customMap,
            DataContext dataContext) : base(contactRepository, customMap)
        {
            _contactService = contactService;
            this.contactRepository = contactRepository;
            _dataContext = dataContext;
        }

        [HttpGet("search")]
        public IActionResult Search( [FromQuery] List<Guid> tags, [FromQuery] string? firstName = null, [FromQuery] string? lastName = null)
        {
            return Ok(_contactService.GetAll(tags,firstName,lastName));
        }

       

        [HttpPut("{id}")]
        public override IActionResult Update(Guid id, [FromBody] ContactDetailDto record)
        {

            _contactService.Update(id, record);
            return  Ok(record);
        }

        [HttpPut("UpdateHandler/{id}")]
        public IActionResult UpdateHandler(Guid id, string name)
        {

            var contact = this.contactRepository.GetById(id).First();

            contact.FirstName = name;

            this.contactRepository.Update(contact);

            return Ok(contact);
        }

    }
}
