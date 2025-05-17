using Microsoft.EntityFrameworkCore;
using UniversityAPI.EntityFramework;
using UniversityAPI.Models;
using UniversityAPI.Repositories.Base;

namespace UniversityAPI.Repositories
{
    public class FacultyRepository(UniversityDbContext context) : IBaseRepository<Faculty>
    {
        readonly UniversityDbContext _context = context;
        public async Task<int> Create(Faculty entity)
        {
            await _context.Faculties.AddAsync(entity);
            _context.SaveChanges();
            return entity.Id;
        }

        public async Task Delete(int id)
        {
            var entity = await Get(id) ?? throw new KeyNotFoundException();
            _context.Faculties.Remove(entity);
            _context.SaveChanges();
        }

        public async Task<List<Faculty>> Get()
        {
            return await _context.Faculties.ToListAsync();
        }

        public async Task<Faculty?> Get(int id)
        {
            return await _context.Faculties.FirstOrDefaultAsync(d => d.Id == id);
        }

        public void Update(Faculty entity)
        {
            _context.Faculties.Update(entity);
            _context.SaveChanges();
        }
    }
}
