using Microsoft.EntityFrameworkCore;
using UniversityAPI.EntityFramework;
using UniversityAPI.Models;
using UniversityAPI.Repositories.Base;

namespace UniversityAPI.Repositories
{
    public class MajorRepository(UniversityDbContext context) : IBaseRepository<Major>
    {
        readonly UniversityDbContext _context = context;
        public async Task<int> Create(Major entity)
        {
            await _context.Majors.AddAsync(entity);
            _context.SaveChanges();
            return entity.Id;
        }

        public async Task Delete(int id)
        {
            var entity = await Get(id) ?? throw new KeyNotFoundException();
            _context.Majors.Remove(entity);
            _context.SaveChanges();
        }

        public async Task<List<Major>> Get()
        {
            return await _context.Majors.ToListAsync();
        }

        public async Task<Major?> Get(int id)
        {
            return await _context.Majors.FirstOrDefaultAsync(d => d.Id == id);
        }

        public void Update(Major entity)
        {
            _context.Majors.Update(entity);
            _context.SaveChanges();
        }
    }
}
