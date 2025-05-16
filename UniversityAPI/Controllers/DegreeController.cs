using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniversityAPI.Database;
using UniversityAPI.Models;

namespace UniversityAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DegreesController : ControllerBase
    {
        private readonly UniversityDbContext _context;

        public DegreesController(UniversityDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Degree>>> GetDegrees()
        {
            return await _context.Degrees.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Degree>> GetDegree(int id)
        {
            var degree = await _context.Degrees.FindAsync(id);
            if (degree == null)
                return NotFound();
            return degree;
        }

        [HttpPost]
        public async Task<ActionResult<Degree>> CreateDegree(Degree degree)
        {
            _context.Degrees.Add(degree);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetDegree), new { id = degree.Id }, degree);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDegree(int id, Degree degree)
        {
            if (id != degree.Id)
                return BadRequest();

            _context.Entry(degree).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DegreeExists(id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDegree(int id)
        {
            var degree = await _context.Degrees.FindAsync(id);
            if (degree == null)
                return NotFound();

            _context.Degrees.Remove(degree);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DegreeExists(int id)
        {
            return _context.Degrees.Any(e => e.Id == id);
        }
    }
}
