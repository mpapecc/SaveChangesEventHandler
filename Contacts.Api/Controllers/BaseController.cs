using SaveChangesEventHandlers.Example.Dtos;
using SaveChangesEventHandlers.Example.Models;
using SaveChangesEventHandlers.Example.Repositories.Abstraction;
using SaveChangesEventHandlers.Example.Utils.Mappings;
using SaveChangesEventHandlers.Example.Utils.Mappings.Abstraction;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace SaveChangesEventHandlers.Example.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public abstract class BaseController<T,TDto,TDetails> : ControllerBase where T  : BaseEntity
    {
        private IBaseRepository<T> _baseRepository;
        public readonly ICustomMap _customMap;


        public BaseController(IBaseRepository<T> baseRepository, ICustomMap customMap)
        {
            _baseRepository = baseRepository;
            _customMap = customMap;
        }

        [HttpGet]
        public virtual ActionResult GetAll()
        {
            var result = _baseRepository.GetAll().ToList();
            return Ok(_customMap.MapList<TDto,T>(result));
        }

        [HttpGet("{id}")]
        public virtual IActionResult Find(Guid id)
        {
            var record =  _baseRepository.GetById(id).FirstOrDefault();
            if (record == null)
                return NotFound();

            return Ok(_customMap.Map<TDetails, T>(record));
        }

        [HttpPost]
        public IActionResult Create([FromBody] TDetails record)
        {
            _baseRepository.Add(_customMap.Map<T, TDetails>(record));
            _baseRepository.Commit();
            return Ok(record); 
        }

        [HttpPut("{id}")]
        public virtual IActionResult Update(Guid id, [FromBody] TDetails record)
        {
            var data = _customMap.Map<T,TDetails>(record);

            if (id != data.Id)
                return BadRequest();


            _baseRepository.Update(data);
            return Ok(data);
        }

        [HttpDelete("{id}")]
        public virtual IActionResult Delete(Guid id)
        {
            _baseRepository.Delete(id);
            return Ok();
        }
    }
}
