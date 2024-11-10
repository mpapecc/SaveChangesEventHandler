using SaveChangesEventHandlers.Example.Dtos;
using SaveChangesEventHandlers.Example.Models;
using SaveChangesEventHandlers.Example.Repositories.Abstraction;
using SaveChangesEventHandlers.Example.Services.Abstraction;
using SaveChangesEventHandlers.Example.Utils.Mappings;
using SaveChangesEventHandlers.Example.Utils.Mappings.Abstraction;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SaveChangesEventHandlers.Example;

namespace SaveChangesEventHandlers.Example.Controllers
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
    }
}
