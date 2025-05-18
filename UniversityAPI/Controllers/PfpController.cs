using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversityAPI.Repositories;
using UniversityAPI.EntityFramework;
using UniversityAPI.Services;

namespace UniversityAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PfpController(UserRepository userRepository, UniversityDbContext context, BlobService blobService) : ControllerBase
    {
        private readonly UserRepository _userRepository = userRepository;
        private readonly UniversityDbContext _context = context;
        private readonly BlobService _blobService = blobService;

        [HttpPost]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User ID not found in token.");

            var imageUrl = await _userRepository.UploadProfilePictureAsync(userId, file);
            if (imageUrl == null)
                return NotFound("User not found.");

            return Ok(new { imageUrl });
        }

        [HttpDelete]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> Delete()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();

            var user = await _context.Users.FindAsync(userId);
            if (user == null) return NotFound("User not found.");

            if (!string.IsNullOrEmpty(user.ProfilePictureUrl))
            {
                var fileName = Path.GetFileName(new Uri(user.ProfilePictureUrl).AbsolutePath);
                await _blobService.DeleteAsync(fileName);
                user.ProfilePictureUrl = null;
                await _context.SaveChangesAsync();
            }

            return NoContent();
        }
    }
}
