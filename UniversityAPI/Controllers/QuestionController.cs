
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
    public class QuestionsController : ControllerBase
    {
        private readonly UniversityDbContext _context;

        public QuestionsController(UniversityDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<QuestionDto>>> GetAll()
        {
            var questions = await _context.Questions
                .Include(q => q.Answers)
                .Include(q => q.Test)
                .ToListAsync();

            var dtoList = questions.Select(q => new QuestionDto
            {
                Id = q.Id,
                Title = q.Title,
                CorrectAnswerTitle = q.CorrectAnswerTitle,
                TestId = q.TestId,
                AnswerTitles = q.Answers.Select(a => a.Title).ToList()
            }).ToList();

            return Ok(dtoList);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<QuestionDto>> GetById(int id)
        {
            var question = await _context.Questions
                .Include(q => q.Answers)
                .Include(q => q.Test)
                .FirstOrDefaultAsync(q => q.Id == id);

            if (question == null) return NotFound();

            return Ok(new QuestionDto
            {
                Id = question.Id,
                Title = question.Title,
                CorrectAnswerTitle = question.CorrectAnswerTitle,
                TestId = question.TestId,
                AnswerTitles = question.Answers.Select(a => a.Title).ToList()
            });
        }

        [HttpPost]
        public async Task<ActionResult<QuestionDto>> Create(CreateQuestionDto dto)
        {
            var question = new Question
            {
                Title = dto.Title,
                CorrectAnswerTitle = dto.CorrectAnswerTitle,
                TestId = dto.TestId,
                Test = null! // suppress required warning
            };

            _context.Questions.Add(question);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = question.Id }, new QuestionDto
            {
                Id = question.Id,
                Title = question.Title,
                CorrectAnswerTitle = question.CorrectAnswerTitle,
                TestId = question.TestId,
                AnswerTitles = new()
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateQuestionDto dto)
        {
            if (id != dto.Id) return BadRequest("ID mismatch");

            var question = await _context.Questions.FindAsync(id);
            if (question == null) return NotFound();

            question.Title = dto.Title;
            question.CorrectAnswerTitle = dto.CorrectAnswerTitle;
            question.TestId = dto.TestId;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var question = await _context.Questions.FindAsync(id);
            if (question == null) return NotFound();

            _context.Questions.Remove(question);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
