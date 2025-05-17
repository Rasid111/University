using Microsoft.EntityFrameworkCore;
using UniversityAPI.EntityFramework;
using UniversityAPI.Models;
using UniversityAPI.Repositories.Base;

namespace UniversityAPI.Repositories
{
    public class TeacherProfileRepository(UniversityDbContext context) : IBaseRepository<TeacherProfile>
    {
        readonly UniversityDbContext _context = context;
        public async Task<int> Create(TeacherProfile entity)
        {
            await _context.TeachersProfiles.AddAsync(entity);
            _context.SaveChanges();
            return entity.Id;
        }

        public async Task Delete(int id)
        {
            var entity = await Get(id) ?? throw new KeyNotFoundException();
            _context.TeachersProfiles.Remove(entity);
            _context.SaveChanges();
        }

        public async Task<List<TeacherProfile>> Get()
        {
            return await _context.TeachersProfiles.ToListAsync();
        }

        public async Task<TeacherProfile?> Get(int id)
        {
            return await _context.TeachersProfiles.FirstOrDefaultAsync(d => d.Id == id);
        }

        public void Update(TeacherProfile entity)
        {
            _context.TeachersProfiles.Update(entity);
            _context.SaveChanges();
        }
    }
}