using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
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
    public class TeacherGroupSubjectController(
        TeacherGroupSubjectRepository teacherGroupSubjectRepository,
        GroupRepository groupRepository,
        TeacherProfileRepository teacherProfileRepository,
        SubjectRepository subjectRepository) : ControllerBase
    {
        readonly TeacherGroupSubjectRepository _teacherGroupSubjectRepository = teacherGroupSubjectRepository;
        readonly GroupRepository _groupRepository = groupRepository;
        readonly TeacherProfileRepository _teacherProfileRepository = teacherProfileRepository;
        readonly SubjectRepository _subjectRepository = subjectRepository;
        [HttpGet]
        [ProducesResponseType(200)]
        public async Task<IActionResult> Get()
        {
            var subjects = await _teacherGroupSubjectRepository.Get();
            return Ok(subjects);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetScheduleByGroupId(int id)
        {
            var scheduleElements = await _teacherGroupSubjectRepository.GetScheduleByGroupId(id);
            if (scheduleElements == null || !scheduleElements.Any())
                return NotFound("No schedule found for this group.");

            return Ok(scheduleElements);
        }
        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetScheduleByTeacherId(int id)
        {
            var scheduleElements = await _teacherGroupSubjectRepository.GetScheduleByTeacherId(id);
            if (scheduleElements == null || !scheduleElements.Any())
                return NotFound("No schedule found for this group.");

            return Ok(scheduleElements);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Get(int id)
        {
            return Ok(await _teacherGroupSubjectRepository.Get(id));
        }
        [HttpPost]
        [ProducesResponseType(203)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Create(TeacherGroupSubjectCreateDto dto)
        {
            var group = await _groupRepository.Get(dto.GroupId);
            var teacherProfile = await _teacherProfileRepository.Get(dto.TeacherProfileId);
            var subject = await _subjectRepository.Get(dto.SubjectId);
            await _teacherGroupSubjectRepository.Create(new TeacherGroupSubject()
            {
                Classroom = dto.Classroom,
                Group = group ?? throw new ArgumentException(nameof(dto.GroupId)),
                TeacherProfile = teacherProfile ?? throw new ArgumentException(nameof(dto.TeacherProfileId)),
                Subject = subject ?? throw new ArgumentException(nameof(dto.SubjectId))
            });
            return NoContent();
        }
        [HttpPut]
        [ProducesResponseType(203)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult Update(TeacherGroupSubject teacherGroupSubject)
        {
            _teacherGroupSubjectRepository.Update(teacherGroupSubject);
            return NoContent();
        }
        [HttpDelete]
        [ProducesResponseType(203)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(int id)
        {
            await _teacherGroupSubjectRepository.Delete(id);
            return NoContent();
        }

        [HttpGet("{teacherId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetGroupsByTeacher(int teacherId)
        {
            var groups = await _teacherGroupSubjectRepository.GetGroupsByTeacherId(teacherId);
            if (groups == null || !groups.Any())
                return NotFound("No groups found for this teacher.");

            return Ok(groups);
        }

    }
}
