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
    [Authorize(Roles = "Teacher,Admin")]
    public class TeacherProfilesController : ControllerBase
    {
        private readonly UniversityDbContext _context;
        private readonly BlobService _blobService;

        public TeacherProfilesController(UniversityDbContext context, BlobService blobService)
        {
            _context = context;
            _blobService = blobService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var teachers = await _context.TeacherProfiles
                .Include(t => t.User)
                .Include(t => t.Faculty)
                .Include(t => t.Degree)
                .Include(t => t.Groups)
                .Include(t => t.Subjects)
                .Include(t => t.TeacherGroupSubjects)
                .ToListAsync();

            return Ok(teachers);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var teacher = await _context.TeacherProfiles
                .Include(t => t.User)
                .Include(t => t.Faculty)
                .Include(t => t.Degree)
                .Include(t => t.Groups)
                .Include(t => t.Subjects)
                .Include(t => t.TeacherGroupSubjects)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (teacher == null)
                return NotFound("Teacher not found.");

            return Ok(teacher);
        }

        // [HttpGet("profile")]
        // public async Task<IActionResult> GetOwnProfile()
        // {
        //     var username = User.Identity?.Name;

        //     var user = await _context.Users
        //         .Include(u => u.TeacherProfile)
        //         .ThenInclude(p => p.Faculty)
        //         .Include(u => u.TeacherProfile.Degree)
        //         .Include(u => u.TeacherProfile.Groups)
        //         .Include(u => u.TeacherProfile.Subjects)
        //         .Include(u => u.TeacherProfile.TeacherGroupSubjects)
        //         .FirstOrDefaultAsync(u => u.UserName == username);

        //     if (user == null || user.TeacherProfile == null)
        //         return NotFound("Teacher profile not found.");

        //     return Ok(user.TeacherProfile);
        // }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(TeacherProfile profile)
        {
            _context.TeacherProfiles.Add(profile);
            await _context.SaveChangesAsync();
            return Ok(profile);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, TeacherProfile profile)
        {
            if (id != profile.Id)
                return BadRequest("ID mismatch");

            _context.Update(profile);
            await _context.SaveChangesAsync();
            return Ok("Updated successfully");
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var teacher = await _context.TeacherProfiles.FindAsync(id);
            if (teacher == null)
                return NotFound("Not found");

            _context.TeacherProfiles.Remove(teacher);
            await _context.SaveChangesAsync();
            return Ok("Deleted");
        }

        // [HttpPost("upload-profile-picture")]
        // public async Task<IActionResult> UploadProfilePicture([FromForm] IFormFile profilePicture)
        // {
        //     var username = User.Identity?.Name;

        //     var user = await _context.Users
        //         .FirstOrDefaultAsync(u => u.UserName == username);

        //     if (user == null || user.TeacherProfileId == null)
        //         return NotFound("User or profile not found");

        //     if (profilePicture == null || profilePicture.Length == 0)
        //         return BadRequest("No file uploaded");

        //     string fileName = $"{user.UserName}_{DateTime.UtcNow.Ticks}{Path.GetExtension(profilePicture.FileName)}";
        //     string imageUrl = await _blobService.UploadFileAsync(profilePicture, fileName);

        //     user.ProfilePictureUrl = imageUrl;
        //     await _context.SaveChangesAsync();

        //     return Ok(new { message = "Picture uploaded", imageUrl });
        // }
    }
}
