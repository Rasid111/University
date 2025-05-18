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
    public class TeacherProfileController(
        TeacherProfileRepository teachertProfileRepository,
        UserManager<User> userManager,
        FacultyRepository facultyRepository,
        DegreeRepository degreeRepository) : ControllerBase
    {
        readonly TeacherProfileRepository _teacherProfileRepository = teachertProfileRepository;
        readonly UserManager<User> _userManager = userManager;
        readonly FacultyRepository _facultyRepository = facultyRepository;
        readonly DegreeRepository _degreeRepository = degreeRepository;

        [HttpGet]
        [ProducesResponseType(200)]
        public async Task<IActionResult> Get()
        {
            return Ok(await _teacherProfileRepository.Get());
        }
        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Get(int id)
        {
            var subject = await _teacherProfileRepository.Get(id);
            return Ok(subject);
        }
        [HttpPost]
        [ProducesResponseType(203)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Create(TeacherProfileCreateDto dto)
        {
            var user = await _userManager.FindByIdAsync(dto.UserId.ToString());
            var faculty = await _facultyRepository.Get(dto.DegreeId);
            var degree= await _degreeRepository.Get(dto.FacultyId);

            var tp = new TeacherProfile()
            {
                UserId = dto.UserId,
                User = user ?? throw new ArgumentException(nameof(dto.UserId)),
                Degree = degree ?? throw new ArgumentException(nameof(dto.UserId)),
                Faculty = faculty ?? throw new ArgumentException(nameof(dto.FacultyId))
            };

            var id = await _teacherProfileRepository.Create(tp);
            user.TeacherProfile = tp;
            user.TeacherProfileId = id;
            await _userManager.AddToRoleAsync(user, "Teacher");
            await _userManager.UpdateAsync(user);
            return NoContent();
        }
        [HttpPut]
        [ProducesResponseType(203)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult Update(TeacherProfile teacherProfile)
        {
            _teacherProfileRepository.Update(teacherProfile);
            return NoContent();
        }
        [HttpDelete]
        [ProducesResponseType(203)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(int id)
        {
            await _teacherProfileRepository.Delete(id);
            return NoContent();
        }
    }
}
