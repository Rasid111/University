using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Reflection.Metadata.Ecma335;
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
    public class TestController(TestRepository testRepository, SubjectRepository subjectRepository) : ControllerBase
    {
        readonly TestRepository _testRepository = testRepository;
        readonly SubjectRepository _subjectRepository = subjectRepository;
        [HttpPost("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Pass(int id, List<string> answers)
        {
            Test? test = await _testRepository.Get(id);
            if (test is null)
                return NotFound();
            if (test.Questions.Count != answers.Count)
                return BadRequest();

            var questionCorrectAnswers = new Dictionary<string, string>();
            decimal result = 0;
            for (int i = 0; i < test.Questions.Count; i++)
            {
                if (test.Questions[i].CorrectAnswerTitle == answers[i])
                {
                    result++;
                }
                questionCorrectAnswers.Add(test.Questions[i].Title, test.Questions[i].CorrectAnswerTitle);
            }

            TestPassResultDto resultDto = new TestPassResultDto() {
                QuestionCorrectAnswers = questionCorrectAnswers,
                StudentAnswers = answers,
                Result = result / test.Questions.Count
            };

            var testResult = new TestResult()
            {
                Test = test,
                Answers = answers.Select((a, i) => new TestAnswer
                {
                    Answer = a,
                    CorrectAnswer = test.Questions[i].CorrectAnswerTitle,
                    Test = test
                }).ToList(),
                TestId = test.Id
            };
            await _testRepository.SaveResults(testResult);
            return Ok(resultDto);
        }

        [HttpGet]
        [ProducesResponseType(200)]
        public async Task<IActionResult> Get()
        {
            return Ok(await _testRepository.Get());
        }
        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Get(int id)
        {
            return Ok(await _testRepository.Get(id));
        }
        [HttpPost]
        [ProducesResponseType(203)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Create(TestCreateDto dto)
        {
            var subject = await _subjectRepository.Get(dto.SubjectId);
            var test = new Test()
            {
                Title = dto.Title,
                Subject = subject ?? throw new ArgumentNullException(nameof(dto.SubjectId)),
            };
            if (!dto.Questions.IsNullOrEmpty())
            {
                foreach (var question in dto.Questions!)
                {
                    var newQuestion = new Question()
                    {
                        Title = question.Title,
                        CorrectAnswerTitle = question.CorrectAnswerTitle,
                        Test = test,
                    };
                    if (!question.Answers.IsNullOrEmpty())
                    {
                        foreach (var answer in question.Answers!)
                        {
                            newQuestion.Answers.Add(new QuestionAnswer()
                            {
                                Title = answer.Title,
                                Question = newQuestion
                            });
                        }
                    }
                    test.Questions.Add(newQuestion);
                }
            }
            await _testRepository.Create(test);
            
            return NoContent();
        }
        [HttpPut]
        [ProducesResponseType(203)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult Update(Test test)
        {
            _testRepository.Update(test);
            return NoContent();
        }
        [HttpDelete]
        [ProducesResponseType(203)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(int id)
        {
            await _testRepository.Delete(id);
            return NoContent();
        }
    }
}
