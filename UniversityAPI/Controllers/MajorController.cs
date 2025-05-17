using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversityAPI.Models;
using UniversityAPI.Repositories;
using UniversityApplication.Dtos;

namespace UniversityAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    [ProducesResponseType(401)]
    [ProducesResponseType(500)]
    public class MajorController(MajorRepository repository) : ControllerBase
    {
        readonly MajorRepository _repository = repository;
        [HttpGet]
        [ProducesResponseType(200)]
        public async Task<IActionResult> Get()
        {
            var majors = await _repository.Get();
            return Ok(majors);
        }
        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Get(int id)
        {
            return Ok(await _repository.Get(id));
        }
        [HttpPost]
        [ProducesResponseType(203)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Create(DegreeCreateDto dto)
        {
            await _repository.Create(new Major() { Name = dto.Name });
            return NoContent();
        }
        [HttpPut]
        [ProducesResponseType(203)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult Update(Major major)
        {
            _repository.Update(major);
            return NoContent();
        }
        [HttpDelete]
        [ProducesResponseType(203)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(int id)
        {
            await _repository.Delete(id);
            return NoContent();
        }
    }
}
