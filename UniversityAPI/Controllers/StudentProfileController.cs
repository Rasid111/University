using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UniversityAPI.Models;
using UniversityAPI.Repositories;
using UniversityApplication.Dtos;

namespace UniversityAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [ProducesResponseType(401)]
    [ProducesResponseType(500)]
    public class StudentProfileController(StudentProfileRepository studentProfileRepository, UserManager<User> userManager, GroupRepository groupRepository) : ControllerBase
    {
        readonly StudentProfileRepository _studentProfileRepository = studentProfileRepository;
        readonly UserManager<User> _userManager = userManager;
        readonly GroupRepository _groupRepository = groupRepository;

        [HttpGet]
        [ProducesResponseType(200)]
        public async Task<IActionResult> Get()
        {
            return Ok(await _studentProfileRepository.Get());
        }
        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Get(int id)
        {
            var subject = await _studentProfileRepository.Get(id);
            return Ok(subject);
        }
        [HttpPost]
        [ProducesResponseType(203)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Create(StudentProfileCreateDto dto)
        {
            var user = await _userManager.FindByIdAsync(dto.UserId.ToString());
            var group = await _groupRepository.Get(dto.GroupId);
            await _studentProfileRepository.Create(new StudentProfile() {
                User = user ?? throw new ArgumentException(nameof(dto.UserId)),
                Group = group ?? throw new ArgumentException(nameof(dto.UserId)) });
            return NoContent();
        }
        [HttpPut]
        [ProducesResponseType(203)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult Update(StudentProfile studentProfile)
        {
            _studentProfileRepository.Update(studentProfile);
            return NoContent();
        }
        [HttpDelete]
        [ProducesResponseType(203)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(int id)
        {
            await _studentProfileRepository.Delete(id);
            return NoContent();
        }
    }
}
