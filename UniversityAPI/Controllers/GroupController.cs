
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
    [Authorize(Roles = "Teacher,Admin")]
    public class GroupsController : ControllerBase
    {
        private readonly UniversityDbContext _context;

        public GroupsController(UniversityDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GroupDto>>> GetAll()
        {
            var groups = await _context.Groups
                .Include(g => g.Faculty)
                .Include(g => g.Major)
                .Include(g => g.Subjects)
                .Include(g => g.TeacherProfiles).ThenInclude(tp => tp.User)
                .Include(g => g.Students).ThenInclude(s => s.User)
                .ToListAsync();

            var dtoList = groups.Select(group => new GroupDto
            {
                Id = group.Id,
                Name = group.Name,
                Year = group.Year,
                FacultyId = group.FacultyId,
                FacultyName = group.Faculty.Name,
                MajorId = group.MajorId,
                MajorName = group.Major.Name,
                SubjectNames = group.Subjects.Select(s => s.Name).ToList(),
                TeacherNames = group.TeacherProfiles.Select(t => t.User.Name).ToList(),
                StudentNames = group.Students.Select(s => s.User.Name).ToList()
            }).ToList();

            return Ok(dtoList);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GroupDto>> GetById(int id)
        {
            var group = await _context.Groups
                .Include(g => g.Faculty)
                .Include(g => g.Major)
                .Include(g => g.Subjects)
                .Include(g => g.TeacherProfiles).ThenInclude(tp => tp.User)
                .Include(g => g.Students).ThenInclude(s => s.User)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (group == null) return NotFound();

            return Ok(new GroupDto
            {
                Id = group.Id,
                Name = group.Name,
                Year = group.Year,
                FacultyId = group.FacultyId,
                FacultyName = group.Faculty.Name,
                MajorId = group.MajorId,
                MajorName = group.Major.Name,
                SubjectNames = group.Subjects.Select(s => s.Name).ToList(),
                TeacherNames = group.TeacherProfiles.Select(t => t.User.Name).ToList(),
                StudentNames = group.Students.Select(s => s.User.Name).ToList()
            });
        }

        [HttpPost]
        public async Task<ActionResult<GroupDto>> Create(CreateGroupDto dto)
        {
            var group = new Group
            {
                Name = dto.Name,
                Year = dto.Year,
                FacultyId = dto.FacultyId,
                MajorId = dto.MajorId,
                Faculty = null!,
                Major = null!
            };

            _context.Groups.Add(group);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = group.Id }, new GroupDto
            {
                Id = group.Id,
                Name = group.Name,
                Year = group.Year,
                FacultyId = group.FacultyId,
                FacultyName = "", // optional lookup
                MajorId = group.MajorId,
                MajorName = "",
                SubjectNames = new(),
                TeacherNames = new(),
                StudentNames = new()
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateGroupDto dto)
        {
            if (id != dto.Id) return BadRequest("ID mismatch");

            var group = await _context.Groups.FindAsync(id);
            if (group == null) return NotFound();

            group.Name = dto.Name;
            group.Year = dto.Year;
            group.FacultyId = dto.FacultyId;
            group.MajorId = dto.MajorId;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var group = await _context.Groups.FindAsync(id);
            if (group == null) return NotFound();

            _context.Groups.Remove(group);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
