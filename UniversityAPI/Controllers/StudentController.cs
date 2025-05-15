using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniversityAPI.Database;
using UniversityAPI.Models;
using UniversityAPI.Services;

namespace UniversityAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Student,Admin")]
    public class StudentProfilesController : ControllerBase
    {
        private readonly UniversityDbContext _context;
        private readonly BlobService _blobService;

        public StudentProfilesController(UniversityDbContext context, BlobService blobService)
        {
            _context = context;
            _blobService = blobService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<StudentProfile>>> GetAll()
        {
            return await _context.StudentProfiles
                .Include(s => s.User)
                .Include(s => s.Group)
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<StudentProfile>> GetById(int id)
        {
            var student = await _context.StudentProfiles
                .Include(s => s.User)
                .Include(s => s.Group)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (student == null)
                return NotFound();

            return student;
        }

        // [HttpGet("profile")]
        // public async Task<ActionResult<StudentProfile>> GetLoggedInProfile()
        // {
        //     var username = User.Identity?.Name;

        //     var user = await _context.Users
        //         .Include(u => u.StudentProfile)
        //             .ThenInclude(s => s.Group)
        //         .FirstOrDefaultAsync(u => u.UserName == username);

        //     if (user == null || user.StudentProfile == null)
        //         return NotFound("Student profile not found.");

        //     return user.StudentProfile;
        // }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<StudentProfile>> Create(StudentProfile profile)
        {
            _context.StudentProfiles.Add(profile);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = profile.Id }, profile);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, StudentProfile profile)
        {
            if (id != profile.Id)
                return BadRequest();

            _context.Entry(profile).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.StudentProfiles.Any(e => e.Id == id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var student = await _context.StudentProfiles.FindAsync(id);
            if (student == null)
                return NotFound();

            _context.StudentProfiles.Remove(student);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // [HttpPost("upload-profile-picture")]
        // public async Task<IActionResult> UploadProfilePicture([FromForm] IFormFile profilePicture)
        // {
        //     var username = User.Identity?.Name;

        //     var user = await _context.Users
        //         .FirstOrDefaultAsync(u => u.UserName == username);

        //     if (user == null || user.StudentProfileId == null)
        //         return NotFound("Student not found.");

        //     if (profilePicture == null || profilePicture.Length == 0)
        //         return BadRequest("Invalid file");

        //     string fileName = $"{user.UserName}_{DateTime.UtcNow.Ticks}{Path.GetExtension(profilePicture.FileName)}";
        //     var imageUrl = await _blobService.UploadFileAsync(profilePicture, fileName);

        //     user.ProfilePictureUrl = imageUrl;
        //     await _context.SaveChangesAsync();

        //     return Ok(new { message = "Profile picture updated!", imageUrl });
        // }
    }
}
