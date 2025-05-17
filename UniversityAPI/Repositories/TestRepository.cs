using Microsoft.EntityFrameworkCore;
using UniversityAPI.EntityFramework;
using UniversityAPI.Models;
using UniversityAPI.Repositories.Base;

namespace UniversityAPI.Repositories
{
    public class TestRepository(UniversityDbContext context) : IBaseRepository<Test>
    {
        readonly UniversityDbContext _context = context;
        public async Task<int> Create(Test entity)
        {
            await _context.Tests.AddAsync(entity);
            _context.SaveChanges();
            return entity.Id;
        }

        public async Task Delete(int id)
        {
            var entity = await Get(id) ?? throw new KeyNotFoundException();
            _context.Tests.Remove(entity);
            _context.SaveChanges();
        }

        public async Task<List<Test>> Get()
        {
            return await _context.Tests.ToListAsync();
        }

        public async Task<Test?> Get(int id)
        {
            return await _context.Tests.FirstOrDefaultAsync(d => d.Id == id);
        }

        public void Update(Test entity)
        {
            _context.Tests.Update(entity);
            _context.SaveChanges();
        }
    }
}
