
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
    public class SubjectsController : ControllerBase
    {
        private readonly UniversityDbContext _context;

        public SubjectsController(UniversityDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SubjectDto>>> GetAll()
        {
            var subjects = await _context.Subjects.ToListAsync();

            var dtoList = subjects.Select(s => new SubjectDto
            {
                Id = s.Id,
                Name = s.Name
            }).ToList();

            return Ok(dtoList);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SubjectDto>> GetById(int id)
        {
            var subject = await _context.Subjects.FindAsync(id);
            if (subject == null) return NotFound();

            return Ok(new SubjectDto
            {
                Id = subject.Id,
                Name = subject.Name
            });
        }

        [HttpPost]
        public async Task<ActionResult<SubjectDto>> Create(CreateSubjectDto dto)
        {
            var subject = new Subject
            {
                Name = dto.Name
            };

            _context.Subjects.Add(subject);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = subject.Id }, new SubjectDto
            {
                Id = subject.Id,
                Name = subject.Name
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateSubjectDto dto)
        {
            if (id != dto.Id) return BadRequest("ID mismatch");

            var subject = await _context.Subjects.FindAsync(id);
            if (subject == null) return NotFound();

            subject.Name = dto.Name;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var subject = await _context.Subjects.FindAsync(id);
            if (subject == null) return NotFound();

            _context.Subjects.Remove(subject);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
