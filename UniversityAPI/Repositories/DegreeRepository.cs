using Microsoft.EntityFrameworkCore;
using UniversityAPI.EntityFramework;
using UniversityAPI.Models;
using UniversityAPI.Repositories.Base;

namespace UniversityAPI.Repositories
{
    public class DegreeRepository(UniversityDbContext context) : IBaseRepository<Degree>
    {
        readonly UniversityDbContext _context = context;
        public async Task<int> Create(Degree entity)
        {
            await _context.Degrees.AddAsync(entity);
            _context.SaveChanges();
            return entity.Id;
        }

        public async Task Delete(int id)
        {
            var entity = await Get(id) ?? throw new KeyNotFoundException();
            _context.Degrees.Remove(entity);
            _context.SaveChanges();
        }

        public async Task<List<Degree>> Get()
        {
            return await _context.Degrees.ToListAsync();
        }

        public async Task<Degree?> Get(int id)
        {
            return await _context.Degrees.FirstOrDefaultAsync(d => d.Id == id);
        }

        public void Update(Degree entity)
        {
            _context.Degrees.Update(entity);
            _context.SaveChanges();
        }
    }
}
