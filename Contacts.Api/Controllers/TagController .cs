using Contacts.Api.Dtos;
using Contacts.Api.Models;
using Contacts.Api.Repositories.Abstraction;
using Contacts.Api.Services.Abstraction;
using Contacts.Api.Utils.Mappings;
using Contacts.Api.Utils.Mappings.Abstraction;
using Microsoft.AspNetCore.Mvc;

namespace Contacts.Api.Controllers
{

    public class TagController : BaseController<Tag,TagDto,TagDetailDto>
    {
        public TagController(
            IBaseRepository<Tag> tagRepository,
            ICustomMap customMap) : base(tagRepository, customMap)
        {
        }

 
    }
}
