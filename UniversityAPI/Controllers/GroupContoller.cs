using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UniversityAPI.Models;
using UniversityAPI.Repositories;
using UniversityApplication.Dtos;

namespace UniversityAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class GroupContoller(GroupRepository groupRepository, FacultyRepository facultyRepository, MajorRepository majorRepository) : ControllerBase
    {
        readonly GroupRepository _groupRepository = groupRepository;
        readonly FacultyRepository _facultyRepository = facultyRepository;
        readonly MajorRepository _majorRepository = majorRepository;
        [HttpGet]
        [ProducesResponseType(200)]
        public async Task<IActionResult> Get()
        {
            var groups = await _groupRepository.Get();
            return Ok(groups);
        }
        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Get(int id)
        {
            return Ok(await _groupRepository.Get(id));
        }
        [HttpPost]
        [ProducesResponseType(203)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Create(GroupCreateDto dto)
        {
            var faculty = await _facultyRepository.Get(dto.FacultyId);
            var major = await _majorRepository.Get(dto.MajorId);
            await _groupRepository.Create(new Group() {
                Name = dto.Name,
                Faculty = faculty ?? throw new ArgumentException(nameof(dto.FacultyId)),
                Major = major ?? throw new ArgumentException(nameof(dto.MajorId))
            });
            return NoContent();
        }
        [HttpPut]
        [ProducesResponseType(203)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult Update(Group group)
        {
            _groupRepository.Update(group);
            return NoContent();
        }
        [HttpDelete]
        [ProducesResponseType(203)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(int id)
        {
            await _groupRepository.Delete(id);
            return NoContent();
        }
    }
}
