using SaveChangesEventHandlers.Example.Dtos;
using SaveChangesEventHandlers.Example.Models;
using SaveChangesEventHandlers.Example.Repositories.Abstraction;
using SaveChangesEventHandlers.Example.Services.Abstraction;
using SaveChangesEventHandlers.Example.Utils.Mappings;
using SaveChangesEventHandlers.Example.Utils.Mappings.Abstraction;
using Microsoft.AspNetCore.Mvc;

namespace SaveChangesEventHandlers.Example.Controllers
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
