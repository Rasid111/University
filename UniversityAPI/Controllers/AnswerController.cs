
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
    public class AnswersController : ControllerBase
    {
        private readonly UniversityDbContext _context;

        public AnswersController(UniversityDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AnswerDto>>> GetAll()
        {
            var answers = await _context.Answers
                .Include(a => a.Question)
                .ToListAsync();

            var dtoList = answers.Select(a => new AnswerDto
            {
                Id = a.Id,
                Title = a.Title,
                QuestionId = a.QuestionId
            }).ToList();

            return Ok(dtoList);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AnswerDto>> GetById(int id)
        {
            var answer = await _context.Answers
                .Include(a => a.Question)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (answer == null) return NotFound();

            return Ok(new AnswerDto
            {
                Id = answer.Id,
                Title = answer.Title,
                QuestionId = answer.QuestionId
            });
        }

        [HttpPost]
        public async Task<ActionResult<AnswerDto>> Create(CreateAnswerDto dto)
        {
            var answer = new Answer
            {
                Title = dto.Title,
                QuestionId = dto.QuestionId,
                Question = null! // EF will resolve this via QuestionId
            };

            _context.Answers.Add(answer);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = answer.Id }, new AnswerDto
            {
                Id = answer.Id,
                Title = answer.Title,
                QuestionId = answer.QuestionId
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateAnswerDto dto)
        {
            if (id != dto.Id) return BadRequest("ID mismatch");

            var answer = await _context.Answers.FindAsync(id);
            if (answer == null) return NotFound();

            answer.Title = dto.Title;
            answer.QuestionId = dto.QuestionId;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var answer = await _context.Answers.FindAsync(id);
            if (answer == null) return NotFound();

            _context.Answers.Remove(answer);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
