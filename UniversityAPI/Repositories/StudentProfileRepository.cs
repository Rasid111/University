using Microsoft.EntityFrameworkCore;
using UniversityAPI.EntityFramework;
using UniversityAPI.Models;
using UniversityAPI.Repositories.Base;

namespace UniversityAPI.Repositories
{
    public class StudentProfileRepository(UniversityDbContext dbContext) : IBaseRepository<StudentProfile>
    {
        readonly UniversityDbContext _context = dbContext;
        public async Task<int> Create(StudentProfile entity)
        {
            await _context.StudentProfiles.AddAsync(entity);
            _context.SaveChanges();
            return entity.Id;
        }

        public async Task Delete(int id)
        {
            var entity = await Get(id) ?? throw new KeyNotFoundException();
            _context.StudentProfiles.Remove(entity);
            _context.SaveChanges();
        }

        public async Task<List<StudentProfile>> Get()
        {
            return await _context.StudentProfiles.ToListAsync();
        }

        public async Task<StudentProfile?> Get(int id)
        {
            return await _context.StudentProfiles.FirstOrDefaultAsync(d => d.Id == id);
        }

        public void Update(StudentProfile entity)
        {
            _context.StudentProfiles.Update(entity);
            _context.SaveChanges();
        }
    }
}
