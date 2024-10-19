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
        private readonly DataContext _dataContext;

        public ContactController(
            IBaseRepository<Contact> contactRepository,
            IContactService contactService,
            ICustomMap customMap,
            DataContext dataContext) : base(contactRepository, customMap)
        {
            _contactService = contactService;
            _dataContext = dataContext;
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search( [FromQuery] List<Guid> tags, [FromQuery] string? firstName = null, [FromQuery] string? lastName = null)
        {
            return Ok(await _contactService.GetAll(tags,firstName,lastName));
        }

       

        [HttpPut("{id}")]
        public override async Task<IActionResult> Update(Guid id, [FromBody] ContactDetailDto record)
        {

            var result = await _contactService.Update(id, record);
            return result == null ? NotFound() : Ok(record);
        }

        public override async Task<IActionResult> Delete(Guid id)
        {
            var result = await _dataContext.Database.ExecuteSqlRawAsync("dbo.DeleteContact @Id",
                new SqlParameter("@Id", id));

            return Ok(result);
        }

        [HttpGet("di-test")]
        public void TestDi()
        {

        }

    }
}
