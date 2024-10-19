using Contacts.Api.Dtos;
using Contacts.Api.Models;
using Contacts.Api.Repositories.Abstraction;
using Contacts.Api.Utils.Mappings;
using Contacts.Api.Utils.Mappings.Abstraction;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Contacts.Api.Controllers
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
        public virtual async Task<IActionResult> GetAll()
        {
            var result = await _baseRepository.GetAll().ToListAsync();
            return Ok(_customMap.MapList<TDto,T>(result));
        }

        [HttpGet("{id}")]
        public virtual async Task<IActionResult> Find(Guid id)
        {
            var record =  await _baseRepository.GetById(id).FirstOrDefaultAsync();
            if (record == null)
                return NotFound();

            return Ok(_customMap.Map<TDetails, T>(record));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TDetails record)
        {
            if (await _baseRepository.AddAsync(_customMap.Map<T, TDetails>(record)))
                return Ok(record); 
            return BadRequest();
            
        }

        [HttpPut("{id}")]
        public virtual async Task<IActionResult> Update(Guid id, [FromBody] TDetails record)
        {
            var data = _customMap.Map<T,TDetails>(record);

            if (id != data.Id)
                return BadRequest();

            
            if (await _baseRepository.UpdateAsync(data))
                return Ok(record);
            return BadRequest();
            
        }

        [HttpDelete("{id}")]
        public virtual async Task<IActionResult> Delete(Guid id)
        {
            
            if (await _baseRepository.DeleteAsync(id))
                return NoContent();
            return BadRequest();
            
        }
    }
}
