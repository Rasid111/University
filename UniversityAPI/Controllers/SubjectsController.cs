using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniversityAPI.Database;
using UniversityAPI.Dtos;
using UniversityAPI.Models;

namespace UniversityAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SubjectsController(UniversityDbContext context) : ControllerBase
    {
        private readonly UniversityDbContext _context = context;
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(await _context.Subjects.ToListAsync());
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var res = await _context.Subjects.FirstOrDefaultAsync(s => s.Id == id);
            return res == null ? NotFound() : Ok(res);
        }
        [HttpPost]
        public async Task<IActionResult> Create(SubjectCreateDto dto)
        {
            try
            {
                var subject = new Subject() { Name = dto.Name };
                await _context.Subjects.AddAsync(subject);
                var res = _context.SaveChanges();
                return res > 0 ? Ok(subject.Id) : BadRequest();
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }
    }
}
