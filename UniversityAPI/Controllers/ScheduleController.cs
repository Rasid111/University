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
    public class ScheduleContoller(ScheduleRepository scheduleRepository, TeacherGroupSubjectRepository tgcRepository) : ControllerBase
    {
        readonly ScheduleRepository _scheduleRepository = scheduleRepository;
        TeacherGroupSubjectRepository _tgcRepository = tgcRepository;
        [HttpGet]
        [ProducesResponseType(200)]
        public async Task<IActionResult> Get()
        {
            return Ok(await _scheduleRepository.Get());
        }
        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Get(int id)
        {
            return Ok(await _scheduleRepository.Get(id));
        }
        [HttpPost]
        [ProducesResponseType(203)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Create(ScheduleElementCreateDto dto)
        {
            var tgc = await _tgcRepository.Get(dto.TeacherGroupSubjectId);
            await _scheduleRepository.Create(new ScheduleElement()
            {
                TeacherGroupSubject = tgc ?? throw new ArgumentException(nameof(dto.TeacherGroupSubjectId)),
                DayOfWeek = dto.DayOfWeek,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
            });
            return NoContent();
        }
        [HttpPut]
        [ProducesResponseType(203)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult Update(ScheduleElement scheduleElement)
        {
            _scheduleRepository.Update(scheduleElement);
            return NoContent();
        }
        [HttpDelete]
        [ProducesResponseType(203)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(int id)
        {
            await _scheduleRepository.Delete(id);
            return NoContent();
        }
    }
}
