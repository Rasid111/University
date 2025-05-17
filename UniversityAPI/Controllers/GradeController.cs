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
    public class GradeController(GradeRepository repository) : ControllerBase
    {
        readonly GradeRepository _repository = repository;

        [HttpGet]
        [ProducesResponseType(200)]
        public async Task<IActionResult> Get()
        {
            var grades = await _repository.Get();
            return Ok(grades);
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
        public async Task<IActionResult> Create(GradeCreateDto dto)
        {
            var grade = new Grade
            {
                Value = dto.Value,
                Message = dto.Message,
                SubjectId = dto.SubjectId,
                StudentProfileId = dto.StudentProfileId,
                TeacherProfileId = dto.TeacherProfileId,
                Date = dto.Date ?? DateTime.Now
            };

            await _repository.Create(grade);
            return NoContent();
        }

        [HttpPut]
        [ProducesResponseType(203)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult Update(Grade grade)
        {
            _repository.Update(grade);
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
