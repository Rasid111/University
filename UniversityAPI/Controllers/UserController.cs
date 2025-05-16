
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniversityAPI.Dtos;
using UniversityAPI.Models;

namespace UniversityAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<User> _userManager;

        public UsersController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAll()
        {
            var users = await _userManager.Users.ToListAsync();

            var dtoList = users.Select(u => new UserDto
            {
                Id = u.Id,
                UserName = u.UserName,
                FullName = $"{u.Name} {u.Surname}",
                Email = u.Email,
                Role = _userManager.GetRolesAsync(u).Result.FirstOrDefault() ?? "N/A",
                ProfilePictureUrl = u.ProfilePictureUrl
            }).ToList();

            return Ok(dtoList);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetById(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var roles = await _userManager.GetRolesAsync(user);

            return Ok(new UserDto
            {
                Id = user.Id,
                UserName = user.UserName,
                FullName = $"{user.Name} {user.Surname}",
                Email = user.Email,
                Role = roles.FirstOrDefault() ?? "N/A",
                ProfilePictureUrl = user.ProfilePictureUrl
            });
        }
    }
}
