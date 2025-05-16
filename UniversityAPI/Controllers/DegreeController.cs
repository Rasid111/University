
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
    public class DegreesController : ControllerBase
    {
        private readonly UniversityDbContext _context;

        public DegreesController(UniversityDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DegreeDto>>> GetAll()
        {
            var degrees = await _context.Degrees.ToListAsync();
            var dtoList = degrees.Select(d => new DegreeDto
            {
                Id = d.Id,
                Name = d.Name
            }).ToList();

            return Ok(dtoList);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DegreeDto>> GetById(int id)
        {
            var degree = await _context.Degrees.FindAsync(id);
            if (degree == null) return NotFound();

            return Ok(new DegreeDto
            {
                Id = degree.Id,
                Name = degree.Name
            });
        }

        [HttpPost]
        public async Task<ActionResult<DegreeDto>> Create(CreateDegreeDto dto)
        {
            var degree = new Degree
            {
                Name = dto.Name
            };

            _context.Degrees.Add(degree);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = degree.Id }, new DegreeDto
            {
                Id = degree.Id,
                Name = degree.Name
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateDegreeDto dto)
        {
            if (id != dto.Id) return BadRequest("ID mismatch");

            var degree = await _context.Degrees.FindAsync(id);
            if (degree == null) return NotFound();

            degree.Name = dto.Name;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var degree = await _context.Degrees.FindAsync(id);
            if (degree == null) return NotFound();

            _context.Degrees.Remove(degree);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
