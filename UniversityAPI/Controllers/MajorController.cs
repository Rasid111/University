
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
    public class MajorsController : ControllerBase
    {
        private readonly UniversityDbContext _context;

        public MajorsController(UniversityDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MajorDto>>> GetAll()
        {
            var majors = await _context.Majors.ToListAsync();

            var dtoList = majors.Select(m => new MajorDto
            {
                Id = m.Id,
                Name = m.Name
            }).ToList();

            return Ok(dtoList);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MajorDto>> GetById(int id)
        {
            var major = await _context.Majors.FindAsync(id);
            if (major == null) return NotFound();

            return Ok(new MajorDto
            {
                Id = major.Id,
                Name = major.Name
            });
        }

        [HttpPost]
        public async Task<ActionResult<MajorDto>> Create(CreateMajorDto dto)
        {
            var major = new Major
            {
                Name = dto.Name
            };

            _context.Majors.Add(major);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = major.Id }, new MajorDto
            {
                Id = major.Id,
                Name = major.Name
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateMajorDto dto)
        {
            if (id != dto.Id) return BadRequest("ID mismatch");

            var major = await _context.Majors.FindAsync(id);
            if (major == null) return NotFound();

            major.Name = dto.Name;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var major = await _context.Majors.FindAsync(id);
            if (major == null) return NotFound();

            _context.Majors.Remove(major);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
