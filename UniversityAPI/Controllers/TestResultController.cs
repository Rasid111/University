
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
    [Authorize(Roles = "Student,Admin")]
    public class TestResultsController : ControllerBase
    {
        private readonly UniversityDbContext _context;

        public TestResultsController(UniversityDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<TestResultDto>>> GetAll()
        {
            var results = await _context.TestResults
                .Include(r => r.Test)
                .ToListAsync();

            var dtoList = results.Select(r => new TestResultDto
            {
                Id = r.Id,
                TestId = r.TestId,
                TestTitle = r.Test.Title,
                CorrectAnswers = r.CorrectAnswers,
                Mistakes = r.Mistakes
            }).ToList();

            return Ok(dtoList);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TestResultDto>> GetById(int id)
        {
            var result = await _context.TestResults
                .Include(r => r.Test)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (result == null) return NotFound();

            return Ok(new TestResultDto
            {
                Id = result.Id,
                TestId = result.TestId,
                TestTitle = result.Test.Title,
                CorrectAnswers = result.CorrectAnswers,
                Mistakes = result.Mistakes
            });
        }

        [HttpPost]
        public async Task<ActionResult<TestResultDto>> Create(CreateTestResultDto dto)
        {
            var result = new TestResult
            {
                TestId = dto.TestId,
                CorrectAnswers = dto.CorrectAnswers,
                Mistakes = dto.Mistakes,
                Test = null!
            };

            _context.TestResults.Add(result);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = result.Id }, new TestResultDto
            {
                Id = result.Id,
                TestId = result.TestId,
                TestTitle = "",
                CorrectAnswers = result.CorrectAnswers,
                Mistakes = result.Mistakes
            });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, UpdateTestResultDto dto)
        {
            if (id != dto.Id) return BadRequest("ID mismatch");

            var result = await _context.TestResults.FindAsync(id);
            if (result == null) return NotFound();

            result.CorrectAnswers = dto.CorrectAnswers;
            result.Mistakes = dto.Mistakes;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _context.TestResults.FindAsync(id);
            if (result == null) return NotFound();

            _context.TestResults.Remove(result);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
