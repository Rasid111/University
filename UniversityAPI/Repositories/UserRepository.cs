using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UniversityAPI.EntityFramework;
using UniversityAPI.Models;
using UniversityAPI.Services;

namespace UniversityAPI.Repositories
{
    public class UserRepository(
        UniversityDbContext context,
        UserManager<User> userManager,
        BlobService blobService)
    {
        readonly UniversityDbContext _context = context;
        readonly UserManager<User> _userManager = userManager;
        readonly BlobService _blobService = blobService;

        public async Task<User?> Get(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user is not null)
            {
                if (user.StudentProfileId is not null)
                {
                    user.StudentProfile = _context.StudentProfiles
                        .Include(sp => sp.User)
                        .FirstOrDefault(sp => sp.Id == user.StudentProfileId);
                }
                if (user.TeacherProfileId is not null)
                {
                    user.TeacherProfile = _context.TeachersProfiles
                        .Include(sp => sp.User)
                        .FirstOrDefault(sp => sp.Id == user.TeacherProfileId);
                }
            }
            return user;
        }

        public async Task<string?> UploadProfilePictureAsync(string userId, IFormFile file)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return null;

            var fileName = $"pfp-{userId}{Path.GetExtension(file.FileName)}";
            await using var stream = file.OpenReadStream();
            var imageUrl = await _blobService.UploadAsync(stream, fileName); // ✅ fixed

            user.ProfilePictureUrl = imageUrl;
            await _context.SaveChangesAsync();

            return imageUrl;
        }
    }
}
