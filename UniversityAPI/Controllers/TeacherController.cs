
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniversityAPI.Database;
using UniversityAPI.Dtos;
using UniversityAPI.Models;

namespace UniversityAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,Teacher")]
    public class TeacherProfilesController : ControllerBase
    {
        private readonly UniversityDbContext _context;

        public TeacherProfilesController(UniversityDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<TeacherProfileDto>>> GetAll()
        {
            var teachers = await _context.TeacherProfiles
                .Include(t => t.User)
                .Include(t => t.Degree)
                .Include(t => t.Faculty)
                .Include(t => t.Subjects)
                .Include(t => t.Groups)
                .ToListAsync();

            var dtoList = teachers.Select(t => new TeacherProfileDto
            {
                Id = t.Id,
                UserName = t.User.UserName,
                FullName = t.User.Name + " " + t.User.Surname,
                DegreeId = t.DegreeId,
                DegreeName = t.Degree.Name,
                FacultyId = t.FacultyId,
                FacultyName = t.Faculty.Name,
                SubjectNames = t.Subjects.Select(s => s.Name).ToList(),
                GroupNames = t.Groups.Select(g => g.Name).ToList()
            }).ToList();

            return Ok(dtoList);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TeacherProfileDto>> GetById(int id)
        {
            var teacher = await _context.TeacherProfiles
                .Include(t => t.User)
                .Include(t => t.Degree)
                .Include(t => t.Faculty)
                .Include(t => t.Subjects)
                .Include(t => t.Groups)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (teacher == null) return NotFound();

            return Ok(new TeacherProfileDto
            {
                Id = teacher.Id,
                UserName = teacher.User.UserName,
                FullName = teacher.User.Name + " " + teacher.User.Surname,
                DegreeId = teacher.DegreeId,
                DegreeName = teacher.Degree.Name,
                FacultyId = teacher.FacultyId,
                FacultyName = teacher.Faculty.Name,
                SubjectNames = teacher.Subjects.Select(s => s.Name).ToList(),
                GroupNames = teacher.Groups.Select(g => g.Name).ToList()
            });
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<TeacherProfileDto>> Create(CreateTeacherProfileDto dto)
        {
            var user = new User
            {
                UserName = dto.UserName,
                Name = dto.FullName.Split(' ').FirstOrDefault() ?? "N/A",
                Surname = dto.FullName.Split(' ').Skip(1).FirstOrDefault() ?? "N/A",
                Email = $"{dto.UserName}@example.com",
                EmailConfirmed = true
            };

            var profile = new TeacherProfile
            {
                User = user,
                DegreeId = dto.DegreeId,
                FacultyId = dto.FacultyId,
                Degree = null!,
                Faculty = null!
            };

            _context.TeacherProfiles.Add(profile);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = profile.Id }, new TeacherProfileDto
            {
                Id = profile.Id,
                UserName = user.UserName,
                FullName = user.Name + " " + user.Surname,
                DegreeId = profile.DegreeId,
                DegreeName = "",
                FacultyId = profile.FacultyId,
                FacultyName = "",
                SubjectNames = new(),
                GroupNames = new()
            });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, UpdateTeacherProfileDto dto)
        {
            if (id != dto.Id) return BadRequest("ID mismatch");

            var profile = await _context.TeacherProfiles.FindAsync(id);
            if (profile == null) return NotFound();

            profile.DegreeId = dto.DegreeId;
            profile.FacultyId = dto.FacultyId;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var profile = await _context.TeacherProfiles.FindAsync(id);
            if (profile == null) return NotFound();

            _context.TeacherProfiles.Remove(profile);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
