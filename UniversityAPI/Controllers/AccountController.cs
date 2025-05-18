using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using UniversityApplication.Dtos;
using UniversityAPI.Entity;
using UniversityAPI.EntityFramework;
using UniversityAPI.Models;
using UniversityAPI.Options;
using UniversityAPI.Repositories;

namespace UniversityAPI.Controllers
{
    [ApiController]
    [Route("/api/[controller]/[action]")]
    public class AccountController(
        IOptionsSnapshot<JwtOptions> jwtOptionsSnapshot,
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        RoleManager<IdentityRole> roleManager,
        UniversityDbContext dbContext,
        UserRepository userRepository
    ) : ControllerBase
    {
        private readonly JwtOptions _jwtOptions = jwtOptionsSnapshot.Value;
        private readonly UserManager<User> _userManager = userManager;
        private readonly SignInManager<User> _signInManager = signInManager;
        private readonly RoleManager<IdentityRole> _roleManager = roleManager;
        private readonly UniversityDbContext _dbContext = dbContext;
        private readonly UserRepository _userRepository = userRepository;

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var user = await _userRepository.Get(id);
            return user == null ? NotFound() : Ok(user);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var foundUser = await _userManager.FindByEmailAsync(dto.Email);
            if (foundUser == null)
                return BadRequest("Incorrect Email or Password");

            var signInResult = await _signInManager.PasswordSignInAsync(foundUser, dto.Password, true, true);

            if (signInResult.IsLockedOut)
                return BadRequest("User locked");

            if (!signInResult.Succeeded)
                return BadRequest("Incorrect Login or Password");

            var roles = await _userManager.GetRolesAsync(foundUser);

            var claims = roles
                .Select(roleStr => new Claim(ClaimTypes.Role, roleStr))
                .Append(new Claim(ClaimTypes.NameIdentifier, foundUser.Id))
                .Append(new Claim(ClaimTypes.Email, foundUser.Email ?? "not set"))
                .Append(new Claim(ClaimTypes.Name, foundUser.UserName ?? "not set"));

            var signingKey = new SymmetricSecurityKey(_jwtOptions.KeyInBytes);
            var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(_jwtOptions.LifeTimeInMinutes),
                signingCredentials: signingCredentials
            );

            var handler = new JwtSecurityTokenHandler();
            var tokenStr = handler.WriteToken(token);

            var refreshToken = new RefreshToken
            {
                UserId = foundUser.Id,
                Token = Guid.NewGuid(),
            };

            await _dbContext.RefreshTokens.AddAsync(refreshToken);
            await _dbContext.SaveChangesAsync();

            return Ok(new
            {
                refresh = refreshToken.Token,
                access = tokenStr,
            });
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            var newUser = new User()
            {
                UserName = dto.Email,
                Email = dto.Email,
                Name = dto.Name,
                Surname = dto.Surname,
            };

            var result = await _userManager.CreateAsync(newUser, dto.Password);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            await _userManager.AddToRoleAsync(newUser, UserRoleDefaults.User);

            return Ok();
        }

        [HttpPut]
        [ActionName("Token")]
        public async Task<IActionResult> UpdateToken([Required] Guid refresh)
        {
            var tokenStr = HttpContext.Request.Headers.Authorization.FirstOrDefault();
            if (tokenStr is null)
                return StatusCode(401);

            if (tokenStr.StartsWith("Bearer "))
                tokenStr = tokenStr["Bearer ".Length..];

            var handler = new JwtSecurityTokenHandler();
            var tokenValidationResult = await handler.ValidateTokenAsync(
                tokenStr,
                new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = _jwtOptions.Issuer,
                    ValidateAudience = true,
                    ValidAudience = _jwtOptions.Audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(_jwtOptions.KeyInBytes)
                }
            );

            if (!tokenValidationResult.IsValid)
                return BadRequest(tokenValidationResult.Exception);

            var token = handler.ReadJwtToken(tokenStr);
            var idClaim = token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (idClaim is null)
                return BadRequest($"Token has no claim with type '{ClaimTypes.NameIdentifier}'");

            var userId = idClaim.Value;
            var foundUser = await _userManager.FindByIdAsync(userId);
            if (foundUser is null)
                return BadRequest($"User not found by id: '{userId}'");

            var oldRefreshToken = await _dbContext.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == refresh && rt.UserId == foundUser.Id);

            if (oldRefreshToken is null)
            {
                var allTokens = _dbContext.RefreshTokens.Where(rt => rt.UserId == foundUser.Id);
                _dbContext.RefreshTokens.RemoveRange(allTokens);
                await _dbContext.SaveChangesAsync();
                return BadRequest("Refresh token not found!");
            }

            _dbContext.RefreshTokens.Remove(oldRefreshToken);
            var newRefreshToken = new RefreshToken
            {
                UserId = foundUser.Id,
                Token = Guid.NewGuid()
            };
            await _dbContext.RefreshTokens.AddAsync(newRefreshToken);
            await _dbContext.SaveChangesAsync();

            var roles = await _userManager.GetRolesAsync(foundUser);

            var claims = roles
                .Select(roleStr => new Claim(ClaimTypes.Role, roleStr))
                .Append(new Claim(ClaimTypes.NameIdentifier, foundUser.Id))
                .Append(new Claim(ClaimTypes.Email, foundUser.Email ?? "not set"))
                .Append(new Claim(ClaimTypes.Name, foundUser.Name ?? "not set"))
                .Append(new Claim(ClaimTypes.Surname, foundUser.Surname ?? "not set"));

            var signingKey = new SymmetricSecurityKey(_jwtOptions.KeyInBytes);
            var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            var newToken = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(_jwtOptions.LifeTimeInMinutes),
                signingCredentials: signingCredentials
            );

            var newTokenStr = handler.WriteToken(newToken);

            return Ok(new
            {
                refresh = newRefreshToken.Token,
                access = newTokenStr,
            });
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword([FromBody] PasswordChangeDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized();

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound("User not found");

            var result = await _userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok("Password changed successfully.");
        }
    }
}
