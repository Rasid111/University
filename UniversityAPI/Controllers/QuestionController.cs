using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversityAPI.Models;
using UniversityAPI.Repositories;
using UniversityApplication.Dtos;

namespace UniversityAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    [ProducesResponseType(401)]
    [ProducesResponseType(500)]
    public class QuestionController(TestRepository testRepository, QuestionRepository questionRepository) : ControllerBase
    {
        readonly TestRepository _testRepository = testRepository;
        readonly QuestionRepository _questionRepository = questionRepository;
        [HttpGet]
        [ProducesResponseType(200)]
        public async Task<IActionResult> Get()
        {
            return Ok(await _questionRepository.Get());
        }
        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Get(int id)
        {
            return Ok(await _questionRepository.Get(id));
        }
        [HttpPost]
        [ProducesResponseType(203)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Create(QuestionCreateDto dto)
        {
            var test = await _testRepository.Get(dto.TestId);
            await _questionRepository.Create(new Question()
            {
                Title = dto.Title,
                CorrectAnswerTitle = dto.CorrectAnswerTitle,
                Test = test ?? throw new ArgumentNullException(nameof(dto.TestId)),
            });
            return NoContent();
        }
        [HttpPut]
        [ProducesResponseType(203)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult Update(Question question)
        {
            _questionRepository.Update(question);
            return NoContent();
        }
        [HttpDelete]
        [ProducesResponseType(203)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(int id)
        {
            await _questionRepository.Delete(id);
            return NoContent();
        }
    }
}
