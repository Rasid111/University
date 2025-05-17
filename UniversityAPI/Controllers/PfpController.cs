using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversityAPI.Repositories;

namespace UniversityAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PfpController(UserRepository userRepository) : ControllerBase
    {
        private readonly UserRepository _userRepository = userRepository;

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
    }
}
