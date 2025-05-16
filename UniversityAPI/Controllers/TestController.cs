// File: Controllers/TestsController.cs

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
    public class TestsController : ControllerBase
    {
        private readonly UniversityDbContext _context;

        public TestsController(UniversityDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TestDto>>> GetAll()
        {
            var tests = await _context.Tests
                .Include(t => t.Subject)
                .Include(t => t.Questions)
                .ToListAsync();

            var dtoList = tests.Select(t => new TestDto
            {
                Id = t.Id,
                Title = t.Title,
                SubjectId = t.SubjectId,
                SubjectName = t.Subject.Name,
                QuestionTitles = t.Questions.Select(q => q.Title).ToList()
            }).ToList();

            return Ok(dtoList);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TestDto>> GetById(int id)
        {
            var test = await _context.Tests
                .Include(t => t.Subject)
                .Include(t => t.Questions)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (test == null) return NotFound();

            return Ok(new TestDto
            {
                Id = test.Id,
                Title = test.Title,
                SubjectId = test.SubjectId,
                SubjectName = test.Subject.Name,
                QuestionTitles = test.Questions.Select(q => q.Title).ToList()
            });
        }

        [HttpPost]
        public async Task<ActionResult<TestDto>> Create(CreateTestDto dto)
        {
            var test = new Test
            {
                Title = dto.Title,
                SubjectId = dto.SubjectId,
                Subject = null!
            };

            _context.Tests.Add(test);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = test.Id }, new TestDto
            {
                Id = test.Id,
                Title = test.Title,
                SubjectId = test.SubjectId,
                SubjectName = "",
                QuestionTitles = new()
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateTestDto dto)
        {
            if (id != dto.Id) return BadRequest("ID mismatch");

            var test = await _context.Tests.FindAsync(id);
            if (test == null) return NotFound();

            test.Title = dto.Title;
            test.SubjectId = dto.SubjectId;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var test = await _context.Tests.FindAsync(id);
            if (test == null) return NotFound();

            _context.Tests.Remove(test);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
