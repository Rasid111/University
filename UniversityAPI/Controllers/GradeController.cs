using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniversityAPI.EntityFramework;
using UniversityAPI.Models;
using UniversityAPI.Repositories;
using UniversityApplication.Dtos;
using System.Security.Claims;


namespace UniversityAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    [ProducesResponseType(401)]
    [ProducesResponseType(500)]
    public class GradeController(GradeRepository repository, UniversityDbContext context) : ControllerBase
    {
        private readonly GradeRepository _repository = repository;
        private readonly UniversityDbContext _context = context;

        [HttpGet]
        [ProducesResponseType(200)]
        public async Task<IActionResult> Get()
        {
            var grades = await _repository.Get();
            return Ok(grades);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> Get(int id)
        {
            var grade = await _repository.Get(id);
            if (grade is null)
                return NotFound();

            var userIdClaim = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                return Unauthorized("Invalid or missing user ID.");

            var userRole = User.FindFirst("Role")?.Value;

            var isAuthorized = userRole switch
            {
                "Student" => grade.StudentProfileId == userId,
                "Teacher" => grade.TeacherProfileId == userId,
                "Admin" => true,
                _ => false
            };

            if (!isAuthorized)
                return Forbid("You are not authorized to view this grade.");

            return Ok(grade);
        }

        [HttpPost]
        [ProducesResponseType(203)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Create(GradeCreateDto dto)
        {
            var grade = new Grade
            {
                Value = dto.Value,
                Message = dto.Message,
                SubjectId = dto.SubjectId,
                StudentProfileId = dto.StudentProfileId,
                TeacherProfileId = dto.TeacherProfileId,
                Date = dto.Date ?? DateTime.Now
            };

            await _repository.Create(grade);
            return NoContent();
        }

        [HttpPut]
        [ProducesResponseType(203)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult Update(Grade grade)
        {
            _repository.Update(grade);
            return NoContent();
        }

        [HttpDelete]
        [ProducesResponseType(203)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(int id)
        {
            await _repository.Delete(id);
            return NoContent();
        }

        [HttpGet("{studentId}")]
        public async Task<IActionResult> GetGradesForStudent(int studentId)
        {
            var student = await _context.StudentProfiles
                .Include(s => s.Grades)
                .FirstOrDefaultAsync(s => s.Id == studentId);

            if (student == null)
                return NotFound();

            var teacherIds = student.Grades.Select(g => g.TeacherProfileId).Distinct().ToList();
            var teachers = await _context.TeachersProfiles
                .Include(tp => tp.User)
                .Where(tp => teacherIds.Contains(tp.Id))
                .ToListAsync();

            var subjectIds = student.Grades.Select(g => g.SubjectId).Distinct().ToList();
            var subjects = await _context.Subjects
                .Where(s => subjectIds.Contains(s.Id))
                .ToListAsync();

            var dtoList = student.Grades
                .OrderByDescending(g => g.Date)
                .Select(g => new GradeBoxDto
                {
                    Value = g.Value,
                    Message = g.Message ?? "",
                    Date = g.Date,
                    TeacherName = teachers.FirstOrDefault(t => t.Id == g.TeacherProfileId)?.User?.Name + " " +
                                  teachers.FirstOrDefault(t => t.Id == g.TeacherProfileId)?.User?.Surname,
                    SubjectName = subjects.FirstOrDefault(s => s.Id == g.SubjectId)?.Name ?? "Unknown"
                })
                .ToList();

            return Ok(dtoList);
        }

        [HttpGet]
public async Task<IActionResult> GetGradeBoxes()
{
    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
    if (userId == null) return Unauthorized();

    var student = await _context.StudentProfiles
        .Include(s => s.Grades)
        .FirstOrDefaultAsync(s => s.UserId == userId);

    if (student == null)
        return NotFound("Student profile not found.");

    var teacherIds = student.Grades.Select(g => g.TeacherProfileId).Distinct().ToList();
    var teachers = await _context.TeachersProfiles
        .Include(tp => tp.User)
        .Where(tp => teacherIds.Contains(tp.Id))
        .ToListAsync();

    var subjectIds = student.Grades.Select(g => g.SubjectId).Distinct().ToList();
    var subjects = await _context.Subjects
        .Where(s => subjectIds.Contains(s.Id))
        .ToListAsync();

    var dtoList = student.Grades
        .OrderByDescending(g => g.Date)
        .Select(g => new GradeBoxDto
        {
            Value = g.Value,
            Message = g.Message ?? "",
            Date = g.Date,
            TeacherName = teachers.FirstOrDefault(t => t.Id == g.TeacherProfileId)?.User?.Name + " " +
                          teachers.FirstOrDefault(t => t.Id == g.TeacherProfileId)?.User?.Surname,
            SubjectName = subjects.FirstOrDefault(s => s.Id == g.SubjectId)?.Name ?? "Unknown"
        })
        .ToList();

    return Ok(dtoList);
}

    }
}
