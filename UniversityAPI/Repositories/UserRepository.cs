using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using UniversityAPI.EntityFramework;
using UniversityAPI.Models;

namespace UniversityAPI.Repositories
{
    public class UserRepository(UniversityDbContext context, UserManager<User> userManager)
    {
        readonly UniversityDbContext _context = context;
        readonly UserManager<User> _userManager = userManager;
        public async Task<User?> Get(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user is not null)
            {
                if (user.StudentProfileId is not null)
                {
                    user.StudentProfile = _context.StudentProfiles
                        .Include(sp => sp.Group)
                        .Include(sp => sp.Grades)
                        .Include(sp => sp.TestResults)
                        .FirstOrDefault(sp => sp.Id == user.StudentProfileId);
                }
                if (user.TeacherProfileId is not null)
                {
                    user.TeacherProfile = _context.TeachersProfiles
                        .Include(tp => tp.Degree)
                        .Include(tp => tp.Faculty)
                        .FirstOrDefault(tp => tp.Id == user.TeacherProfileId);
                }
            }
            return user;
        }
    }
}