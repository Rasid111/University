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
    public class QuestionAnswerController(QuestionAnswerRepository questionAnswerRepository, QuestionRepository questionRepository) : ControllerBase
    {
        readonly QuestionAnswerRepository _questionAnswerRepository = questionAnswerRepository;
        readonly QuestionRepository _questionRepository = questionRepository;
        [HttpGet]
        [ProducesResponseType(200)]
        public async Task<IActionResult> Get()
        {
            return Ok(await _questionAnswerRepository.Get());
        }
        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Get(int id)
        {
            return Ok(await _questionAnswerRepository.Get(id));
        }
        [HttpPost]
        [ProducesResponseType(203)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Create(QuestionAnswerCreateDto dto)
        {
            var question = await _questionRepository.Get(dto.QuestionId);
            await _questionAnswerRepository.Create(new QuestionAnswer()
            {
                Title = dto.Title,
                Question = question ?? throw new ArgumentNullException(nameof(dto.QuestionId)),
            });
            return NoContent();
        }
        [HttpPut]
        [ProducesResponseType(203)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult Update(QuestionAnswer questionAnswer)
        {
            _questionAnswerRepository.Update(questionAnswer);
            return NoContent();
        }
        [HttpDelete]
        [ProducesResponseType(203)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(int id)
        {
            await _questionAnswerRepository.Delete(id);
            return NoContent();
        }
    }
}
