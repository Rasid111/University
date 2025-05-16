
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
    public class FacultiesController : ControllerBase
    {
        private readonly UniversityDbContext _context;

        public FacultiesController(UniversityDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<FacultyDto>>> GetAll()
        {
            var faculties = await _context.Faculties.ToListAsync();

            var dtoList = faculties.Select(f => new FacultyDto
            {
                Id = f.Id,
                Name = f.Name
            }).ToList();

            return Ok(dtoList);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<FacultyDto>> GetById(int id)
        {
            var faculty = await _context.Faculties.FindAsync(id);
            if (faculty == null) return NotFound();

            return Ok(new FacultyDto
            {
                Id = faculty.Id,
                Name = faculty.Name
            });
        }

        [HttpPost]
        public async Task<ActionResult<FacultyDto>> Create(CreateFacultyDto dto)
        {
            var faculty = new Faculty
            {
                Name = dto.Name
            };

            _context.Faculties.Add(faculty);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = faculty.Id }, new FacultyDto
            {
                Id = faculty.Id,
                Name = faculty.Name
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateFacultyDto dto)
        {
            if (id != dto.Id) return BadRequest("ID mismatch");

            var faculty = await _context.Faculties.FindAsync(id);
            if (faculty == null) return NotFound();

            faculty.Name = dto.Name;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var faculty = await _context.Faculties.FindAsync(id);
            if (faculty == null) return NotFound();

            _context.Faculties.Remove(faculty);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
