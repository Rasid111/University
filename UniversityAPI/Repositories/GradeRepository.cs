using Microsoft.EntityFrameworkCore;
using UniversityAPI.EntityFramework;
using UniversityAPI.Models;
using UniversityAPI.Repositories.Base;

namespace UniversityAPI.Repositories
{
    public class GradeRepository(UniversityDbContext context) : IBaseRepository<Grade>
    {
        private readonly UniversityDbContext _context = context;

        public async Task<int> Create(Grade entity)
        {
            await _context.Grades.AddAsync(entity);
            _context.SaveChanges();
            return entity.Id;
        }

        public async Task Delete(int id)
        {
            var entity = await Get(id) ?? throw new KeyNotFoundException();
            _context.Grades.Remove(entity);
            _context.SaveChanges();
        }

        public async Task<List<Grade>> Get()
        {
            return await _context.Grades.ToListAsync();
        }

        public async Task<Grade?> Get(int id)
        {
            return await _context.Grades.FirstOrDefaultAsync(g => g.Id == id);
        }

        public void Update(Grade entity)
        {
            _context.Grades.Update(entity);
            _context.SaveChanges();
        }
    }
}
