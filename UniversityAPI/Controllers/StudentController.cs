
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
    [Authorize(Roles = "Admin,Student")]
    public class StudentProfilesController : ControllerBase
    {
        private readonly UniversityDbContext _context;

        public StudentProfilesController(UniversityDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<StudentProfileDto>>> GetAll()
        {
            var students = await _context.StudentProfiles
                .Include(s => s.User)
                .Include(s => s.Group)
                .ToListAsync();

            var dtoList = students.Select(s => new StudentProfileDto
            {
                Id = s.Id,
                UserName = s.User.UserName,
                FullName = s.User.Name + " " + s.User.Surname,
                GroupId = s.GroupId,
                GroupName = s.Group.Name
            }).ToList();

            return Ok(dtoList);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<StudentProfileDto>> GetById(int id)
        {
            var student = await _context.StudentProfiles
                .Include(s => s.User)
                .Include(s => s.Group)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (student == null) return NotFound();

            return Ok(new StudentProfileDto
            {
                Id = student.Id,
                UserName = student.User.UserName,
                FullName = student.User.Name + " " + student.User.Surname,
                GroupId = student.GroupId,
                GroupName = student.Group.Name
            });
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<StudentProfileDto>> Create(CreateStudentProfileDto dto)
        {
            var user = new User
            {
                UserName = dto.UserName,
                Name = dto.FullName.Split(' ').FirstOrDefault() ?? "N/A",
                Surname = dto.FullName.Split(' ').Skip(1).FirstOrDefault() ?? "N/A",
                Email = $"{dto.UserName}@example.com", // Customize if needed
                EmailConfirmed = true
            };

            var student = new StudentProfile
            {
                User = user,
                GroupId = dto.GroupId,
                Group = null!
            };

            _context.StudentProfiles.Add(student);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = student.Id }, new StudentProfileDto
            {
                Id = student.Id,
                UserName = user.UserName,
                FullName = user.Name + " " + user.Surname,
                GroupId = dto.GroupId,
                GroupName = ""
            });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, UpdateStudentProfileDto dto)
        {
            if (id != dto.Id) return BadRequest("ID mismatch");

            var student = await _context.StudentProfiles.FindAsync(id);
            if (student == null) return NotFound();

            student.GroupId = dto.GroupId;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var student = await _context.StudentProfiles.FindAsync(id);
            if (student == null) return NotFound();

            _context.StudentProfiles.Remove(student);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
