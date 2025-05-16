using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniversityAPI.Database;
using UniversityAPI.Models;

namespace UniversityAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeacherGroupSubjectsController : ControllerBase
    {
        private readonly UniversityDbContext _context;

        public TeacherGroupSubjectsController(UniversityDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TeacherGroupSubject>>> GetTeacherGroupSubjects()
        {
            return await _context.TeacherGroupSubjects
                .Include(tgs => tgs.TeacherProfile)
                .Include(tgs => tgs.Group)
                .Include(tgs => tgs.Subject)
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TeacherGroupSubject>> GetTeacherGroupSubject(int id)
        {
            var teacherGroupSubject = await _context.TeacherGroupSubjects
                .Include(tgs => tgs.TeacherProfile)
                .Include(tgs => tgs.Group)
                .Include(tgs => tgs.Subject)
                .FirstOrDefaultAsync(tgs => tgs.Id == id);

            if (teacherGroupSubject == null)
            {
                return NotFound();
            }

            return teacherGroupSubject;
        }

        [HttpPost]
        public async Task<ActionResult<TeacherGroupSubject>> CreateTeacherGroupSubject(TeacherGroupSubject teacherGroupSubject)
        {
            _context.TeacherGroupSubjects.Add(teacherGroupSubject);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTeacherGroupSubject), new { id = teacherGroupSubject.Id }, teacherGroupSubject);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTeacherGroupSubject(int id, TeacherGroupSubject teacherGroupSubject)
        {
            if (id != teacherGroupSubject.Id)
            {
                return BadRequest();
            }

            _context.Entry(teacherGroupSubject).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TeacherGroupSubjectExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTeacherGroupSubject(int id)
        {
            var teacherGroupSubject = await _context.TeacherGroupSubjects.FindAsync(id);
            if (teacherGroupSubject == null)
            {
                return NotFound();
            }

            _context.TeacherGroupSubjects.Remove(teacherGroupSubject);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TeacherGroupSubjectExists(int id)
        {
            return _context.TeacherGroupSubjects.Any(e => e.Id == id);
        }
    }
}
