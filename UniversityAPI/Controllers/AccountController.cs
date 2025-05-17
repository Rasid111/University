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
        readonly JwtOptions _jwtOptions = jwtOptionsSnapshot.Value;
        readonly UserManager<User> _userManager = userManager;
        readonly SignInManager<User> _signInManager = signInManager;
        readonly RoleManager<IdentityRole> _roleManager = roleManager;
        readonly UniversityDbContext _dbContext = dbContext;
        readonly UserRepository _userRepository = userRepository;

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var user = await _userRepository.Get(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var foundUser = await _userManager.FindByEmailAsync(dto.Email);

            if (foundUser == null)
            {
                return base.BadRequest("Incorrect Email or Password");
            }

            var signInResult = await _signInManager.PasswordSignInAsync(foundUser, dto.Password, true, true);

            if (signInResult.IsLockedOut)
            {
                return base.BadRequest("User locked");
            }

            if (signInResult.Succeeded == false)
            {
                return base.BadRequest("Incorrect Login or Password");
            }

            var roles = await userManager.GetRolesAsync(foundUser);
            
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

            await dbContext.RefreshTokens.AddAsync(refreshToken);
            await dbContext.SaveChangesAsync();

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

            var result = await userManager.CreateAsync(newUser, dto.Password);

            if (result.Succeeded == false)
            {
                return base.BadRequest(result.Errors);
            }

            await userManager.AddToRoleAsync(newUser, UserRoleDefaults.User);

            return Ok();
        }

        [HttpPut]
        [ActionName("Token")]
        public async Task<IActionResult> UpdateToken([Required] Guid refresh)
        {
            var tokenStr = base.HttpContext.Request.Headers.Authorization.FirstOrDefault();

            if (tokenStr is null)
            {
                return base.StatusCode(401);
            }

            if (tokenStr.StartsWith("Bearer "))
            {
                tokenStr = tokenStr["Bearer ".Length..];
                System.Console.WriteLine(tokenStr);
            }

            var handler = new JwtSecurityTokenHandler();
            var tokenValidationResult = await handler.ValidateTokenAsync(
                tokenStr,
                new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = "University",

                    ValidateAudience = true,
                    ValidAudience = "User",

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(_jwtOptions.KeyInBytes)
                }
            );

            if (tokenValidationResult.IsValid == false)
            {
                return BadRequest(tokenValidationResult.Exception);
            }

            var token = handler.ReadJwtToken(tokenStr);

            Claim? idClaim = token.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier);

            if (idClaim is null)
            {
                return BadRequest($"Token has no claim with type '{ClaimTypes.NameIdentifier}'");
            }

            var userId = idClaim.Value;

            User? foundUser = await _userManager.FindByIdAsync(userId);

            if (foundUser is null)
            {
                return BadRequest($"User not found by id: '{userId}'");
            }

            var oldRefreshToken = await _dbContext.RefreshTokens.FirstOrDefaultAsync(rt => (rt.Token == refresh) && (rt.UserId == foundUser.Id));


            if (oldRefreshToken is null)
            {
                var allUserRefreshTokens = _dbContext.RefreshTokens.Where(rt => rt.UserId == foundUser.Id);

                dbContext.RefreshTokens.RemoveRange(allUserRefreshTokens);
                await _dbContext.SaveChangesAsync();

                return BadRequest("Refresh token not found!");
            }

            dbContext.RefreshTokens.Remove(oldRefreshToken);
            var newRefreshToken = new RefreshToken
            {
                UserId = foundUser.Id,
                Token = Guid.NewGuid()
            };
            await _dbContext.RefreshTokens.AddAsync(newRefreshToken);
            await _dbContext.SaveChangesAsync();

            var roles = await userManager.GetRolesAsync(foundUser);

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
                expires: DateTime.Now.AddSeconds(10),
                signingCredentials: signingCredentials
            );

            var newTokenStr = handler.WriteToken(newToken);

            return Ok(new
            {
                refresh = newRefreshToken.Token,
                access = newTokenStr,
            });
        }
    }
}
